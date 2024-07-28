//-----------------------------------------------------------------------
// <copyright file="ExtractedPropertyDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a property, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------

using Csla.Generator.AutoImplementProperties.CSharp.Internals;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement
{

  /// <summary>
  /// The definition of a property, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedPropertyDefinition : IMemberDefinition, IEquatable<ExtractedPropertyDefinition>
  {

    /// <summary>
    /// The name of the property
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// The definition of the type of this property
    /// </summary>
    public ExtractedMemberTypeDefinition TypeDefinition { get; } = new ExtractedMemberTypeDefinition();

    /// <summary>
    /// The member name for the field
    /// </summary>
    string IMemberDefinition.MemberName => PropertyName;

    /// <summary>
    /// The attribute definitions for this property
    /// </summary>
    public List<ExtractedAttributeDefinition> AttributeDefinitions { get; } = new List<ExtractedAttributeDefinition>();

    /// <summary>
    /// Gets or sets a value indicating whether this property has a getter.
    /// </summary>
    public bool Getter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this property has a setter.
    /// </summary>
    public bool Setter { get; set; }

    /// <summary>
    /// The modifiers for this property
    /// </summary>
    public string[] Modifiers { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether this property is partial.
    /// </summary>
    public bool Partial { get; internal set; }

    /// <summary>
    /// The modifiers for the setter of this property
    /// </summary>
    public string[] SetterModifiers { get; internal set; }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as ExtractedPropertyDefinition);
    }

    /// <summary>
    /// Determines whether the specified ExtractedPropertyDefinition is equal to the current ExtractedPropertyDefinition.
    /// </summary>
    /// <param name="other">The ExtractedPropertyDefinition to compare with the current ExtractedPropertyDefinition.</param>
    /// <returns>True if the specified ExtractedPropertyDefinition is equal to the current ExtractedPropertyDefinition; otherwise, false.</returns>
    public bool Equals(ExtractedPropertyDefinition other)
    {
      if (other == null)
        return false;

      if (ReferenceEquals(this, other))
        return true;

      return PropertyName == other.PropertyName &&
             TypeDefinition.Equals(other.TypeDefinition) &&
             Getter == other.Getter &&
             Setter == other.Setter &&
             Modifiers.SequenceEqual(other.Modifiers) &&
             Partial == other.Partial &&
             SetterModifiers.SequenceEqual(other.SetterModifiers);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      return HashCode.Combine(PropertyName, TypeDefinition, Getter, Setter, Modifiers, Partial, SetterModifiers);
    }
  }
}
