using System;
using System.Threading;
using System.Collections.Specialized;
using System.Configuration;

namespace Csla
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  public sealed class ApplicationContext
  {
    /// <summary>
    /// Returns the application-specific context data provided
    /// by the client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a HybridDictionary. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is transferred from
    /// the client to the server. No data is transferred from
    /// the server to the client.
    /// </para>
    /// </remarks>
    public static HybridDictionary ClientContext
    {
      get
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot("Csla.ClientContext");
        HybridDictionary ctx = (HybridDictionary)Thread.GetData(slot);
        if (ctx == null)
        {
          ctx = new HybridDictionary();
          Thread.SetData(slot, ctx);
        }
        return ctx;
      }
    }

    /// <summary>
    /// Returns the application-specific context data shared
    /// on both client and server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a HybridDictionary. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is transferred to and from
    /// the client and server. Any objects or data in this context
    /// will be transferred bi-directionally across the network.
    /// </para>
    /// </remarks>
    public static HybridDictionary GlobalContext
    {
      get
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot("Csla.GlobalContext");
        HybridDictionary ctx = (HybridDictionary)Thread.GetData(slot);
        if (ctx == null)
        {
          ctx = new HybridDictionary();
          Thread.SetData(slot, ctx);
        }
        return ctx;
      }
    }

    /// <summary>
    /// Returns the authentication type being used by the
    /// CSLA .NET framework.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// This value is read from the application configuration
    /// file with the key value "CslaAuthentication". The value
    /// "Windows" indicates CSLA .NET should use Windows integrated
    /// (or AD) security. Any other value indicates the use of
    /// custom security derived from BusinessPrincipalBase.
    /// </remarks>
    public static string AuthenticationType
    {
      get { return ConfigurationManager.AppSettings["CslaAuthentication"]; }
    }

    /// <summary>
    /// Returns the channel or network protocol
    /// for the DataPortal server.
    /// </summary>
    /// <value>Fully qualified assembly/type name of the proxy class.</value>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This value is read from the application configuration
    /// file with the key value "CslaDataPortalProxy". 
    /// </para><para>
    /// The proxy class must implement Csla.Server.IDataPortalServer.
    /// </para><para>
    /// The value "Local" is a shortcut to running the DataPortal
    /// "server" in the client process.
    /// </para><para>
    /// Other built-in values include:
    /// <list>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.RemotingProxy</term>
    /// <description>Use .NET Remoting to communicate with the server</description>
    /// </item>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.EnterpriseServicesProxy</term>
    /// <description>Use Enterprise Services (DCOM) to communicate with the server</description>
    /// </item>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.WebServicesProxy</term>
    /// <description>Use Web Services (asmx) to communicate with the server</description>
    /// </item>
    /// </list>
    /// Each proxy type does require that the DataPortal server be hosted using the appropriate
    /// technology. For instance, Web Services and Remoting should be hosted in IIS, while
    /// Enterprise Services must be hosted in COM+.
    /// </para>
    /// </remarks>
    public static string DataPortalProxy
    {
      get
      {
        string result = ConfigurationManager.AppSettings["CslaDataPortalProxy"];
        if (string.IsNullOrEmpty(result))
          result = "Local";
        return result;
      }
    }

    /// <summary>
    /// Returns the URL for the DataPortal server.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// This value is read from the application configuration
    /// file with the key value "CslaDataPortalUrl". 
    /// </remarks>
    public static Uri DataPortalUrl
    {
      get { return new Uri(ConfigurationManager.AppSettings["CslaDataPortalUrl"]); }
    }

    /// <summary>
    /// Enum representing the locations code can execute.
    /// </summary>
    public enum ExecutionLocations
    {
      Client,
      Server
    }

    private static ExecutionLocations _executionLocation = ExecutionLocations.Client;

    /// <summary>
    /// Returns a value indicating whether the application code
    /// is currently executing on the client or server.
    /// </summary>
    public static ExecutionLocations ExecutionLocation
    {
      get { return _executionLocation; }
    }

    internal static void SetExecutionLocation(ExecutionLocations location)
    {
      _executionLocation = location;
    }

    internal static HybridDictionary GetClientContext()
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot("Csla.ClientContext");
      return (HybridDictionary)Thread.GetData(slot);
    }

    internal static HybridDictionary GetGlobalContext()
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot("Clsa.GlobalContext");
      return (HybridDictionary)Thread.GetData(slot);
    }

    internal static void SetContext(object clientContext, object globalContext)
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot("Csla.ClientContext");
      Thread.SetData(slot, clientContext);

      slot = Thread.GetNamedDataSlot("Csla.GlobalContext");
      Thread.SetData(slot, globalContext);
    }

    public static void Clear()
    {
      SetContext(Nothing, Nothing);
    }

    private ApplicationContext()
    {
      // prevent instantiation
    }
  }
}