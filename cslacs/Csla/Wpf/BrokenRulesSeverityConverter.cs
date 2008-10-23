using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Validation;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Data;

namespace Csla.Wpf
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
      bmp.BeginInit();
      bmp.StreamSource = sr.Stream;
      bmp.EndInit();
      
      return bmp;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return RuleSeverity.Error;
    }

    #endregion
  }
}
