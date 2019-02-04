//-----------------------------------------------------------------------
// <copyright file="LocalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;
using Csla.Server;
using Csla.Threading;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to an application server hosted locally 
  /// in the client process and AppDomain.
  /// </summary>
  public class LocalProxy : DataPortalClient.IDataPortalProxy
  {
    private Server.IDataPortalServer _portal = new Server.DataPortal();
    private readonly TaskFactory _taskFactory = new TaskFactory(new CslaTaskScheduler());

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
      if (isSync || Csla.ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        return await _portal.Create(objectType, criteria, context, isSync);
      }
      else
      {
        if (SynchronizationContext.Current == null)
          return await await this._taskFactory.StartNew(() => this._portal.Create(objectType, criteria, context, isSync));

        return await await this._taskFactory.StartNew(() => this._portal.Create(objectType, criteria, context, isSync),
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
      if (isSync || Csla.ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        return await _portal.Fetch(objectType, criteria, context, isSync);
      }
      else
      {
        if (SynchronizationContext.Current == null)
          return await await this._taskFactory.StartNew(() => this._portal.Fetch(objectType, criteria, context, isSync));

        return await await this._taskFactory.StartNew(() => this._portal.Fetch(objectType, criteria, context, isSync),
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
      if (isSync || Csla.ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        return await _portal.Update(obj, context, isSync);
      }
      else
      {
        if (SynchronizationContext.Current == null)
          return await await this._taskFactory.StartNew(() => this._portal.Update(obj, context, isSync));

        return await await this._taskFactory.StartNew(() => this._portal.Update(obj, context, isSync),
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
      if (isSync || Csla.ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server)
      {
        return await _portal.Delete(objectType, criteria, context, isSync);
      }
      else
      {
        if (SynchronizationContext.Current == null)
          return await await this._taskFactory.StartNew(() => this._portal.Delete(objectType, criteria, context, isSync));

        return await await this._taskFactory.StartNew(() => this._portal.Delete(objectType, criteria, context, isSync),
          CancellationToken.None,
          TaskCreationOptions.None,
          TaskScheduler.FromCurrentSynchronizationContext());
      }
    }

    /// <summary>
    /// Get a value indicating whether this proxy will invoke
    /// a remote data portal server, or run the "server-side"
    /// data portal in the caller's process and AppDomain.
    /// </summary>
    public bool IsServerRemote
    {
      get { return false; }
    }

  }
}