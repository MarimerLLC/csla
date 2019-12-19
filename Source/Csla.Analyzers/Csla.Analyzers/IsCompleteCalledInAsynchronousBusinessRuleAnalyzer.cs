using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class IsCompleteCalledInAsynchronousBusinessRuleAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor completeCalledInAsyncBusinessRuleRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.CompleteInExecuteAsync,
        IsCompleteCalledInAsynchronousBusinessRuleConstants.Title,
        IsCompleteCalledInAsynchronousBusinessRuleConstants.Message,
        Constants.Categories.Usage, DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.CompleteInExecuteAsync, nameof(IsCompleteCalledInAsynchronousBusinessRuleAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
      ImmutableArray.Create(completeCalledInAsyncBusinessRuleRule);

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

        if (typeSymbol.IsBusinessRule() && methodSymbol.Name == "ExecuteAsync" &&
          methodSymbol.Parameters.Length > 0)
        {
          var contextParameter = methodSymbol.Parameters[0];
          var wasCompleteMethodCalled =
            methodNode.DescendantNodes(_ => true).OfType<InvocationExpressionSyntax>()
            .Any(invocation =>
            {
              return context.SemanticModel.GetSymbolInfo(invocation.Expression).Symbol is IMethodSymbol invocationSymbol &&
                invocationSymbol.Name == "Complete" && Equals(invocationSymbol.ContainingType, contextParameter.Type);
            });

          if (wasCompleteMethodCalled)
          {
            context.ReportDiagnostic(Diagnostic.Create(
              completeCalledInAsyncBusinessRuleRule, methodSymbol.Locations[0]));
          }
        }
      }
    }
  }
}