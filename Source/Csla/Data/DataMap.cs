//-----------------------------------------------------------------------
// <copyright file="DataMap.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a mapping between two sets of</summary>
//-----------------------------------------------------------------------

using Csla.Properties;
using Csla.Reflection;

namespace Csla.Data
{
  /// <summary>
  /// Defines a mapping between two sets of
  /// properties/fields for use by
  /// DataMapper.
  /// </summary>
  public class DataMap
  {
    #region MapElement

    internal class MemberMapping
    {
      public DynamicMemberHandle FromMemberHandle { get; private set; }
      public DynamicMemberHandle ToMemberHandle { get; private set; }

      public MemberMapping(DynamicMemberHandle fromMemberHandle, DynamicMemberHandle toMemberHandle)
      {
        FromMemberHandle = fromMemberHandle;
        ToMemberHandle = toMemberHandle;
      }
    }

    #endregion

    private readonly Type _sourceType;
    private readonly Type _targetType;
    private readonly List<MemberMapping> _map = [];

    /// <summary>
    /// Initializes an instance of the type.
    /// </summary>
    /// <param name="sourceType">
    /// Type of source object.
    /// </param>
    /// <param name="targetType">
    /// Type of target object.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="sourceType"/> or <paramref name="targetType"/> is <see langword="null"/>.</exception>
    public DataMap(Type sourceType, Type targetType)
    {
      _sourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
      _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
    }

    /// <summary>
    /// Initializes an instance of the type.
    /// </summary>
    /// <param name="sourceType">
    /// Type of source object.
    /// </param>
    /// <param name="targetType">
    /// Type of target object.
    /// </param>
    /// <param name="includedPropertyNames">List of property names to map 1:1.</param>
    /// <exception cref="ArgumentNullException"><paramref name="sourceType"/>, <paramref name="targetType"/> or <paramref name="includedPropertyNames"/> is <see langword="null"/>.</exception>
    public DataMap(Type sourceType, Type targetType, IEnumerable<string> includedPropertyNames) : this(sourceType, targetType)
    {
      if (includedPropertyNames is null)
        throw new ArgumentNullException(nameof(includedPropertyNames));

      foreach (var item in includedPropertyNames)
        AddPropertyMapping(item, item);
    }

    internal List<MemberMapping> GetMap()
    {
      return _map;
    }

    /// <summary>
    /// Adds a property to property
    /// mapping.
    /// </summary>
    /// <param name="sourceProperty">
    /// Name of source property.
    /// </param>
    /// <param name="targetProperty">
    /// Name of target property.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="sourceProperty"/> or <paramref name="targetProperty"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public void AddPropertyMapping(string sourceProperty, string targetProperty)
    {
      if (string.IsNullOrWhiteSpace(sourceProperty))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(sourceProperty)), nameof(sourceProperty));
      if (string.IsNullOrWhiteSpace(targetProperty))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(targetProperty)), nameof(targetProperty));

      _map.Add(new MemberMapping(
        MethodCaller.GetCachedProperty(_sourceType, sourceProperty),
        MethodCaller.GetCachedProperty(_targetType, targetProperty)
      ));
    }

    /// <summary>
    /// Adds a field to field mapping.
    /// </summary>
    /// <param name="sourceField">
    /// Name of source field.
    /// </param>
    /// <param name="targetField">
    /// Name of target field.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="sourceField"/> or <paramref name="targetField"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public void AddFieldMapping(string sourceField, string targetField)
    {
      if (string.IsNullOrWhiteSpace(sourceField))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(sourceField)), nameof(sourceField));
      if (string.IsNullOrWhiteSpace(targetField))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(targetField)), nameof(targetField));

      _map.Add(new MemberMapping(
        MethodCaller.GetCachedField(_sourceType, sourceField),
        MethodCaller.GetCachedField(_targetType, targetField)
      ));
    }

    /// <summary>
    /// Adds a field to property mapping.
    /// </summary>
    /// <param name="sourceField">
    /// Name of source field.
    /// </param>
    /// <param name="targetProperty">
    /// Name of target property.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="sourceField"/> or <paramref name="targetProperty"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public void AddFieldToPropertyMapping(string sourceField, string targetProperty)
    {
      if (string.IsNullOrWhiteSpace(sourceField))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(sourceField)), nameof(sourceField));
      if (string.IsNullOrWhiteSpace(targetProperty))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(targetProperty)), nameof(targetProperty));

      _map.Add(new MemberMapping(
        MethodCaller.GetCachedField(_sourceType, sourceField),
        MethodCaller.GetCachedProperty(_targetType, targetProperty)
      ));
    }

    /// <summary>
    /// Adds a property to field mapping.
    /// </summary>
    /// <param name="sourceProperty">
    /// Name of source property.
    /// </param>
    /// <param name="targetField">
    /// Name of target field.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="sourceProperty"/> or <paramref name="targetField"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public void AddPropertyToFieldMapping(string sourceProperty, string targetField)
    {
      if (string.IsNullOrWhiteSpace(sourceProperty))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(sourceProperty)), nameof(sourceProperty));
      if (string.IsNullOrWhiteSpace(targetField))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(targetField)), nameof(targetField));

      _map.Add(new MemberMapping(
        MethodCaller.GetCachedProperty(_sourceType, sourceProperty),
        MethodCaller.GetCachedField(_targetType, targetField)
      ));
    }
  }
}