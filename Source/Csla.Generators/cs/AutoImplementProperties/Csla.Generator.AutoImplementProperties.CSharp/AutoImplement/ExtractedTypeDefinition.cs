using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;

using Csla.Generator.AutoImplementProperties.CSharp.Internals;

/// <summary>
/// The definition of a type, extracted from the syntax tree provided by Roslyn
/// </summary>
public class ExtractedTypeDefinition : IEquatable<ExtractedTypeDefinition>
{
  /// <summary>
  /// The namespace in which the type resides
  /// </summary>
  public string Namespace { get; set; }

  /// <summary>
  /// The scope of the class
  /// </summary>
  public string Scope { get; set; } = "public";

  /// <summary>
  /// The name of the type, excluding any namespace
  /// </summary>
  public string TypeName { get; set; }

  /// <summary>
  /// The name of the kind of type being represented
  /// </summary>
  public string TypeKind { get; set; }

  /// <summary>
  /// The fully qualified name of the type, including namespace
  /// </summary>
  public string FullyQualifiedName { get; set; }

  /// <summary>
  /// The properties to be included in auto implementation
  /// </summary>
  public IList<ExtractedPropertyDefinition> Properties { get; private set; } = new List<ExtractedPropertyDefinition>();

  /// <summary>
  /// The name of the base class for the type
  /// </summary>
  public string BaseClassTypeName { get; internal set; }

  /// <summary>
  /// The modifiers for this property
  /// </summary>
  public string[] DefaultPropertyModifiers { get; internal set; }

  /// <summary>
  /// The modifiers for the setter of this property
  /// </summary>
  public string[] DefaultPropertySetterModifiers { get; internal set; }
  /// <summary>
  /// Determines whether the current <see cref="ExtractedTypeDefinition"/> object is equal to another object of the same type.
  /// </summary>
  /// <param name="other">The object to compare with the current object.</param>
  /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
  public bool Equals(ExtractedTypeDefinition other)
  {
    if (other == null)
      return false;

    if (ReferenceEquals(this, other))
      return true;

    // Add your comparison logic here
    // Compare properties, fields, or any other relevant data

    return Namespace == other.Namespace &&
           Scope == other.Scope &&
           TypeName == other.TypeName &&
           TypeKind == other.TypeKind &&
           FullyQualifiedName == other.FullyQualifiedName &&
           Properties.SequenceEqual(other.Properties) &&
           BaseClassTypeName == other.BaseClassTypeName &&
           DefaultPropertyModifiers.SequenceEqual(other.DefaultPropertyModifiers) &&
           DefaultPropertySetterModifiers.SequenceEqual(other.DefaultPropertySetterModifiers);
  }
  /// <summary>
  /// Determines whether the current <see cref="ExtractedTypeDefinition"/> object is equal to another object.
  /// </summary>
  /// <param name="obj">The object to compare with the current object.</param>
  /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
  public override bool Equals(object obj)
  {
    if (obj == null || GetType() != obj.GetType())
      return false;

    return Equals(obj as ExtractedTypeDefinition);
  }
  /// <summary>
  /// Calculates the hash code for the <see cref="ExtractedTypeDefinition"/> object.
  /// </summary>
  /// <returns>The calculated hash code.</returns>
  public override int GetHashCode()
  {
    // Calculate and return the hash code based on the properties, fields, or any other relevant data
    return HashCode.Combine(Namespace + Scope + TypeName, TypeKind, FullyQualifiedName, Properties, BaseClassTypeName, DefaultPropertyModifiers, DefaultPropertySetterModifiers);
  }
}
