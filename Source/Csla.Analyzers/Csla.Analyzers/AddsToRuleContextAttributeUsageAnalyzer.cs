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
  /// (abstract, interface or extern methods) are not checked. A class that implements an
  /// attributed interface method with a method inherited from its base class is reported unless
  /// the inherited method is checked itself (for example because it has the attribute).
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

    private static readonly DiagnosticDescriptor inheritedImplementationRule =
      new(
        Constants.AnalyzerIdentifiers.AddsToRuleContextUsage, AddsToRuleContextAttributeUsageAnalyzerConstants.Title,
        AddsToRuleContextAttributeUsageAnalyzerConstants.InheritedImplementationMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.AddsToRuleContextUsage, nameof(AddsToRuleContextAttributeUsageAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [rule, inheritedImplementationRule];

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
        startContext.RegisterOperationAction(c => AnalyzeLocalFunction(c, attributeType, ruleContextType), OperationKind.LocalFunction);
        startContext.RegisterSymbolAction(c => AnalyzeInheritedImplementations(c, attributeType), SymbolKind.NamedType);
      });
    }

    private static void AnalyzeOperationBlock(OperationBlockAnalysisContext context,
      INamedTypeSymbol attributeType, INamedTypeSymbol ruleContextType)
    {
      if (context.OwningSymbol is not IMethodSymbol method || !IsMarked(method, attributeType))
        return;

      // The operation blocks also include attribute applications and parameter default
      // values; only the body (a block, also for an expression body) is analyzed, so
      // abstract, interface and extern methods are skipped
      var bodies = context.OperationBlocks.Where(b => b is IBlockOperation).ToImmutableArray();
      if (!bodies.IsEmpty)
        Analyze(context.ReportDiagnostic, method, bodies, attributeType, ruleContextType);
    }

    private static void AnalyzeLocalFunction(OperationAnalysisContext context,
      INamedTypeSymbol attributeType, INamedTypeSymbol ruleContextType)
    {
      var localFunction = (ILocalFunctionOperation)context.Operation;
      if (localFunction.Body is { } body && RuleContextAddCallDetector.HasAttribute(localFunction.Symbol, attributeType))
        Analyze(context.ReportDiagnostic, localFunction.Symbol, [body], attributeType, ruleContextType);
    }

    /// <summary>
    /// A type that implements an <c>[AddsToRuleContext]</c> interface method with a method it
    /// inherits from a base class, which doesn't implement that interface itself. The operation
    /// block of the inherited method doesn't know about this type, so its body isn't checked;
    /// it must have <c>[AddsToRuleContext]</c> itself to be checked.
    /// </summary>
    private static void AnalyzeInheritedImplementations(SymbolAnalysisContext context, INamedTypeSymbol attributeType)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      if (type.TypeKind != TypeKind.Class || type.BaseType is not { } baseType || type.Locations.IsEmpty)
        return;

      foreach (var interfaceType in type.AllInterfaces)
      {
        // An interface the base class already implements is checked on the base class
        if (baseType.AllInterfaces.Contains(interfaceType, SymbolEqualityComparer.Default))
          continue;

        foreach (var interfaceMethod in interfaceType.GetMembers().OfType<IMethodSymbol>())
        {
          if (!RuleContextAddCallDetector.HasAttribute(interfaceMethod, attributeType)
            || type.FindImplementationForInterfaceMember(interfaceMethod) is not IMethodSymbol implementation
            || SymbolEqualityComparer.Default.Equals(implementation.ContainingType, type)
            // An inherited method from a referenced assembly is reported too: its body can't be
            // checked here, so the class should implement the method itself with the attribute
            || IsMarked(implementation, attributeType))
            continue;

          context.ReportDiagnostic(Diagnostic.Create(inheritedImplementationRule, type.Locations[0],
            type.Name,
            interfaceMethod.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
            implementation.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
        }
      }
    }

    private static void Analyze(Action<Diagnostic> reportDiagnostic, IMethodSymbol method,
      ImmutableArray<IOperation> blocks, INamedTypeSymbol attributeType, INamedTypeSymbol ruleContextType)
    {
      var contextParameters = method.Parameters
        .Where(p => IsRuleContext(p.Type, ruleContextType))
        .ToImmutableArray();

      string? reason = null;
      if (contextParameters.IsEmpty)
        reason = nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_NoRuleContextParameter);
      else if (!AddsToContext(method, blocks, contextParameters, attributeType, ruleContextType))
        reason = nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_NoAddCall);

      if (reason is null || method.Locations.IsEmpty)
        return;

      reportDiagnostic(Diagnostic.Create(rule, method.Locations[0],
        method.Name, new LocalizableResourceString(reason, Resources.ResourceManager, typeof(Resources))));
    }

    /// <summary>
    /// The method has <c>[AddsToRuleContext]</c>, or overrides or implements a method that has it.
    /// </summary>
    private static bool IsMarked(IMethodSymbol method, INamedTypeSymbol attributeType)
    {
      if (RuleContextAddCallDetector.HasAttribute(method, attributeType))
        return true;

      // The overridden method may be marked by an interface it implements, not only by the attribute
      if (method.OverriddenMethod is { } overridden && IsMarked(overridden, attributeType))
        return true;

      if (method.ExplicitInterfaceImplementations.Any(m => RuleContextAddCallDetector.HasAttribute(m, attributeType)))
        return true;

      // Static methods too: they implement static abstract and static virtual interface members
      if (method.MethodKind != MethodKind.Ordinary || method.ContainingType is not { } containingType)
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
    /// The body calls an <c>Add</c> method of a rule context type on a context parameter, or
    /// another <c>[AddsToRuleContext]</c> method that it passes a context parameter to, as the
    /// receiver or as an argument (which includes the <c>this</c> argument of an extension method).
    /// The rule context type check rejects an <c>Add</c> call through a cast to an unrelated
    /// type, such as <c>((ICollection&lt;string&gt;)context).Add(...)</c>.
    /// </summary>
    private static bool AddsToContext(IMethodSymbol method, ImmutableArray<IOperation> blocks,
      ImmutableArray<IParameterSymbol> contextParameters, INamedTypeSymbol attributeType, INamedTypeSymbol ruleContextType)
    {
      foreach (var invocation in blocks.SelectMany(b => b.DescendantsAndSelf()).OfType<IInvocationOperation>())
      {
        var target = invocation.TargetMethod;
        if (!target.IsStatic && target.Name.StartsWith("Add")
          && IsRuleContext(target.ContainingType, ruleContextType)
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
