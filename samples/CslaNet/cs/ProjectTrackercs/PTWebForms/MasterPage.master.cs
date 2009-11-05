using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PTWebForms
{
  public partial class MasterPage : System.Web.UI.MasterPage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      PageTitle.Text = Page.Title;
    }

    protected void LoginStatus1_LoggingOut(
      object sender, LoginCancelEventArgs e)
    {
      ProjectTracker.Library.Security.PTPrincipal.Logout();
      Session["CslaPrincipal"] =
        Csla.ApplicationContext.User;
      System.Web.Security.FormsAuthentication.SignOut();
    }
  }
}
