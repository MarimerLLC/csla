using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PTWcfClient
{
  public partial class Form3 : Form
  {
    public Form3()
    {
      InitializeComponent();
    }

    public Form3(Guid projectId)
      : this()
    {
      _projectId = projectId;
    }

    private Guid _projectId;

    private void Form3_Load(object sender, EventArgs e)
    {
      PTWcfService.ProjectData project = null;

      if (Guid.Empty.Equals(_projectId))
      {
        project = new PTWcfClient.PTWcfService.ProjectData();
      }
      else
      {
        var svc = new PTWcfClient.PTWcfService.PTServiceClient("WSHttpBinding_IPTService");
        try
        {
          svc.ClientCredentials.UserName.UserName = "pm";
          svc.ClientCredentials.UserName.Password = "pm";
          var request = new PTWcfService.ProjectRequest();
          request.Id = _projectId;
          project = svc.GetProject(request);
        }
        catch (Exception ex)
        {
          MessageBox.Show(
            ex.ToString(), "Service error",
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        finally
        {
          svc.Close();
        }
      }

      this.projectDataBindingSource.DataSource = project;
    }

    private void projectDataBindingNavigatorSaveItem_Click(object sender, EventArgs e)
    {
      var project = this.projectDataBindingSource.DataSource as PTWcfService.ProjectData;
      if (project != null)
      {
        var svc = new PTWcfClient.PTWcfService.PTServiceClient("WSHttpBinding_IPTService");
        try
        {
          svc.ClientCredentials.UserName.UserName = "pm";
          svc.ClientCredentials.UserName.Password = "pm";
          if (Guid.Empty.Equals(_projectId))
            project = svc.AddProject(
              new PTWcfService.AddProjectRequest() { ProjectData = project});
          else
            project = svc.UpdateProject(
              new PTWcfService.UpdateProjectRequest() { ProjectData = project });
          MessageBox.Show(
            string.Format("Project updated ({0})",project.Id), "Service success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
          this.Close();
        }
        catch (Exception ex)
        {
          MessageBox.Show(
            ex.ToString(), "Service error", 
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        finally
        {
          svc.Close();
        }
      }
    }
  }
}
