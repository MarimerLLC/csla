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
  public class ChooseRole : System.Web.UI.Page
  {
    protected System.Web.UI.WebControls.Button btnCancel;
    protected System.Web.UI.WebControls.Button btnUpdate;
    protected System.Web.UI.WebControls.ListBox lstRoles;
    protected System.Web.UI.WebControls.Label lblValue;
    protected System.Web.UI.WebControls.Label lblLabel;

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
      this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion
  
    private void Page_Load(object sender, System.EventArgs e)
    {
      if(!Page.IsPostBack)
      {
        foreach(string role in Assignment.Roles)
          lstRoles.Items.Add(Assignment.Roles[role]);

        if((string)Session["Source"] == "ProjectEdit.aspx")
        {
          // we are dealing with a ProjectResource

          // check security
          if(!User.IsInRole("ProjectManager"))
          {
            // they should not be here
            SendUserBack();
          }

          ProjectResource obj = 
            (ProjectResource)Session["ProjectResource"];
          lblLabel.Text = "Resource";
          lblValue.Text = obj.FirstName + " " + obj.LastName;

          // TODO: this line only works in 1.1, so is replaced with next line for 1.0
          lstRoles.SelectedValue = obj.Role;
          //SelectItem(lstRoles, obj.Role);
        }
        else
        {
          if((string)Session["Source"] == "ResourceEdit.aspx")
          {
            // we are dealing with a ResourceAssignment

            // check security
            if(!User.IsInRole("Supervisor") &&
              !User.IsInRole("ProjectManager"))
            {
              // they should not be here
              SendUserBack();
            }

  	    ResourceAssignment obj = 
              (ResourceAssignment)Session["ResourceAssignment"];
            lblLabel.Text = "Project";
            lblValue.Text = obj.ProjectName;

            // TODO: this line only works in 1.1, so is replaced with next line for 1.0
            lstRoles.SelectedValue = obj.Role;
            //SelectItem(lstRoles, obj.Role);
          }
          else
	  {
            // we came from an invalid location
	    Response.Redirect("Default.aspx");
	  }
        }
      }
    }

    private void btnUpdate_Click(object sender, System.EventArgs e)
    {
      if((string)Session["Source"] == "ProjectEdit.aspx")
      {
        // we are dealing with a ProjectResource
        ProjectResource obj = (ProjectResource)Session["ProjectResource"];

        // TODO: this line only works in 1.1, so is replaced with next line for 1.0
        obj.Role = lstRoles.SelectedValue;
        //obj.Role = lstRoles.SelectedItem.Value;
      }
      else
      {
        // we are dealing with a ResourceAssignment
        ResourceAssignment obj = 
          (ResourceAssignment)Session["ResourceAssignment"];

        // TODO: this line only works in 1.1, so is replaced with next line for 1.0
        obj.Role = lstRoles.SelectedValue;
        //obj.Role = lstRoles.SelectedItem.Value;
      }
      SendUserBack();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      SendUserBack();
    }

    private void SendUserBack()
    {
      string src = (string)Session["Source"];
      Session.Remove("Source");
      Response.Redirect(src);
    }

//    TODO: uncomment this method for .NET 1.0
//    private void SelectItem(System.Web.UI.WebControls.ListBox lst, string item)
//    {
//      int index = 0;
//
//      foreach(ListItem entry in lst.Items)
//      {
//        if(entry.Value == item)
//        {
//          lst.SelectedIndex = index;
//          return;
//        }
//        index++;
//      }
//    }
  }
}
