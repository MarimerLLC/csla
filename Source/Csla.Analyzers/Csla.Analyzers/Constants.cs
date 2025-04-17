namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class Constants
  {
    public static class AnalyzerIdentifiers
    {
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
      public const string ObjectAuthorizationRulesAttributeMissing = "CSLA0020";
      public const string ObjectAuthorizationRulesPublic = "CSLA0021";
      public const string ObjectAuthorizationRulesStatic = "CSLA0022";
      public const string EvaluateManagedBackingFieldsNameof = "CSLA0023";
    }

    public static class Categories
    {
      public const string Design = "Design";
      public const string Usage = "Usage";
      public const string Refactoring = "Refactoring";
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