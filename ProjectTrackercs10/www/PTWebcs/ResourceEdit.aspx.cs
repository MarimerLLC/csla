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
	/// <summary>
	/// Summary description for ResourceEdit.
	/// </summary>
	public class ResourceEdit : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.Button btnCancel;
    protected System.Web.UI.WebControls.Button btnSave;
    protected System.Web.UI.WebControls.LinkButton btnAssign;
    protected System.Web.UI.WebControls.DataGrid dgProjects;
    protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator2;
    protected System.Web.UI.WebControls.TextBox txtLastname;
    protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
    protected System.Web.UI.WebControls.TextBox txtFirstname;
    protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator3;
    protected System.Web.UI.WebControls.TextBox txtID;
    protected System.Web.UI.WebControls.LinkButton btnNewResource;
    protected System.Web.UI.WebControls.HyperLink HyperLink2;
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
      this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
      this.dgProjects.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgProjects_ItemCommand);
      this.dgProjects.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgProjects_DeleteCommand);
      this.dgProjects.SelectedIndexChanged += new System.EventHandler(this.dgProjects_SelectedIndexChanged);
      this.btnNewResource.Click += new System.EventHandler(this.btnNewResource_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    // we make this a page-level variable and then
    // always load it in Page_Load. That way the object
    // is available to all our code at all times
    protected Resource _resource;

    private void Page_Load(object sender, System.EventArgs e)
    {
      // get the existing Resource (if any)
      _resource = (Resource)Session["Resource"];

      if(_resource == null)
      {
        // either we're adding a new object or the user hit the back button
        if(txtID.Text.Length == 0)
        {
          // we are adding a new resource
          txtID.ReadOnly = false;
          btnAssign.Visible = false;
        }
        else
        {
          if(txtID.ReadOnly)
          {
            // we've returned here via the Browser's back button
            // load object based on ID value
            _resource = Resource.GetResource(txtID.Text);
            Session["Resource"] = _resource;
          }
          else
          {
            // we are adding a new resource
            _resource = Resource.NewResource(txtID.Text);
            Session["Resource"] = _resource;
            txtID.ReadOnly = true;
            btnAssign.Visible = true;
          }
        }
      }

      if(_resource != null)
      {
        // we have a resource to which we can bind
        dgProjects.DataSource = _resource.Assignments;

        if(IsPostBack)
          dgProjects.DataBind();
        else
          DataBind();
      }

      // set security
      if(!User.IsInRole("Supervisor") &&
        !User.IsInRole("ProjectManager"))
      {
        btnNewResource.Visible = false;
        btnSave.Visible = false;
        btnAssign.Visible = false;
        dgProjects.Columns[4].Visible = false;
      }
    }

    private void dgProjects_DeleteCommand(object source, 
      System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
      string id = e.Item.Cells[0].Text;
      _resource.Assignments.Remove(new Guid(id));

      // rebind grid to update display
      dgProjects.DataSource = _resource.Assignments;
      dgProjects.DataBind();
    }

    private void btnSave_Click(object sender, System.EventArgs e)
    {
      SaveFormToObject();

      if(_resource.IsNew)
      {
        _resource = (Resource)_resource.Save();
        Session["Resource"] = _resource;
        Response.Redirect("ResourceEdit.aspx");
      }
      else
      {
        _resource.Save();
        Session.Remove("Resource");
        Response.Redirect("Resources.aspx");
      }
    }

    private void dgProjects_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      // check security
      if(User.IsInRole("Supervisor") ||
        User.IsInRole("ProjectManager"))
      {
        // only do save if user is in a valid role
        SaveFormToObject();
        _resource = (Resource)_resource.Save();
      }

      Session.Remove("Resource");

      Guid id = new Guid(dgProjects.SelectedItem.Cells[0].Text);
      Session["Project"] = Project.GetProject(id);
      Response.Redirect("ProjectEdit.aspx");
    }

    private void dgProjects_ItemCommand(object source, 
      System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
      if(e.CommandName == "SelectRole")
      {
        Guid id = new Guid(e.Item.Cells[0].Text);
        Session["ResourceAssignment"] = _resource.Assignments[id];
        Session["Source"] = "ResourceEdit.aspx";
        Response.Redirect("ChooseRole.aspx");
      }
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      Session.Remove("Resource");
      Response.Redirect("Resources.aspx");
    }

    private void btnAssign_Click(object sender, System.EventArgs e)
    {
      SaveFormToObject();
      Response.Redirect("AssignToProject.aspx");
    }

    private void SaveFormToObject()
    {
      _resource.FirstName = txtFirstname.Text;
      _resource.LastName = txtLastname.Text;
    }

    private void btnNewResource_Click(object sender, System.EventArgs e)
    {
      Session.Remove("Resource");
      Response.Redirect("ResourceEdit.aspx");
    }
  }
}
