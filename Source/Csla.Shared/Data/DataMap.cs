//-----------------------------------------------------------------------
// <copyright file="DataMap.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a mapping between two sets of</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
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

    private Type _sourceType;
    private Type _targetType;
    private List<MemberMapping> _map = new List<MemberMapping>();

    /// <summary>
    /// Initializes an instance of the type.
    /// </summary>
    /// <param name="sourceType">
    /// Type of source object.
    /// </param>
    /// <param name="targetType">
    /// Type of target object.
    /// </param>
    public DataMap(Type sourceType, Type targetType)
    {
      _sourceType = sourceType;
      _targetType = targetType;
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
    public DataMap(Type sourceType, Type targetType, IEnumerable<string> includedPropertyNames)
    {
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
    public void AddPropertyMapping(string sourceProperty, string targetProperty)
    {
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
    public void AddFieldMapping(string sourceField, string targetField)
    {
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
    public void AddFieldToPropertyMapping(string sourceField, string targetProperty)
    {
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
    public void AddPropertyToFieldMapping(string sourceProperty, string targetField)
    {
      _map.Add(new MemberMapping(
         MethodCaller.GetCachedProperty(_sourceType, sourceProperty),
         MethodCaller.GetCachedField(_targetType, targetField)
      ));
    }
  }
}