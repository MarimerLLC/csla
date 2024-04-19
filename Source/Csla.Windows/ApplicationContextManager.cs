﻿//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
using System.Security.Principal;
using Csla.Configuration;

namespace Csla.Windows
{
  /// <summary>
  /// ApplicationContextManager for Windows Forms applications
  /// </summary>
  /// <param name="securityOptions"></param>
  public class ApplicationContextManager(SecurityOptions securityOptions) : Csla.Core.ApplicationContextManager
  {
    private static IPrincipal _principal;
    private SecurityOptions SecurityOptions { get; set; } = securityOptions;

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public override IPrincipal GetUser()
    {
      if (_principal == null)
      {
        if (SecurityOptions.FlowSecurityPrincipalFromClient)
          SetUser(new System.Security.Claims.ClaimsPrincipal());
        else
#pragma warning disable CA1416 // Validate platform compatibility
          SetUser(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
#pragma warning restore CA1416 // Validate platform compatibility
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