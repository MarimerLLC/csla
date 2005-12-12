using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    PageTitle.Text = Page.Title;
  }
  protected void LoginStatus1_LoggingOut(object sender, LoginCancelEventArgs e)
  {
    ProjectTracker.Library.Security.PTPrincipal.Logout();
    Session["CslaPrincipal"] = System.Threading.Thread.CurrentPrincipal;
    System.Web.Security.FormsAuthentication.SignOut();
  }
}
