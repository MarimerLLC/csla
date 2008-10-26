using System;
using System.Security.Principal;
using Csla.Serialization;
namespace Csla.Security
{
  /// <summary>
  /// Implementation of a .NET principal object that represents
  /// an unauthenticated user. Contains an UnauthenticatedIdentity
  /// object.
  /// </summary>
  [Serializable]
  public sealed class UnauthenticatedPrincipal : BusinessPrincipalBase
  {
    public UnauthenticatedPrincipal() : base(new UnauthenticatedIdentity()) { }

    public override bool IsInRole(string role)
    {
      if (Csla.DataPortal.IsInDesignMode)
        return true;
      else
        return base.IsInRole(role);
    }
  }
}
