#if !NETFX_CORE && !XAMARIN
//-----------------------------------------------------------------------
// <copyright file="IsNotConverter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Converts a Boolean value into its inverse.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return !(bool)value;
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">Original value.</param>
    /// <param name="targetType">Desired output type (ignored).</param>
    /// <param name="parameter">Extra parameter (ignored).</param>
    /// <param name="culture">Desired culture (ignored).</param>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return !(bool)value;
    }
  }
}
#endif