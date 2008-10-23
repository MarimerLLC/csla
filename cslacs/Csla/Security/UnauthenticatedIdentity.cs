using System;
using System.Security.Principal;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.Core;

namespace Csla.Security
{
  [Serializable()]
  public sealed class UnauthenticatedIdentity : CslaIdentity
  {
    public UnauthenticatedIdentity()
    {
      IsAuthenticated = false;
      Name = string.Empty;
      AuthenticationType = string.Empty;
      Roles = new MobileList<string>();
    }
  }
}
