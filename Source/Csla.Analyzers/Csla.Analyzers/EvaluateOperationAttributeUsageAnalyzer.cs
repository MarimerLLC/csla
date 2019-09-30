using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class EvaluateOperationAttributeUsageAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor operationUsageRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.IsOperationAttributeUsageCorrect, EvaluateOperationAttributeUsageAnalyzerConstants.Title,
        EvaluateOperationAttributeUsageAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.IsOperationAttributeUsageCorrect, nameof(EvaluateOperationAttributeUsageAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(operationUsageRule);

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(AnalyzerAttributeDeclaration, SyntaxKind.Attribute);
    }

    private static void AnalyzerAttributeDeclaration(SyntaxNodeAnalysisContext context)
    {
      var attributeNode = (AttributeSyntax)context.Node;
      var attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeNode).Symbol.ContainingSymbol as ITypeSymbol;

      if (attributeSymbol.IsDataPortalOperationAttribute())
      {
        var methodNode = attributeNode.FindParent<MethodDeclarationSyntax>();
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);
        var typeSymbol = methodSymbol.ContainingType;

        if (!typeSymbol.IsStereotype() || methodSymbol.IsStatic)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            operationUsageRule, attributeNode.GetLocation()));
        }
      }
    }
  }
}