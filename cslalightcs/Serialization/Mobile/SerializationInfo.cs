using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Object containing the serialization
  /// data for a specific object.
  /// </summary>
  public class SerializationInfo
  {
    private class ValueEntry
    {
      public string Name { get; set; }
      public object Value { get; set; }

      public ValueEntry(string name, object value)
      {
        this.Name = name;
        this.Value = value;
      }
    }

    private Dictionary<string, ValueEntry> _values = new Dictionary<string, ValueEntry>();

    internal SerializationInfo(int referenceId)
    {
      this.ReferenceId = referenceId;
    }

    internal int ReferenceId { get; private set; }
    /// <summary>
    /// Assembly-qualified type name of the
    /// object being serialized.
    /// </summary>
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
      _values.Add(name, new ValueEntry(name, value));
    }

    /// <summary>
    /// Gets a value from the serialization stream.
    /// </summary>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    /// <returns>
    /// Value of the field.
    /// </returns>
    public object GetValue(string name)
    {
      ValueEntry result;
      if (_values.TryGetValue(name, out result))
        return result.Value;
      else
        return null;
    }

    internal XElement ToXElement()
    {
      var root = new XElement("o");
      root.Add(new XAttribute("i", this.ReferenceId));
      root.Add(new XAttribute("t", this.TypeName));
      //root.Add(new XAttribute("a", this.AssemblyName));
      foreach (var item in _values)
      {
        var info = item.Value.Value as SerializationInfo;
        if (info == null)
        {
          var list = item.Value.Value as List<SerializationInfo>;
          if (list == null)
          {
            if (item.Value.Value != null)
              root.Add(new XElement("f",
                new XAttribute("n", item.Value.Name),
                new XAttribute("v", item.Value.Value)));
                //new XAttribute("t", item.Value.ValueType.FullName),
          }
          else
          {
            var listElement = new XElement("l",
              new XAttribute("n", item.Value.Name));
            foreach (var listItem in list)
              listElement.Add(new XElement("r",
                new XAttribute("i", listItem.ReferenceId)));
            root.Add(listElement);
          }
        }
        else
          root.Add(new XElement("r",
            new XAttribute("n", item.Value.Name),
            new XAttribute("i", info.ReferenceId)));
      }
      return root;
    }

    internal SerializationInfo(XElement data)
    {
      this.ReferenceId = Convert.ToInt32(data.Attribute("i").Value);
      if (data.Name == "o")
        this.TypeName = (string)data.Attribute("t").Value;
      //this.AssemblyName = (string)data.Attribute("a").Value;
    }

    internal void Deserialize(XElement data, MobileFormatter formatter)
    {
      foreach (var item in data.Elements())
      {
        if (item.Name == "f")
        {
          var entry = new ValueEntry(
            item.Attribute("n").Value, item.Attribute("v").Value);
          _values.Add(entry.Name, entry);
        }
        else if (item.Name == "l")
        {
          var listItems = new List<SerializationInfo>();
          foreach (var content in item.Elements())
            listItems.Add(new SerializationInfo(content));

          var entry = new ValueEntry(item.Attribute("n").Value, listItems);
          _values.Add(entry.Name, entry);
        }
        else
        {
          int referenceId = Convert.ToInt32(item.Attribute("i").Value);
          var entry = new ValueEntry(
            item.Attribute("n").Value, 
            new SerializationInfo(referenceId));
          _values.Add(entry.Name, entry);
        }
      }
    }
  }
}
