using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace WpUI.Views
{
  public partial class ProjectResourceEdit : PhoneApplicationPage
  {
    public ProjectResourceEdit()
    {
      InitializeComponent();
    }

    private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
    {
      var context = (ViewModels.ProjectResourceEdit)DataContext;
      if (context.EditMode)
        Pivot.Items.RemoveAt(0);
    }

    private void AcceptButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var context = (ViewModels.ProjectResourceEdit)DataContext;

      // copy lostfocus-based view values to model

      context.Accept();
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var context = (ViewModels.ProjectResourceEdit)DataContext;
      context.Close();
    }

    private void RemoveButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var context = (ViewModels.ProjectResourceEdit)DataContext;
      context.Remove();
    }
  }
}