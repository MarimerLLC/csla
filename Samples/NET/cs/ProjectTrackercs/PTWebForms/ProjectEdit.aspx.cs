using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjectTracker.Library;

namespace PTWebForms
{
  public partial class ProjectEdit : System.Web.UI.Page
  {
    private enum Views
    {
      MainView = 0,
      AssignView = 1
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      if (!Page.IsPostBack)
      {
        Session["currentObject"] = null;
        ApplyAuthorizationRules();
      }
      else
      {
        ShowError(string.Empty);
      }
    }

    private void ApplyAuthorizationRules()
    {
      Project obj = GetProject();
      // project display
      var canEdit = Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(Project));
      if (canEdit)
      {
        if (obj.IsNew)
          this.DetailsView1.DefaultMode = DetailsViewMode.Insert;
        else
          this.DetailsView1.DefaultMode = DetailsViewMode.Edit;
        this.AddResourceButton.Visible = !obj.IsNew;
      }
      else
      {
        this.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly;
        this.AddResourceButton.Visible = false;
      }
      this.DetailsView1.Rows[
        this.DetailsView1.Rows.Count - 1].Visible = canEdit;

      // resource display
      this.GridView1.Columns[
        this.GridView1.Columns.Count - 1].Visible = canEdit;
    }

    #region Project DetailsView

    protected void DetailsView1_ItemCreated(object sender, EventArgs e)
    {
      if (DetailsView1.DefaultMode == DetailsViewMode.Insert)
      {
        Project obj = GetProject();
        ((TextBox)DetailsView1.Rows[1].Cells[1].Controls[0]).Text = obj.Name;
        ((TextBox)DetailsView1.Rows[2].Cells[1].Controls[0]).Text = obj.Started;
        ((TextBox)DetailsView1.Rows[3].Cells[1].Controls[0]).Text = obj.Ended;
        ((TextBox)DetailsView1.FindControl("TextBox1")).Text = obj.Description;
      }
    }

    protected void DetailsView1_ItemInserted(
      object sender, DetailsViewInsertedEventArgs e)
    {
      Project project = GetProject();
      if (!project.IsNew)
        Response.Redirect("ProjectEdit.aspx?id=" + project.Id.ToString());
    }

    protected void DetailsView1_ItemUpdated(
      object sender, DetailsViewUpdatedEventArgs e)
    {
      ApplyAuthorizationRules();
    }

    protected void DetailsView1_ItemDeleted(
      object sender, DetailsViewDeletedEventArgs e)
    {
      Response.Redirect("ProjectList.aspx");
    }

    #endregion

    #region Resource Grid

    protected void AddResourceButton_Click(
      object sender, EventArgs e)
    {
      this.MultiView1.ActiveViewIndex =
        (int)Views.AssignView;
    }

    protected void GridView2_SelectedIndexChanged(
      object sender, EventArgs e)
    {
      Project obj = GetProject();
      try
      {
        obj.Resources.Assign(int.Parse(
          this.GridView2.SelectedDataKey.Value.ToString()));
        if (SaveProject(obj) > 0)
        {
          this.GridView1.DataBind();
          this.MultiView1.ActiveViewIndex = (int)Views.MainView;
        }
      }
      catch (InvalidOperationException ex)
      {
        ShowError(ex.Message);
      }
    }

    protected void CancelAssignButton_Click(
      object sender, EventArgs e)
    {
      this.MultiView1.ActiveViewIndex =
        (int)Views.MainView;
    }

    #endregion

    #region ProjectDataSource

    protected void ProjectDataSource_DeleteObject(
      object sender, Csla.Web.DeleteObjectArgs e)
    {
      try
      {
        Project.DeleteProject(new Guid(e.Keys["Id"].ToString()));
        Session["currentObject"] = null;
        e.RowsAffected = 1;
      }
      catch (Csla.DataPortalException ex)
      {
        ShowError(ex.BusinessException.Message);
        e.RowsAffected = 0;
      }
      catch (Exception ex)
      {
        ShowError(ex.Message);
        e.RowsAffected = 0;
      }
    }

