using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Security;

namespace Csla.Validation.Test
{
  [Serializable]
  public class VIdentity : CslaIdentity
  {
    public VIdentity() { }

    public VIdentity(params string[] roles)
    {
      Name = "Test";
      IsAuthenticated = true;
      Roles = new MobileList<string>(roles);
    }
  }
}
