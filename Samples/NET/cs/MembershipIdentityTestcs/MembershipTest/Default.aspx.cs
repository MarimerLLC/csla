using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MembershipTest
{
  public partial class _Default : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      Csla.ApplicationContext.User = 
        MyPrincipal.Login("test", "Test4|ife");

      Response.Write(User.Identity.Name);
      Response.Write("<br>");
      Response.Write(User.Identity.IsAuthenticated.ToString());
      Response.Write("<br>");
      Response.Write(User.IsInRole("Admin").ToString());
      Response.Write("<br>");
    }
  }
}
