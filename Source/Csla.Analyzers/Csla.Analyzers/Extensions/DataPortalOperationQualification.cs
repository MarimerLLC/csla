namespace Csla.Analyzers.Extensions
{
  public readonly struct DataPortalOperationQualification
  {
    public DataPortalOperationQualification(bool byNamingConvention, bool byAttribute) =>
      (ByNamingConvention, ByAttribute) = (byNamingConvention, byAttribute);

    public DataPortalOperationQualification Combine(DataPortalOperationQualification qualification) =>
      new DataPortalOperationQualification(
        qualification.ByNamingConvention | ByNamingConvention,
        qualification.ByAttribute | ByAttribute);

    public static implicit operator bool(DataPortalOperationQualification qualification) =>
      qualification.ByAttribute | qualification.ByNamingConvention;

    public void Deconstruct(out bool byNamingConvention, out bool byAttribute) =>
      (byNamingConvention, byAttribute) = (ByNamingConvention, ByAttribute);

    public bool ByAttribute { get; }
    public bool ByNamingConvention { get; }
  }
}