﻿//-----------------------------------------------------------------------
// <copyright file="SerializationInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object containing the serialization</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Object containing the serialization
  /// data for a specific object.
  /// </summary>
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [Serializable]
  [DataContract]
  public class SerializationInfo : IMobileObject
  {
    /// <summary>
    /// Object that contains information about
    /// a single field.
    /// </summary>
    [Serializable]
    [DataContract]
    public class FieldData : IMobileObject
    {
      /// <summary>
      /// Field value.
      /// </summary>
      [DataMember]
      public object Value { get; set; }

      /// <summary>
      /// If non-null, indicates that the value is a integer value representing the
      /// specified enum type. Upon deserialization, the integer will be converted back
      /// to the enum type.
      /// </summary>
      [DataMember]
      public string EnumTypeName { get; set; }

      /// <summary>
      /// Indicates whether the field is dirty.
      /// </summary>
      [DataMember]
      public bool IsDirty { get; set; }

      #region IMobileObject Members

      /// <summary>
      /// Creates an instance of the type.
      /// </summary>
      public FieldData() { }

      /// <summary>
      /// Gets state information.
      /// </summary>
      /// <param name="info">Serialization context.</param>
      public void GetState(SerializationInfo info)
      {
        info.AddValue("FieldData.Value", Value);
        info.AddValue("FieldData.EnumTypeName", EnumTypeName);
        info.AddValue("FieldData.IsDirty", IsDirty);
      }

      /// <summary>
      /// Gets child serialization information.
      /// </summary>
      /// <param name="info">Serialization context.</param>
      /// <param name="formatter">Serializer instance.</param>
      public void GetChildren(SerializationInfo info, MobileFormatter formatter)
      {
      }

      /// <summary>
      /// Sets state information.
      /// </summary>
      /// <param name="info">Serialization context.</param>
      public void SetState(SerializationInfo info)
      {
        Value = info.GetValue<object>("FieldData.Value");
        EnumTypeName = info.GetValue<string>("FieldData.EnumTypeName");
        IsDirty = info.GetValue<bool>("FieldData.IsDirty");
      }

      /// <summary>
      /// Sets child serialization information.
      /// </summary>
      /// <param name="info">Serialization context.</param>
      /// <param name="formatter">Serializer instance.</param>
      public void SetChildren(SerializationInfo info, MobileFormatter formatter)
      {
      }

      #endregion
    }

    /// <summary>
    /// Object that contains information about
    /// a single child reference.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ChildData : IMobileObject
    {
      /// <summary>
      /// Reference number for the child.
      /// </summary>
      [DataMember]
      public int ReferenceId { get; set; }
      /// <summary>
      /// Indicates whether the child is dirty.
      /// </summary>
      [DataMember]
      public bool IsDirty { get; set; }

      #region IMobileObject Members

      /// <summary>
      /// Creates instance of object.
      /// </summary>
      public ChildData() { }

      /// <summary>
      /// Method called by MobileFormatter when an object
      /// should serialize its data. The data should be
      /// serialized into the SerializationInfo parameter.
      /// </summary>
      /// <param name="info">
      /// Object to contain the serialized data.
      /// </param>
      public void GetState(SerializationInfo info)
      {
        info.AddValue("ChildData.ReferenceId", ReferenceId);
        info.AddValue("ChildData.IsDirty", IsDirty);
      }

      /// <summary>
      /// Method called by MobileFormatter when an object
      /// should serialize its child references. The data should be
      /// serialized into the SerializationInfo parameter.
      /// </summary>
      /// <param name="info">
      /// Object to contain the serialized data.
      /// </param>
      /// <param name="formatter">
      /// Reference to the formatter performing the serialization.
      /// </param>
      public void GetChildren(SerializationInfo info, MobileFormatter formatter)
      {
      }

      /// <summary>
      /// Method called by MobileFormatter when an object
      /// should be deserialized. The data should be
      /// deserialized from the SerializationInfo parameter.
      /// </summary>
      /// <param name="info">
      /// Object containing the serialized data.
      /// </param>
      public void SetState(SerializationInfo info)
      {
        ReferenceId = info.GetValue<int>("ChildData.ReferenceId");
        IsDirty = info.GetValue<bool>("ChildData.IsDirty");
      }

      /// <summary>
      /// Method called by MobileFormatter when an object
      /// should deserialize its child references. The data should be
      /// deserialized from the SerializationInfo parameter.
      /// </summary>
      /// <param name="info">
      /// Object containing the serialized data.
      /// </param>
      /// <param name="formatter">
      /// Reference to the formatter performing the deserialization.
      /// </param>
      public void SetChildren(SerializationInfo info, MobileFormatter formatter)
      {
      }

      #endregion
    }

    private Dictionary<string, ChildData> _children = [];
    /// <summary>
    /// Dictionary containing child reference data.
    /// </summary>
    [DataMember]
    public Dictionary<string, ChildData> Children
    {
      get { return _children; }
      set { _children = value; }
    }

    private Dictionary<string, FieldData> _values = [];
    /// <summary>
    /// Dictionary containg field data.
    /// </summary>
    [DataMember]
    public Dictionary<string, FieldData> Values
    {
      get { return _values; }
      set { _values = value; }
    }

    internal SerializationInfo(int referenceId)
    {
      ReferenceId = referenceId;
    }

    /// <summary>
    /// Reference number for this object.
    /// </summary>
    [DataMember]
    public int ReferenceId { get; set; }
    /// <summary>
    /// Assembly-qualified type name of the
    /// object being serialized.
    /// </summary>
    [DataMember]
    public string TypeName { get; set; }

    /// <summary>
    /// Adds a value to the serialization stream.
    /// </summary>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    /// <param name="value">
    /// Value of the field.
    /// </param>
    public void AddValue(string name, object value)
    {
      AddValue(name, value, false);
    }

    /// <summary>
    /// Adds a value to the list of fields.
    /// </summary>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    /// <param name="value">
    /// Value of the field.
    /// </param>
    /// <param name="isDirty">
    /// Flag indicating whether the value is dirty.
    /// </param>
    public void AddValue(string name, object value, bool isDirty)
    {
      _values.Add(name, new FieldData { Value = value, IsDirty = isDirty });
    }

    /// <summary>
    /// Adds a value to the list of fields.
    /// </summary>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    /// <param name="value">
    /// Value of the field.
    /// </param>
    /// <param name="isDirty">
    /// Flag indicating whether the value is dirty.
    /// </param>
    /// <param name="enumTypeName">
    /// Name of the enumeration
    /// </param>
    public void AddValue(string name, object value, bool isDirty, string enumTypeName)
    {
      _values.Add(name, new FieldData { Value = value, IsDirty = isDirty, EnumTypeName = enumTypeName});
    }

    /// <summary>
    /// Gets a value from the list of fields.
    /// </summary>
    /// <typeparam name="T">
    /// Type to which the value should be coerced.
    /// </typeparam>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    public T GetValue<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(string name)
    {
      try
      {
        var value = _values[name].Value;
        return (value != null ? Utilities.CoerceValue<T>(value.GetType(), null, value) : (T)value);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"SerializationInfo.GetValue: {name}", ex);
      }
    }

    /// <summary>
    /// Adds a child to the list of child references.
    /// </summary>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    /// <param name="referenceId">
    /// Reference id for the child object.
    /// </param>
    public void AddChild(string name, int referenceId)
    {
      AddChild(name, referenceId, false);
    }

    /// <summary>
    /// Adds a child to the list of child references.
    /// </summary>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    /// <param name="referenceId">
    /// Reference id for the child object.
    /// </param>
    /// <param name="isDirty">
    /// Flag indicating whether the child is dirty.
    /// </param>
    public void AddChild(string name, int referenceId, bool isDirty)
    {
      _children.Add(name, new ChildData { ReferenceId = referenceId, IsDirty = isDirty });
    }

    #region IMobileObject Members

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public SerializationInfo() { }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should serialize its data. The data should be
    /// serialized into the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object to contain the serialized data.
    /// </param>
    public void GetState(SerializationInfo info)
    {
      info.AddValue("SerializationInfo.ReferenceId", ReferenceId);
      info.AddValue("SerializationInfo.TypeName", TypeName);
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should serialize its child references. The data should be
    /// serialized into the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object to contain the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the formatter performing the serialization.
    /// </param>
    public void GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (string key in _children.Keys)
      {
        ChildData value = _children[key];
        SerializationInfo si = formatter.SerializeObject(value);
        info.AddChild(key, si.ReferenceId);
      }
      foreach (string key in _values.Keys)
      {
        FieldData value = _values[key];
        SerializationInfo si = formatter.SerializeObject(value);
        info.AddChild(key, si.ReferenceId);
      }
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should be deserialized. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    public void SetState(SerializationInfo info)
    {
      ReferenceId = info.GetValue<int>("SerializationInfo.ReferenceId");
      TypeName = info.GetValue<string>("SerializationInfo.TypeName");
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should deserialize its child references. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the formatter performing the deserialization.
    /// </param>
    public void SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (string key in info.Children.Keys)
      {
        int referenceId = info.Children[key].ReferenceId;
        object serialized = formatter.GetObject(referenceId);
        if (serialized is ChildData data)
        {
          _children.Add(key, data);
        }
        else
        {
          _values.Add(key, (FieldData)serialized);
        }
      }
    }

    #endregion

    /// <summary>
    /// Returns true if the type is considered a simple
    /// (non-complex) serializable type by MobileFormatter.
    /// </summary>
    /// <param name="type">Type to evaluate</param>
    /// <returns></returns>
    public static bool IsNativeType(Type type)
    {
      bool result;
      if (type is IMobileObject)
        result = false;
      else if (type.IsPrimitive || type.IsEnum)
        result = true;
      else
        result = Enum.TryParse<CslaKnownTypes>(type.Name.Replace("[]", "Array"), out _);

      return result;
    }
  }
}