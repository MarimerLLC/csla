using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class EvaluatePropertiesForSimplicityAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor onlyUseCslaPropertyMethodsInGetSetRule =
      new(
        Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule, OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Title,
        OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule, nameof(EvaluatePropertiesForSimplicityAnalyzer)));

    /// <summary>
    /// 
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [onlyUseCslaPropertyMethodsInGetSetRule];

    /// <summary>
    /// 
    /// </summary>
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

        var fields = GetFieldDeclarations(propertyNode,context.SemanticModel);

        if (propertySymbol != null && classSymbol != null &&
          classSymbol.IsStereotype() && !propertySymbol.IsAbstract &&
          !propertySymbol.IsStatic && fields.Any())
        {
          if (propertySymbol.GetMethod != null)
          {
            AnalyzePropertyGetter(propertyNode, context);
          }
          else
          {
            context.ReportDiagnostic(Diagnostic.Create(
              onlyUseCslaPropertyMethodsInGetSetRule,
              propertyNode.GetLocation()));
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
      if (propertyNode.ExpressionBody == null)
      {
        return;
      }

      var getterExpression = propertyNode.ExpressionBody.Expression;
      var getterChildren = getterExpression.DescendantNodes(_ => true).ToArray();

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
      if (propertyNode.AccessorList == null)
      {
        return;
      }

      var accessor = propertyNode.AccessorList.Accessors.Single(_ => _.IsKind(SyntaxKind.GetAccessorDeclaration));
      var getterBody = accessor.Body;
      var getterExpression = accessor.ExpressionBody;

      var getterWalkerBody = new FindGetOrReadInvocationsWalker(getterBody, context.SemanticModel);
      var getterWalkerExpression = new FindGetOrReadInvocationsWalker(getterExpression, context.SemanticModel);

      if (getterWalkerBody.Invocation != null)
      {
        var getterStatements = getterBody.Statements;

        if (getterStatements.Count != 1)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            onlyUseCslaPropertyMethodsInGetSetRule,
            getterBody.GetLocation()));
        }
        else
        {
          if (!(getterStatements[0] is ReturnStatementSyntax returnNode))
          {
            context.ReportDiagnostic(Diagnostic.Create(
              onlyUseCslaPropertyMethodsInGetSetRule,
              getterBody.GetLocation()));
          }
          else
          {

            if (!(returnNode.ChildNodes().SingleOrDefault(
                  _ => _.IsKind(SyntaxKind.InvocationExpression)) is InvocationExpressionSyntax invocation) || invocation != getterWalkerBody.Invocation)
            {
              context.ReportDiagnostic(Diagnostic.Create(
                onlyUseCslaPropertyMethodsInGetSetRule,
                getterBody.GetLocation()));
            }
          }
        }
      }
      else if (getterWalkerExpression.Invocation != null)
      {
        if (!(getterExpression.Expression is InvocationExpressionSyntax invocation) || invocation != getterWalkerExpression.Invocation)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            onlyUseCslaPropertyMethodsInGetSetRule,
            getterExpression.GetLocation()));
        }
      }
      else
      {
        context.ReportDiagnostic(Diagnostic.Create(
          onlyUseCslaPropertyMethodsInGetSetRule,
          accessor.GetLocation()));
      }
    }

    private static void AnalyzePropertySetter(PropertyDeclarationSyntax propertyNode, SyntaxNodeAnalysisContext context)
    {
      if (propertyNode.AccessorList == null)
      {
        return;
      }

      var accessor = propertyNode.AccessorList.Accessors.Single(_ => _.IsKind(SyntaxKind.SetAccessorDeclaration));
      var setterBody = accessor.Body;
      var setterExpression = accessor.ExpressionBody;

      var setterWalkerBody = new FindSetOrLoadInvocationsWalker(setterBody, context.SemanticModel);
      var setterWalkerExpression = new FindGetOrReadInvocationsWalker(setterExpression, context.SemanticModel);

      if (setterWalkerBody.Invocation != null)
      {
        var setterStatements = setterBody.Statements;

        if (setterStatements.Count != 1)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            onlyUseCslaPropertyMethodsInGetSetRule,
            setterBody.GetLocation()));
        }
        else
        {
          if (!(setterStatements[0] is ExpressionStatementSyntax expressionNode))
          {
            context.ReportDiagnostic(Diagnostic.Create(
              onlyUseCslaPropertyMethodsInGetSetRule,
              setterBody.GetLocation()));
          }
          else
          {

            if (!(expressionNode.ChildNodes().SingleOrDefault(
                  _ => _.IsKind(SyntaxKind.InvocationExpression)) is InvocationExpressionSyntax invocation) || invocation != setterWalkerBody.Invocation)
            {
              context.ReportDiagnostic(Diagnostic.Create(
                onlyUseCslaPropertyMethodsInGetSetRule,
                setterBody.GetLocation()));
            }
          }
        }
      }
      else if (setterWalkerExpression.Invocation != null)
      {
        if (!(setterExpression.Expression is InvocationExpressionSyntax invocation) || invocation != setterWalkerExpression.Invocation)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            onlyUseCslaPropertyMethodsInGetSetRule,
            setterExpression.GetLocation()));
        }
      }
      else
      {
        context.ReportDiagnostic(Diagnostic.Create(
          onlyUseCslaPropertyMethodsInGetSetRule,
          accessor.GetLocation()));
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public static IEnumerable<FieldDeclarationSyntax> GetFieldDeclarations(PropertyDeclarationSyntax propertyDeclaration,SemanticModel semanticModel)
    {
      var classDeclaration = propertyDeclaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();
      var propertyType = semanticModel.GetTypeInfo(propertyDeclaration.Type).Type;
      // Check if the classDeclaration is null
      if (classDeclaration == null)
      {
        throw new ArgumentNullException(nameof(classDeclaration));
      }

      // Find all field declarations in the class
      var fieldDeclarations = classDeclaration.Members
          .OfType<FieldDeclarationSyntax>();

      // Filter for static fields
      return fieldDeclarations
          .Where(field => FilterField(field,propertyDeclaration, propertyType,semanticModel));
    }

    private static bool FilterField(FieldDeclarationSyntax fieldDeclaration, PropertyDeclarationSyntax propertyDeclaration, ITypeSymbol propertyType,SemanticModel semanticModel)
    {
      var fieldType = semanticModel.GetTypeInfo(fieldDeclaration.Declaration.Type).Type;
      if (fieldType != null && fieldType.OriginalDefinition.ToString() == "Csla.PropertyInfo<T>")
      {
        var typeArgument = ((INamedTypeSymbol)fieldType).TypeArguments[0];
        if (SymbolEqualityComparer.Default.Equals(typeArgument, propertyType))
        {
          var initializer = fieldDeclaration.Declaration.Variables
            .Select(a => a.Initializer.Value)
            .OfType<InvocationExpressionSyntax>().Select(w=>w.ArgumentList.Arguments.FirstOrDefault()?.Expression);
          var lambda = initializer
            .OfType<SimpleLambdaExpressionSyntax>()
            .Select(s => s.Body)
            .OfType<MemberAccessExpressionSyntax>()
            .Any(a=>a.Name.Identifier.ValueText == propertyDeclaration.Identifier.ValueText);

          var name = initializer
            .OfType<InvocationExpressionSyntax>()
            .Select(s=>s.ArgumentList.Arguments.FirstOrDefault()?.Expression)
            .OfType<IdentifierNameSyntax>()
            .Any(a=>a.Identifier.ValueText == propertyDeclaration.Identifier.ValueText);
          return name || lambda;
        }
      }
      return false;
    }
  }
}