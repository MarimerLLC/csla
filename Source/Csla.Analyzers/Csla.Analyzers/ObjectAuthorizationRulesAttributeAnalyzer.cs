using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class ObjectAuthorizationRulesAttributeAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor missingAttributeRule =
      new(
        Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesAttributeMissing, ObjectAuthorizationRulesAttributeAnalyzerConstants.AttributeMissingTitle,
        ObjectAuthorizationRulesAttributeAnalyzerConstants.AttributeMissingMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesAttributeMissing, nameof(ObjectAuthorizationRulesAttributeAnalyzer)));
    private static readonly DiagnosticDescriptor shouldBePublicRule =
      new(
        Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesPublic, ObjectAuthorizationRulesAttributeAnalyzerConstants.RulesPublicTitle,
        ObjectAuthorizationRulesAttributeAnalyzerConstants.RulesPublicMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Info, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesPublic, nameof(ObjectAuthorizationRulesAttributeAnalyzer)));
    private static readonly DiagnosticDescriptor shouldBeStaticRule =
      new(
        Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesStatic, ObjectAuthorizationRulesAttributeAnalyzerConstants.RulesStaticTitle,
        ObjectAuthorizationRulesAttributeAnalyzerConstants.RulesStaticMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesStatic, nameof(ObjectAuthorizationRulesAttributeAnalyzer)));

    /// <summary>
    /// 
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(missingAttributeRule, shouldBePublicRule, shouldBeStaticRule);

    /// <summary>
    /// 
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
      var methodNode = (MethodDeclarationSyntax)context.Node;

      var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);
      var typeSymbol = methodSymbol.ContainingType;

      if (typeSymbol.IsStereotype())
      {
        var qualification = methodSymbol.IsAddObjectAuthorizationRulesOperation();

        if(qualification.ByNamingConvention && !qualification.ByAttribute)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            missingAttributeRule, methodSymbol.Locations[0]));
        }
        if (methodSymbol.DeclaredAccessibility != Accessibility.Public && (qualification.ByNamingConvention || qualification.ByAttribute))
        {
          context.ReportDiagnostic(Diagnostic.Create(
            shouldBePublicRule, methodSymbol.Locations[0]));
        }
        if (!methodSymbol.IsStatic && (qualification.ByNamingConvention || qualification.ByAttribute))
        {
          context.ReportDiagnostic(Diagnostic.Create(
            shouldBeStaticRule, methodSymbol.Locations[0]));
        }
      }
    }
  }
}