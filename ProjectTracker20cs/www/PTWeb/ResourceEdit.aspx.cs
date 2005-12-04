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

public partial class ResourceEdit : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      string idString = Request.QueryString["id"];
      ProjectTracker.Library.Resource obj;
      if (!string.IsNullOrEmpty(idString))
      {
        obj = ProjectTracker.Library.Resource.GetResource(idString);
        if (ProjectTracker.Library.Resource.CanSaveObject())
          this.DetailsView1.DefaultMode = DetailsViewMode.Edit;
        else
          this.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly;
      }
      else
      {
        obj = ProjectTracker.Library.Resource.NewResource("new");
        this.DetailsView1.DefaultMode = DetailsViewMode.Insert;
      }
      Session["currentObject"] = obj;
    }
  }
  protected void ResourceListButton_Click(object sender, EventArgs e)
  {
    Response.Redirect("ResourceList.aspx");
  }
  protected void DetailsView1_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
  {
    Response.Redirect("ResourceList.aspx");
  }

  protected void RoleListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = ProjectTracker.Library.RoleList.GetList();
  }

  #region ResourceDataSource

  protected void ResourceDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    ProjectTracker.Library.Resource.DeleteResource(e.Keys["Id"].ToString());
    Session.Remove("currentObject");
    e.RowsAffected = 1;
  }

  protected void ResourceDataSource_InsertObject(object sender, Csla.Web.InsertObjectArgs e)
  {
    ProjectTracker.Library.Resource obj = (ProjectTracker.Library.Resource)Session["currentObject"];
    Csla.Data.DataMapper.Map(e.Values, obj, "Id");
    Session["currentObject"] = obj.Save();
    e.RowsAffected = 1;
  }

  protected void ResourceDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = Session["currentObject"];
  }

  protected void ResourceDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    ProjectTracker.Library.Resource obj = (ProjectTracker.Library.Resource)Session["currentObject"];
    Csla.Data.DataMapper.Map(e.Values, obj);
    Session["currentObject"] = obj.Save();
    e.RowsAffected = 1;
  }

  #endregion

  #region AssignmentsDataSource

  protected void AssignmentsDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    ProjectTracker.Library.Resource obj = (ProjectTracker.Library.Resource)Session["currentObject"];
    ProjectTracker.Library.ResourceAssignment res;
    Guid rid = new Guid(e.Keys["ProjectId"].ToString());
    res = obj.Assignments[rid];
    obj.Assignments.Remove(res.ProjectID);
    Session["currentObject"] = obj.Save();
    e.RowsAffected = 1;
  }

  protected void AssignmentsDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    ProjectTracker.Library.Resource obj = (ProjectTracker.Library.Resource)Session["currentObject"];
    e.BusinessObject = obj.Assignments;
  }

  protected void AssignmentsDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    ProjectTracker.Library.Resource obj = (ProjectTracker.Library.Resource)Session["currentObject"];
    ProjectTracker.Library.ResourceAssignment res;
    Guid rid = new Guid(e.Keys["ProjectId"].ToString());
    res = obj.Assignments[rid];
    Csla.Data.DataMapper.Map(e.Values, res);
    Session["currentObject"] = obj.Save();
    e.RowsAffected = 1;
  }

  #endregion

}
