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
  public partial class Login : PhoneApplicationPage
  {
    public Login()
    {
      InitializeComponent();
    }

    private void LoginButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var viewmodel = this.DataContext as ViewModels.Login;
      if (viewmodel != null)
      {
        // binding may not have finalized, so copy values manually
        viewmodel.Username = this.usernameTextBox.Text;
        viewmodel.Password = this.passwordTextBox.Password;
        // now run viewmodel method
        viewmodel.LoginUser();
      }
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
      if (App.ViewModel.AppBusy) return;
      var viewmodel = this.DataContext as ViewModels.Login;
      if (viewmodel != null)
        viewmodel.Cancel();
    }
  }
}