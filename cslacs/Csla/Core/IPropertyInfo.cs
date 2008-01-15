using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Maintains metadata about a property.
  /// </summary>
  public interface IPropertyInfo
  {
    /// <summary>
    /// Gets the property name value.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    Type Type { get; }
    /// <summary>
    /// Gets the friendly display name
    /// for the property.
    /// </summary>
    string FriendlyName { get; }
    /// <summary>
    /// Gets the default initial value for the property.
    /// </summary>
    /// <remarks>
    /// This value is used to initialize the property's
    /// value, and is returned from a property get
    /// if the user is not authorized to 
    /// read the property.
    /// </remarks>
    object DefaultValue { get; }
    /// <summary>
    /// Gets a new field data container for the property.
    /// </summary>
    Core.FieldManager.IFieldData NewFieldData(string name);
  }
}
