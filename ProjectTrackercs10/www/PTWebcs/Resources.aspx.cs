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
	public class Resources : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.DataGrid dgResources;
    protected System.Web.UI.WebControls.LinkButton btnNewResource;
    protected System.Web.UI.WebControls.HyperLink HyperLink1;

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
      this.dgResources.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgResources_DeleteCommand);
      this.dgResources.SelectedIndexChanged += new System.EventHandler(this.dgResources_SelectedIndexChanged);
      this.btnNewResource.Click += new System.EventHandler(this.btnNewResource_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion
  
    private void Page_Load(object sender, System.EventArgs e)
    {
      dgResources.DataSource = ResourceList.GetResourceList();
      DataBind();

      // set security
      btnNewResource.Visible = 
        User.IsInRole("Supervisor") ||
        User.IsInRole("ProjectManager");
      dgResources.Columns[2].Visible = 
        User.IsInRole("Supervisor") ||
        User.IsInRole("ProjectManager") ||
        User.IsInRole("Administrator");
    }

    private void dgResources_SelectedIndexChanged(
      object sender, System.EventArgs e)
    {
      string id = dgResources.SelectedItem.Cells[0].Text;
      Session["Resource"] = Resource.GetResource(id);
      Response.Redirect("ResourceEdit.aspx");
    }

    private void dgResources_DeleteCommand(object source, 
      System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
      string id = e.Item.Cells[0].Text;
      Resource.DeleteResource(id);

      dgResources.DataSource = ResourceList.GetResourceList();
      DataBind();
    }

    private void btnNewResource_Click(object sender, System.EventArgs e)
    {
      // make sure there's no active resource so ResourceEdit knows
      // to add a new object
      Session.Remove("Resource");
      Response.Redirect("ResourceEdit.aspx");
    }
  }
}
