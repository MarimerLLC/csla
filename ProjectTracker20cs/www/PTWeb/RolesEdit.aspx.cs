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

public partial class RolesEdit : System.Web.UI.Page
{
  private enum Views
  {
    MainView = 0,
    InsertView = 1
  }


  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
      ApplyAuthorizationRules();
  }

  private void ApplyAuthorizationRules()
  {
    this.GridView1.Columns[this.GridView1.Columns.Count - 1].Visible = ProjectTracker.Library.Admin.Roles.CanEditObject();
    this.AddRoleButton.Visible = ProjectTracker.Library.Admin.Roles.CanAddObject();
  }

  protected void AddRoleButton_Click(object sender, EventArgs e)
  {
    this.DetailsView1.DefaultMode = DetailsViewMode.Insert;
    MultiView1.ActiveViewIndex = (int)Views.InsertView;
  }

  protected void DetailsView1_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
  {
    MultiView1.ActiveViewIndex = (int)Views.MainView;
    this.GridView1.DataBind();
  }

  protected void DetailsView1_ModeChanged(object sender, EventArgs e)
  {
    MultiView1.ActiveViewIndex = (int)Views.MainView;
  }

  #region RolesDataSource

  protected void RolesDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    ProjectTracker.Library.Admin.Roles obj = (ProjectTracker.Library.Admin.Roles)Session["currentObject"];
    int id = (int)e.Keys["Id"];
    obj.Remove(id);
    Session["currentObject"] = obj.Save();
  }

  protected void RolesDataSource_InsertObject(object sender, Csla.Web.InsertObjectArgs e)
  {
    ProjectTracker.Library.Admin.Roles obj = (ProjectTracker.Library.Admin.Roles)Session["currentObject"];
    ProjectTracker.Library.Admin.Role role = obj.AddNew();
    Csla.Data.DataMapper.Map(e.Values, role);
    Session["currentObject"] = obj.Save();
  }

  protected void RolesDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    ProjectTracker.Library.Admin.Roles obj;
    obj = ProjectTracker.Library.Admin.Roles.GetRoles();
    Session["currentObject"] = obj;
    e.BusinessObject = obj;
  }

  protected void RolesDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
  {
    ProjectTracker.Library.Admin.Roles obj = (ProjectTracker.Library.Admin.Roles)Session["currentObject"];
    ProjectTracker.Library.Admin.Role role = obj.GetRoleById(int.Parse(e.Keys["Id"].ToString()));
    role.Name = e.Values["Name"].ToString();
    Session["currentObject"] = obj.Save();
  }

  #endregion

}
