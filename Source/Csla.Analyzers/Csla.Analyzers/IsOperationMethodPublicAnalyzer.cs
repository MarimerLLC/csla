using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using static Csla.Analyzers.Extensions.IMethodSymbolExtensions;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class IsOperationMethodPublicAnalyzer
    : DiagnosticAnalyzer
  {
    private static DiagnosticDescriptor makeNonPublicRule = new DiagnosticDescriptor(
      Constants.AnalyzerIdentifiers.IsOperationMethodPublic, IsOperationMethodPublicAnalyzerConstants.Title,
      IsOperationMethodPublicAnalyzerConstants.Message, Constants.Categories.Design,
      DiagnosticSeverity.Warning, true);

    private static DiagnosticDescriptor makeNonPublicForInterfaceRule = new DiagnosticDescriptor(
      Constants.AnalyzerIdentifiers.IsOperationMethodPublicForInterface, IsOperationMethodPublicAnalyzerConstants.Title,
      IsOperationMethodPublicAnalyzerConstants.Message, Constants.Categories.Design,
      DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get
      {
        return ImmutableArray.Create(
          IsOperationMethodPublicAnalyzer.makeNonPublicRule,
          IsOperationMethodPublicAnalyzer.makeNonPublicForInterfaceRule);
      }
    }

    public override void Initialize(AnalysisContext context)
    {
      context.RegisterSyntaxNodeAction<SyntaxKind>(
        IsOperationMethodPublicAnalyzer.AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
      var methodNode = (MethodDeclarationSyntax)context.Node;
      var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);
      var typeSymbol = methodSymbol.ContainingType;

      if (typeSymbol.IsStereotype() && methodSymbol.IsDataPortalOperation() &&
        methodSymbol.DeclaredAccessibility == Accessibility.Public)
      {
        if(typeSymbol.TypeKind == TypeKind.Interface)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            IsOperationMethodPublicAnalyzer.makeNonPublicForInterfaceRule,
            methodNode.Identifier.GetLocation()));
        }
        else
        {
          var properties = new Dictionary<string, string>()
          {
            [IsOperationMethodPublicAnalyzerConstants.IsSealed] = typeSymbol.IsSealed.ToString()
          }.ToImmutableDictionary();

          context.ReportDiagnostic(Diagnostic.Create(IsOperationMethodPublicAnalyzer.makeNonPublicRule,
            methodNode.Identifier.GetLocation(), properties));
        }
      }
    }
  }
}
