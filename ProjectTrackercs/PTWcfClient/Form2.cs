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
  public partial class Form2 : Form
  {
    public Form2()
    {
      InitializeComponent();
    }

    private void Form2_Load(object sender, EventArgs e)
    {
      PTWcfService.RoleData[] list = null;

      var svc = new PTWcfClient.PTWcfService.PTServiceClient("WSHttpBinding_IPTService");
      try
      {
        svc.ClientCredentials.UserName.UserName = "anonymous";
        //svc.ClientCredentials.UserName.Password = "pm";
        list =
          svc.GetRoles(new PTWcfClient.PTWcfService.RoleRequest());
      }
      finally
      {
        svc.Close();
      }

      this.roleDataBindingSource.DataSource = list;
    }
  }
}
