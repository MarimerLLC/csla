using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla
{
  /// <summary>
  /// Maintains metadata about a property.
  /// </summary>
  /// <typeparam name="T">
  /// Data type of the property.
  /// </typeparam>
  public class PropertyInfo<T> : Core.IPropertyInfo
  {
    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    public PropertyInfo(string name)
      : this(name, "")
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    public PropertyInfo(string name, string friendlyName)
    {
      _name = name;
      _friendlyName = friendlyName;
      if (typeof(T).Equals(typeof(string)))
        _defaultValue = (T)((object)string.Empty);
      else
        _defaultValue = default(T);
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    public PropertyInfo(string name, T defaultValue)
    {
      _name = name;
      _defaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    public PropertyInfo(string name, string friendlyName, T defaultValue)
    {
      _name = name;
      _defaultValue = defaultValue;
      _friendlyName = friendlyName;
    }

    private string _name;
    /// <summary>
    /// Gets the property name value.
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
    }

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    public Type Type
    {
      get
      {
        return typeof(T);
      }
    }

    private string _friendlyName;
    /// <summary>
    /// Gets the friendly display name
    /// for the property.
    /// </summary>
    /// <remarks>
    /// If no friendly name was provided, the
    /// property name itself is returned as a
    /// result.
    /// </remarks>
    public string FriendlyName
    {
      get
      {
        if (!(string.IsNullOrEmpty(_friendlyName)))
        {
          return _friendlyName;

        }
        else
        {
          return _name;
        }
      }
    }

    private T _defaultValue;
    /// <summary>
    /// Gets the default initial value for the property.
    /// </summary>
    /// <remarks>
    /// This value is used to initialize the property's
    /// value, and is returned from a property get
    /// if the user is not authorized to 
    /// read the property.
    /// </remarks>
    public T DefaultValue
    {
      get
      {
        return _defaultValue;
      }
    }

    object Core.IPropertyInfo.DefaultValue
    {
      get
      {
        return _defaultValue;
      }
    }

    Core.FieldManager.IFieldData Core.IPropertyInfo.NewFieldData(string name)
    {
      return new Core.FieldManager.FieldData<T>(name);
    }
  }
}
