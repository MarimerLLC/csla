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
      /// <summary>
      /// The source generator can't implement the properties for <c>[CslaImplementProperties]</c>
      /// or <c>[CslaImplementPropertiesInterface&lt;T&gt;]</c>, for example because the type argument
      /// is not an interface declared in the same project or the type doesn't inherit a supported base class.
      /// </summary>
      public const string InvalidImplementPropertiesInterface = "CSLA0030";
      /// <summary>
      /// A method with <c>[AddsToRuleContext]</c> has no <c>IRuleContext</c> parameter, or doesn't
      /// call an <c>Add</c> method on that context or another <c>[AddsToRuleContext]</c> method
      /// that it passes the context to.
      /// </summary>
      public const string AddsToRuleContextUsage = "CSLA0031";
      /// <summary>
      /// A property with <c>[DoNotSerialize]</c> or <c>[CalculatedProperty]</c> is registered as a
      /// not-null reference-type property without a default value: it is not serialized, so it
      /// arrives null and fails the data portal check.
      /// </summary>
      public const string NotNullPropertyNotSerializedWithoutDefault = "CSLA0032";
      /// <summary>
      /// A data portal operation that completes an object doesn't visibly load a not-null
      /// reference-type property that has no default value.
      /// </summary>
      public const string NotNullPropertyNotLoaded = "CSLA0033";
      /// <summary>
      /// A not-null reference-type property without a default value has a read authorization rule:
      /// a denied read returns null through its non-nullable getter.
      /// </summary>
      public const string NotNullPropertyWithReadAuthorization = "CSLA0034";
      /// <summary>
      /// A two-argument <c>RegisterNotNullProperty&lt;string&gt;</c> call passes an empty or
      /// whitespace string, which binds to <c>friendlyName</c> and registers no default value.
      /// </summary>
      public const string NotNullPropertyEmptyFriendlyName = "CSLA0035";
      /// <summary>
      /// A not-null property is used with a delayed-load method, which not-null properties don't support.
      /// </summary>
      public const string NotNullPropertyDelayedLoad = "CSLA0036";
      /// <summary>
      /// A <c>PropertyInfo&lt;T&gt;</c> registration can be migrated to <c>RegisterNotNullProperty</c>
      /// with compile-time consequences only: a value type, a <c>string</c>, or an explicit non-null default.
      /// </summary>
      public const string MigrateToNotNullProperty = "CSLA0037";
      /// <summary>
      /// A <c>PropertyInfo&lt;T&gt;</c> registration of a non-nullable reference type without a
      /// default can be migrated to <c>RegisterNotNullProperty</c>, after checking that every
      /// Create and Fetch loads it.
      /// </summary>
      public const string MigrateToNotNullPropertyWithReview = "CSLA0038";

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