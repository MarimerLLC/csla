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

    public EnterpriseServicesPortal()
    {
      SerializationWorkaround();
    }

    public virtual DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new Server.DataPortal();
      return portal.Create(objectType, criteria, context);
    }

    public virtual DataPortalResult Fetch(object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new Server.DataPortal();
      return portal.Fetch(criteria, context);
    }

    public virtual DataPortalResult Update(object obj, DataPortalContext context)
    {
      Server.DataPortal portal = new Server.DataPortal();
      return portal.Update(obj, context);
    }

    public virtual DataPortalResult Delete(object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new Server.DataPortal();
      return portal.Delete(criteria, context);
    }

    #region Serialization bug workaround

    private static void SerializationWorkaround()
    {
      // hook up the AssemblyResolve
      // event so deep serialization works properly
      // this is a workaround for a bug in the .NET runtime
      AppDomain currentDomain = AppDomain.CurrentDomain;

      currentDomain.AssemblyResolve += new ResolveEventHandler(ResolveEventHandler);
    }

    private static Assembly ResolveEventHandler(object sender, ResolveEventArgs args)
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
