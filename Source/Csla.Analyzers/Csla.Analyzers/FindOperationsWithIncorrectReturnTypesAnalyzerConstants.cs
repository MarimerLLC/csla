using System.Threading.Tasks;

namespace Csla.Analyzers
{
  public static class FindOperationsWithIncorrectReturnTypesAnalyzerConstants
  {
    public const string Title = "Find Operations With Incorrect Return Types";
    public const string IdentifierText = "FindOperationsWithIncorrectReturnTypes";
    public const string Message = "The return type from an operation should be either void or Task";
  }

  public static class FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants
  {
    public const string ChangeReturnTypeToTaskDescription = "Change return type to Task";
    public const string ChangeReturnTypeToVoidDescription = "Change return type to void";
    public static readonly string SystemThreadingTasksNamespace = typeof(Task).Namespace;
  }
}
