using System;
using System.Windows;

namespace Rolodex.Silverlight.Core
{
  public static class ErrorHandler
  {
    public static void HandleException(Exception ex)
    {
      MessageBox.Show(string.Format("Error has occurred.{0}{1}", Environment.NewLine, ex.ToString()));
    }
  }
}