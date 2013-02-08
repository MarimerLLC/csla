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
  public partial class RoleListEdit : PhoneApplicationPage
  {
    public RoleListEdit()
    {
      InitializeComponent();
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var viewmodel = (ViewModels.RoleListEdit)this.DataContext;

      // copy lostfocus-based view values to model
      
      viewmodel.Save();
    }
  }
}