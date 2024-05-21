//-----------------------------------------------------------------------
// <copyright file="SmartDateConverter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Converts values to and from a SmartDate.</summary>
//-----------------------------------------------------------------------
using System.ComponentModel;

namespace Csla.Core.TypeConverters
{
  /// <summary>
  /// Converts values to and from a SmartDate.
  /// </summary>
  public class SmartDateConverter : TypeConverter
  {
    /// <summary>
    /// Determines if a value can be converted
    /// to a SmartDate.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sourceType"></param>
    /// <remarks></remarks>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string))
        return true;
      else if (sourceType == typeof(DateTime))
        return true;
      else if (sourceType == typeof(DateTimeOffset))
        return true;
      else if (sourceType == typeof(DateTime?))
        return true;
      return base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Converts values to a SmartDate.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="culture"></param>
    /// <param name="value"></param>
    /// <remarks></remarks>
    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      if (value is string)
        return new SmartDate(Convert.ToString(value));
      else if (value is DateTime)
        return new SmartDate(Convert.ToDateTime(value));
      else if (value == null)
        return new SmartDate();
      else if (value is DateTime?)
        return new SmartDate((DateTime?)value);
      else if (value is DateTimeOffset offset)
        return new SmartDate(offset.DateTime);
      return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Determines if a SmartDate can be
    /// convert to a value.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="destinationType"></param>
    /// <remarks></remarks>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(string))
        return true;
      else if (destinationType == typeof(DateTime))
        return true;
      else if (destinationType == typeof(DateTimeOffset))
        return true;
      else if (destinationType == typeof(DateTime?))
        return true;
      return base.CanConvertTo(context, destinationType);
    }

    /// <summary>
    /// Converts a SmartDate to a value.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="culture"></param>
    /// <param name="value"></param>
    /// <param name="destinationType"></param>
    /// <remarks></remarks>
    public override object ConvertTo(
      ITypeDescriptorContext context,
      System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      SmartDate sd = (SmartDate)value;
      if (destinationType == typeof(string))
        return sd.Text;
      else if (destinationType == typeof(DateTime))
        return sd.Date;
      else if (destinationType == typeof(DateTimeOffset))
        return new DateTimeOffset(sd.Date);
      else if (destinationType == typeof(DateTime?))
        return new DateTime?(sd.Date);
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}