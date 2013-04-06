using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
      public FieldData() { }
      public void GetState(SerializationInfo info)
      {
        info.AddValue("FieldData.Value", Value);
        info.AddValue("FieldData.EnumTypeName", EnumTypeName);
        info.AddValue("FieldData.IsDirty", IsDirty);
      }

      public void GetChildren(SerializationInfo info, MobileFormatter formatter)
      {
      }

      public void SetState(SerializationInfo info)
      {
        Value = info.GetValue<object>("FieldData.Value");
        EnumTypeName = info.GetValue<string>("FieldData.EnumTypeName");
        IsDirty = info.GetValue<bool>("FieldData.IsDirty");
      }

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
      public ChildData() { }
      public void GetState(SerializationInfo info)
      {
        info.AddValue("ChildData.ReferenceId", ReferenceId);
        info.AddValue("ChildData.IsDirty", IsDirty);
      }

      public void GetChildren(SerializationInfo info, MobileFormatter formatter)
      {
      }

      public void SetState(SerializationInfo info)
      {
        ReferenceId = info.GetValue<int>("ChildData.ReferenceId");
        IsDirty = info.GetValue<bool>("ChildData.IsDirty");
      }

      public void SetChildren(SerializationInfo info, MobileFormatter formatter)
      {
      }

      #endregion
    }

    private Dictionary<string, ChildData> _children = new Dictionary<string, ChildData>();
    /// <summary>
    /// Dictionary containing child reference data.
    /// </summary>
    [DataMember()]
    public Dictionary<string, ChildData> Children
    {
      get { return _children; }
      set { _children = value; }
    }

    private Dictionary<string, FieldData> _values = new Dictionary<string, FieldData>();
    /// <summary>
    /// Dictionary containg field data.
    /// </summary>
    [DataMember()]
    public Dictionary<string, FieldData> Values
    {
      get { return _values; }
      set { _values = value; }
    }

    internal SerializationInfo(int referenceId)
    {
      this.ReferenceId = referenceId;
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
    /// Gets a value from the list of fields.
    /// </summary>
    /// <typeparam name="T">
    /// Type to which the value should be coerced.
    /// </typeparam>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    /// <returns></returns>
    public T GetValue<T>(string name)
    {
      var value = _values[name].Value;
      return (value != null ? (T)Utilities.CoerceValue<T>(value.GetType(), null, value) : (T)value);
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

    public SerializationInfo() { }

    public void GetState(SerializationInfo info)
    {
      info.AddValue("SerializationInfo.ReferenceId", ReferenceId);
      info.AddValue("SerializationInfo.TypeName", TypeName);
    }

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

    public void SetState(SerializationInfo info)
    {
      ReferenceId = info.GetValue<int>("SerializationInfo.ReferenceId");
      TypeName = info.GetValue<string>("SerializationInfo.TypeName");
    }

    public void SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (string key in info.Children.Keys)
      {
        int referenceId = info.Children[key].ReferenceId;
        object serialized = formatter.GetObject(referenceId);
        if (serialized is ChildData)
        {
          _children.Add(key, (ChildData)serialized);
        }
        else
        {
          _values.Add(key, (FieldData)serialized);
        }
      }
    }

    #endregion
  }
}
