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
	public class AssignResource : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.Button btnCancel;
    protected System.Web.UI.WebControls.DataGrid dgResources;
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
      this.dgResources.SelectedIndexChanged += new System.EventHandler(this.dgResources_SelectedIndexChanged);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion
    
    private void Page_Load(object sender, System.EventArgs e)
    {
      // check security
      if(!User.IsInRole("ProjectManager"))
      {
        // they should not be here
        Response.Redirect("ProjectEdit.aspx");
      }

      if(!Page.IsPostBack)
      {
        foreach(string role in Assignment.Roles)
          lstRoles.Items.Add(Assignment.Roles[role]);
        if(lstRoles.Items.Count > 0) lstRoles.SelectedIndex = 0;
      }

      dgResources.DataSource = ResourceList.GetResourceList();
      dgResources.DataBind();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      Response.Redirect("ProjectEdit.aspx");
    }

    private void dgResources_SelectedIndexChanged(
      object sender, System.EventArgs e)
    {
      Project project = (Project)Session["Project"];
      string id = dgResources.SelectedItem.Cells[0].Text;

      // TODO: this line only works in 1.1, so is replaced with next line for 1.0
      project.Resources.Assign(id, lstRoles.SelectedValue);
      //project.Resources.Assign(id, lstRoles.SelectedItem.Value);

      Response.Redirect("ProjectEdit.aspx");
    }
  }
}
