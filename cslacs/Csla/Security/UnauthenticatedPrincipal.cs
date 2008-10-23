using System;
using System.Security.Principal;
using Csla.Serialization;
namespace Csla.Security
{

  [Serializable()]
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
