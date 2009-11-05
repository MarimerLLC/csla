using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTWpf
{
  public class VisibilityConverter : System.Windows.Data.IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, 
      object parameter, System.Globalization.CultureInfo culture)
    {
      if ((bool)value)
        return System.Windows.Visibility.Visible;
      else
        return System.Windows.Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, 
      object parameter, System.Globalization.CultureInfo culture)
    {
      return false;
    }

    #endregion
  }
}
