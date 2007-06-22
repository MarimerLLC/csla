using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProjectTracker.Library;

namespace PTWpf
{
  /// <summary>
  /// Interaction logic for ResourceSelect.xaml
  /// </summary>
  public partial class ResourceSelect : Window
  {
    public ResourceSelect()
    {
      InitializeComponent();
    }

    private int _resourceId;

    public int ResourceId
    {
      get { return _resourceId; }
    }


    void OkButton(object sender, EventArgs e)
    {
      ResourceInfo item = (ResourceInfo)ResourceListBox.SelectedItem;
      if (item != null)
      {
        DialogResult = true;
        _resourceId = (item).Id;
      }
      else
        DialogResult = false;
      Hide();
    }

    void CancelButton(object sender, EventArgs e)
    {
      DialogResult = false;
      Hide();
    }

    void ResourceSelected(object sender, EventArgs e)
    {
      OkButton(sender, e);
    }
  }
}
