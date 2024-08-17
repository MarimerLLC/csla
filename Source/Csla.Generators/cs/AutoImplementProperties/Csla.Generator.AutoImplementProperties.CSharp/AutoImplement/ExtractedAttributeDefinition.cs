/// <summary>
/// class for extracted attribute definition
/// </summary>
public class ExtractedAttributeDefinition : IEquatable<ExtractedAttributeDefinition>
{
  /// <summary>
  /// The name of the attribute.
  /// </summary>
  public string AttributeName { get; set; }

  /// <summary>
  /// The namespace of the attribute.
  /// </summary>
  public string AttributeNamespace { get; set; }

  /// <summary>
  /// A list of arguments passed to the attribute's constructor.
  /// </summary>
  public List<object> ConstructorArguments { get; set; } = new List<object>();

  /// <summary>
  /// A dictionary to hold named properties and their values.
  /// </summary>
  public Dictionary<string, object> NamedProperties { get; set; } = new Dictionary<string, object>();
  /// <summary>
  /// Determines whether the current <see cref="ExtractedAttributeDefinition"/> object is equal to another object of the same type.
  /// </summary>
  /// <param name="other">The object to compare with the current object.</param>
  /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
  public bool Equals(ExtractedAttributeDefinition other)
  {
    if (ReferenceEquals(null, other))
      return false;
    if (ReferenceEquals(this, other))
      return true;
    return AttributeName == other.AttributeName && AttributeNamespace == other.AttributeNamespace &&
           ConstructorArguments.SequenceEqual(other.ConstructorArguments) &&
           NamedProperties.SequenceEqual(other.NamedProperties);
  }
  /// <summary>
  /// Determines whether the current <see cref="ExtractedAttributeDefinition"/> object is equal to another object.
  /// </summary>
  /// <param name="obj">The object to compare with the current object.</param>
  /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
  public override bool Equals(object obj)
  {
    if (ReferenceEquals(null, obj))
      return false;
    if (ReferenceEquals(this, obj))
      return true;
    if (obj.GetType() != GetType())
      return false;
    return Equals((ExtractedAttributeDefinition)obj);
  }
  /// <summary>
  /// Calculates the hash code for the <see cref="ExtractedAttributeDefinition"/> object.
  /// </summary>
  /// <returns>The calculated hash code.</returns>
  public override int GetHashCode()
  {
    unchecked
    {
      int hashCode = AttributeName != null ? AttributeName.GetHashCode() : 0;
      hashCode = (hashCode * 397) ^ (AttributeNamespace != null ? AttributeNamespace.GetHashCode() : 0);
      hashCode = (hashCode * 397) ^ (ConstructorArguments != null ? ConstructorArguments.GetHashCode() : 0);
      hashCode = (hashCode * 397) ^ (NamedProperties != null ? NamedProperties.GetHashCode() : 0);
      return hashCode;
    }
  }
}
