using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class EvaluatePropertiesForSimplicityAnalyzer
    : DiagnosticAnalyzer
  {
    private static DiagnosticDescriptor onlyUseCslaPropertyMethodsInGetSetRule = new DiagnosticDescriptor(
      OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId, OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Title,
      OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Message, OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Category,
      DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get
      {
        return ImmutableArray.Create(EvaluatePropertiesForSimplicityAnalyzer.onlyUseCslaPropertyMethodsInGetSetRule);
      }
    }

    public override void Initialize(AnalysisContext context)
    {
      context.RegisterSyntaxNodeAction<SyntaxKind>(
        EvaluatePropertiesForSimplicityAnalyzer.AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
      var propertyNode = (PropertyDeclarationSyntax)context.Node;

      if(!propertyNode.ContainsDiagnostics)
      {
        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyNode);
        var classSymbol = propertySymbol.ContainingType;

        if (propertySymbol != null && classSymbol != null &&
          classSymbol.IsStereotype() && !propertySymbol.IsAbstract &&
          !propertySymbol.IsStatic)
        {
          if (propertySymbol.GetMethod != null)
          {
            EvaluatePropertiesForSimplicityAnalyzer.AnalyzePropertyGetter(propertyNode, context);
          }

          if (propertySymbol.SetMethod != null)
          {
            EvaluatePropertiesForSimplicityAnalyzer.AnalyzePropertySetter(propertyNode, context);
          }
        }
      }
    }

    private static void AnalyzePropertyGetter(PropertyDeclarationSyntax propertyNode, SyntaxNodeAnalysisContext context)
    {
      if(propertyNode.ExpressionBody == null)
      {
        var getter = propertyNode.AccessorList.Accessors.Single(
          _ => _.IsKind(SyntaxKind.GetAccessorDeclaration)).Body;

        var getterWalker = new FindGetOrReadInvocationsWalker(getter, context.SemanticModel);

        if (getterWalker.Invocation != null)
        {
          var getterStatements = getter.Statements;

          if (getterStatements.Count != 1)
          {
            context.ReportDiagnostic(Diagnostic.Create(
              EvaluatePropertiesForSimplicityAnalyzer.onlyUseCslaPropertyMethodsInGetSetRule,
              getter.GetLocation()));
          }
          else
          {
            var returnNode = getterStatements[0] as ReturnStatementSyntax;

            if (returnNode == null)
            {
              context.ReportDiagnostic(Diagnostic.Create(
                EvaluatePropertiesForSimplicityAnalyzer.onlyUseCslaPropertyMethodsInGetSetRule,
                getter.GetLocation()));
            }
            else
            {
              var invocation = returnNode.ChildNodes().SingleOrDefault(
                _ => _.IsKind(SyntaxKind.InvocationExpression)) as InvocationExpressionSyntax;

              if (invocation == null || invocation != getterWalker.Invocation)
              {
                context.ReportDiagnostic(Diagnostic.Create(
                  EvaluatePropertiesForSimplicityAnalyzer.onlyUseCslaPropertyMethodsInGetSetRule,
                  getter.GetLocation()));
              }
            }
          }
        }
      }
    }

    private static void AnalyzePropertySetter(PropertyDeclarationSyntax propertyNode, SyntaxNodeAnalysisContext context)
    {
      var setter = propertyNode.AccessorList.Accessors.Single(
        _ => _.IsKind(SyntaxKind.SetAccessorDeclaration)).Body;

      var setterWalker = new FindSetOrLoadInvocationsWalker(setter, context.SemanticModel);

      if (setterWalker.Invocation != null)
      {
        var setterStatements = setter.Statements;

        if (setterStatements.Count() != 1)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            EvaluatePropertiesForSimplicityAnalyzer.onlyUseCslaPropertyMethodsInGetSetRule,
            setter.GetLocation()));
        }
        else
        {
          var expressionNode = setterStatements[0] as ExpressionStatementSyntax;

          if (expressionNode == null)
          {
            context.ReportDiagnostic(Diagnostic.Create(
              EvaluatePropertiesForSimplicityAnalyzer.onlyUseCslaPropertyMethodsInGetSetRule,
              setter.GetLocation()));
          }
          else
          {
            var invocation = expressionNode.ChildNodes().SingleOrDefault(
              _ => _.IsKind(SyntaxKind.InvocationExpression)) as InvocationExpressionSyntax;

            if (invocation == null || invocation != setterWalker.Invocation)
            {
              context.ReportDiagnostic(Diagnostic.Create(
                EvaluatePropertiesForSimplicityAnalyzer.onlyUseCslaPropertyMethodsInGetSetRule,
                setter.GetLocation()));
            }
          }
        }
      }
    }
  }
}