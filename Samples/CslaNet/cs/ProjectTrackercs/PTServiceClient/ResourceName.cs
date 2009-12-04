using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PTServiceClient
{
  public partial class ResourceName : Form
  {
    public ResourceName(string id, string name)
    {
      InitializeComponent();
      this.IdLabel1.Text = id;
      this.NameLabel1.Text = name;
    }

    private void OK_Button_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
  }
}