using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class FindRefAndOutParametersInOperationsAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor incorrectParameterRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.RefOrOutParameterInOperation,
        FindRefAndOutParametersInOperationsAnalyzerConstants.Title,
        FindRefAndOutParametersInOperationsAnalyzerConstants.Message,
        Constants.Categories.Usage, DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.RefOrOutParameterInOperation, nameof(FindRefAndOutParametersInOperationsAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
      ImmutableArray.Create(incorrectParameterRule);

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

        if (typeSymbol.IsBusinessBase() && methodSymbol.IsDataPortalOperation())
        {
          foreach(var parameterSymbol in methodSymbol.Parameters)
          {
            if(parameterSymbol.RefKind == RefKind.Out ||
              parameterSymbol.RefKind == RefKind.Ref)
            {
              context.ReportDiagnostic(Diagnostic.Create(
                incorrectParameterRule, parameterSymbol.Locations[0]));
            }
          }
        }
      }
    }
  }
}
