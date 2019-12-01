namespace Csla.Analyzers
{
  public static class Constants
  {
    public static class AnalyzerIdentifiers
    {
      public const string IsBusinessObjectSerializable = "CSLA0001";
      public const string IsOperationMethodPublic = "CSLA0002";
      public const string PublicNoArgumentConstructorIsMissing = "CSLA0003";
      public const string ConstructorHasParameters = "CSLA0004";
      public const string FindSaveAssignmentIssue = "CSLA0005";
      public const string FindSaveAsyncAssignmentIssue = "CSLA0006";
      public const string OnlyUseCslaPropertyMethodsInGetSetRule = "CSLA0007";
      public const string EvaluateManagedBackingFields = "CSLA0008";
      public const string IsOperationMethodPublicForInterface = "CSLA0009";
      public const string FindOperationsWithNonSerializableArguments = "CSLA0010";
      public const string FindBusinessObjectCreation = "CSLA0011";
      public const string FindOperationsWithIncorrectReturnTypes = "CSLA0012";
      public const string DoesChildOperationHaveRunLocal = "CSLA0013";
      public const string DoesOperationHaveAttribute = "CSLA0014";
      public const string IsOperationAttributeUsageCorrect = "CSLA0015";
      public const string AsynchronousBusinessRuleInheritance = "CSLA0016";
      public const string BusinessRuleContextUsage = "CSLA0017";
      public const string CompleteInExecuteAsync = "CSLA0018";
      public const string RefOrOutParameterInOperation = "CSLA0019";
    }

    public static class Categories
    {
      public const string Design = "Design";
      public const string Usage = "Usage";
    }

    public static class Namespaces
    {
      public const string System = "System";
      public const string SystemThreadingTasks = "System.Threading.Tasks";
    }

    public static class SaveMethodNames
    {
      public const string Save = "Save";
      public const string SaveAsync = "SaveAsync";
    }
  }
}