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
  public partial class ResourceEdit : PhoneApplicationPage
  {
    public ResourceEdit()
    {
      InitializeComponent();
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var viewmodel = (ViewModels.ResourceEdit)this.DataContext;

      // copy lostfocus-based view values to model
      var project = viewmodel.Model.Resource;
      project.FirstName = FirstNameTextBox.Text;
      project.LastName = LastNameTextBox.Text;

      viewmodel.Save();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var viewmodel = (ViewModels.ResourceEdit)this.DataContext;
      viewmodel.Close();
    }
  }
}