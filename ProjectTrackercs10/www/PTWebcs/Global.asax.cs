using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Threading;
using System.Security.Principal;

namespace PTWebcs 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		public Global()
		{
			InitializeComponent();
		}	
		
    protected void Global_AcquireRequestState(Object sender, EventArgs e)
    {
      // set the security principal to our BusinessPrincipal
      if(Session["CSLA-Principal"] != null)
      {
        Thread.CurrentPrincipal = (IPrincipal)Session["CSLA-Principal"];
        HttpContext.Current.User = Thread.CurrentPrincipal;
      }
      else
      {
        if(Thread.CurrentPrincipal.Identity.IsAuthenticated)
        {
          System.Web.Security.FormsAuthentication.SignOut();
          Server.Transfer("Login.aspx");
        }
      }
    }
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
      // 
      // Global
      // 
      this.AcquireRequestState += new System.EventHandler(this.Global_AcquireRequestState);

    }
		#endregion

	}
}

