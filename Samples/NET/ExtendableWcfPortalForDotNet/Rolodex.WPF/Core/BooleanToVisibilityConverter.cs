using System;
using System.Windows;
using System.Windows.Data;

namespace Rolodex.Silverlight.Core
{
  public class BooleanToVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null && value is bool)
      {
        if ((bool) value == true)
          return Visibility.Visible;
        else
          return Visibility.Collapsed;
      }
      else
      {
        return Visibility.Collapsed;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return null;
    }

    #endregion
  }
}