using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class FindOperationsWithIncorrectReturnTypesAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.FindOperationsWithIncorrectReturnTypes_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new(nameof(Resources.FindOperationsWithIncorrectReturnTypes_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants
  {
    public static string ChangeReturnTypeToTaskDescription => Resources.FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCode_ChangeReturnTypeToTaskDescription;
    public static string ChangeReturnTypeToVoidDescription => Resources.FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCode_ChangeReturnTypeToVoidDescription;
    public static readonly string SystemThreadingTasksNamespace = typeof(Task).Namespace;
  }
}
