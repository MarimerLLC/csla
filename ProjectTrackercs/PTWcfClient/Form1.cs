using System;
using System.Linq;
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
      PTWcfService.PTServiceClient svc = new PTWcfClient.PTWcfService.PTServiceClient();
      PTWcfService.ProjectData[] list = svc.GetProjectList();
      this.projectDataBindingSource.DataSource = list;
      svc.Close();
    }
  }
}