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

public partial class ProjectList : System.Web.UI.Page
{

  protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
  {
    string idString = GridView1.SelectedDataKey.Value.ToString();
    Response.Redirect("ProjectEdit.aspx?id=" + idString);
  }
  protected void NewProjectButton_Click(object sender, EventArgs e)
  {
    Response.Redirect("ProjectEdit.aspx");
  }
  protected void Page_Load(object sender, EventArgs e)
  {
    NewProjectButton.Visible = ProjectTracker.Library.Project.CanAddObject();
  }
  protected void ProjectListDataSource_DeleteObject(object sender, Csla.Web.DeleteObjectArgs e)
  {
    ProjectTracker.Library.Project.DeleteProject(new Guid(e.Keys["Id"].ToString()));
  }
  protected void ProjectListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = ProjectTracker.Library.ProjectList.GetProjectList();
  }
}
