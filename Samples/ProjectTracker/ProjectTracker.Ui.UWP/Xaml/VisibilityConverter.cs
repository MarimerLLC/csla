using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UwpUI.Xaml
{
  /// <summary>
  /// Converts a bool to a Visibility value. By default
  /// true means Visible, false means Collapsed.
  /// </summary>
  public class VisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Inverts the normal bool to Visibility conversion.
    /// </summary>
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      bool v = (bool)value;
      if (Invert) v = !v;
      if (v)
        return Visibility.Visible;
      else
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      return null;
    }
  }
}

