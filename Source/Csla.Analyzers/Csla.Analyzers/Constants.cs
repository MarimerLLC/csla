namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class Constants
  {
    /// <summary>
    /// 
    /// </summary>
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
      /// <summary>
      ///
      /// </summary>
      public const string TypeWithOperationsShouldBePartial = "CSLA0024";
      /// <summary>
      ///
      /// </summary>
      public const string UseGeneratedDataPortalExtension = "CSLA0025";
      /// <summary>
      ///
      /// </summary>
      public const string CalculatedPropertyMustBeReadOnly = "CSLA0026";
      /// <summary>
      ///
      /// </summary>
      public const string CalculatedPropertyRequiresBusinessBase = "CSLA0027";
      /// <summary>
      /// A <c>[MobileSerializer]</c>, <c>[MobileSerializerFor]</c> or
      /// <c>[assembly: MobileSerializerRegistration]</c> declaration is not valid.
      /// </summary>
      public const string InvalidCustomSerializerDeclaration = "CSLA0028";
      /// <summary>
      /// A type is mapped to more than one custom serializer.
      /// </summary>
      public const string ConflictingCustomSerializers = "CSLA0029";

    }

    /// <summary>
    /// Identifiers of the suppressions reported by CSLA diagnostic suppressors.
    /// </summary>
    public static class SuppressorIdentifiers
    {
      /// <summary>
      /// Suppresses IDE0051 (unused private member) on methods CSLA calls through reflection.
      /// </summary>
      public const string UnusedOperation = "CSLA1001";
      /// <summary>
      /// Suppresses CA1822 (mark member as static) on data portal operations.
      /// </summary>
      public const string OperationCanBeStatic = "CSLA1002";
      /// <summary>
      /// Suppresses IDE0060 (unused parameter) on methods CSLA calls through reflection.
      /// </summary>
      public const string UnusedOperationParameter = "CSLA1003";
      /// <summary>
      /// Suppresses IDE0058 (expression value is never used) on CSLA fluent configuration methods.
      /// </summary>
      public const string FluentConfigurationResultUnused = "CSLA1004";
    }

    /// <summary>
    /// Identifiers of diagnostics from other analyzers that CSLA suppresses.
    /// </summary>
    public static class SuppressedDiagnosticIdentifiers
    {
      /// <summary>
      /// Remove unused private members.
      /// </summary>
      public const string UnusedPrivateMember = "IDE0051";
      /// <summary>
      /// Mark members as static.
      /// </summary>
      public const string MarkMembersAsStatic = "CA1822";
      /// <summary>
      /// Remove unused parameter.
      /// </summary>
      public const string UnusedParameter = "IDE0060";
      /// <summary>
      /// Expression value is never used.
      /// </summary>
      public const string ExpressionValueIsUnused = "IDE0058";
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