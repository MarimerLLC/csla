using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Csla.Reflection;

namespace MultipleBindingSources
{
  public partial class SingleRootDualBindingSorcesFrom : Form
  {
    private Root MyRoot { get; set;}

    public SingleRootDualBindingSorcesFrom()
    {
      InitializeComponent();

      MyRoot = Root.NewEditableRoot();
      MyRoot.BeginEdit();
      //rootBindingSource.DataSource = myRoot;
      rootBindingSource.BindDataSource(MyRoot);
      rootBindingSource2.BindDataSource(MyRoot);

      var editLevel = (int)MethodCaller.CallMethod(MyRoot, "get_EditLevel");
      Debug.Print("Root editlevel after databinding: {0}", editLevel);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var editLevel = (int)MethodCaller.CallMethod(MyRoot, "get_EditLevel");
      Debug.Print("Root editlevel before unbinding: {0}", editLevel);

      rootBindingSource2.UnbindDataSource(false, true);
      rootBindingSource.UnbindDataSource(false, true);
      MyRoot.ApplyEdit();

      editLevel = (int) MethodCaller.CallMethod(MyRoot, "get_EditLevel");
      Debug.Print("Root editlevel: {0}", editLevel);
    }

    private void button2_Click(object sender, EventArgs e)
    {
      MyRoot.DumpEditLevels();
    }
  }
}
