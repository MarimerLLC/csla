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
  public partial class RootWithChildListForm : Form
  {
    Root MyRoot { get; set; }

    public RootWithChildListForm()
    {
      InitializeComponent();

    }

    public void BindUI(Root root)
    {
      MyRoot = root;
      rootBindingSource.BindDataSource(MyRoot);
      childrenBindingSource.BindDataSource(MyRoot.Children);
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
      var editLevel = (int)MethodCaller.CallMethod(MyRoot, "get_EditLevel");
      Debug.Print("Root editlevel before unbinding: {0}", editLevel);

      childrenBindingSource.UnbindDataSource(false, false);
      rootBindingSource.UnbindDataSource(false, true);
      MyRoot.ApplyEdit();

      editLevel = (int)MethodCaller.CallMethod(MyRoot, "get_EditLevel");
      Debug.Print("Root editlevel: {0}", editLevel);

      MyRoot.DumpEditLevels();

      // Object is now ready for save and should be dirty and savable with editlevel 0 root and all children
    }

    private void button2_Click(object sender, EventArgs e)
    {
      MyRoot.DumpEditLevels();
    }

    private void addButton_Click(object sender, EventArgs e)
    {
      childrenBindingSource.AddNew();
    }

    private void deleteButton_Click(object sender, EventArgs e)
    {
      if (childrenBindingSource.Position > -1)
        childrenBindingSource.RemoveCurrent();
    }

    private void button3_Click(object sender, EventArgs e)
    {
      childrenBindingSource.Insert(1, Child.NewEditableChild());
    }
  }
}
