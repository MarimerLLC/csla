//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
using System.Security.Principal;
using System.Threading;

namespace Csla.Xaml
{
  /// <summary>
  /// ApplicationContextManager for WPF applications
  /// </summary>
  public class ApplicationContextManager : Csla.Core.ApplicationContextManager
  {
    private static IPrincipal _principal = null;

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    /// <returns></returns>
    public override IPrincipal GetUser()
    {
      if (_principal == null)
      {
#if !__WASM__
        if (ApplicationContext.AuthenticationType == "Windows")
          SetUser(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
        else
#endif
          SetUser(new Csla.Security.UnauthenticatedPrincipal());
      }
      return _principal;
    }

    /// <summary>
    /// Sets the current principal.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public override void SetUser(IPrincipal principal)
    {
      _principal = principal;
      Thread.CurrentPrincipal = principal;
    }
  }
}