    protected void ProjectDataSource_InsertObject(
      object sender, Csla.Web.InsertObjectArgs e)
    {
      Project obj = GetProject();
      Csla.Data.DataMapper.Map(e.Values, obj, "Id");
      e.RowsAffected = SaveProject(obj);
    }

    protected void ProjectDataSource_SelectObject(
      object sender, Csla.Web.SelectObjectArgs e)
    {
      e.BusinessObject = GetProject();
    }

    protected void ProjectDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
    {
      Project obj = GetProject();
      Csla.Data.DataMapper.Map(e.Values, obj);
      e.RowsAffected = SaveProject(obj);
    }

    #endregion

    #region ResourcesDataSource

    protected void ResourcesDataSource_DeleteObject(
      object sender, Csla.Web.DeleteObjectArgs e)
    {
      Project obj = GetProject();
      int rid = int.Parse(e.Keys["ResourceId"].ToString());
      obj.Resources.Remove(rid);
      e.RowsAffected = SaveProject(obj);
    }

    protected void ResourcesDataSource_SelectObject(
      object sender, Csla.Web.SelectObjectArgs e)
    {
      Project obj = GetProject();
      e.BusinessObject = obj.Resources;
    }

    protected void ResourcesDataSource_UpdateObject(
      object sender, Csla.Web.UpdateObjectArgs e)
    {
      Project obj = GetProject();
      int rid = int.Parse(e.Keys["ResourceId"].ToString());
      ProjectResource res =
        obj.Resources.Where(r => r.ResourceId == rid).First();
      Csla.Data.DataMapper.Map(e.Values, res);
      e.RowsAffected = SaveProject(obj);
    }

    #endregion

    #region ResourceListDataSource

    protected void ResourceListDataSource_SelectObject(
      object sender, Csla.Web.SelectObjectArgs e)
    {
      e.BusinessObject =
        ProjectTracker.Library.ResourceList.GetResourceList();
    }

    #endregion

    #region RoleListDataSource

    protected void RoleListDataSource_SelectObject(
      object sender, Csla.Web.SelectObjectArgs e)
    {
      e.BusinessObject = RoleList.GetList();
    }

    #endregion

    private Project GetProject()
    {
      object businessObject = Session["currentObject"];
      if (businessObject == null ||
        !(businessObject is Project))
      {
        try
        {
          string idString = Request.QueryString["id"];
          if (!string.IsNullOrEmpty(idString))
          {
            Guid id = new Guid(idString);
            businessObject = Project.GetProject(id);
          }
          else
            businessObject = Project.NewProject();
          Session["currentObject"] = businessObject;
        }
        catch (System.Security.SecurityException)
        {
          Response.Redirect("ProjectList.aspx");
        }
      }
      return (Project)businessObject;
    }

    private int SaveProject(Project project)
    {
      int rowsAffected;
      try
      {
        Session["currentObject"] = project.Save();
        rowsAffected = 1;
      }
      catch (Csla.Rules.ValidationException ex)
      {
        System.Text.StringBuilder message = new System.Text.StringBuilder();
        message.AppendFormat("{0}", ex.Message);
        if (project.BrokenRulesCollection.Count > 0)
        {
          message.Append("<ul>");
          foreach (Csla.Rules.BrokenRule rule in project.BrokenRulesCollection)
            message.AppendFormat("<li>{0}: {1}</li>", rule.Property, rule.Description);
          message.Append("</ul>");
        }
        ShowError(message.ToString());
        rowsAffected = 0;
      }
      catch (Csla.DataPortalException ex)
      {
        ShowError(ex.BusinessException.Message);
        rowsAffected = 0;
      }
      catch (Exception ex)
      {
        ShowError(ex.Message);
        rowsAffected = 0;
      }
      return rowsAffected;
    }

    private void ShowError(string error)
    {
      this.ErrorLabel.Text = error;
      //this.ErrorLabel2.Text = error;
    }
  }
}
