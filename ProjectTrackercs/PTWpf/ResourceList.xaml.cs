using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectTracker.Library;

namespace PTWpf
{
  /// <summary>
  /// Interaction logic for ResourceList.xaml
  /// </summary>
  public partial class ResourceList : EditForm
  {
    public ResourceList()
    {
      InitializeComponent();
    }

    void ShowResource(object sender, EventArgs e)
    {
      ResourceInfo item =
        (ResourceInfo)this.listBox1.SelectedItem;

      if (item != null)
      {
        ResourceEdit frm = new ResourceEdit(item.Id);
        MainForm.ShowControl(frm);
      }
    }
  }
}