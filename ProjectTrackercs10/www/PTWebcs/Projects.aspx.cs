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
	public class Projects : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.DataGrid dgProjects;
    protected System.Web.UI.WebControls.LinkButton btnNewProject;
    protected System.Web.UI.WebControls.HyperLink HyperLink2;
  
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
      this.dgProjects.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgProjects_DeleteCommand);
      this.dgProjects.SelectedIndexChanged += new System.EventHandler(this.dgProjects_SelectedIndexChanged);
      this.btnNewProject.Click += new System.EventHandler(this.btnNewProject_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    private void Page_Load(object sender, System.EventArgs e)
    {
      dgProjects.DataSource = ProjectList.GetProjectList();
      DataBind();

      // set security
      btnNewProject.Visible = User.IsInRole("ProjectManager");
      dgProjects.Columns[2].Visible = 
        HttpContext.Current.User.IsInRole("ProjectManager") || 
        HttpContext.Current.User.IsInRole("Administrator");
    }

    private void dgProjects_SelectedIndexChanged(object sender, 
      System.EventArgs e)
    {
      Guid id = new Guid(dgProjects.SelectedItem.Cells[0].Text);
      Session["Project"] = Project.GetProject(id);
      Response.Redirect("ProjectEdit.aspx");
    }

    private void dgProjects_DeleteCommand(object source, 
      System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
      Guid id = new Guid(e.Item.Cells[0].Text);
      Project.DeleteProject(id);

      dgProjects.DataSource = ProjectList.GetProjectList();
      DataBind();
    }

    private void btnNewProject_Click(object sender, 
      System.EventArgs e)
    {
      Session["Project"] = Project.NewProject();
      Response.Redirect("ProjectEdit.aspx");
    }
  }
}
