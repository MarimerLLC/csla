namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Defines the members required by a field
  /// data storage object.
  /// </summary>
  public interface IFieldData<T> : IFieldData
  {
    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    /// <value>The value of the field.</value>
    /// <returns>The value of the field.</returns>
    new T Value { get; set; }
  }
}
