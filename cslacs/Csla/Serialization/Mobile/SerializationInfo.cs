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
  [DataContract]
  public class SerializationInfo
  {
    /// <summary>
    /// Object that contains information about
    /// a single field.
    /// </summary>
    [DataContract]
    public class FieldData
    {
      /// <summary>
      /// Field value.
      /// </summary>
      [DataMember]
      public object Value { get; set; }
      /// <summary>
      /// Indicates whether the field is dirty.
      /// </summary>
      [DataMember]
      public bool IsDirty { get; set; }
    }

    /// <summary>
    /// Object that contains information about
    /// a single child reference.
    /// </summary>
    [DataContract]
    public class ChildData
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
  }
}
