using System;
using System.EnterpriseServices;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Csla.Server.Hosts
{

  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through Enterprise Services.
  /// </summary>
  [EventTrackingEnabled(true)]
  [ComVisible(true)]
  public abstract class EnterpriseServicesPortal : ServicedComponent, Server.IDataPortalServer
  {

    /// <summary>
    /// Set up event handler to deal with
    /// serialization issue as discussed
    /// in Chapter 4.
    /// </summary>
    static EnterpriseServicesPortal()
    {
      SerializationWorkaround();
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public virtual DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new Server.DataPortal();
      return portal.Create(objectType, criteria, context);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new Server.DataPortal();
      return portal.Fetch(objectType, criteria, context);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public virtual DataPortalResult Update(object obj, DataPortalContext context)
    {
      Server.DataPortal portal = new Server.DataPortal();
      return portal.Update(obj, context);
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public virtual DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new Server.DataPortal();
      return portal.Delete(objectType, criteria, context);
    }

    #region Serialization bug workaround

    private static void SerializationWorkaround()
    {
      // hook up the AssemblyResolve
      // event so deep serialization works properly
      // this is a workaround for a bug in the .NET runtime
      AppDomain currentDomain = AppDomain.CurrentDomain;

      currentDomain.AssemblyResolve += 
        new ResolveEventHandler(ResolveEventHandler);
    }

    private static Assembly ResolveEventHandler(
      object sender, ResolveEventArgs args)
    {
      // get a list of all the assemblies loaded in our appdomain
      Assembly[] list = AppDomain.CurrentDomain.GetAssemblies();

      // search the list to find the assembly that was not found automatically
      // and return the assembly from the list

      foreach (Assembly asm in list)
        if (asm.FullName == args.Name)
          return asm;

      // if the assembly wasn't already in the appdomain, then try to load it.
      return Assembly.Load(args.Name);
    }

    #endregion
  }
}
