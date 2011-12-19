using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpUI.ViewModels
{
  public class ProjectResourceEdit : ViewModelLocalEdit<ProjectTracker.Library.ProjectResourceEdit>
  {
    public ProjectResourceEdit(string querystring)
    {

    }

    public class ResourceItem
    {
      public string FullName { get; set; }

      public void SelectResource()
      {
        
      }
    }
  }

}
