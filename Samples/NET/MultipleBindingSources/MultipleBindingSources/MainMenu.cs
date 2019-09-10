using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultipleBindingSources
{
  public partial class MainMenu : Form
  {
    public MainMenu()
    {
      InitializeComponent();
    }

    private void MainMenu_Load(object sender, EventArgs e)
    {

    }

    private void RootEditbutton_Click(object sender, EventArgs e)
    {
      // single form and root object - dual bindingsorces and edit controls 
      using (var form = new SingleRootDualBindingSorcesFrom())
      {
        form.ShowDialog();
      }
    }

    private void RootAndChildListButton_Click(object sender, EventArgs e)
    {
      // display 2 forms bound to same roo object with child list 
      var root = Root.NewEditableRoot();

      var form = new RootWithChildListForm();
      form.BindUI(root);
      form.Top = 100;
      form.Left = 100;
      form.Show();

      var form2 = new RootWithChildListForm();
      form2.BindUI(root);
      form2.Top = form.Top;
      form2.Left = form.Left + form.Width + 30;
 
      form2.Show();

    }

    private void closeButton_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      using (var form = new RootWithActionExtenderForm())
      {
        form.ShowDialog();
      }
    }
  }
}
