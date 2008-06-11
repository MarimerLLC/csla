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
    private Dictionary<string, int> _children = new Dictionary<string, int>();
    [DataMember()]
    public Dictionary<string, int> Children
    {
      get { return _children; }
      set { _children = value; }
    }

    private Dictionary<string, string> _values = new Dictionary<string, string>();
    [DataMember()]
    public Dictionary<string, string> Values
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
      // TODO: convert to string properly
      _values.Add(name, value.ToString());
    }

    public void AddChild(string name, int referenceId)
    {
      _children.Add(name, referenceId);
    }
  }
}
