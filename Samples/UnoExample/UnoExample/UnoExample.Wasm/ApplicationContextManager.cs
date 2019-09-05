//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default context manager for the user property</summary>
//-----------------------------------------------------------------------
using System.Security.Principal;

namespace Csla.Uno
{
  public class ApplicationContextManager: Core.ApplicationContextManager
  {
    private IPrincipal _principal;

    public override IPrincipal GetUser()
    {
      if (_principal == null)
      {
        _principal = new Security.UnauthenticatedPrincipal();
        SetUser(_principal);
      }
      return _principal;
    }

    public override void SetUser(IPrincipal principal)
    {
      _principal = principal;
    }
  }
}
