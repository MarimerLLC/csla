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
using ProjectTracker.Library;

namespace PTWebcs
{
	public class AssignToProject : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.Button btnCancel;
    protected System.Web.UI.WebControls.DataGrid dgProjects;
    protected System.Web.UI.WebControls.ListBox lstRoles;

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
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      this.dgProjects.SelectedIndexChanged += new System.EventHandler(this.dgProjects_SelectedIndexChanged);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion
  
    private void Page_Load(object sender, System.EventArgs e)
    {
      // check security
      if(!User.IsInRole("Supervisor") &&
        !User.IsInRole("ProjectManager"))
      {
        // they should not be here
        Response.Redirect("ResourceEdit.aspx");
      }

      if(!Page.IsPostBack)
      {
        foreach(string role in Assignment.Roles)
          lstRoles.Items.Add(Assignment.Roles[role]);

        // set the default role to the first in the list
        if(lstRoles.Items.Count > 0) lstRoles.SelectedIndex = 0;
      }

      dgProjects.DataSource = ProjectList.GetProjectList();
      dgProjects.DataBind();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      Response.Redirect("ResourceEdit.aspx");
    }

    private void dgProjects_SelectedIndexChanged(
      object sender, System.EventArgs e)
    {
      Resource resource = (Resource)Session["Resource"];
      Guid id = new Guid(dgProjects.SelectedItem.Cells[0].Text);

      // TODO: this line only works in 1.1, so is replaced with next line for 1.0
      //resource.Assignments.AssignTo(id, lstRoles.SelectedValue)
      resource.Assignments.AssignTo(id, lstRoles.SelectedItem.Value);

      Response.Redirect("ResourceEdit.aspx");
    }
  }
}
