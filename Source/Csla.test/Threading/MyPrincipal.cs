using System;
using System.Linq;
#if SILVERLIGHT
using Csla.Serialization;
#endif

namespace Csla.Test.Threading
{
  [Serializable]
  public class MyPrincipal : Csla.Security.CslaPrincipal
  {
    private string[] Roles { get; set; }

    public MyPrincipal()
      : base(new MyIdentity("Demo"))
    {
      this.Roles = new string[] { "Admin", "Tester" };
    }

    public override bool IsInRole(string role)
    {
      return Roles.Contains(role);
    }
  }
}
