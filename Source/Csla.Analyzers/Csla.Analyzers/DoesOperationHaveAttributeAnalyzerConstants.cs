﻿using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class DoesOperationHaveAttributeAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.DoesOperationHaveAttribute_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.DoesOperationHaveAttribute_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants
  {
    public static string AddAttributeAndUsingDescription => Resources.Shared_AddAttributeAndUsingDescription;

    public static string AddAttributeDescription => Resources.Shared_AddAttributeDescription;

    public const string CslaNamespace = "Csla";
  }
}
