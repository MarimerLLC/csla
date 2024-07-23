﻿using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class ObjectAuthorizationRulesAttributeAnalyzerConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString AttributeMissingTitle = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_AttributeMissingTitle), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString AttributeMissingMessage = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_AttributeMissingMessage), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString RulesPublicTitle = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_RulesPublicTitle), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString RulesPublicMessage = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_RulesPublicMessage), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString RulesStaticTitle = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_RulesStaticTitle), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString RulesStaticMessage = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_RulesStaticMessage), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class ObjectAuthorizationRulesAttributeAnalyzerAddAttributeCodeFixConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static string AddAttributeAndUsingDescription => Resources.Shared_AddAttributeAndUsingDescription;
    /// <summary>
    /// 
    /// </summary>
    public static string AddAttributeDescription => Resources.Shared_AddAttributeDescription;
    /// <summary>
    /// 
    /// </summary>
    public const string CslaNamespace = "Csla";
  }
}
