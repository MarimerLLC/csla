//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using System.Threading;
using Csla.Core;

namespace Csla.Windows
{
  /// <summary>
  /// ApplicationContextManager for Windows Forms applications
  /// </summary>
  public class ApplicationContextManager : Csla.ApplicationContext.ApplicationContextManager
  {
    private static IPrincipal _principal;

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    /// <returns></returns>
    public override IPrincipal GetUser()
    {
      IPrincipal current;
      if (_principal == null)
      {
        if (ApplicationContext.AuthenticationType != "Windows")
          _principal = new Csla.Security.UnauthenticatedPrincipal();
        else
          _principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
      }
      current = _principal;
      return current;
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