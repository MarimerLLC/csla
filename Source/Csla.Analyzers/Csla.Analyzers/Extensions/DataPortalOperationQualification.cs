namespace Csla.Analyzers.Extensions
{
  /// <summary>
  /// 
  /// </summary>
  public readonly struct DataPortalOperationQualification
  {
    /// <summary>
    /// 
    /// </summary>
    public DataPortalOperationQualification(bool byNamingConvention, bool byAttribute) =>
      (ByNamingConvention, ByAttribute) = (byNamingConvention, byAttribute);

    /// <summary>
    /// 
    /// </summary>
    public DataPortalOperationQualification Combine(DataPortalOperationQualification qualification) =>
      new(
        qualification.ByNamingConvention | ByNamingConvention,
        qualification.ByAttribute | ByAttribute);

    /// <summary>
    /// The data portal only invokes methods that have an operation attribute,
    /// so a method qualifies as an operation only by attribute.
    /// <see cref="ByNamingConvention"/> is used only to find methods that
    /// look like legacy operations but are missing their attribute.
    /// </summary>
    public static implicit operator bool(DataPortalOperationQualification qualification) =>
      qualification.ByAttribute;

    /// <summary>
    /// 
    /// </summary>
    public void Deconstruct(out bool byNamingConvention, out bool byAttribute) =>
      (byNamingConvention, byAttribute) = (ByNamingConvention, ByAttribute);

    /// <summary>
    /// 
    /// </summary>
    public bool ByAttribute { get; }
    /// <summary>
    /// 
    /// </summary>
    public bool ByNamingConvention { get; }
  }
}