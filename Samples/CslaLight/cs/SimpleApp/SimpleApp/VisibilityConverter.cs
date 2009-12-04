using System;

namespace SimpleApp
{
  public class VisibilityConverter : System.Windows.Data.IValueConverter
  {
    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value.ToString() == "True")
        return System.Windows.Visibility.Visible;
      else
        return System.Windows.Visibility.Collapsed;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }
  }
}