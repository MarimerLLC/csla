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
