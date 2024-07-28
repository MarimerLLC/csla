﻿using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class DoesChildOperationHaveRunLocalAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor childHasRunLocalRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.DoesChildOperationHaveRunLocal, DoesChildOperationHaveRunLocalAnalyzerConstants.Title,
        DoesChildOperationHaveRunLocalAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.DoesChildOperationHaveRunLocal, nameof(DoesChildOperationHaveRunLocalAnalyzer)));

    /// <summary>
    /// 
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(childHasRunLocalRule);

    /// <summary>
    /// 
    /// </summary>
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

      if (typeSymbol.IsStereotype() && methodSymbol.IsChildDataPortalOperation() &&
        methodSymbol.GetAttributes().Any(_ => _.AttributeClass.IsRunLocalAttribute()))
      {
        context.ReportDiagnostic(Diagnostic.Create(
          childHasRunLocalRule, methodSymbol.Locations[0]));
      }
    }
  }
}