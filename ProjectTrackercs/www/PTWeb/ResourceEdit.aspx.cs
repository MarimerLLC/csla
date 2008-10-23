using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ProjectTracker.Library;
using Csla.Security;

public partial class ResourceEdit : System.Web.UI.Page
{

  private enum Views
  {
    MainView = 0,
    AssignView = 1
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      Session["currentObject"] = null;
      ApplyAuthorizationRules();
    }
    else
      this.ErrorLabel.Text = "";
  }

  private void ApplyAuthorizationRules()
  {
    Resource obj = GetResource();
    var canEdit = AuthorizationRules.CanEditObject(typeof(Resource));
    // Resource display
    if (canEdit)
    {
      if (obj.IsNew)
        this.DetailsView1.DefaultMode = DetailsViewMode.Insert;
      else
        this.DetailsView1.DefaultMode = DetailsViewMode.Edit;
      this.AssignProjectButton.Visible = !obj.IsNew;
    }
    else
    {
      this.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly;
      this.AssignProjectButton.Visible = false;
    }
    this.DetailsView1.Rows[this.DetailsView1.Rows.Count - 1].Visible = canEdit;

    // resources display
    this.GridView1.Columns[this.GridView1.Columns.Count - 1].Visible = canEdit;
  }

  #region DetailsView

  protected void DetailsView1_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
  {
    Resource resource = GetResource();
    if (!resource.IsNew)
      Response.Redirect("ResourceEdit.aspx?id=" + resource.Id.ToString());
  }

  protected void DetailsView1_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
  {
    ApplyAuthorizationRules();
  }

  protected void DetailsView1_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
  {
    Response.Redirect("ResourceList.aspx");
  }

  #endregion

  #region Project Grid

  protected void AssignProjectButton_Click(object sender, EventArgs e)
  {
    this.MultiView1.ActiveViewIndex = (int)Views.AssignView;
  }

  protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
  {
    Resource obj = GetResource();
    try
    {
      obj.Assignments.AssignTo(new Guid(this.GridView2.SelectedDataKey.Value.ToString()));
      if (SaveResource(obj) > 0)
      {
        this.GridView1.DataBind();
        this.MultiView1.ActiveViewIndex = (int)Views.MainView;
      }
    }
    catch (InvalidOperationException ex)
    {
      ErrorLabel.Text = ex.Message;
    }
  }

  protected void CancelAssignButton_Click(object sender, EventArgs e)
  {
    this.MultiView1.ActiveViewIndex = (int)Views.MainView;
  }

  #endregion

  #region RoleListDataSource

  protected void RoleListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = RoleList.GetList();
  }

  #endregion

  #region ResourceDataSource

  protected void ResourceDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    Resource.DeleteResource(int.Parse(e.Keys["Id"].ToString()));
    Session.Remove("currentObject");
    e.RowsAffected = 1;
  }

  protected void ResourceDataSource_InsertObject(object sender, Csla.Web.InsertObjectArgs e)
  {
    Resource obj = Resource.NewResource();
    Csla.Data.DataMapper.Map(e.Values, obj, "Id");
    e.RowsAffected = SaveResource(obj);
  }

  protected void ResourceDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = GetResource();
  }

  protected void ResourceDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    Resource obj = GetResource();
    Csla.Data.DataMapper.Map(e.Values, obj);
    e.RowsAffected = SaveResource(obj);
  }

  #endregion

  #region AssignmentsDataSource

  protected void AssignmentsDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    Resource obj = GetResource();
    Guid rid = new Guid(e.Keys["ProjectId"].ToString());
    obj.Assignments.Remove(rid);
    e.RowsAffected = SaveResource(obj);
  }

  protected void AssignmentsDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    Resource obj = GetResource();
    e.BusinessObject = obj.Assignments;
  }

  protected void AssignmentsDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    Resource obj = GetResource();
    ResourceAssignment res;
    Guid rid = new Guid(e.Keys["ProjectId"].ToString());
    res = obj.Assignments[rid];
    Csla.Data.DataMapper.Map(e.Values, res);
    e.RowsAffected = SaveResource(obj);
  }

  #endregion

  #region ProjectListDataSource
  
  protected void ProjectListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = ProjectTracker.Library.ProjectList.GetProjectList();
  }

  #endregion

  private Resource GetResource()
  {
    object businessObject = Session["currentObject"];
    if (businessObject == null ||
      !(businessObject is Resource))
    {
      try
      {
        string idString = Request.QueryString["id"];
        if (!string.IsNullOrEmpty(idString))
        {
          int id = Int32.Parse(idString);
          businessObject = Resource.GetResource(id);
        }
        else
          businessObject = Resource.NewResource();
        Session["currentObject"] = businessObject;
      }
      catch (System.Security.SecurityException)
      {
        Response.Redirect("ResourceList.aspx");
      }
    }
    return (Resource)businessObject;
  }

  private int SaveResource(Resource resource)
  {
    int rowsAffected;
    try
    {
      Session["currentObject"] = resource.Save();
      rowsAffected = 1;
    }
    catch (Csla.Validation.ValidationException ex)
    {
      System.Text.StringBuilder message = new System.Text.StringBuilder();
      message.AppendFormat("{0}", ex.Message);
      if (resource.BrokenRulesCollection.Count > 0)
      {
        message.Append("<ul>");
        foreach (Csla.Validation.BrokenRule rule in resource.BrokenRulesCollection)
          message.AppendFormat("<li>{0}: {1}</li>", rule.Property, rule.Description);
        message.Append("</ul>");
      }
      this.ErrorLabel.Text = message.ToString();
      rowsAffected = 0;
    }
    catch (Csla.DataPortalException ex)
    {
      this.ErrorLabel.Text = ex.BusinessException.Message;
      rowsAffected = 0;
    }
    catch (Exception ex)
    {
      this.ErrorLabel.Text = ex.Message;
      rowsAffected = 0;
    }
    return rowsAffected;
  }

}
