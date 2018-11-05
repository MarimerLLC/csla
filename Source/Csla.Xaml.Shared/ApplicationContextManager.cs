//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
#if !NETFX_CORE && !PCL36
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Csla.Core;

namespace Csla.Xaml
{
  /// <summary>
  /// ApplicationContextManager for WPF applications
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
#if !XAMARIN
      if (System.Windows.Application.Current != null)
      {
#endif
        if (_principal == null)
        {
          if (ApplicationContext.AuthenticationType != "Windows")
            _principal = new Csla.Security.UnauthenticatedPrincipal();
          else
            _principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        }
        current = _principal;
#if !XAMARIN
      }
      else
        current = Thread.CurrentPrincipal;
#endif
      return current;
    }

    /// <summary>
    /// Sets the current principal.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public override void SetUser(IPrincipal principal)
    {
#if !XAMARIN
      if (System.Windows.Application.Current != null)
#endif
        _principal = principal;
      Thread.CurrentPrincipal = principal;
    }
  }
}
#endif