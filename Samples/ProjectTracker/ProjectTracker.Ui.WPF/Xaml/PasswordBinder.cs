// Adapted from this source:
// http://blog.functionalfun.net/2008/06/wpf-passwordbox-and-data-binding.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.Xaml
{
  public static class PasswordBinder
  {
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password",
        typeof(string), typeof(PasswordBinder),
        new FrameworkPropertyMetadata(string.Empty, (o, e) =>
          {
            var passwordBox = o as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!GetIsUpdating(passwordBox))
              passwordBox.Password = (string)e.NewValue;
            passwordBox.PasswordChanged += PasswordChanged;
          }));

    public static string GetPassword(DependencyObject dp)
    {
      return (string)dp.GetValue(PasswordProperty);
    }

    public static void SetPassword(DependencyObject dp, string value)
    {
      dp.SetValue(PasswordProperty, value);
    }

    private static readonly DependencyProperty IsUpdatingProperty =
       DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
       typeof(PasswordBinder));

    private static bool GetIsUpdating(DependencyObject dp)
    {
      return (bool)dp.GetValue(IsUpdatingProperty);
    }

    private static void SetIsUpdating(DependencyObject dp, bool value)
    {
      dp.SetValue(IsUpdatingProperty, value);
    }

    private static void PasswordChanged(object sender, RoutedEventArgs e)
    {
      var passwordBox = sender as PasswordBox;
      SetIsUpdating(passwordBox, true);
      SetPassword(passwordBox, passwordBox.Password);
      SetIsUpdating(passwordBox, false);
    }
  }
}
