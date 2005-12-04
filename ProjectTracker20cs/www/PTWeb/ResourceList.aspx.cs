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

public partial class ResourceList : System.Web.UI.Page
{
  protected void NewResourceButton_Click(object sender, EventArgs e)
  {
    Response.Redirect("ResourceEdit.aspx");
  }
  protected void ResourceListDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    ProjectTracker.Library.Resource.DeleteResource(e.Keys["Id"].ToString());
  }
  protected void ResourceListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = ProjectTracker.Library.ResourceList.GetResourceList();
  }
  protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
  {
    string idString = GridView1.SelectedDataKey.Value.ToString();
    Response.Redirect("ResourceEdit.aspx?id=" + idString);
  }
  protected void Page_Load(object sender, EventArgs e)
  {
    NewResourceButton.Visible = ProjectTracker.Library.Resource.CanAddObject();
  }
}
