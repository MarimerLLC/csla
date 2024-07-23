namespace Csla.Analyzers.Extensions
{
  /// <summary>
  /// 
  /// </summary>
  public readonly struct CslaOperationQualification
  {
    /// <summary>
    /// 
    /// </summary>
    public CslaOperationQualification(bool byNamingConvention, bool byAttribute) =>
      (ByNamingConvention, ByAttribute) = (byNamingConvention, byAttribute);

    /// <summary>
    /// 
    /// </summary>
    public static implicit operator bool(CslaOperationQualification qualification) =>
      qualification.ByAttribute | qualification.ByNamingConvention;

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