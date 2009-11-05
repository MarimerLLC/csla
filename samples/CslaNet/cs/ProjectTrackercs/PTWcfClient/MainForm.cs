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
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void ProjectListButton_Click(object sender, EventArgs e)
    {
      new Form1().ShowDialog(this);
    }

    private void RoleListButton_Click(object sender, EventArgs e)
    {
      new Form2().ShowDialog(this);
    }

    private void NewProjectButton_Click(object sender, EventArgs e)
    {
      new Form3().ShowDialog(this);
    }
  }
}
