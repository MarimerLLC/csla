using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a method or local function with <c>[AddsToRuleContext]</c> that has no
  /// <c>IRuleContext</c> parameter, or whose body calls neither an <c>Add</c> method on that
  /// context nor another <c>[AddsToRuleContext]</c> method that it passes the context to (CSLA0031).
  /// Overrides and implementations of an attributed method are checked too, because CSLA0017
  /// trusts the attribute on the method they override or implement. Methods without a body
  /// (abstract, interface or extern methods) are not checked.
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
      // CSLA0017 trusts the attribute in generated code, so check it there too
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(startContext =>
      {
        var attributeType = startContext.Compilation.GetTypeByMetadataName(RuleContextAddCallDetector.AttributeMetadataName);
        var ruleContextType = startContext.Compilation.GetTypeByMetadataName(RuleContextAddCallDetector.RuleContextMetadataName);
        if (attributeType is null || ruleContextType is null)
          return;

        startContext.RegisterOperationBlockAction(c => AnalyzeOperationBlock(c, attributeType, ruleContextType));
      });
    }

    private static void AnalyzeOperationBlock(OperationBlockAnalysisContext context,
      INamedTypeSymbol attributeType, INamedTypeSymbol ruleContextType)
    {
      // The operation blocks also include attribute applications and parameter default
      // values; only the body (a block, also for an expression body) is analyzed, so
      // abstract, interface and extern methods are skipped
      var bodies = context.OperationBlocks.Where(b => b is IBlockOperation).ToImmutableArray();
      if (!bodies.IsEmpty && context.OwningSymbol is IMethodSymbol method && IsMarked(method, attributeType))
        Analyze(context, method, bodies, attributeType, ruleContextType);

      foreach (var localFunction in context.OperationBlocks
        .SelectMany(b => b.DescendantsAndSelf())
        .OfType<ILocalFunctionOperation>())
      {
        if (localFunction.Body is { } body && RuleContextAddCallDetector.HasAttribute(localFunction.Symbol, attributeType))
          Analyze(context, localFunction.Symbol, [body], attributeType, ruleContextType);
      }
    }

    private static void Analyze(OperationBlockAnalysisContext context, IMethodSymbol method,
      ImmutableArray<IOperation> blocks, INamedTypeSymbol attributeType, INamedTypeSymbol ruleContextType)
    {
      var contextParameters = method.Parameters
        .Where(p => IsRuleContext(p.Type, ruleContextType))
        .ToImmutableArray();

      string? reason = null;
      if (contextParameters.IsEmpty)
        reason = nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_NoRuleContextParameter);
      else if (!AddsToContext(method, blocks, contextParameters, attributeType))
        reason = nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_NoAddCall);

      if (reason is null || method.Locations.IsEmpty)
        return;

      context.ReportDiagnostic(Diagnostic.Create(rule, method.Locations[0],
        method.Name, new LocalizableResourceString(reason, Resources.ResourceManager, typeof(Resources))));
    }

    /// <summary>
    /// The method has <c>[AddsToRuleContext]</c>, or overrides or implements a method that has it.
    /// </summary>
    private static bool IsMarked(IMethodSymbol method, INamedTypeSymbol attributeType)
    {
      if (RuleContextAddCallDetector.HasAttribute(method, attributeType))
        return true;

      for (var overridden = method.OverriddenMethod; overridden is not null; overridden = overridden.OverriddenMethod)
      {
        if (RuleContextAddCallDetector.HasAttribute(overridden, attributeType))
          return true;
      }

      if (method.ExplicitInterfaceImplementations.Any(m => RuleContextAddCallDetector.HasAttribute(m, attributeType)))
        return true;

      if (method.MethodKind != MethodKind.Ordinary || method.IsStatic || method.ContainingType is not { } containingType)
        return false;

      foreach (var interfaceMethod in containingType.AllInterfaces
        .SelectMany(i => i.GetMembers(method.Name))
        .OfType<IMethodSymbol>())
      {
        if (RuleContextAddCallDetector.HasAttribute(interfaceMethod, attributeType)
          && SymbolEqualityComparer.Default.Equals(containingType.FindImplementationForInterfaceMember(interfaceMethod), method))
          return true;
      }
      return false;
    }

    /// <summary>
    /// The parameter is <c>IRuleContext</c>, a type that implements it (such as <c>RuleContext</c>),
    /// or a type parameter constrained to one of those.
    /// </summary>
    private static bool IsRuleContext(ITypeSymbol type, INamedTypeSymbol ruleContextType)
    {
      if (type is ITypeParameterSymbol typeParameter)
        return typeParameter.ConstraintTypes.Any(t => IsRuleContext(t, ruleContextType));

      return SymbolEqualityComparer.Default.Equals(type, ruleContextType)
        || type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, ruleContextType));
    }

    /// <summary>
    /// The body calls an <c>Add</c> method on a context parameter, or another
    /// <c>[AddsToRuleContext]</c> method that it passes a context parameter to, as the
    /// receiver or as an argument (which includes the <c>this</c> argument of an extension method).
    /// </summary>
    private static bool AddsToContext(IMethodSymbol method, ImmutableArray<IOperation> blocks,
      ImmutableArray<IParameterSymbol> contextParameters, INamedTypeSymbol attributeType)
    {
      foreach (var invocation in blocks.SelectMany(b => b.DescendantsAndSelf()).OfType<IInvocationOperation>())
      {
        var target = invocation.TargetMethod;
        if (!target.IsStatic && target.Name.StartsWith("Add")
          && IsContextParameterReference(invocation.Instance, contextParameters))
          return true;

        if (RuleContextAddCallDetector.HasAttribute(target, attributeType)
          && !SymbolEqualityComparer.Default.Equals(target.OriginalDefinition, method.OriginalDefinition)
          && (IsContextParameterReference(invocation.Instance, contextParameters)
            || invocation.Arguments.Any(a => IsContextParameterReference(a.Value, contextParameters))))
          return true;
      }
      return false;
    }

    private static bool IsContextParameterReference(IOperation? value, ImmutableArray<IParameterSymbol> contextParameters)
    {
      while (true)
      {
        if (value is IConversionOperation conversion)
          value = conversion.Operand;
        else if (value is IConditionalAccessInstanceOperation instance)
          value = FindConditionalAccess(instance)?.Operation;
        else
          break;
      }
      return value is IParameterReferenceOperation reference
        && contextParameters.Any(p => SymbolEqualityComparer.Default.Equals(p, reference.Parameter));
    }

    /// <summary>
    /// Finds the <c>?.</c> whose receiver the instance stands for: the nearest enclosing
    /// conditional access that it is in the <c>WhenNotNull</c> part of.
    /// </summary>
    private static IConditionalAccessOperation? FindConditionalAccess(IConditionalAccessInstanceOperation instance)
    {
      IOperation child = instance;
      for (var parent = instance.Parent; parent is not null; child = parent, parent = parent.Parent)
      {
        if (parent is IConditionalAccessOperation access && access.WhenNotNull == child)
          return access;
      }
      return null;
    }
  }
}
