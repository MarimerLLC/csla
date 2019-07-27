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
    private static readonly DiagnosticDescriptor onlyUseCslaPropertyMethodsInGetSetRule =
      new DiagnosticDescriptor(
        Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule, OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Title,
        OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule, nameof(EvaluatePropertiesForSimplicityAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(onlyUseCslaPropertyMethodsInGetSetRule);

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
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
            AnalyzePropertyGetter(propertyNode, context);
          }

          if (propertySymbol.SetMethod != null)
          {
            AnalyzePropertySetter(propertyNode, context);
          }
        }
      }
    }

    private static void AnalyzePropertyGetter(PropertyDeclarationSyntax propertyNode, SyntaxNodeAnalysisContext context)
    {
      if (propertyNode.ExpressionBody == null)
      {
        AnalyzePropertyGetterWithGet(propertyNode, context);
      }
      else
      {
        AnalyzePropertyGetterWithExpressionBody(propertyNode, context);
      }
    }

    private static void AnalyzePropertyGetterWithExpressionBody(PropertyDeclarationSyntax propertyNode, SyntaxNodeAnalysisContext context)
    {
      var getterExpression = propertyNode.ExpressionBody.Expression;
      var getterChildren = getterExpression.DescendantNodes(_ => true).ToImmutableArray();

      if (getterChildren.Length > 1)
      {
        var getterWalker = new FindGetOrReadInvocationsWalker(
          getterExpression, context.SemanticModel);

        if (getterWalker.Invocation != null &&
          getterChildren.Contains(getterWalker.Invocation))
        {
          context.ReportDiagnostic(Diagnostic.Create(
            onlyUseCslaPropertyMethodsInGetSetRule,
            getterExpression.GetLocation()));
        }
      }
    }

    private static void AnalyzePropertyGetterWithGet(PropertyDeclarationSyntax propertyNode, SyntaxNodeAnalysisContext context)
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
            onlyUseCslaPropertyMethodsInGetSetRule,
            getter.GetLocation()));
        }
        else
        {

          if (!(getterStatements[0] is ReturnStatementSyntax returnNode))
          {
            context.ReportDiagnostic(Diagnostic.Create(
              onlyUseCslaPropertyMethodsInGetSetRule,
              getter.GetLocation()));
          }
          else
          {

            if (!(returnNode.ChildNodes().SingleOrDefault(
              _ => _.IsKind(SyntaxKind.InvocationExpression)) is InvocationExpressionSyntax invocation) || invocation != getterWalker.Invocation)
            {
              context.ReportDiagnostic(Diagnostic.Create(
                onlyUseCslaPropertyMethodsInGetSetRule,
                getter.GetLocation()));
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
            onlyUseCslaPropertyMethodsInGetSetRule,
            setter.GetLocation()));
        }
        else
        {

          if (!(setterStatements[0] is ExpressionStatementSyntax expressionNode))
          {
            context.ReportDiagnostic(Diagnostic.Create(
              onlyUseCslaPropertyMethodsInGetSetRule,
              setter.GetLocation()));
          }
          else
          {

            if (!(expressionNode.ChildNodes().SingleOrDefault(
              _ => _.IsKind(SyntaxKind.InvocationExpression)) is InvocationExpressionSyntax invocation) || invocation != setterWalker.Invocation)
            {
              context.ReportDiagnostic(Diagnostic.Create(
                onlyUseCslaPropertyMethodsInGetSetRule,
                setter.GetLocation()));
            }
          }
        }
      }
    }
  }
}