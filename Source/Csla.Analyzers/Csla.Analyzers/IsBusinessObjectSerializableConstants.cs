namespace Csla.Analyzers
{
  public static class IsBusinessObjectSerializableConstants
  {
    public const string Category = "Usage";
    public const string DiagnosticId = "CSLA0001";
    public const string Title = "Find CSLA Business Objects That are Not Serializable";
    public const string IdentifierText = "IsBusinessObjectSerializable";
    public const string Message = "CSLA business objects must be serializable.";
  }

  public static class IsBusinessObjectSerializableMakeSerializableCodeFixConstants
  {
    public const string AddSerializableAndUsingDescription = "Add [Serializable] and using statement(s)";
    public const string CslaSerializationNamespace = "Csla.Serialization";
    public const string SerializableName = "Serializable";
    public const string SystemNamespace = "System";
  }
}
