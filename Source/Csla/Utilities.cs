//-----------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains utility methods used by the</summary>
//-----------------------------------------------------------------------

using System.Reflection;
using Csla.Reflection;
using System.ComponentModel;
using Csla.Properties;

namespace Csla
{
  /// <summary>
  /// Contains utility methods used by the
  /// CSLA .NET framework.
  /// </summary>
  public static class Utilities
  {
    #region Replacements for VB runtime functionality

    /// <summary>
    /// Determines whether the specified
    /// value can be converted to a valid number.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool IsNumeric(object value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      return double.TryParse(value.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out var _);
    }

    /// <summary>
    /// Allows late bound invocation of
    /// properties and methods.
    /// </summary>
    /// <param name="target">Object implementing the property or method.</param>
    /// <param name="methodName">Name of the property or method.</param>
    /// <param name="callType">Specifies how to invoke the property or method.</param>
    /// <param name="args">List of arguments to pass to the method.</param>
    /// <returns>The result of the property or method invocation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> or <paramref name="args"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public static object? CallByName(object target, string methodName, CallType callType, params object?[] args)
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (string.IsNullOrWhiteSpace(methodName))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(methodName)), nameof(methodName));

      switch (callType)
      {
        case CallType.Get:
          {
            return MethodCaller.CallPropertyGetter(target, methodName);
          }
        case CallType.Let:
        case CallType.Set:
          {
            MethodCaller.CallPropertySetter(target, methodName, args[0]);
            return null;
          }
        case CallType.Method:
          {
            return MethodCaller.CallMethod(target, methodName, args);
          }
      }
      return null;
    }

    #endregion

    /// <summary>
    /// Returns a property's type, dealing with
    /// Nullable(Of T) if necessary.
    /// </summary>
    /// <param name="propertyType">Type of the
    /// property as returned by reflection.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyType"/> is <see langword="null"/>.</exception>
    public static Type GetPropertyType(Type propertyType)
    {
      if (propertyType is null)
        throw new ArgumentNullException(nameof(propertyType));

      if (propertyType.IsGenericType && (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
        return Nullable.GetUnderlyingType(propertyType) ?? throw new InvalidOperationException($"The underlying type of {propertyType} is an open type. But we expect to resolve a closed type.");
      return propertyType;
    }

    /// <summary>
    /// Returns the type of child object
    /// contained in a collection or list.
    /// </summary>
    /// <param name="listType">Type of the list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="listType"/> is <see langword="null"/>.</exception>
    public static Type? GetChildItemType(Type listType)
    {
      if (listType is null)
        throw new ArgumentNullException(nameof(listType));

      Type? result = null;
      if (listType.IsArray)
        result = listType.GetElementType();
      else
      {
        var indexer = (DefaultMemberAttribute?)Attribute.GetCustomAttribute(listType, typeof(DefaultMemberAttribute));
        if (indexer != null)
        {
          foreach (PropertyInfo prop in listType.GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy))
          {
            if (prop.Name == indexer.MemberName)
              result = GetPropertyType(prop.PropertyType);
          }
        }
        if (result == null)
        {
          result = listType.GetMethod("get_Item")?.ReturnType;
        }
      }
      return result;
    }

    #region  CoerceValue

    /// <summary>
    /// Attempts to coerce a value of one type into
    /// a value of a different type.
    /// </summary>
    /// <param name="desiredType">
    /// Type to which the value should be coerced.
    /// </param>
    /// <param name="valueType">
    /// Original type of the value.
    /// </param>
    /// <param name="oldValue">
    /// The previous value (if any) being replaced by
    /// the new coerced value.
    /// </param>
    /// <param name="value">
    /// The value to coerce.
    /// </param>
    /// <remarks>
    /// <para>
    /// If the desired type is a primitive type or Decimal, 
    /// empty string and null values will result in a 0 
    /// or equivalent.
    /// </para>
    /// <para>
    /// If the desired type is a Nullable type, empty string
    /// and null values will result in a null result.
    /// </para>
    /// <para>
    /// If the desired type is an enum the value's ToString()
    /// result is parsed to convert into the enum value.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="desiredType"/> or <paramref name="valueType"/> is <see langword="null"/>.</exception>
    public static object? CoerceValue(Type desiredType, Type valueType, object? oldValue, object? value)
    {
      if (desiredType is null)
        throw new ArgumentNullException(nameof(desiredType));
      if (valueType is null)
        throw new ArgumentNullException(nameof(valueType));

      
      if (desiredType.IsAssignableFrom(valueType))
      {
        // types match, just return value
        return value;
      }
      
      if (desiredType.IsGenericType)
      {
        if (desiredType.GetGenericTypeDefinition() == typeof(Nullable<>))
          if (value == null)
            return null;
          else if (valueType.Equals(typeof(string)) && Convert.ToString(value) == string.Empty)
            return null;
      }

      var propertyType = GetPropertyType(desiredType) ?? throw new InvalidOperationException($"Type for ");
      if (propertyType.IsEnum)
      {
        if (value is byte? && ((byte?) value).HasValue)
          return Enum.Parse(desiredType, ((byte?) value).Value.ToString());
        if (value is short? && ((short?) value).HasValue)
          return Enum.Parse(desiredType, ((short?) value).Value.ToString());
        if (value is int? && ((int?) value).HasValue)
          return Enum.Parse(desiredType, ((int?) value).Value.ToString());
        if (value is long? && ((long?) value).HasValue)
          return Enum.Parse(desiredType, ((long?) value).Value.ToString());
      }

      if (propertyType.IsEnum && (valueType.Equals(typeof(string)) || Enum.GetUnderlyingType(propertyType).Equals(valueType)))
        return Enum.Parse(propertyType, value?.ToString() ?? throw new ArgumentException($"The value '{value ?? "<null>"}' could not be parsed into type {desiredType}.", nameof(value)));

      if (propertyType.Equals(typeof(SmartDate)) && oldValue != null)
      {
        if (value == null)
          value = string.Empty;

        var tmp = (SmartDate)oldValue;
        if (valueType.Equals(typeof(DateTime)))
          tmp.Date = (DateTime)value;
        else
          tmp.Text = value.ToString();
        return tmp;
      }

      if ((propertyType.IsPrimitive || propertyType.Equals(typeof(decimal))) && valueType.Equals(typeof(string)) && string.IsNullOrEmpty((string?)value))
        value = 0;

      try
      {
        if (propertyType.Equals(typeof(string)) && value != null)
        {
          return value.ToString();
        }
        else
          return Convert.ChangeType(value, propertyType);
      }
      catch when (value is not null)
      {
        TypeConverter cnv = TypeDescriptor.GetConverter(propertyType);
        TypeConverter cnv1 = TypeDescriptor.GetConverter(valueType);
        if (cnv != null && cnv.CanConvertFrom(valueType))
          return cnv.ConvertFrom(value);
        else if (cnv1 != null && cnv1.CanConvertTo(propertyType))
          return cnv1.ConvertTo(value, propertyType);
        else
          throw;
      }
    }

    /// <summary>
    /// Attempts to coerce a value of one type into
    /// a value of a different type.
    /// </summary>
    /// <typeparam name="D">
    /// Type to which the value should be coerced.
    /// </typeparam>
    /// <param name="valueType">
    /// Original type of the value.
    /// </param>
    /// <param name="oldValue">
    /// The previous value (if any) being replaced by
    /// the new coerced value.
    /// </param>
    /// <param name="value">
    /// The value to coerce.
    /// </param>
    /// <remarks>
    /// <para>
    /// If the desired type is a primitive type or Decimal, 
    /// empty string and null values will result in a 0 
    /// or equivalent.
    /// </para>
    /// <para>
    /// If the desired type is a Nullable type, empty string
    /// and null values will result in a null result.
    /// </para>
    /// <para>
    /// If the desired type is an enum the value's ToString()
    /// result is parsed to convert into the enum value.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="valueType"/> is <see langword="null"/>.</exception>
    public static D? CoerceValue<D>(Type valueType, object? oldValue, object? value)
    {
      if (valueType is null)
        throw new ArgumentNullException(nameof(valueType));

      return (D?)CoerceValue(typeof(D), valueType, oldValue, value);
    }

    #endregion

    internal static void ThrowIfAsyncMethodOnSyncClient(ApplicationContext applicationContext, bool isSync, System.Reflection.MethodInfo method)
    {
      if (isSync
        && applicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server
        && MethodCaller.IsAsyncMethod(method))
      {
        throw new NotSupportedException(string.Format(Resources.AsyncMethodOnSyncClientNotAllowed, method.Name));
      }
    }

    /// <summary>
    /// Throws an exception if a synchronous data portal call is trying to invoke an asynchronous method on the client.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <param name="obj">Object containing method.</param>
    /// <param name="methodName">Name of the method.</param>
    internal static void ThrowIfAsyncMethodOnSyncClient(ApplicationContext applicationContext, bool isSync, object obj, string methodName)
    {
      if (isSync
        && applicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server
        && MethodCaller.IsAsyncMethod(obj, methodName))
      {
        throw new NotSupportedException(string.Format(Resources.AsyncMethodOnSyncClientNotAllowed, methodName));
      }
    }

    /// <summary>
    /// Throws an exception if a synchronous data portal call is trying to invoke an asynchronous method on the client.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <param name="obj">Object containing method.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    internal static void ThrowIfAsyncMethodOnSyncClient(ApplicationContext applicationContext, bool isSync, object obj, string methodName, params object[] parameters)
    {
      if (isSync
        && applicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server
        && MethodCaller.IsAsyncMethod(obj, methodName, parameters))
      {
        throw new NotSupportedException(string.Format(Resources.AsyncMethodOnSyncClientNotAllowed, methodName));
      }
    }
  }

  /// <summary>
  /// Valid options for calling a property or method
  /// via the <see cref="Csla.Utilities.CallByName"/> method.
  /// </summary>
  public enum CallType
  {
    /// <summary>
    /// Gets a value from a property.
    /// </summary>
    Get,
    /// <summary>
    /// Sets a value into a property.
    /// </summary>
    Let,
    /// <summary>
    /// Invokes a method.
    /// </summary>
    Method,
    /// <summary>
    /// Sets a value into a property.
    /// </summary>
    Set
  }
}