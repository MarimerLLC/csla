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
      System.ServiceModel.ChannelFactory<PTWcfService.IPTService> factory =
        new System.ServiceModel.ChannelFactory<PTWcfService.IPTService>("WSHttpBinding_IPTService");
      factory.Credentials.UserName.UserName = "pm";
      factory.Credentials.UserName.Password = "pm";
      PTWcfService.IPTService proxy = factory.CreateChannel();
      PTWcfService.ProjectData[] list;
      using (proxy as IDisposable)
      {
        list = proxy.GetProjectList();
      }
      this.projectDataBindingSource.DataSource = list;

      //PTWcfService.PTServiceClient svc = new PTWcfClient.PTWcfService.PTServiceClient();
      //svc.ClientCredentials.UserName.UserName = "anonymous";
      ////svc.ClientCredentials.UserName.Password = "pm";
      //PTWcfService.ProjectData[] list = svc.GetProjectList();
      //this.projectDataBindingSource.DataSource = list;
      //svc.Close();
    }
  }
}