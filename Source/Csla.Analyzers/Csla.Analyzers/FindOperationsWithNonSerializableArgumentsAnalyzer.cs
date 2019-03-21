using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class FindOperationsWithNonSerializableArgumentsAnalyzer
    : DiagnosticAnalyzer
  {
    private static DiagnosticDescriptor shouldUseSerializableTypesRule = new DiagnosticDescriptor(
      Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, FindOperationsWithNonSerializableArgumentsConstants.Title,
      FindOperationsWithNonSerializableArgumentsConstants.Message, Constants.Categories.Design,
      DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get
      {
        return ImmutableArray.Create(
          FindOperationsWithNonSerializableArgumentsAnalyzer.shouldUseSerializableTypesRule);
      }
    }

    public override void Initialize(AnalysisContext context)
    {
      context.RegisterSyntaxNodeAction<SyntaxKind>(
        FindOperationsWithNonSerializableArgumentsAnalyzer.AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
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
          if (!argument.Type.IsPrimitive() &&
            (argument.Type is INamedTypeSymbol namedArgument && !namedArgument.IsSerializable))
          {
            context.ReportDiagnostic(Diagnostic.Create(
              FindOperationsWithNonSerializableArgumentsAnalyzer.shouldUseSerializableTypesRule,
              argument.Locations[0]));
          }
        }
      }
    }
  }
}
