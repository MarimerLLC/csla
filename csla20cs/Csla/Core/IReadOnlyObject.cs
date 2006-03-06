namespace Csla.Core
{
  /// <summary>
  /// Specifies that the object is a readonly
  /// business object.
  /// </summary>
  public interface IReadOnlyObject : IBusinessObject
  {
    bool CanReadProperty(string propertyName);
  }
}
