//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Xaml
{
  /// <summary>
  /// ApplicationContextManager for WPF applications
  /// </summary>
  public class ApplicationContextManager : Csla.Core.ApplicationContextManager
  {
    private static IPrincipal _principal = null;
    private static ApplicationContext applicationContext;

    /// <summary>
    /// Method called when the ApplicationContext
    /// property has been set to a new value.
    /// </summary>
    protected override void OnApplicationContextSet() => applicationContext = base.ApplicationContext;

    /// <summary>
    /// Gets the current ApplicationContext.
    /// </summary>
    public static ApplicationContext GetApplicationContext()
    {
      if (applicationContext == null)
        throw new InvalidOperationException($"{nameof(applicationContext)} == null");
      return applicationContext;
    }

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    /// <returns></returns>
    public override IPrincipal GetUser()
    {
      if (_principal == null)
      {
        if (ApplicationContext.AuthenticationType == "Windows")
#pragma warning disable CA1416 // Validate platform compatibility
          SetUser(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
#pragma warning restore CA1416 // Validate platform compatibility
        else
          SetUser(new System.Security.Claims.ClaimsPrincipal());
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