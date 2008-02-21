using System;
using System.Security.Principal;
using System.Collections.Specialized;

namespace Csla.Server
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  [Serializable()]
  public class DataPortalContext
  {
    private IPrincipal _principal;
    private bool _remotePortal;
    private string _clientCulture;
    private string _clientUICulture;
    private HybridDictionary _clientContext;
    private HybridDictionary _globalContext;

    /// <summary>
    /// The current principal object
    /// if CSLA security is being used.
    /// </summary>
    public IPrincipal Principal
    {
      get { return _principal; }
    }

    /// <summary>
    /// Returns <see langword="true" /> if the 
    /// server-side DataPortal is running
    /// on a remote server via remoting.
    /// </summary>
    public bool IsRemotePortal
    {
      get { return _remotePortal; }
    }

    /// <summary>
    /// The culture setting on the client
    /// workstation.
    /// </summary>
    public string ClientCulture
    {
      get { return _clientCulture; }
    }

    /// <summary>
    /// The culture setting on the client
    /// workstation.
    /// </summary>
    public string ClientUICulture
    {
      get { return _clientUICulture; }
    }
    internal HybridDictionary ClientContext
    {
      get { return _clientContext; }
    }

    internal HybridDictionary GlobalContext
    {
      get { return _globalContext; }
    }

    /// <summary>
    /// Creates a new DataPortalContext object.
    /// </summary>
    /// <param name="principal">The current Principal object.</param>
    /// <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    public DataPortalContext(IPrincipal principal, bool isRemotePortal)
    {
      if (isRemotePortal)
      {
        _principal = principal;
        _remotePortal = isRemotePortal;
        _clientCulture = 
          System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        _clientUICulture = 
          System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
        _clientContext = Csla.ApplicationContext.GetClientContext();
        _globalContext = Csla.ApplicationContext.GetGlobalContext();
      }
    }
  }
}
