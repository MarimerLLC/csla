//-----------------------------------------------------------------------
// <copyright file="RelationshipTypes.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>List of valid relationship types</summary>
//-----------------------------------------------------------------------

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
    None = 0x0,
    /// <summary>
    /// Property is a reference to a lazy
    /// loaded object. Attempting to get
    /// or read the property value
    /// prior to a set or load will result in
    /// an exception.
    /// </summary>
    LazyLoad=0x2,
    /// <summary>
    /// Property is stored in a private field. Attempting
    /// to read or write the property in FieldManager
    /// (managed fields) will throw an exception.
    /// NonGeneric ReadProperty/LoadProperty will call
    /// property get/set methods. 
    /// </summary>
    PrivateField=0x4,
    /// <summary>
    /// Property must have a non-null value when a
    /// data portal operation (create, fetch, update
    /// or execute) completes. If the value is null
    /// a <see cref="RequiredPropertyNotSetException"/>
    /// is thrown. A lazy loaded property is only
    /// checked once it has been loaded.
    /// </summary>
    /// <remarks>
    /// SetProperty converts a null string to
    /// <see cref="string.Empty"/>, so a required string
    /// property is only null if it is loaded with null.
    /// An object whose operation didn't read or write
    /// any managed property has no field manager yet
    /// and is not checked.
    /// Property setters never throw for a null value,
    /// so data binding is not affected. However, when a
    /// Windows Forms grid adds a row to an editable list,
    /// the list calls the child data portal to create the
    /// item, so a CreateChild method that leaves a required
    /// property null throws during the grid's AddNew.
    /// </remarks>
    Required=0x8,
  }
}