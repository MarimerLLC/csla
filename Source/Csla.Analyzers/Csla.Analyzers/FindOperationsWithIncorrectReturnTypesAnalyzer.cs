using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class FindOperationsWithIncorrectReturnTypesAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor shouldOnlyReturnVoidOrTaskRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.FindOperationsWithIncorrectReturnTypes, FindOperationsWithIncorrectReturnTypesAnalyzerConstants.Title,
        FindOperationsWithIncorrectReturnTypesAnalyzerConstants.Message, Constants.Categories.Design,
        DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.FindOperationsWithIncorrectReturnTypes, nameof(FindOperationsWithIncorrectReturnTypesAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
      ImmutableArray.Create(shouldOnlyReturnVoidOrTaskRule);

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

      if (typeSymbol.IsStereotype() && methodSymbol.IsDataPortalOperation())
      {
        var taskType = context.Compilation.GetTypeByMetadataName(typeof(Task).FullName);
        if(!(methodSymbol.ReturnsVoid || Equals(methodSymbol.ReturnType, taskType)))
        {
          context.ReportDiagnostic(Diagnostic.Create(
            shouldOnlyReturnVoidOrTaskRule, methodSymbol.Locations[0]));
        }
      }
    }
  }
}