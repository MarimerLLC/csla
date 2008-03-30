using System;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to an application server hosted in COM+.
  /// </summary>
  public abstract class EnterpriseServicesProxy : DataPortalClient.IDataPortalProxy
  {
    /// <summary>
    /// Override this method to return a reference to
    /// the server-side COM+ (ServicedComponent) object
    /// implementing the data portal server functionality.
    /// </summary>
    protected abstract Server.Hosts.EnterpriseServicesPortal GetServerObject();

    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public virtual Server.DataPortalResult Create(Type objectType, object criteria, Server.DataPortalContext context)
    {
      Server.Hosts.EnterpriseServicesPortal svc = GetServerObject();
      try
      {
        return svc.Create(objectType, criteria, context);
      }
      finally
      {
        if (svc != null)
          svc.Dispose();
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
    public virtual Server.DataPortalResult Fetch(Type objectType, object criteria, Server.DataPortalContext context)
    {
      Server.Hosts.EnterpriseServicesPortal svc = GetServerObject();
      try
      {
        return svc.Fetch(objectType, criteria, context);
      }
      finally
      {
        if (svc != null)
          svc.Dispose();
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
    public virtual Server.DataPortalResult Update(object obj, Server.DataPortalContext context)
    {
      Server.Hosts.EnterpriseServicesPortal svc = GetServerObject();
      try
      {
        return svc.Update(obj, context);
      }
      finally
      {
        if (svc != null)
          svc.Dispose();
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
    public virtual Server.DataPortalResult Delete(Type objectType, object criteria, Server.DataPortalContext context)
    {
      Server.Hosts.EnterpriseServicesPortal svc = GetServerObject();
      try
      {
        return svc.Delete(objectType, criteria, context);
      }
      finally
      {
        if (svc != null)
          svc.Dispose();
      }
    }

    /// <summary>
    /// Get a value indicating whether this proxy will invoke
    /// a remote data portal server, or run the "server-side"
    /// data portal in the caller's process and AppDomain.
    /// </summary>
    public virtual bool IsServerRemote
    {
      get { return true; }
    }
  }
}
