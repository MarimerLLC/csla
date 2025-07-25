//-----------------------------------------------------------------------
// <copyright file="LocalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.DataPortalClient;
using Csla.Runtime;
using Csla.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Channels.Local
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to an application server hosted locally 
  /// in the client process and AppDomain.
  /// </summary>
  public class LocalProxy : DataPortalProxy
  {
    private readonly LocalProxyOptions _options;

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext">ApplicationContext</param>
    /// <param name="options">Options instance</param>
    public LocalProxy(ApplicationContext applicationContext, LocalProxyOptions options)
      : base(applicationContext)
    {
      _options = options;
    }

    /// <summary>
    /// Application Context supplied in CTOR.  If this LocalProxy was created on client side or 
    /// if <see cref="LocalProxyOptions.UseLocalScope"/> is false then it is the client ApplicationContext 
    /// </summary>
    protected ApplicationContext CallerApplicationContext => ApplicationContext;

    private void InitializeContext(out IDataPortalServer _portal, out IServiceScope? _logicalServerScope, out ApplicationContext _logicalServerApplicationContext)
    {
      var logicalServerServiceProvider = CallerApplicationContext.CurrentServiceProvider;

      if (_options.UseLocalScope
        && CallerApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Client
        && CallerApplicationContext.IsAStatefulContextManager)
      {
        // create new DI scope and provider
        _logicalServerScope = CallerApplicationContext.CurrentServiceProvider.CreateScope();
        logicalServerServiceProvider = _logicalServerScope.ServiceProvider;

        // set runtime info to reflect that we're in a logical server-side
        // data portal operation, so this "runtime" is stateless and
        // we can't count on HttpContext
        var serverRuntimeInfo = logicalServerServiceProvider.GetRequiredService<IRuntimeInfo>();
        serverRuntimeInfo.LocalProxyNewScopeExists = true;
      }
      else
      {
        _logicalServerScope = null;
      }

      _logicalServerApplicationContext = logicalServerServiceProvider.GetRequiredService<ApplicationContext>();
      _portal = logicalServerServiceProvider.GetRequiredService<IDataPortalServer>();
    }

    private void SetApplicationContext(object? obj, ApplicationContext applicationContext)
    {
      // if there's no isolated scope, there's no reason to 
      // change the object graph's ApplicationContext
      if (!_options.UseLocalScope)
        return;

      if (obj is IUseApplicationContext useApplicationContext)
        SetApplicationContext(useApplicationContext, applicationContext);
    }

    private void SetApplicationContext(IUseApplicationContext? obj, ApplicationContext applicationContext)
    {
      // if there's no isolated scope, there's no reason to 
      // change the object graph's ApplicationContext
      if (!_options.UseLocalScope)
        return;

      if (obj != null && !ReferenceEquals(obj.ApplicationContext, applicationContext))
      {
        obj.ApplicationContext = applicationContext;

        if (obj is IUseFieldManager useFieldManager)
          SetApplicationContext(useFieldManager.FieldManager, applicationContext);

        if (obj is IUseBusinessRules useBusinessRules)
          SetApplicationContext(useBusinessRules.BusinessRules, applicationContext);

        if (obj is IManageProperties target)
        {
          foreach (var managedProperty in target.GetManagedProperties())
          {
            var isLazyLoadedProperty = (managedProperty.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad;

            //only dig into property if its not a lazy loaded property 
            //  or if it is, then only if its loaded
            if (!isLazyLoadedProperty
              || (isLazyLoadedProperty && target.FieldExists(managedProperty)))
            {
              if (typeof(IUseApplicationContext).IsAssignableFrom(managedProperty.Type))
              {
                //property is directly assignable to the IUseApplicationContext so set it
                SetApplicationContext((IUseApplicationContext?)target.ReadProperty(managedProperty), applicationContext);
              }
              else if (typeof(IEnumerable).IsAssignableFrom(managedProperty.Type))
              {
                //property is a list and needs to be processed (could be a Csla.Core.MobileList or something like that)
                var enumerable = (IEnumerable?)target.ReadProperty(managedProperty);
                if (enumerable != null)
                {
                  foreach (var item in enumerable)
                  {
                    SetApplicationContext(item, applicationContext);
                  }
                }
              }
            }
          }
        }

        if (obj is IContainsDeletedList containsDeletedList)
          foreach (var item in containsDeletedList.DeletedList)
            SetApplicationContext(item, applicationContext);

        if (obj is IEnumerable list)
          foreach (var item in list)
            SetApplicationContext(item, applicationContext);
      }
    }

    /// <summary>
    /// Reset the temporary scoped ApplicationContext object's
    /// context manager back to the original calling scope's
    /// context manager, thus resetting the entire resulting
    /// object graph to use the original context manager.
    /// </summary>
    private void ResetApplicationContext(ApplicationContext logicalServerApplicationContext)
    {
      if (_options.UseLocalScope)
      {
        if (logicalServerApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Client)
        {
          RestoringClientSideApplicationContext(logicalServerApplicationContext, CallerApplicationContext);
        }

        logicalServerApplicationContext.ApplicationContextAccessor = CallerApplicationContext.ApplicationContextAccessor;
      }
    }

    private async Task DisposeScope(IServiceScope? logicalServerScope)
    {
      if (logicalServerScope is IAsyncDisposable asyncDisposable)
        await asyncDisposable.DisposeAsync();
      logicalServerScope?.Dispose();
    }

    /// <inheritdoc />
    public override async Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      IServiceScope? _logicalServerScope = null;
      DataPortalResult result;
      try
      {
        InitializeContext(out var _portal, out _logicalServerScope, out var _logicalServerApplicationContext);
        SetApplicationContext(criteria, _logicalServerApplicationContext);
        if (isSync || CallerApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
        {
          result = await _portal.Create(objectType, criteria, context, isSync);
        }
        else
        {
          if (!_options.FlowSynchronizationContext || SynchronizationContext.Current == null)
            result = await Task.Run(() => _portal.Create(objectType, criteria, context, isSync));
          else
            result = await await Task.Factory.StartNew(() => _portal.Create(objectType, criteria, context, isSync),
              CancellationToken.None,
              TaskCreationOptions.None,
              TaskScheduler.FromCurrentSynchronizationContext());
        }
        ResetApplicationContext(_logicalServerApplicationContext);
      }
      finally
      {
        await DisposeScope(_logicalServerScope);
      }

      var operation = DataPortalOperations.Create;
      OnServerComplete(result, objectType, operation);

      if (ExecutionIsNotOnLogicalOrPhysicalServer)
        OnServerCompleteClient(result, objectType, operation);

      return result;
    }

    /// <inheritdoc />
    public override async Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      IServiceScope? _logicalServerScope = null;
      DataPortalResult result;
      try
      {
        InitializeContext(out var _portal, out _logicalServerScope, out var _logicalServerApplicationContext);
        SetApplicationContext(criteria, _logicalServerApplicationContext);
        if (isSync || CallerApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
        {
          result = await _portal.Fetch(objectType, criteria, context, isSync);
        }
        else
        {
          if (!_options.FlowSynchronizationContext || SynchronizationContext.Current == null)
            result = await Task.Run(() => _portal.Fetch(objectType, criteria, context, isSync));
          else
            result = await await Task.Factory.StartNew(() => _portal.Fetch(objectType, criteria, context, isSync),
              CancellationToken.None,
              TaskCreationOptions.None,
              TaskScheduler.FromCurrentSynchronizationContext());
        }
        ResetApplicationContext(_logicalServerApplicationContext);
      }
      finally
      {
        await DisposeScope(_logicalServerScope);
      }

      var operation = DataPortalOperations.Fetch;
      OnServerComplete(result, objectType, operation);

      if (ExecutionIsNotOnLogicalOrPhysicalServer)
        OnServerCompleteClient(result, objectType, operation);

      return result;
    }

    /// <inheritdoc />
    public override async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      IServiceScope? _logicalServerScope = null;
      DataPortalResult result;
      try
      {
        InitializeContext(out var _portal, out _logicalServerScope, out var _logicalServerApplicationContext);
        SetApplicationContext(obj, _logicalServerApplicationContext);
        if (isSync || CallerApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
        {
          result = await _portal.Update(obj, context, isSync);
        }
        else
        {
          if (!_options.FlowSynchronizationContext || SynchronizationContext.Current == null)
            result = await Task.Run(() => _portal.Update(obj, context, isSync));
          else
            result = await await Task.Factory.StartNew(() => _portal.Update(obj, context, isSync),
              CancellationToken.None,
              TaskCreationOptions.None,
              TaskScheduler.FromCurrentSynchronizationContext());
        }
        ResetApplicationContext(_logicalServerApplicationContext);
      }
      finally
      {
        await DisposeScope(_logicalServerScope);
      }

      var operation = DataPortalOperations.Update;
      OnServerComplete(result, obj.GetType(), operation);

      if (ExecutionIsNotOnLogicalOrPhysicalServer)
        OnServerCompleteClient(result, obj.GetType(), operation);

      return result;
    }

    /// <inheritdoc />
    public override async Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      IServiceScope? _logicalServerScope = null;
      DataPortalResult result;
      try
      {
        InitializeContext(out var _portal, out _logicalServerScope, out var _logicalServerApplicationContext);
        SetApplicationContext(criteria, _logicalServerApplicationContext);
        if (isSync || CallerApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
        {
          result = await _portal.Delete(objectType, criteria, context, isSync);
        }
        else
        {
          if (!_options.FlowSynchronizationContext || SynchronizationContext.Current == null)
            result = await Task.Run(() => _portal.Delete(objectType, criteria, context, isSync));
          else
            result = await await Task.Factory.StartNew(() => _portal.Delete(objectType, criteria, context, isSync),
              CancellationToken.None,
              TaskCreationOptions.None,
              TaskScheduler.FromCurrentSynchronizationContext());
        }
        ResetApplicationContext(_logicalServerApplicationContext);
      }
      finally
      {
        await DisposeScope(_logicalServerScope);
      }

      var operation = DataPortalOperations.Delete;
      OnServerComplete(result, objectType, operation);

      if (ExecutionIsNotOnLogicalOrPhysicalServer)
        OnServerCompleteClient(result, objectType, operation);

      return result;
    }

    /// <summary>
    /// Not needed/implemented for Local Data Portal - throws not supported exception
    /// </summary>
    /// <param name="serialized">Serialized request</param>
    /// <param name="operation">DataPortal operation</param>
    /// <param name="routingToken">Routing Tag for server</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>Serialized response from server</returns>
    protected override Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string? routingToken, bool isSync)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Gets a value indicating whether this proxy will invoke
    /// a remote data portal server, or run the "server-side"
    /// data portal in the caller's process and AppDomain.
    /// </summary>
    public override bool IsServerRemote => false;

    /// <inheritdoc />
    public override string DataPortalUrl => throw new NotSupportedException();

    /// <summary>
    /// Method called once when execution returns to <see cref="ApplicationContext.LogicalExecutionLocations.Client"/> after a call to 
    /// the dataportal.  Only called when <see cref="LocalProxyOptions.UseLocalScope"/> is true.
    /// </summary>
    /// <param name="serverScopedApplicationContext">Context created in a new scope for use during logical server side operations</param>
    /// <param name="clientSideApplicationContext">Original context from the client side</param>
    protected virtual void RestoringClientSideApplicationContext(ApplicationContext serverScopedApplicationContext, ApplicationContext clientSideApplicationContext)
    {

    }
  }
}