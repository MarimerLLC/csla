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
using System.Windows.Data;
using Csla.Validation;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;

namespace Csla.Silverlight
{
  public class BrokenRuleSeverityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      RuleSeverity severity = (RuleSeverity)value;
      string uri = string.Format("/Csla;component/Resources/{0}.png", severity);
      StreamResourceInfo sr = Application.GetResourceStream(new Uri(uri, UriKind.Relative));
      BitmapImage bmp = new BitmapImage();
      bmp.SetSource(sr.Stream);

      return bmp;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return RuleSeverity.Error;
    }

    #endregion
  }
}
