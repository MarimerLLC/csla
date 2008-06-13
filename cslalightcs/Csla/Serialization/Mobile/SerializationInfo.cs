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
  [DataContract]
  public class SerializationInfo
  {
    [DataContract]
    public class FieldData
    {
      [DataMember]
      public object Value { get; set; }
      [DataMember]
      public bool IsDirty { get; set; }
    }
    [DataContract]
    public class ChildData
    {
      [DataMember]
      public int ReferenceId { get; set; }
      [DataMember]
      public bool IsDirty { get; set; }
    }

    private Dictionary<string, ChildData> _children = new Dictionary<string, ChildData>();
    [DataMember()]
    public Dictionary<string, ChildData> Children
    {
      get { return _children; }
      set { _children = value; }
    }

    private Dictionary<string, FieldData> _values = new Dictionary<string, FieldData>();
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

    public void AddValue(string name, object value, bool isDirty)
    {
      _values.Add(name, new FieldData { Value = value, IsDirty = isDirty });
    }

    public void AddChild(string name, int referenceId)
    {
      AddChild(name, referenceId, false);
    }
    public void AddChild(string name, int referenceId, bool isDirty)
    {
      _children.Add(name, new ChildData { ReferenceId = referenceId, IsDirty = isDirty });
    }
  }
}
