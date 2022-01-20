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
      ApplicationContext = applicationContext;
      Options = options;

      if (Options.CreateScopePerCall && 
        ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Client)
      {
        // create new DI scope and provider
        _scope = ApplicationContext.CurrentServiceProvider.CreateScope();
        var provider = _scope.ServiceProvider;

        // create and initialize new "server-side" ApplicationContext and manager
        var parentContext = applicationContext;
        var ctx = provider.GetRequiredService<Core.IContextManager>();
        ctx.SetClientContext(parentContext.ClientContext, parentContext.ExecutionLocation);
        ctx.SetUser(parentContext.User);
        ApplicationContext = new ApplicationContext(ctx, provider);
      }

      _portal = ApplicationContext.CurrentServiceProvider.GetRequiredService<Server.IDataPortalServer>();
    }

    private ApplicationContext ApplicationContext { get; set; }
    private readonly LocalProxyOptions Options;

    private readonly IServiceScope _scope;
    private readonly Server.IDataPortalServer _portal;
    private bool disposedValue;

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
      if (isSync || ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        return await _portal.Create(objectType, criteria, context, isSync);
      }
      else
      {
        if (!Options.FlowSynchronizationContext || SynchronizationContext.Current == null)
          return await Task.Run(() => this._portal.Create(objectType, criteria, context, isSync));
        else
          return await await Task.Factory.StartNew(() => this._portal.Create(objectType, criteria, context, isSync),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
      }
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
      if (isSync || ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        return await _portal.Fetch(objectType, criteria, context, isSync);
      }
      else
      {
        if (!Options.FlowSynchronizationContext || SynchronizationContext.Current == null)
          return await Task.Run(() => this._portal.Fetch(objectType, criteria, context, isSync));
        else
          return await await Task.Factory.StartNew(() => this._portal.Fetch(objectType, criteria, context, isSync),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
      }
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
      if (isSync || ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        return await _portal.Update(obj, context, isSync);
      }
      else
      {
        if (!Options.FlowSynchronizationContext || SynchronizationContext.Current == null)
          return await Task.Run(() => this._portal.Update(obj, context, isSync));
        else
          return await await Task.Factory.StartNew(() => this._portal.Update(obj, context, isSync),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
      }
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
      if (isSync || ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        return await _portal.Delete(objectType, criteria, context, isSync);
      }
      else
      {
        if (!Options.FlowSynchronizationContext || SynchronizationContext.Current == null)
          return await Task.Run(() => this._portal.Delete(objectType, criteria, context, isSync));
        else
          return await await Task.Factory.StartNew(() => this._portal.Delete(objectType, criteria, context, isSync),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
      }
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
          if (Options.CreateScopePerCall)
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