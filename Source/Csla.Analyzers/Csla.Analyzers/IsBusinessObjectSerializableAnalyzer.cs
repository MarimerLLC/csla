using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class IsBusinessObjectSerializableAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor makeSerializableRule = 
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.IsBusinessObjectSerializable, IsBusinessObjectSerializableConstants.Title,
        IsBusinessObjectSerializableConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Error, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(makeSerializableRule);

    public override void Initialize(AnalysisContext context) => 
      context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
      var classNode = (ClassDeclarationSyntax)context.Node;
      var classSymbol = context.SemanticModel.GetDeclaredSymbol(classNode);

      if (classSymbol.IsMobileObject() && !classSymbol.IsSerializable)
      {
        context.ReportDiagnostic(Diagnostic.Create(makeSerializableRule,
          classNode.Identifier.GetLocation()));
        return;
      }
    }
  }
}