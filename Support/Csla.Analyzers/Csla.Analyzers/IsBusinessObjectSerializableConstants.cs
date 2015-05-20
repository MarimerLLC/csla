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
		public const string AddSerializableDescription = "Add [Serializable]";
		public const string AddSystemSerializableDescription = "Add [System.Serializable]";
		public const string AddSerializableAndUsingDescription = "Add [Serializable] and using statement";
	}
}
