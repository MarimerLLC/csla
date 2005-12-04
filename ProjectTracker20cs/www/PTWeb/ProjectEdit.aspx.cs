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

public partial class ProjectEdit : System.Web.UI.Page
{
  bool _objectLoaded;

  void LoadObject()
  {
    if (_objectLoaded) return;

    string idString = Request.QueryString["id"];
    ProjectTracker.Library.Project obj;
    if (!string.IsNullOrEmpty(idString))
    {
      Guid id = new Guid(idString);
      obj = ProjectTracker.Library.Project.GetProject(id);
      if (ProjectTracker.Library.Project.CanSaveObject())
        this.DetailsView1.DefaultMode = DetailsViewMode.Edit;
      else
        this.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly;
    }
    else
    {
      obj = ProjectTracker.Library.Project.NewProject();
      this.DetailsView1.DefaultMode = DetailsViewMode.Insert;
    }
    Session["currentObject"] = obj;
  }

  protected void DetailsView1_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
  {
    Response.Redirect("ProjectList.aspx");
  }

  protected void RoleListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = ProjectTracker.Library.RoleList.GetList();
  }

  protected void ProjectListButton_Click(object sender, EventArgs e)
  {
    Response.Redirect("ProjectList.aspx");
  }

  #region ProjectDataSource

  protected void ProjectDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    ProjectTracker.Library.Project.DeleteProject(new Guid(e.Keys["id"].ToString()));
    Session["currentObject"] = null;
    e.RowsAffected = 1;
  }

  protected void ProjectDataSource_InsertObject(object sender, Csla.Web.InsertObjectArgs e)
  {
    ProjectTracker.Library.Project obj = (ProjectTracker.Library.Project)Session["currentObject"];
    Csla.Data.DataMapper.Map(e.Values, obj, "Id");
    Session["currentObject"] = obj.Save();
    e.RowsAffected = 1;
  }

  protected void ProjectDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    LoadObject();
    e.BusinessObject = Session["currentObject"];
  }

  protected void ProjectDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    ProjectTracker.Library.Project obj = (ProjectTracker.Library.Project)Session["currentObject"];
    Csla.Data.DataMapper.Map(e.Values, obj);
    Session["currentObject"] = obj.Save();
    e.RowsAffected = 1;
  }

  #endregion

  #region ResourcesDataSource

  protected void ResourcesDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    ProjectTracker.Library.Project obj = (ProjectTracker.Library.Project)Session["currentObject"];
    ProjectTracker.Library.ProjectResource res;
    string rid = (string)e.Keys["ResourceId"];
    res = obj.Resources[rid];
    obj.Resources.Remove(res.ResourceId);
    Session["currentObject"] = obj.Save(true);
    e.RowsAffected = 1;
  }

  protected void ResourcesDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    LoadObject();
    ProjectTracker.Library.Project obj = (ProjectTracker.Library.Project)Session["currentObject"];
    e.BusinessObject = obj.Resources;
  }

  protected void ResourcesDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    ProjectTracker.Library.Project obj = (ProjectTracker.Library.Project)Session["currentObject"];
    ProjectTracker.Library.ProjectResource res;
    string rid = (string)e.Keys["ResourceId"];
    res = obj.Resources[rid];
    Csla.Data.DataMapper.Map(e.Values, res);
    Session["currentObject"] = obj.Save();
    e.RowsAffected = 1;
  }

  #endregion
}
