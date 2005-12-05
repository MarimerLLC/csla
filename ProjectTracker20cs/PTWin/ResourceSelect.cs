using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectTracker.Library;

namespace PTWin
{
  public partial class ResourceSelect : Form
  {

    private string _resourceId;
    public string ResourceId
    {
      get { return _resourceId; }
    }



    public ResourceSelect()
    {
      InitializeComponent();
    }

    private void OK_Button_Click(object sender, EventArgs e)
    {
      _resourceId = ((ResourceList.ResourceInfo)this.ResourceListListBox.SelectedValue).Id;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void ResourceSelect_Load(object sender, EventArgs e)
    {
      this.ResourceListBindingSource.DataSource = ResourceList.GetResourceList();
    }
  }
}