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
	public class ProjectEdit : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.Button btnCancel;
    protected System.Web.UI.WebControls.Button btnSave;
    protected System.Web.UI.WebControls.LinkButton btnAssignResource;
    protected System.Web.UI.WebControls.DataGrid dgResources;
    protected System.Web.UI.WebControls.TextBox txtDescription;
    protected System.Web.UI.WebControls.CompareValidator CompareValidator2;
    protected System.Web.UI.WebControls.TextBox txtEnded;
    protected System.Web.UI.WebControls.CompareValidator CompareValidator1;
    protected System.Web.UI.WebControls.TextBox txtStarted;
    protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
    protected System.Web.UI.WebControls.TextBox txtName;
    protected System.Web.UI.WebControls.TextBox txtID;
    protected System.Web.UI.WebControls.LinkButton btnNewProject;
    protected System.Web.UI.WebControls.HyperLink HyperLink3;
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
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      this.btnAssignResource.Click += new System.EventHandler(this.btnAssignResource_Click);
      this.dgResources.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgResources_ItemCommand);
      this.dgResources.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgResources_DeleteCommand);
      this.dgResources.SelectedIndexChanged += new System.EventHandler(this.dgResources_SelectedIndexChanged);
      this.btnNewProject.Click += new System.EventHandler(this.btnNewProject_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    // we make this a page-level variable and then
    // always load it in Page_Load. That way the object
    // is available to all our code at all times
    protected Project _project;

    private void Page_Load(object sender, System.EventArgs e)
    {
      // get the existing Project (if any)
      _project = (Project)Session["Project"];

      if(_project == null)
      {
        // got here via either direct nav or back button
        if(txtID.Text.Length == 0)
        {
          // got here via direct nav
          // we are creating a new project
          _project = Project.NewProject();
        }
        else
        {
          // we've returned here via the Browser's back button
          // load object based on ID value
          _project = Project.GetProject(new Guid(txtID.Text));
        }
        Session["Project"] = _project;
      }

      // bind the grid every time (we're not using
      // viewstate to keep it populated)
      dgResources.DataSource = _project.Resources;

      if(IsPostBack)
        dgResources.DataBind();
      else
        DataBind();

      // set security
      if(!User.IsInRole("ProjectManager"))
      {
        btnNewProject.Visible = false;
        btnSave.Visible = false;
        btnAssignResource.Visible = false;
        dgResources.Columns[5].Visible = false;
      }
    }

    private void dgResources_ItemCommand(object source, 
      System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
      if(e.CommandName == "SelectRole")
      {
        string id = e.Item.Cells[0].Text;
        Session["ProjectResource"] = _project.Resources[id];
        Session["Source"] = "ProjectEdit.aspx";
        Response.Redirect("ChooseRole.aspx");
      }
    }

    private void dgResources_DeleteCommand(object source, 
      System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
      string id = e.Item.Cells[0].Text;
      _project.Resources.Remove(id);

      // rebind grid to update display
      dgResources.DataSource = _project.Resources;
      dgResources.DataBind();
    }

    private void btnSave_Click(object sender, System.EventArgs e)
    {
      SaveFormToObject();
      _project = (Project)_project.Save();
      Session.Remove("Project");
      Response.Redirect("Projects.aspx");
    }

    private void dgResources_SelectedIndexChanged(
      object sender, System.EventArgs e)
    {
      // check security
      if(User.IsInRole("ProjectManager"))
      {
        // only do save if user is in a valid role
        SaveFormToObject();
        _project = (Project)_project.Save();
      }

      Session.Remove("Project");

      string id = dgResources.SelectedItem.Cells[0].Text;
      Session["Resource"] = Resource.GetResource(id);
      Response.Redirect("ResourceEdit.aspx");
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      Session.Remove("Project");
      Response.Redirect("Projects.aspx");
    }

    private void btnAssignResource_Click(object sender, System.EventArgs e)
    {
      SaveFormToObject();
      Response.Redirect("AssignResource.aspx");
    }

    private void SaveFormToObject()
    {
      _project.Name = txtName.Text;
      _project.Started = txtStarted.Text;
      _project.Ended = txtEnded.Text;
      _project.Description = txtDescription.Text;
    }

    private void btnNewProject_Click(object sender, System.EventArgs e)
    {
      Session["Project"] = Project.NewProject();
      Response.Redirect("ProjectEdit.aspx");
    }
  }
}
