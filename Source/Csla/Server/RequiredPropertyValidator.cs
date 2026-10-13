//-----------------------------------------------------------------------
// <copyright file="RequiredPropertyValidator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Checks required properties after a data portal operation</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Server
{
  /// <summary>
  /// Checks that properties registered with
  /// <see cref="RelationshipTypes.Required"/> have
  /// a non-null value after a data portal operation.
  /// </summary>
  internal static class RequiredPropertyValidator
  {
    /// <summary>
    /// Throws a <see cref="RequiredPropertyNotSetException"/>
    /// if any required property of the object is null.
    /// </summary>
    /// <param name="instance">Object to check.</param>
    public static void ThrowIfNotSet(object? instance)
    {
      // HasManagedProperties is false when the field manager
      // hasn't been created, so the check doesn't force it
      // (and the type's static property registration) into
      // existence on objects that never used it.
      if (instance is not IManageProperties { HasManagedProperties: true } target)
        return;

      List<string>? missing = null;
      foreach (var property in target.GetManagedProperties())
      {
        var relationship = property.RelationshipType;
        if ((relationship & RelationshipTypes.Required) != RelationshipTypes.Required)
          continue;
        if ((relationship & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad && !target.FieldExists(property))
          continue;
        if (target.ReadProperty(property) is null)
          (missing ??= []).Add(property.Name);
      }

      if (missing is not null)
        throw new RequiredPropertyNotSetException(instance.GetType(), missing);
    }
  }
}
