namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class Constants
  {
    public static class AnalyzerIdentifiers
    {
      /// <summary>
      /// 
      /// </summary>
      public const string IsOperationMethodPublic = "CSLA0002";
      /// <summary>
      /// 
      /// </summary>
      public const string PublicNoArgumentConstructorIsMissing = "CSLA0003";
      /// <summary>
      /// 
      /// </summary>
      public const string ConstructorHasParameters = "CSLA0004";
      /// <summary>
      /// 
      /// </summary>
      public const string FindSaveAssignmentIssue = "CSLA0005";
      /// <summary>
      /// 
      /// </summary>
      public const string FindSaveAsyncAssignmentIssue = "CSLA0006";
      /// <summary>
      /// 
      /// </summary>
      public const string OnlyUseCslaPropertyMethodsInGetSetRule = "CSLA0007";
      /// <summary>
      /// 
      /// </summary>
      public const string EvaluateManagedBackingFields = "CSLA0008";
      /// <summary>
      /// 
      /// </summary>
      public const string IsOperationMethodPublicForInterface = "CSLA0009";
      /// <summary>
      /// 
      /// </summary>
      public const string FindOperationsWithNonSerializableArguments = "CSLA0010";
      /// <summary>
      /// 
      /// </summary>
      public const string FindBusinessObjectCreation = "CSLA0011";
      /// <summary>
      /// 
      /// </summary>
      public const string FindOperationsWithIncorrectReturnTypes = "CSLA0012";
      /// <summary>
      /// 
      /// </summary>
      public const string DoesChildOperationHaveRunLocal = "CSLA0013";
      /// <summary>
      /// 
      /// </summary>
      public const string DoesOperationHaveAttribute = "CSLA0014";
      /// <summary>
      /// 
      /// </summary>
      public const string IsOperationAttributeUsageCorrect = "CSLA0015";
      /// <summary>
      /// 
      /// </summary>
      public const string AsynchronousBusinessRuleInheritance = "CSLA0016";
      /// <summary>
      /// 
      /// </summary>
      public const string BusinessRuleContextUsage = "CSLA0017";
      /// <summary>
      /// 
      /// </summary>
      public const string CompleteInExecuteAsync = "CSLA0018";
      /// <summary>
      /// 
      /// </summary>
      public const string RefOrOutParameterInOperation = "CSLA0019";
      /// <summary>
      /// 
      /// </summary>
      public const string ObjectAuthorizationRulesAttributeMissing = "CSLA0020";
      /// <summary>
      /// 
      /// </summary>
      public const string ObjectAuthorizationRulesPublic = "CSLA0021";
      /// <summary>
      /// 
      /// </summary>
      public const string ObjectAuthorizationRulesStatic = "CSLA0022";
      /// <summary>
      /// 
      /// </summary>
      public const string EvaluateManagedBackingFieldsNameof = "CSLA0023";

    }

    /// <summary>
    /// 
    /// </summary>
    public static class Categories
    {
      /// <summary>
      /// 
      /// </summary>
      public const string Design = "Design";
      /// <summary>
      /// 
      /// </summary>
      public const string Usage = "Usage";
      /// <summary>
      /// 
      /// </summary>
      public const string Refactoring = "Refactoring";
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Namespaces
    {
      /// <summary>
      /// 
      /// </summary>
      public const string System = "System";
      /// <summary>
      /// 
      /// </summary>
      public const string SystemThreadingTasks = "System.Threading.Tasks";
    }

    /// <summary>
    /// 
    /// </summary>
    public static class SaveMethodNames
    {
      /// <summary>
      /// 
      /// </summary>
      public const string Save = "Save";
      /// <summary>
      /// 
      /// </summary>
      public const string SaveAsync = "SaveAsync";
    }
  }
}