//-----------------------------------------------------------------------
// <copyright file="LocalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
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

    private void SetServiceProvider(object criteria)
    {
      if (criteria is IUseApplicationContext useApplicationContext && 
          !ReferenceEquals(OriginalApplicationContext.CurrentServiceProvider, CurrentApplicationContext.CurrentServiceProvider))
        {
          var accessor = CurrentApplicationContext.CurrentServiceProvider.GetRequiredService<ApplicationContextAccessor>();
          useApplicationContext.ApplicationContext.ApplicationContextAccessor = accessor;
        }
    }

    private void ResetServiceProvider()
    {
      if (!ReferenceEquals(OriginalApplicationContext.CurrentServiceProvider, CurrentApplicationContext.CurrentServiceProvider))
      {
        var accessor = OriginalApplicationContext.CurrentServiceProvider.GetRequiredService<ApplicationContextAccessor>();
        CurrentApplicationContext.ApplicationContextAccessor = accessor;
      }
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
      SetServiceProvider(criteria);
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
      ResetServiceProvider();
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
      SetServiceProvider(criteria);
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
      ResetServiceProvider();
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
      SetServiceProvider(obj);
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
      ResetServiceProvider();
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
      SetServiceProvider(criteria);
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
      ResetServiceProvider();
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
          if (OriginalApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Client
            && OriginalApplicationContext.IsAStatefulContextManager)
          {
            _scope?.Dispose();
          }
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