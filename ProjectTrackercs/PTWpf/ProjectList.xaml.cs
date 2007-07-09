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
  /// Interaction logic for ProjectList.xaml
  /// </summary>
  public partial class ProjectList : EditForm
  {
    public ProjectList()
    {
      InitializeComponent();
    }
    
    void ShowProject(object sender, EventArgs e)
    {
      ProjectTracker.Library.ProjectInfo item =
        (ProjectTracker.Library.ProjectInfo)this.listBox1.SelectedItem;

      if (item != null)
      {
        ProjectEdit frm = new ProjectEdit(item.Id);
        MainForm.ShowControl(frm);
      }
    }
  }
}