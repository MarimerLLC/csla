using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CSLA.Security;
using System.Threading;
using System.Security.Principal;

namespace PTWebcs
{
  public class Login : System.Web.UI.Page
  {
    protected System.Web.UI.WebControls.Button btnLogin;
    protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator2;
    protected System.Web.UI.WebControls.TextBox txtPassword;
    protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
    protected System.Web.UI.WebControls.TextBox txtUsername;
  
		#region Web Form Designer generated code
    override protected void OnInit(EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit(e);
    }
		
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {    
      this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    private void Page_Load(object sender, System.EventArgs e)
    {
    
    }

    private void btnLogin_Click(object sender, System.EventArgs e)
    {
      string userName = txtUsername.Text;
      string password = txtPassword.Text;

      // if we're logging in, clear the current session
      Session.Clear();

      // log into the system
      BusinessPrincipal.Login(userName, password);

      // see if we logged in successfully 
      if(Thread.CurrentPrincipal.Identity.IsAuthenticated)
      {
        Session["CSLA-Principal"] = Thread.CurrentPrincipal;
        HttpContext.Current.User = Thread.CurrentPrincipal;

        // redirect to the page the user requested
        System.Web.Security.FormsAuthentication.RedirectFromLoginPage(
          userName, false);
      }
    }

  }
}
