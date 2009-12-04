using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjectTracker.Library;

namespace PTWebForms
{
  public partial class ProjectList : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        Session["currentObject"] = null;
        ApplyAuthorizationRules();
      }
      else
        ErrorLabel.Text = string.Empty;
    }

    private void ApplyAuthorizationRules()
    {
      this.GridView1.Columns[
        this.GridView1.Columns.Count - 1].Visible =
        Csla.Security.AuthorizationRules.CanDeleteObject(typeof(Project));
      NewProjectButton.Visible =
        Csla.Security.AuthorizationRules.CanCreateObject(typeof(Project));
    }

    #region GridView1

    protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
      Session["currentObject"] = null;
      GridView1.DataBind();
    }

    protected void NewProjectButton_Click(object sender, EventArgs e)
    {
      // allow user to add a new project
      Response.Redirect("ProjectEdit.aspx");
    }

    #endregion

    #region ProjectListDataSource

    protected void ProjectListDataSource_DeleteObject(
      object sender, Csla.Web.DeleteObjectArgs e)
    {
      try
      {
        ProjectTracker.Library.Project.DeleteProject(
          new Guid(e.Keys["Id"].ToString()));
        e.RowsAffected = 1;
      }
      catch (Csla.DataPortalException ex)
      {
        this.ErrorLabel.Text = ex.BusinessException.Message;
        e.RowsAffected = 0;
      }
      catch (Exception ex)
      {
        this.ErrorLabel.Text = ex.Message;
        e.RowsAffected = 0;
      }
    }

    protected void ProjectListDataSource_SelectObject(
      object sender, Csla.Web.SelectObjectArgs e)
    {
      e.BusinessObject = GetProjectList();
    }

    #endregion

    private ProjectTracker.Library.ProjectList GetProjectList()
    {
      object businessObject = Session["currentObject"];
      if (businessObject == null ||
        !(businessObject is ProjectTracker.Library.ProjectList))
      {
        businessObject =
          ProjectTracker.Library.ProjectList.GetProjectList();
        Session["currentObject"] = businessObject;
      }
      return (ProjectTracker.Library.ProjectList)businessObject;
    }
  }
}