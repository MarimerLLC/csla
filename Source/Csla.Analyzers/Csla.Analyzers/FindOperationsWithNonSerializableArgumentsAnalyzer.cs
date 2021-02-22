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
  public sealed class FindOperationsWithNonSerializableArgumentsAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor shouldUseSerializableTypesRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, FindOperationsWithNonSerializableArgumentsConstants.Title,
        FindOperationsWithNonSerializableArgumentsConstants.Message, Constants.Categories.Design,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, nameof(FindOperationsWithNonSerializableArgumentsAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
      ImmutableArray.Create(shouldUseSerializableTypesRule);

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

      if (typeSymbol.IsStereotype() && methodSymbol.IsRootDataPortalOperation())
      {
        foreach(var argument in methodSymbol.Parameters)
        {
          var argumentType = argument.Type;
          if (!argumentType.IsMobileObject() && !argumentType.IsSpecialTypeSerializable() &&
            !argumentType.IsSerializableByMobileFormatter(context.Compilation) &&
            !argument.GetAttributes().Any(_ => _.AttributeClass.IsInjectable()) &&
            argumentType is INamedTypeSymbol namedArgument && !namedArgument.IsSerializable)
          {
            context.ReportDiagnostic(Diagnostic.Create(
              shouldUseSerializableTypesRule, argument.Locations[0]));
          }
        }
      }
    }
  }
}