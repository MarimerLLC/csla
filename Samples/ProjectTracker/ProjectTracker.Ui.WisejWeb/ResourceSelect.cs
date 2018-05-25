using System;
using ProjectTracker.Library;
using Wisej.Web;

namespace PTWisej
{
  public partial class ResourceSelect : Form
  {
    private int _resourceId;

    public int ResourceId
    {
      get { return _resourceId; }
    }

    public ResourceSelect()
    {
      InitializeComponent();
    }

    private void OK_Button_Click(object sender, EventArgs e)
    {
      _resourceId =
        ((ResourceInfo)
          this.ResourceListListBox.SelectedValue).Id;
      this.Close();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void ResourceSelect_Load(object sender, EventArgs e)
    {
      this.ResourceListBindingSource.DataSource =
        ResourceList.GetResourceList();
    }

    private void ResourceListListBox_DoubleClick(object sender, EventArgs e)
    {
      _resourceId =
        ((ResourceInfo)
          this.ResourceListListBox.SelectedValue).Id;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }
  }
}