//-----------------------------------------------------------------------
// <copyright file="LocalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Csla.Core;
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
  public class LocalProxy : DataPortalClient.IDataPortalProxy, IDisposable
  {
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext">ApplicationContext</param>
    /// <param name="options">Options instance</param>
    public LocalProxy(ApplicationContext applicationContext, LocalProxyOptions options)
    {
      OriginalApplicationContext = applicationContext;
      var currentServiceProvider = OriginalApplicationContext.CurrentServiceProvider;
      Options = options;

      if (OriginalApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Client 
        && OriginalApplicationContext.IsAStatefulContextManager)
      {
        // create new DI scope and provider
        _scope = OriginalApplicationContext.CurrentServiceProvider.CreateScope();
        currentServiceProvider = _scope.ServiceProvider;

        // set runtime info to reflect that we're in a logical server-side
        // data portal operation, so this "runtime" is stateless and
        // we can't count on HttpContext
        var runtimeInfo = currentServiceProvider.GetRequiredService<IRuntimeInfo>();
        runtimeInfo.LocalProxyNewScopeExists = true;
      }

      CurrentApplicationContext = currentServiceProvider.GetRequiredService<ApplicationContext>();
      _portal = currentServiceProvider.GetRequiredService<Server.IDataPortalServer>();
    }

    private ApplicationContext OriginalApplicationContext { get; set; }
    private ApplicationContext CurrentApplicationContext { get; set; }
    private readonly LocalProxyOptions Options;

    private readonly IServiceScope _scope;
    private readonly Server.IDataPortalServer _portal;
    private bool disposedValue;

    private void SetApplicationContext(object obj, ApplicationContext applicationContext)
    {
      if (obj is IUseApplicationContext useApplicationContext)
        SetApplicationContext(useApplicationContext, applicationContext);
    }

    private void SetApplicationContext(IUseApplicationContext useApplicationContext, ApplicationContext applicationContext)
    {
      if (useApplicationContext != null && !ReferenceEquals(useApplicationContext.ApplicationContext, applicationContext))
      {
        useApplicationContext.ApplicationContext = applicationContext;

        if (useApplicationContext is IUseFieldManager useFieldManager)
          SetApplicationContext(useFieldManager.FieldManager, applicationContext);
        
        if (useApplicationContext is IUseBusinessRules useBusinessRules)
          SetApplicationContext(useBusinessRules.BusinessRules, applicationContext);

        if (useApplicationContext is IManageProperties target)
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
                SetApplicationContext((IUseApplicationContext)target.ReadProperty(managedProperty), applicationContext);
              }
              else if (typeof(IEnumerable).IsAssignableFrom(managedProperty.Type))
              {
                //property is a list and needs to be processed (could be a Csla.Core.MobileList or something like that)
                var enumerable = (IEnumerable)target.ReadProperty(managedProperty);
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

        if (useApplicationContext is IContainsDeletedList containsDeletedList)
          foreach (var item in containsDeletedList.DeletedList)
            SetApplicationContext(item, applicationContext);

        if (useApplicationContext is IEnumerable list)
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
    private void ResetApplicationContext()
    {
      CurrentApplicationContext.ApplicationContextAccessor = OriginalApplicationContext.ApplicationContextAccessor;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Create(
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      SetApplicationContext(criteria, CurrentApplicationContext);
      if (isSync || OriginalApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        result = await _portal.Create(objectType, criteria, context, isSync);
      }
      else
      {
        if (!Options.FlowSynchronizationContext || SynchronizationContext.Current == null)
          result = await Task.Run(() => this._portal.Create(objectType, criteria, context, isSync));
        else
          result = await await Task.Factory.StartNew(() => this._portal.Create(objectType, criteria, context, isSync),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
      }
      ResetApplicationContext();
      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to load an
    /// existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      SetApplicationContext(criteria, CurrentApplicationContext);
      if (isSync || OriginalApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        result = await _portal.Fetch(objectType, criteria, context, isSync);
      }
      else
      {
        if (!Options.FlowSynchronizationContext || SynchronizationContext.Current == null)
          result = await Task.Run(() => this._portal.Fetch(objectType, criteria, context, isSync));
        else
          result = await await Task.Factory.StartNew(() => this._portal.Fetch(objectType, criteria, context, isSync),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
      }
      ResetApplicationContext();
      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to update a
    /// business object.
    /// </summary>
    /// <param name="obj">The business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      SetApplicationContext(obj, CurrentApplicationContext);
      if (isSync || OriginalApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        result = await _portal.Update(obj, context, isSync);
      }
      else
      {
        if (!Options.FlowSynchronizationContext || SynchronizationContext.Current == null)
          result = await Task.Run(() => this._portal.Update(obj, context, isSync));
        else
          result = await await Task.Factory.StartNew(() => this._portal.Update(obj, context, isSync),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
      }
      ResetApplicationContext();
      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to delete a
    /// business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      SetApplicationContext(criteria, CurrentApplicationContext);
      if (isSync || OriginalApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        result = await _portal.Delete(objectType, criteria, context, isSync);
      }
      else
      {
        if (!Options.FlowSynchronizationContext || SynchronizationContext.Current == null)
          result = await Task.Run(() => this._portal.Delete(objectType, criteria, context, isSync));
        else
          result = await await Task.Factory.StartNew(() => this._portal.Delete(objectType, criteria, context, isSync),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
      }
      ResetApplicationContext();
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether this proxy will invoke
    /// a remote data portal server, or run the "server-side"
    /// data portal in the caller's process and AppDomain.
    /// </summary>
    public bool IsServerRemote
    {
      get { return false; }
    }

    /// <summary>
    /// Dispose current object
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          _scope?.Dispose();
        }
        disposedValue = true;
      }
    }

    /// <summary>
    /// Dispose current object
    /// </summary>
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}