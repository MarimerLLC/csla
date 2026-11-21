//-----------------------------------------------------------------------
// <copyright file="NotNullGuard.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Runtime checks for not-null properties.</summary>
//-----------------------------------------------------------------------

using Csla.Core.FieldManager;
using Csla.Properties;

namespace Csla.Core
{
  /// <summary>
  /// Runtime checks shared by the write funnels and read methods
  /// of not-null properties (<see cref="INotNullPropertyInfo"/>).
  /// </summary>
  internal static class NotNullGuard
  {
    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> when <paramref name="value"/>
    /// is null and the property is a not-null property.
    /// </summary>
    /// <remarks>
    /// The null test runs first, so a non-null write pays one comparison
    /// (none for a value type). Value-type properties never throw: an untyped
    /// null becomes <c>default(T)</c> as for any other property.
    /// </remarks>
    internal static void ThrowIfNull<P>(IPropertyInfo propertyInfo, P? value)
    {
      if (value is null && IsNotNullReference(propertyInfo))
        throw new ArgumentNullException(propertyInfo.Name, string.Format(Resources.NotNullPropertyValueNull, propertyInfo.Name));
    }

    /// <summary>
    /// Throws <see cref="NotNullPropertyViolationException"/> when <paramref name="value"/>
    /// is null and the property is a not-null property. Used inside the
    /// <c>try</c> blocks of the write funnels, whose generic <c>catch</c> lets
    /// this exception type pass through unwrapped.
    /// </summary>
    internal static void ThrowIfNullInsideTry<P>(IPropertyInfo propertyInfo, P? value)
    {
      if (value is null && IsNotNullReference(propertyInfo))
        throw new NotNullPropertyViolationException(propertyInfo.Name, string.Format(Resources.NotNullPropertyValueNull, propertyInfo.Name));
    }

    /// <summary>
    /// Throws <see cref="NotSupportedException"/> when the property is a
    /// not-null property: delayed loading is not supported for them.
    /// </summary>
    internal static void ThrowIfDelayedLoad(IPropertyInfo propertyInfo)
    {
      if (propertyInfo is INotNullPropertyInfo)
        throw new NotSupportedException(string.Format(Resources.NotNullPropertyDelayedLoadNotSupported, propertyInfo.Name));
    }

    /// <summary>
    /// Gets a value indicating whether the property is a not-null property
    /// whose type can hold null (a reference type).
    /// </summary>
    internal static bool IsNotNullReference(IPropertyInfo propertyInfo)
    {
      return propertyInfo is INotNullPropertyInfo && !propertyInfo.Type.IsValueType;
    }

    /// <summary>
    /// Reads a not-null property from the field manager. An unloaded property
    /// reads as its default value, which is stored unless it is null.
    /// </summary>
    internal static P Read<P>(FieldDataManager fieldManager, NotNullPropertyInfo<P> propertyInfo)
      where P : notnull
    {
      var data = fieldManager.GetFieldData(propertyInfo);
      if (data != null)
      {
        if (data is IFieldData<P> fd)
          return fd.Value!;
        return (P)data.Value!;
      }

      var result = propertyInfo.DefaultValue;
      if (result is not null)
        fieldManager.LoadFieldData<P>(propertyInfo, result);
      return result!;
    }
  }

  /// <summary>
  /// Thrown when null is written to a not-null property from inside a
  /// write funnel's <c>try</c> block; passes through the funnel's generic
  /// <c>catch</c> unwrapped. Callers see an <see cref="ArgumentNullException"/>.
  /// </summary>
  internal sealed class NotNullPropertyViolationException : ArgumentNullException
  {
    internal NotNullPropertyViolationException(string paramName, string message)
      : base(paramName, message)
    { }
  }
}
