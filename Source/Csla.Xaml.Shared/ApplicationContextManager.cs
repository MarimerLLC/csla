//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;
using Csla.Configuration;

namespace Csla.Xaml
{
  /// <summary>
  /// ApplicationContextManager for WPF applications
  /// </summary>
  /// <param name="securityOptions"></param>
  public class ApplicationContextManager(SecurityOptions securityOptions) : Csla.Core.ApplicationContextManager
  {
    private static IPrincipal _principal = null;
    private static ApplicationContext applicationContext;
    private SecurityOptions _securityOptions = securityOptions;

    /// <summary>
    /// Method called when the ApplicationContext
    /// property has been set to a new value.
    /// </summary>
    protected override void OnApplicationContextSet() => applicationContext = ApplicationContext;

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
    public override IPrincipal GetUser()
    {
      if (_principal == null)
      {
#if NET8_0_OR_GREATER
        if (OperatingSystem.IsWindows() && !_securityOptions .FlowSecurityPrincipalFromClient)
          SetUser(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
        else
          SetUser(new System.Security.Claims.ClaimsPrincipal());
#elif NETFRAMEWORK
        if (!_securityOptions.FlowSecurityPrincipalFromClient)
#pragma warning disable CA1416 // Validate platform compatibility
          SetUser(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
#pragma warning restore CA1416 // Validate platform compatibility
        else
          SetUser(new System.Security.Claims.ClaimsPrincipal());
#else
        SetUser(new System.Security.Claims.ClaimsPrincipal());
#endif
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