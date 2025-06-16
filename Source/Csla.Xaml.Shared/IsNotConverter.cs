#if !NETFX_CORE && !XAMARIN && !MAUI
//-----------------------------------------------------------------------
// <copyright file="IsNotConverter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Converts a Boolean value into its inverse.</summary>
//-----------------------------------------------------------------------
using System.Windows.Data;

namespace Csla.Xaml
{
  /// <summary>
  /// Converts a Boolean value into its inverse.
  /// </summary>
  public class IsNotConverter : IValueConverter
  {
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">Original value.</param>
    /// <param name="targetType">Desired output type (ignored).</param>
    /// <param name="parameter">Extra parameter (ignored).</param>
    /// <param name="culture">Desired culture (ignored).</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not of type <see cref="bool"/>.</exception>
    public object? Convert(object? value, Type? targetType, object? parameter, System.Globalization.CultureInfo? culture)
    {
      if (value is not bool val)
        throw new ArgumentException($"{value?.GetType().ToString()} != typeof(bool)", nameof(value));

      return !val;
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">Original value.</param>
    /// <param name="targetType">Desired output type (ignored).</param>
    /// <param name="parameter">Extra parameter (ignored).</param>
    /// <param name="culture">Desired culture (ignored).</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not of type <see cref="bool"/>.</exception>
    public object? ConvertBack(object? value, Type? targetType, object? parameter, System.Globalization.CultureInfo? culture)
    {
      if (value is not bool val)
        throw new ArgumentException($"{value?.GetType().ToString()} != typeof(bool)", nameof(value));

      return !val;
    }
  }
}
#endif