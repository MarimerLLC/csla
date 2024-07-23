﻿using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class ObjectAuthorizationRulesAttributeAnalyzerConstants
  {
    public static readonly LocalizableResourceString AttributeMissingTitle = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_AttributeMissingTitle), Resources.ResourceManager, typeof(Resources));
    public static readonly LocalizableResourceString AttributeMissingMessage = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_AttributeMissingMessage), Resources.ResourceManager, typeof(Resources));
    public static readonly LocalizableResourceString RulesPublicTitle = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_RulesPublicTitle), Resources.ResourceManager, typeof(Resources));
    public static readonly LocalizableResourceString RulesPublicMessage = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_RulesPublicMessage), Resources.ResourceManager, typeof(Resources));
    public static readonly LocalizableResourceString RulesStaticTitle = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_RulesStaticTitle), Resources.ResourceManager, typeof(Resources));
    public static readonly LocalizableResourceString RulesStaticMessage = new LocalizableResourceString(nameof(Resources.ObjectAuthorizationRulesAttribute_RulesStaticMessage), Resources.ResourceManager, typeof(Resources));
  }

  public static class ObjectAuthorizationRulesAttributeAnalyzerAddAttributeCodeFixConstants
  {
    public static string AddAttributeAndUsingDescription => Resources.Shared_AddAttributeAndUsingDescription;
    public static string AddAttributeDescription => Resources.Shared_AddAttributeDescription;
    public const string CslaNamespace = "Csla";
  }
}
