public class ExtractedAttributeDefinition
{
  /// <summary>
  /// The name of the attribute.
  /// </summary>
  public string AttributeName { get; set; }

  /// <summary>
  /// A list of arguments passed to the attribute's constructor.
  /// </summary>
  public List<object> ConstructorArguments { get; set; } = new List<object>();

  /// <summary>
  /// A dictionary to hold named properties and their values.
  /// </summary>
  public Dictionary<string, object> NamedProperties { get; set; } = new Dictionary<string, object>();
}
