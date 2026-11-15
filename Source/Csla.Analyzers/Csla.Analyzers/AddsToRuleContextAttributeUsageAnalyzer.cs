using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a method with <c>[AddsToRuleContext]</c> that has no <c>IRuleContext</c> parameter,
  /// or whose body calls neither an <c>Add</c> method of that context nor another
  /// <c>[AddsToRuleContext]</c> method that it passes the context to (CSLA0031).
  /// Methods without a body (abstract, interface or extern methods) are not checked.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class AddsToRuleContextAttributeUsageAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor rule =
      new(
        Constants.AnalyzerIdentifiers.AddsToRuleContextUsage, AddsToRuleContextAttributeUsageAnalyzerConstants.Title,
        AddsToRuleContextAttributeUsageAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.AddsToRuleContextUsage, nameof(AddsToRuleContextAttributeUsageAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [rule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(startContext =>
      {
        var attributeType = startContext.Compilation.GetTypeByMetadataName(RuleContextAddCallDetector.AttributeMetadataName);
        var ruleContextType = startContext.Compilation.GetTypeByMetadataName(RuleContextAddCallDetector.RuleContextMetadataName);
        if (attributeType is null || ruleContextType is null)
          return;

        startContext.RegisterSyntaxNodeAction(
          c => AnalyzeMethodDeclaration(c, attributeType, ruleContextType), SyntaxKind.MethodDeclaration);
      });
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context,
      INamedTypeSymbol attributeType, INamedTypeSymbol ruleContextType)
    {
      var methodNode = (MethodDeclarationSyntax)context.Node;
      if (methodNode.ContainsDiagnostics || (methodNode.Body is null && methodNode.ExpressionBody is null))
        return;

      var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode, context.CancellationToken);
      if (methodSymbol is null || !RuleContextAddCallDetector.HasAttribute(methodSymbol, attributeType))
        return;

      var contextParameters = methodSymbol.Parameters
        .Where(p => IsRuleContext(p.Type, ruleContextType))
        .ToImmutableArray();

      string? reason = null;
      if (contextParameters.IsEmpty)
        reason = nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_NoRuleContextParameter);
      else if (!AddsToContext(context, methodNode, methodSymbol, contextParameters, attributeType))
        reason = nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_NoAddCall);

      if (reason is null)
        return;

      context.ReportDiagnostic(Diagnostic.Create(rule, methodNode.Identifier.GetLocation(),
        methodSymbol.Name, new LocalizableResourceString(reason, Resources.ResourceManager, typeof(Resources))));
    }

    /// <summary>
    /// The parameter is <c>IRuleContext</c> or a type that implements it, such as <c>RuleContext</c>.
    /// </summary>
    private static bool IsRuleContext(ITypeSymbol type, INamedTypeSymbol ruleContextType)
      => SymbolEqualityComparer.Default.Equals(type, ruleContextType)
        || type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, ruleContextType));

    /// <summary>
    /// The body calls an <c>Add</c> method of a context parameter's type (the rule CSLA0017 uses),
    /// or another <c>[AddsToRuleContext]</c> method that it passes a context parameter to, as the
    /// receiver or as an argument (which includes the <c>this</c> argument of an extension method).
    /// </summary>
    private static bool AddsToContext(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodNode,
      IMethodSymbol methodSymbol, ImmutableArray<IParameterSymbol> contextParameters, INamedTypeSymbol attributeType)
    {
      foreach (var invocation in methodNode.DescendantNodes().OfType<InvocationExpressionSyntax>())
      {
        if (context.SemanticModel.GetOperation(invocation, context.CancellationToken) is not IInvocationOperation operation)
          continue;

        var target = operation.TargetMethod;
        if (contextParameters.Any(p => RuleContextAddCallDetector.IsAddMethodOn(target, p.Type)))
          return true;

        if (RuleContextAddCallDetector.HasAttribute(target, attributeType)
          && !SymbolEqualityComparer.Default.Equals(target.OriginalDefinition, methodSymbol.OriginalDefinition)
          && PassesContextParameter(operation, contextParameters))
          return true;
      }
      return false;
    }

    private static bool PassesContextParameter(IInvocationOperation operation, ImmutableArray<IParameterSymbol> contextParameters)
      => IsContextParameterReference(operation.Instance, contextParameters)
        || operation.Arguments.Any(a => IsContextParameterReference(a.Value, contextParameters));

    private static bool IsContextParameterReference(IOperation? value, ImmutableArray<IParameterSymbol> contextParameters)
    {
      while (value is IConversionOperation conversion)
        value = conversion.Operand;
      return value is IParameterReferenceOperation reference
        && contextParameters.Any(p => SymbolEqualityComparer.Default.Equals(p, reference.Parameter));
    }
  }
}
