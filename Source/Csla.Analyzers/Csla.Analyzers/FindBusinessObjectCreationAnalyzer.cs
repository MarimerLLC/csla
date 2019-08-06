using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;
using static Csla.Analyzers.Extensions.SyntaxNodeExtensions;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class FindBusinessObjectCreationAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor objectCreatedRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.FindBusinessObjectCreation, FindBusinessObjectCreationConstants.Title,
        FindBusinessObjectCreationConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.FindBusinessObjectCreation, nameof(FindBusinessObjectCreationAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
      ImmutableArray.Create(objectCreatedRule);

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(AnalyzeObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
    }

    private static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
    {
      var constructorNode = (ObjectCreationExpressionSyntax)context.Node;
      var constructorSymbol = context.SemanticModel.GetSymbolInfo(constructorNode).Symbol as IMethodSymbol;
      var containingSymbol = constructorSymbol?.ContainingType;

      if(containingSymbol.IsStereotype())
      {
        context.CancellationToken.ThrowIfCancellationRequested();
        var callerClassNode = constructorNode.FindParent<ClassDeclarationSyntax>();

        if(callerClassNode != null)
        {
          var callerClassSymbol = context.SemanticModel.GetDeclaredSymbol(callerClassNode) as ITypeSymbol;

          if(!callerClassSymbol.IsObjectFactory())
          {
            context.ReportDiagnostic(Diagnostic.Create(objectCreatedRule, constructorNode.GetLocation()));
          }
        }
      }
    }
  }
}