namespace Csla.Analyzers.Extensions
{
  public readonly struct CslaOperationQualification
  {
    public CslaOperationQualification(bool byNamingConvention, bool byAttribute) =>
      (ByNamingConvention, ByAttribute) = (byNamingConvention, byAttribute);

    public CslaOperationQualification Combine(CslaOperationQualification qualification) =>
      new CslaOperationQualification(
        qualification.ByNamingConvention | ByNamingConvention,
        qualification.ByAttribute | ByAttribute);

    public static implicit operator bool(CslaOperationQualification qualification) =>
      qualification.ByAttribute | qualification.ByNamingConvention;

    public void Deconstruct(out bool byNamingConvention, out bool byAttribute) =>
      (byNamingConvention, byAttribute) = (ByNamingConvention, ByAttribute);

    public bool ByAttribute { get; }
    public bool ByNamingConvention { get; }
  }
}