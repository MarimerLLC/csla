using System;
using Windows.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SimpleApp
{
  public class NullVisibilityConverter : IValueConverter
  {
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var condition = (value == null);
      if (Invert) condition = !condition;
      if (condition)
        return Visibility.Collapsed;
      else
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      return value;
    }
  }

  public class BoolVisibilityConverter : IValueConverter
  {
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var condition = (bool)value;
      if (Invert) condition = !condition;
      if (condition)
        return Visibility.Collapsed;
      else
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      return value;
    }
  }
}
