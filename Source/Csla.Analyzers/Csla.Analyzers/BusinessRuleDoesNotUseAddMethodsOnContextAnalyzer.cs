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
  public sealed class BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor usesAddMethodsOnContextRule =
      new(
        Constants.AnalyzerIdentifiers.BusinessRuleContextUsage,
        BusinessRuleDoesNotUseAddMethodsOnContextAnalyzerConstants.Title,
        BusinessRuleDoesNotUseAddMethodsOnContextAnalyzerConstants.Message,
        Constants.Categories.Usage, DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.BusinessRuleContextUsage, nameof(BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [usesAddMethodsOnContextRule];

    /// <param name="context"></param>
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
        if (methodSymbol is null)
        {
          return;
        }
        var typeSymbol = methodSymbol.ContainingType;

        if (typeSymbol.IsBusinessRule() && 
          (methodSymbol.Name == "Execute" || methodSymbol.Name == "ExecuteAsync") &&
          methodSymbol.Parameters.Length > 0)
        {
          var contextParameter = methodSymbol.Parameters[0];
          var wasAddMethodCalled =
            methodNode.DescendantNodes().OfType<InvocationExpressionSyntax>()
            .Any(invocation =>
            {
              return context.SemanticModel.GetSymbolInfo(invocation.Expression).Symbol is IMethodSymbol invocationSymbol &&
                invocationSymbol.Name.StartsWith("Add") && SymbolEqualityComparer.Default.Equals(invocationSymbol.ContainingType, contextParameter.Type);
            });

          if (!wasAddMethodCalled)
          {
            context.ReportDiagnostic(Diagnostic.Create(
              usesAddMethodsOnContextRule, contextParameter.Locations[0]));
          }
        }
      }
    }
  }
}