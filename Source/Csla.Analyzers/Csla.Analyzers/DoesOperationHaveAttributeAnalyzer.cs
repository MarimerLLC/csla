using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class DoesOperationHaveAttributeAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor operationAttributeRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.DoesOperationHaveAttribute, DoesOperationHaveAttributeAnalyzerConstants.Title,
        DoesOperationHaveAttributeAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Info, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.DoesOperationHaveAttribute, nameof(DoesOperationHaveAttributeAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(operationAttributeRule);

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
        var qualification = methodSymbol.IsDataPortalOperation();

        if(qualification.ByNamingConvention && !qualification.ByAttribute)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            operationAttributeRule, methodSymbol.Locations[0]));
        }
      }
    }
  }
}