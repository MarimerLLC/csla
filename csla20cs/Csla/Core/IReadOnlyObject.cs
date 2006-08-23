namespace Csla.Core
{
  /// <summary>
  /// Specifies that the object is a readonly
  /// business object.
  /// </summary>
  public interface IReadOnlyObject : IBusinessObject
  {
    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    bool CanReadProperty(string propertyName);
  }
}
