#if !XAMARIN && !WINDOWS_UWP && !MAUI
//-----------------------------------------------------------------------
// <copyright file="BrokenRulesSeverityConverter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Converts validation severity values into corresponding</summary>
//-----------------------------------------------------------------------
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Csla.Rules;

namespace Csla.Xaml
{
  /// <summary>
  /// Converts validation severity values into corresponding
  /// images.
  /// </summary>
  public class BrokenRuleSeverityConverter : IValueConverter
  {
    /// <summary>
    /// Converts validation severity values into corresponding
    /// images.
    /// </summary>
    /// <param name="value">Original value.</param>
    /// <param name="targetType">Desired output type (ignored).</param>
    /// <param name="parameter">Extra parameter (ignored).</param>
    /// <param name="culture">Desired culture (ignored).</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/> or not of type <see cref="RuleSeverity"/>.</exception>
    public object? Convert(object? value, Type? targetType, object? parameter, System.Globalization.CultureInfo? culture)
    {
      if (value is not RuleSeverity severity)
      {
        throw new ArgumentException($"{value?.GetType().ToString()} != typeof({nameof(RuleSeverity)})", nameof(value));
      }

      string uri = $"/Csla.Xaml;component/Resources/{severity}.png";
      StreamResourceInfo sr = Application.GetResourceStream(new Uri(uri, UriKind.Relative));
      BitmapImage bmp = new BitmapImage();
      bmp.BeginInit();
      bmp.StreamSource = sr.Stream;
      bmp.EndInit();

      return bmp;
    }

    /// <summary>
    /// Returns the original value.
    /// </summary>
    /// <param name="value">Original value.</param>
    /// <param name="targetType">Desired output type (ignored).</param>
    /// <param name="parameter">Extra parameter (ignored).</param>
    /// <param name="culture">Desired culture (ignored).</param>
    public object? ConvertBack(object? value, Type? targetType, object? parameter, System.Globalization.CultureInfo? culture)
    {
      return RuleSeverity.Error;
    }
  }
}
#endif