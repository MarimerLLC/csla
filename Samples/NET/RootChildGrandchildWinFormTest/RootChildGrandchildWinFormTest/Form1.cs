using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Csla;

namespace WindowsApplication2
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      this.childrenBindingSource.CurrentChanged += 
        new EventHandler(childrenBindingSource_CurrentChanged);
      this.grandchildrenBindingSource.CurrentChanged += 
        new EventHandler(grandchildrenBindingSource_CurrentChanged);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      Root root = DataPortal.Create<Root>();
      Child child;
      child = root.RealChildren.AddNew();
      child.Grandchildren.AddNew();
      child.Grandchildren.AddNew();
      child.Grandchildren.AddNew();
      child = root.RealChildren.AddNew();
      child.Grandchildren.AddNew();
      child.Grandchildren.AddNew();
      child.Grandchildren.AddNew();
      child = root.RealChildren.AddNew();
      child.Grandchildren.AddNew();
      child.Grandchildren.AddNew();
      child.Grandchildren.AddNew();
      child = root.RealChildren.AddNew();
      child.Grandchildren.AddNew();
      child.Grandchildren.AddNew();
      child.Grandchildren.AddNew();

      Rebind(root);
    }

    void grandchildrenBindingSource_CurrentChanged(object sender, EventArgs e)
    {
    }

    void childrenBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      this.grandchildrenBindingSource.EndEdit();
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
      this.grandchildrenBindingSource.EndEdit();

      this.grandchildrenBindingSource.RaiseListChangedEvents = false;
      this.childrenBindingSource.RaiseListChangedEvents = false;
      this.rootBindingSource.RaiseListChangedEvents = false;

      Root root = (Root)this.rootBindingSource.DataSource;
      UnbindBindingSource(this.grandchildrenBindingSource, false);
      UnbindBindingSource(this.childrenBindingSource, false);
      UnbindBindingSource(this.rootBindingSource, false);

      root.DumpEditLevels();
      root.CancelEdit();
      root.DumpEditLevels();
      Rebind(root);
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
      this.grandchildrenBindingSource.RaiseListChangedEvents = false;
      this.childrenBindingSource.RaiseListChangedEvents = false;
      this.rootBindingSource.RaiseListChangedEvents = false;

      Root root = (Root)this.rootBindingSource.DataSource;
      UnbindBindingSource(this.grandchildrenBindingSource, true);
      UnbindBindingSource(this.childrenBindingSource, true);
      UnbindBindingSource(this.rootBindingSource, true);
      this.grandchildrenBindingSource.EndEdit();
      this.childrenBindingSource.EndEdit();
      this.rootBindingSource.EndEdit();

      root.ApplyEdit();
      root.DumpEditLevels();
      Rebind(root);
    }

    private void Rebind(Root root)
    {
      root.BeginEdit();
      this.rootBindingSource.DataSource = root;
      this.grandchildrenBindingSource.RaiseListChangedEvents = true;
      this.childrenBindingSource.RaiseListChangedEvents = true;
      this.rootBindingSource.RaiseListChangedEvents = true;

      this.grandchildrenBindingSource.ResetBindings(false);
      this.childrenBindingSource.ResetBindings(false);
      this.rootBindingSource.ResetBindings(false);
    }

    #region Data binding helpers

    protected void UnbindBindingSource(BindingSource source, bool apply)
    {
      System.ComponentModel.IEditableObject current =
        source.Current as System.ComponentModel.IEditableObject;
      if (!(source.DataSource is BindingSource))
        source.DataSource = null;
      if (current != null)
        if (apply)
          current.EndEdit();
        else
          current.CancelEdit();
    }

    #endregion
  }
}