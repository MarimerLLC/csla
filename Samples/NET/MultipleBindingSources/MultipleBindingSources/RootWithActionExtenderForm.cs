using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Csla;
using Csla.Reflection;

namespace MultipleBindingSources
{
  public partial class RootWithActionExtenderForm : Form
  {
    Root MyRoot { get; set; }

    public RootWithActionExtenderForm()
    {
      InitializeComponent();

      MyRoot = Root.NewEditableRoot();
      cslaActionExtender1.ResetActionBehaviors(MyRoot);
    }

    private void editButton_Click(object sender, EventArgs e)
    {
      var child = childrenBindingSource.Current as Child;
      if (child != null)
      {
        child.BeginEdit();

        using (var form = new ModalChildEditForm(child))
        {
          MyRoot.DumpEditLevels();
          var result = form.ShowDialog();
          if (result == DialogResult.OK)
          {
            child.ApplyEdit();
          }
          else
          {
            child.CancelEdit();
          }
        }
      }
      MyRoot.DumpEditLevels();
    }

    private void button1_Click(object sender, EventArgs e)
    {

    }
  }
}
