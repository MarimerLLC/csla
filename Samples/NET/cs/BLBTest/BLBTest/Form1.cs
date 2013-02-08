using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BLBTest
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      DataList list = new DataList();
      list.Add(new DataEdit(1, "Rocky"));
      list.Add(new DataEdit(2, "Fred"));
      list.Add(new DataEdit(3, "Mary"));
      list.Add(new DataEdit(4, "George"));
      list.BeginEdit();
      this.dataListBindingSource.DataSource = list;
      this.dataListBindingSource.ListChanged += new ListChangedEventHandler(dataListBindingSource_ListChanged);
    }

    void dataListBindingSource_ListChanged(object sender, ListChangedEventArgs e)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Format("{0}: {1}, {2}",e.ListChangedType.ToString(), e.NewIndex, e.OldIndex));
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      DataEdit item = new DataEdit(100, "Abdul");
      ((DataList)this.dataListBindingSource.DataSource)[1] = item;
      System.Diagnostics.Debug.WriteLine(
        string.Format("{0}: {1}, {2}", item.Data, item.CurrentEditLevel, item.CurrentEditLevelAdded));
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      // this is the wrong way to perform a cancel / undo operation
      // and an "Edit level mismatch in CopyState" exception will be thrown

      DataList list = (DataList)this.dataListBindingSource.DataSource;
      this.dataListBindingSource.CancelEdit();
      list.CancelEdit();
      list.BeginEdit();
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
      // get business object reference
      DataList list = (DataList)this.dataListBindingSource.DataSource;

      // cancel current row
      this.dataListBindingSource.CancelEdit();

      // unbind the UI
      UnbindBindingSource(this.dataListBindingSource);

      // cancel the list and restart editing
      list.CancelEdit();
      list.BeginEdit();

      // rebind the UI
      this.dataListBindingSource.DataSource = list;
    }

    private void UnbindBindingSource(BindingSource source)
    {
      System.ComponentModel.IEditableObject current = 
        this.dataListBindingSource.Current as System.ComponentModel.IEditableObject;
      this.dataListBindingSource.DataSource = null;
      if (current != null)
        current.EndEdit();
    }
  }
}