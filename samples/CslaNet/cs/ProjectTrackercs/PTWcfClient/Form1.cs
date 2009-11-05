using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PTWcfClient
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      LoadData();
    }

    private void LoadData()
    {
      PTWcfService.ProjectData[] list = null;

      //System.ServiceModel.ChannelFactory<PTWcfService.IPTService> factory =
      //  new System.ServiceModel.ChannelFactory<PTWcfService.IPTService>("WSHttpBinding_IPTService");
      //try
      //{
      //  factory.Credentials.UserName.UserName = "pm";
      //  factory.Credentials.UserName.Password = "pm";
      //  PTWcfService.IPTService proxy = factory.CreateChannel();
      //  using (proxy as IDisposable)
      //  {
      //    list = proxy.GetProjectList(new PTWcfService.ProjectListRequest());
      //  }
      //}
      //finally
      //{
      //  factory.Close();
      //}

      var svc =
        new PTWcfClient.PTWcfService.PTServiceClient("WSHttpBinding_IPTService");
      try
      {
        svc.ClientCredentials.UserName.UserName = "anonymous";
        //svc.ClientCredentials.UserName.Password = "pm";
        list =
          svc.GetProjectList(new PTWcfClient.PTWcfService.ProjectListRequest());
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Service error");
      }
      finally
      {
        svc.Close();
      }

      this.projectDataBindingSource.DataSource = list;
    }

    private void projectDataDataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      Guid projectId = (Guid)projectDataDataGridView.Rows[e.RowIndex].Cells[0].Value;
      new Form3(projectId).ShowDialog(this);
      LoadData();
    }
  }
}