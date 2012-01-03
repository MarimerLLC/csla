using System;
using System.Windows;
using System.Windows.Data;

namespace WpUI.Xaml
{
  public class BoolVisibilityConverter : IValueConverter
  {
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var v = bool.Parse(value.ToString());
      if (Invert)
        v = !v;
      if (v)
        return Visibility.Visible;
      else
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }
  }
}
