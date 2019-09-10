using System;
using System.Linq;

namespace BackgroundWorkerDemo.Security
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
