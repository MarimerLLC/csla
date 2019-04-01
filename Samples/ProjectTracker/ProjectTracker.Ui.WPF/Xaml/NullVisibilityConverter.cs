using System;
using System.Windows.Data;
using System.Windows;

namespace WpfUI.Xaml
{
  public class NullVisibilityConverter : IValueConverter
  {
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var condition = (value == null);
      if (Invert) condition = !condition;
      if (condition)
        return Visibility.Collapsed;
      else
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }
  }
}
