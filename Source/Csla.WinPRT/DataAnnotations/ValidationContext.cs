using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
  /// <summary>
  /// Validation context information.
  /// </summary>
  public class ValidationContext
  {
#if UNIVERSAL_PCL
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="instance">Object to validate.</param>
    public ValidationContext(object instance)
    {
      ObjectInstance = instance;
    }
#endif
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="instance">Object to validate.</param>
    /// <param name="serviceProvider">Ignored.</param>
    /// <param name="items">Collection of name/value pairs.</param>
    public ValidationContext(object instance, object serviceProvider, IDictionary<object, object> items)
    {
      ObjectInstance = instance;
      Items = items;
    }
    /// <summary>
    /// Gets or sets the display name of the member
    /// to validate.
    /// </summary>
    public string DisplayName { get; set; }
    /// <summary>
    /// Gets or sets the name of the member to validate.
    /// </summary>
    public string MemberName { get; set; }
    /// <summary>
    /// Gets the object to validate.
    /// </summary>
    public object ObjectInstance { get; protected set; }
    /// <summary>
    /// Gets the type of the object to validate.
    /// </summary>
    public Type ObjectType { get; protected set; }
    /// <summary>
    /// Gets the dictionary of items that is associated with this context.
    /// </summary>
    public IDictionary<object, object> Items { get; protected set; }
  }
}
