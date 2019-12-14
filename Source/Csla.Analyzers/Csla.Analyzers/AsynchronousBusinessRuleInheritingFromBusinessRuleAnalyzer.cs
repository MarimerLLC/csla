using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor inheritsFromBusinessRuleRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.AsynchronousBusinessRuleInheritance,
        AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzerConstants.Title,
        AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzerConstants.Message,
        Constants.Categories.Usage, DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.AsynchronousBusinessRuleInheritance, nameof(AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
      ImmutableArray.Create(inheritsFromBusinessRuleRule);

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
      var methodNode = (MethodDeclarationSyntax)context.Node;

      if (!methodNode.ContainsDiagnostics)
      {
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);
        var typeSymbol = methodSymbol.ContainingType;

        if(typeSymbol.IsBusinessRule() && methodSymbol.Name == "Execute" && methodSymbol.IsAsync)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            inheritsFromBusinessRuleRule, methodSymbol.Locations[0]));
        }
      }
    }
  }
}