using System;
using System.Security.Principal;

/// <summary>
/// 
/// </summary>
namespace CSLA.Server
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  /// <remarks>
  /// The context includes the current 
  /// <see cref="T:CSLA.Security.BusinessPrincipal" />
  /// object if CSLA security is being used. It also includes a
  /// flag indicating whether the server-side DataPortal is running
  /// locally or remotely.
  /// </remarks>
  [Serializable()]
  public class DataPortalContext
  {
    IPrincipal _principal;
    bool _remotePortal;

    /// <summary>
    /// The current <see cref="T:CSLA.Security.BusinessPrincipal" />
    /// if CSLA security is being used.
    /// </summary>
    public IPrincipal Principal
    {
      get
      {
        return _principal;
      }
    }

    /// <summary>
    /// Returns True if the server-side DataPortal is running
    /// on a remote server via remoting.
    /// </summary>
    public bool IsRemotePortal
    {
      get
      {
        return _remotePortal;
      }
    }

    /// <summary>
    /// Creates a new DataPortalContext object.
    /// </summary>
    /// <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    public DataPortalContext(bool isRemotePortal)
    {
      _principal = null;
      _remotePortal = isRemotePortal;
    }

    /// <summary>
    /// Creates a new DataPortalContext object.
    /// </summary>
    /// <param name="principal">The current Principal object.</param>
    /// <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    public DataPortalContext(IPrincipal principal, bool isRemotePortal)
    {
      _principal = principal;
      _remotePortal = isRemotePortal;
    }
  }
}
