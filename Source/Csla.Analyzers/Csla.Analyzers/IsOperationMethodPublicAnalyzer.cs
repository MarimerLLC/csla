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
    private static readonly DiagnosticDescriptor makeNonPublicRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.IsOperationMethodPublic, IsOperationMethodPublicAnalyzerConstants.Title,
        IsOperationMethodPublicAnalyzerConstants.Message, Constants.Categories.Design,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.IsOperationMethodPublic, nameof(IsOperationMethodPublicAnalyzer)));

    private static readonly DiagnosticDescriptor makeNonPublicForInterfaceRule = 
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.IsOperationMethodPublicForInterface, IsOperationMethodPublicAnalyzerConstants.Title,
        IsOperationMethodPublicAnalyzerConstants.Message, Constants.Categories.Design,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.IsOperationMethodPublicForInterface, nameof(IsOperationMethodPublicAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
      ImmutableArray.Create(makeNonPublicRule, makeNonPublicForInterfaceRule);

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

      if (typeSymbol.IsStereotype() && methodSymbol.IsDataPortalOperation() &&
        methodSymbol.DeclaredAccessibility == Accessibility.Public)
      {
        if(typeSymbol.TypeKind == TypeKind.Interface)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            makeNonPublicForInterfaceRule, methodNode.Identifier.GetLocation()));
        }
        else
        {
          var properties = new Dictionary<string, string>()
          {
            [IsOperationMethodPublicAnalyzerConstants.IsSealed] = typeSymbol.IsSealed.ToString()
          }.ToImmutableDictionary();

          context.ReportDiagnostic(Diagnostic.Create(makeNonPublicRule,
            methodNode.Identifier.GetLocation(), properties));
        }
      }
    }
  }
}