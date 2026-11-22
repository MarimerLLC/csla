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
    /// Checks a value about to be written to a property, and returns the value
    /// to store. For a not-null property, null becomes <see cref="string.Empty"/>
    /// when the property is a <see cref="string"/>, and otherwise throws
    /// <see cref="ArgumentNullException"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A string never throws, as the typed setter never did: Windows Forms data
    /// binding needs a string property that never returns null, and a setter
    /// that throws holds the user in the field.
    /// </para>
    /// <para>
    /// The null test runs first, so a non-null write pays one comparison
    /// (none for a value type). Value-type properties never throw: an untyped
    /// null becomes <c>default(T)</c> as for any other property.
    /// </para>
    /// </remarks>
    internal static P? CheckWrite<P>(IPropertyInfo propertyInfo, P? value)
    {
      if (value is null && IsNotNullReference(propertyInfo))
      {
        if (propertyInfo.Type == typeof(string))
          return (P)(object)string.Empty;
        throw new ArgumentNullException(propertyInfo.Name, string.Format(Resources.NotNullPropertyValueNull, propertyInfo.Name));
      }
      return value;
    }

    /// <summary>
    /// As <see cref="CheckWrite{P}(IPropertyInfo, P)"/>, but throws
    /// <see cref="NotNullPropertyViolationException"/>. Used inside the
    /// <c>try</c> blocks of the write funnels, whose generic <c>catch</c> lets
    /// this exception type pass through unwrapped.
    /// </summary>
    internal static P? CheckWriteInsideTry<P>(IPropertyInfo propertyInfo, P? value)
    {
      if (value is null && IsNotNullReference(propertyInfo))
      {
        if (propertyInfo.Type == typeof(string))
          return (P)(object)string.Empty;
        throw new NotNullPropertyViolationException(propertyInfo.Name, string.Format(Resources.NotNullPropertyValueNull, propertyInfo.Name));
      }
      return value;
    }

    /// <summary>
    /// Gets a value indicating whether a null written to the property is
    /// rejected (a not-null reference-type property other than <see cref="string"/>,
    /// which stores null as <see cref="string.Empty"/>).
    /// </summary>
    internal static bool RejectsNull(IPropertyInfo propertyInfo)
      => IsNotNullReference(propertyInfo) && propertyInfo.Type != typeof(string);

    /// <summary>
    /// For a not-null reference-type property, converts an untyped value to the
    /// property type as <see cref="FieldManager.FieldDataManager"/> would when
    /// storing it, so the guard sees the value that is stored: a non-null input
    /// can convert to null. Other properties get the value unchanged.
    /// </summary>
    internal static object? CoerceUntyped(IPropertyInfo propertyInfo, object? value)
    {
      if (value is null || !IsNotNullReference(propertyInfo))
        return value;
      return Utilities.CoerceValue(propertyInfo.Type, value.GetType(), null, value);
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
