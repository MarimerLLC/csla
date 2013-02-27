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
  public partial class ResourceDetails : PhoneApplicationPage
  {
    public ResourceDetails()
    {
      InitializeComponent();
    }

    private void EditButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var viewmodel = (ViewModels.ResourceDetail)this.DataContext;
      if (!viewmodel.CanEditObject)
        Bxf.Shell.Instance.ShowError("Can't edit", "Authorization");
      else
        viewmodel.Edit();
    }

    private void DeleteButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var viewmodel = (ViewModels.ResourceDetail)this.DataContext;
      if (!viewmodel.CanEditObject)
        Bxf.Shell.Instance.ShowError("Can't delete", "Authorization");
      else if (MessageBox.Show("Delete item?", "Resource", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        viewmodel.Delete();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var viewmodel = (ViewModels.ResourceDetail)this.DataContext;
      viewmodel.Close();
    }
  }
}