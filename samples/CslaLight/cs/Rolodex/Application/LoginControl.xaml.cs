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
using Rolodex.Business.Security;

namespace Rolodex
{
  public partial class LoginControl : UserControl
  {
    public LoginControl()
    {
      InitializeComponent();
    }

    public event EventHandler LoginSuccessfull;

    private void LogInButton_Click(object sender, RoutedEventArgs e)
    {
      LogInButton.IsEnabled = false;
      Status.Text = "Validating credentials...";
      animation.Visibility = Visibility.Visible;
      Status.Visibility = Visibility.Visible;
      animation.IsRunning = true;
      UserIdBox.IsReadOnly = true;
      UserPwdBox.IsReadOnly = true;
      RolodexPrincipal.Login(UserIdBox.Text.Trim(), UserPwdBox.Text.Trim(), (o1, e1) =>
        {
          animation.IsRunning = false;
          UserIdBox.IsReadOnly = false;
          UserPwdBox.IsReadOnly = false;
          if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
          {
            Status.Text = "Login Successfull.";
            if (LoginSuccessfull != null)
              LoginSuccessfull.Invoke(this, EventArgs.Empty);
          }
          else
          {
            Status.Text = "Invalid login. Try again.";
            LogInButton.IsEnabled = true;
          }
        }
      );
    }
  }
}
