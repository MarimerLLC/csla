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
      try
      {
        string idString = Request.QueryString["id"];
        ProjectTracker.Library.Project obj;
        if (!string.IsNullOrEmpty(idString))
        {
          Guid id = new Guid(idString);
          obj = Project.GetProject(id);
        }
        else
          obj = Project.NewProject();
        Session["currentObject"] = obj;
        this.MultiView1.ActiveViewIndex = (int)Views.MainView;
        ApplyAuthorizationRules();
      }
      catch (System.Security.SecurityException)
      {
        Response.Redirect("ProjectList.aspx");
      }
    }
    else
    {
      this.ErrorLabel.Text = "";
    }
  }

  private void ApplyAuthorizationRules()
  {
    Project obj = (Project)Session["currentObject"];
    // project display
    if (Project.CanSaveObject())
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
    this.DetailsView1.Rows[this.DetailsView1.Rows.Count - 1].Visible = Project.CanSaveObject();

    // resource display
    this.GridView1.Columns[this.GridView1.Columns.Count - 1].Visible = Project.CanSaveObject();
  }

  #region Project DetailsView

  protected void DetailsView1_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
  {
    ApplyAuthorizationRules();
  }

  protected void DetailsView1_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
  {
    ApplyAuthorizationRules();
  }

  protected void DetailsView1_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
  {
    Response.Redirect("ProjectList.aspx");
  }

  #endregion

  #region Resource Grid

  protected void AddResourceButton_Click(object sender, EventArgs e)
  {
    this.MultiView1.ActiveViewIndex = (int)Views.AssignView;
  }

  protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
  {
    Project obj = (Project)Session["currentObject"];
    try
    {
      obj.Resources.Assign(int.Parse(this.GridView2.SelectedDataKey.Value.ToString()));
      if (SaveProject(obj) > 0)
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

  #region ProjectDataSource

  protected void ProjectDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    try
    {
      Project.DeleteProject(new Guid(e.Keys["id"].ToString()));
      Session["currentObject"] = null;
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

  protected void ProjectDataSource_InsertObject(object sender, Csla.Web.InsertObjectArgs e)
  {
    Project obj = (Project)Session["currentObject"];
    Csla.Data.DataMapper.Map(e.Values, obj, "Id");
    e.RowsAffected = SaveProject(obj);
  }

  protected void ProjectDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = Session["currentObject"];
  }

  protected void ProjectDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    Project obj = (Project)Session["currentObject"];
    Csla.Data.DataMapper.Map(e.Values, obj);
    e.RowsAffected = SaveProject(obj);
  }

  #endregion

  #region ResourcesDataSource

  protected void ResourcesDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    Project obj = (Project)Session["currentObject"];
    ProjectResource res;
    int rid = int.Parse(e.Keys["ResourceId"].ToString());
    res = obj.Resources[rid];
    obj.Resources.Remove(res.ResourceId);
    e.RowsAffected = SaveProject(obj);
  }

  protected void ResourcesDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    Project obj = (Project)Session["currentObject"];
    e.BusinessObject = obj.Resources;
  }

  protected void ResourcesDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    Project obj = (Project)Session["currentObject"];
    ProjectResource res;
    int rid = int.Parse(e.Keys["ResourceId"].ToString());
    res = obj.Resources[rid];
    Csla.Data.DataMapper.Map(e.Values, res);
    e.RowsAffected = SaveProject(obj);
  }

  #endregion

  #region ResourceListDataSource
  
  protected void ResourceListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = ProjectTracker.Library.ResourceList.GetResourceList();
  }

  #endregion

  #region RoleListDataSource

  protected void RoleListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = RoleList.GetList();
  }

  #endregion

  private int SaveProject(Project project)
  {
    int rowsAffected;
    try
    {
      Session["currentObject"] = project.Save();
      rowsAffected = 1;
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

