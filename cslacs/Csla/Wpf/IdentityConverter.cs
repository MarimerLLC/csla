using System;
using System.Windows.Data;

namespace Csla.Wpf
{
  /// <summary>
  /// Provides the functionality of a WPF
  /// value converter without affecting the
  /// value as it flows to and from the UI.
  /// </summary>
  public class IdentityConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Returns the unchanged value.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    /// <param name="targetType">Desired value type.</param>
    /// <param name="parameter">Conversion parameter.</param>
    /// <param name="culture">Conversion culture.</param>
    public object Convert(
      object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }

    /// <summary>
    /// Returns the unchanged value.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    /// <param name="targetType">Desired value type.</param>
    /// <param name="parameter">Conversion parameter.</param>
    /// <param name="culture">Conversion culture.</param>
    public object ConvertBack(
      object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }

    #endregion
  }
}