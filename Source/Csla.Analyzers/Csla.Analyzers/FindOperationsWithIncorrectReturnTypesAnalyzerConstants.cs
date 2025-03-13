﻿using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class FindOperationsWithIncorrectReturnTypesAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.FindOperationsWithIncorrectReturnTypes_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.FindOperationsWithIncorrectReturnTypes_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants
  {
    public static string ChangeReturnTypeToTaskDescription => Resources.FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCode_ChangeReturnTypeToTaskDescription;

    public static string ChangeReturnTypeToVoidDescription => Resources.FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCode_ChangeReturnTypeToVoidDescription;

    public static readonly string SystemThreadingTasksNamespace = typeof(Task).Namespace;
  }
}
