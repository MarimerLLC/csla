using System;

namespace Csla
{
  /// <summary>
  /// List of valid relationship types
  /// between a parent object and another
  /// object through a managed property.
  /// </summary>
  [Flags]
  public enum RelationshipTypes
  {
    /// <summary>
    /// The default value, indicating all values are cleared
    /// </summary>
    None = 0,
    /// <summary>
    /// Property is a reference to a child
    /// object contained by the parent.
    /// </summary>
    Child,
    /// <summary>
    /// Property is a reference to a lazy
    /// loaded object. Attempting to get
    /// or read the property value
    /// prior to a set or load will result in 
    /// an exception.
    /// </summary>
    LazyLoad,
    /// <summary>
    /// Property is stored in a private field. Attemting 
    /// to read or write the property in FieldManager 
    /// (managed fields) will throw an exception. 
    /// NonGeneric ReadProperty/LoadProperty will call
    /// property get/set methods. 
    /// </summary>
    PrivateField,
  }
}
