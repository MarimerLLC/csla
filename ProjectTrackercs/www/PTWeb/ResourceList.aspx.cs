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

public partial class ResourceList : System.Web.UI.Page
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
    this.GridView1.Columns[this.GridView1.Columns.Count - 1].Visible = 
      AuthorizationRules.CanDeleteObject(typeof(Resource));
    NewResourceButton.Visible = AuthorizationRules.CanCreateObject(typeof(Resource));
  }

  #region GridView1

  protected void NewResourceButton_Click(object sender, EventArgs e)
  {
    Response.Redirect("ResourceEdit.aspx");
  }

  protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
  {
    Session["currentObject"] = null;
    GridView1.DataBind();
  }

  #endregion

  #region ResourceListDataSource

  protected void ResourceListDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    try
    {
      Resource.DeleteResource(int.Parse(e.Keys["Id"].ToString()));
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

  protected void ResourceListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = GetResourceList();
  }

  #endregion

  private ProjectTracker.Library.ResourceList GetResourceList()
  {
    object businessObject = Session["currentObject"];
    if (businessObject == null ||
      !(businessObject is ProjectTracker.Library.ResourceList))
    {
      businessObject =
        ProjectTracker.Library.ResourceList.GetResourceList();
      Session["currentObject"] = businessObject;
    }
    return (ProjectTracker.Library.ResourceList)businessObject;
  }
}
