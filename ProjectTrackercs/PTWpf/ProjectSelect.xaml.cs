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
  /// Interaction logic for ProjectSelect.xaml
  /// </summary>

  public partial class ProjectSelect : Window
  {
    public ProjectSelect()
    {
      InitializeComponent();
    }

    private Guid _projectId;

    public Guid ProjectId
    {
      get { return _projectId; }
    }
	
	
    void OkButton(object sender, EventArgs e)
    {
      ProjectInfo item = (ProjectInfo)ProjectListBox.SelectedItem;
      if (item != null)
      {
        DialogResult = true;
        _projectId = (item).Id;
      }
      else
        DialogResult = false;
      Hide();
    }

    void CancelButton(object sender, EventArgs e)
    {
      DialogResult = true;
      Hide();
    }

    void ProjectSelected(object sender, EventArgs e)
    {
      OkButton(sender, e);
    }
}
}
