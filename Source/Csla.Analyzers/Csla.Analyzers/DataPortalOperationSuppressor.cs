using System.Collections.Immutable;
using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Analyzers
{
  /// <summary>
  /// Suppresses diagnostics from other analyzers that are false positives on
  /// methods CSLA calls through reflection: data portal operations and
  /// object authorization rules methods.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class DataPortalOperationSuppressor
    : DiagnosticSuppressor
  {
    private static readonly SuppressionDescriptor unusedOperationRule =
      new(
        Constants.SuppressorIdentifiers.UnusedOperation,
        Constants.SuppressedDiagnosticIdentifiers.UnusedPrivateMember,
        DataPortalOperationSuppressorConstants.UnusedOperationJustification);
    private static readonly SuppressionDescriptor operationCanBeStaticRule =
      new(
        Constants.SuppressorIdentifiers.OperationCanBeStatic,
        Constants.SuppressedDiagnosticIdentifiers.MarkMembersAsStatic,
        DataPortalOperationSuppressorConstants.OperationCanBeStaticJustification);
    private static readonly SuppressionDescriptor unusedOperationParameterRule =
      new(
        Constants.SuppressorIdentifiers.UnusedOperationParameter,
        Constants.SuppressedDiagnosticIdentifiers.UnusedParameter,
        DataPortalOperationSuppressorConstants.UnusedOperationParameterJustification);

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions =>
      [unusedOperationRule, operationCanBeStaticRule, unusedOperationParameterRule];

    /// <summary>
    ///
    /// </summary>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
      foreach (var diagnostic in context.ReportedDiagnostics)
      {
        var descriptor = diagnostic.Id switch
        {
          Constants.SuppressedDiagnosticIdentifiers.UnusedPrivateMember when IsUnusedMemberFalsePositive(context, diagnostic) => unusedOperationRule,
          Constants.SuppressedDiagnosticIdentifiers.MarkMembersAsStatic when IsOperationCanBeStaticFalsePositive(context, diagnostic) => operationCanBeStaticRule,
          Constants.SuppressedDiagnosticIdentifiers.UnusedParameter when IsUnusedParameterFalsePositive(context, diagnostic) => unusedOperationParameterRule,
          _ => null
        };

        if (descriptor is not null)
        {
          context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
        }
      }
    }

    private static bool IsUnusedMemberFalsePositive(SuppressionAnalysisContext context, Diagnostic diagnostic) =>
      GetMethod(context, diagnostic) is { } method &&
        (IsCalledDataPortalOperation(method, context.CancellationToken) || IsCalledObjectAuthorizationRulesMethod(method));

    private static bool IsOperationCanBeStaticFalsePositive(SuppressionAnalysisContext context, Diagnostic diagnostic) =>
      GetMethod(context, diagnostic) is { } method && IsCalledDataPortalOperation(method, context.CancellationToken);

    // The data portal chooses an operation by its criteria parameters. [Inject]
    // parameters aren't part of that choice, and an object authorization rules
    // method is called with or without its context parameter, so an unused
    // parameter there is a true positive.
    private static bool IsUnusedParameterFalsePositive(SuppressionAnalysisContext context, Diagnostic diagnostic)
    {
      if (!context.TryFindNode(diagnostic, out var node, out var model) ||
        node.AncestorsAndSelf().OfType<ParameterSyntax>().FirstOrDefault() is not { } parameterNode ||
        model.GetDeclaredSymbol(parameterNode, context.CancellationToken) is not IParameterSymbol parameter)
      {
        return false;
      }

      // Lambdas and local functions inside an operation are ordinary code,
      // so only parameters of the operation itself qualify.
      return parameter.ContainingSymbol is IMethodSymbol { MethodKind: MethodKind.Ordinary } method &&
        IsCalledDataPortalOperation(method, context.CancellationToken) &&
        !parameter.GetAttributes().Any(_ => _.AttributeClass.IsInjectable());
    }

    private static IMethodSymbol? GetMethod(SuppressionAnalysisContext context, Diagnostic diagnostic)
    {
      if (!context.TryFindNode(diagnostic, out var node, out var model))
      {
        return null;
      }

      // The nearest member declaration must be the method itself, so a
      // diagnostic on some other member never matches an enclosing method.
      return node.AncestorsAndSelf().OfType<MemberDeclarationSyntax>().FirstOrDefault() is MethodDeclarationSyntax methodNode
        ? model.GetDeclaredSymbol(methodNode, context.CancellationToken) as IMethodSymbol
        : null;
    }

    // An operation is called either by reflection or by the mapping the
    // operations generator emits. Reflection binds only non-public instance
    // methods; private operations in a base type stay candidates, because when
    // the derived type has no match the data portal looks again from the base
    // type.
    private static bool IsCalledDataPortalOperation(IMethodSymbol method, CancellationToken cancellationToken)
    {
      var type = method.ContainingType;

      // Neither path calls static or generic methods.
      if (method.IsStatic || method.IsGenericMethod || !type.IsCslaObject())
      {
        return false;
      }

      var isRoot = method.HasOperationAttribute(_ => IsInvokedOperationAttribute(_, child: false));
      var isChild = method.HasOperationAttribute(_ => IsInvokedOperationAttribute(_, child: true));

      if (!isRoot && !isChild)
      {
        return false;
      }

      var reflectionCanCall = method.DeclaredAccessibility != Accessibility.Public;
      var hasMapping = HasGeneratedOperationMapping(method, cancellationToken);

      if (!type.HasObjectFactoryAttribute())
      {
        return reflectionCanCall || hasMapping;
      }

      // With [ObjectFactory], the factory handles root operations, and
      // reflection sends child operations to the factory too, except
      // [CreateChild]. The generated mapping still calls child operations on
      // the business type.
      return isChild &&
        (hasMapping || (reflectionCanCall && method.HasOperationAttribute(IsCreateChildAttribute)));
    }

    // The data portal looks for the concrete operation attributes ([Fetch],
    // [FetchChild], ...) and attributes derived from them, never for the
    // DataPortal*OperationAttribute base classes themselves.
    private static bool IsInvokedOperationAttribute(INamedTypeSymbol? attribute, bool child)
    {
      for (var current = attribute; current is not null; current = current.BaseType)
      {
        if (current.ContainingAssembly?.Name != CslaMemberConstants.AssemblyName)
        {
          continue;
        }

        return current.Name is not (CslaMemberConstants.Types.DataPortalOperationAttribute or
            CslaMemberConstants.Types.DataPortalRootOperationAttribute or
            CslaMemberConstants.Types.DataPortalChildOperationAttribute) &&
          (child ? current.IsDataPortalChildOperationAttribute() : current.IsDataPortalRootOperationAttribute());
      }

      return false;
    }

    private static bool IsCreateChildAttribute(INamedTypeSymbol? attribute)
    {
      for (var current = attribute; current is not null; current = current.BaseType)
      {
        if (current.Name == CslaMemberConstants.Types.CreateChildAttribute &&
          current.ContainingAssembly?.Name == CslaMemberConstants.AssemblyName)
        {
          return true;
        }
      }

      return false;
    }

    // The operations generator emits a mapping for a non-abstract type that is
    // partial, along with every type that contains it. It only picks up
    // methods that carry a CSLA operation attribute directly (not one derived
    // from it, and not one inherited through an override).
    private static bool HasGeneratedOperationMapping(IMethodSymbol method, CancellationToken cancellationToken)
    {
      var type = method.ContainingType;

      if (type.IsAbstract ||
        !method.GetAttributes().Any(_ =>
          _.AttributeClass?.ContainingAssembly?.Name == CslaMemberConstants.AssemblyName &&
          (IsInvokedOperationAttribute(_.AttributeClass, child: false) || IsInvokedOperationAttribute(_.AttributeClass, child: true))))
      {
        return false;
      }

      return type.DeclaringSyntaxReferences
        .Select(_ => _.GetSyntax(cancellationToken))
        .OfType<TypeDeclarationSyntax>()
        .FirstOrDefault() is { } declaration &&
        TypeWithOperationsShouldBePartialAnalyzer.GetNonPartialTypeDeclarations(declaration).Count == 0;
    }

    // AuthorizationRuleManager calls the public static method marked with
    // exactly [ObjectAuthorizationRules]. Only when the type has none does it
    // look up AddObjectAuthorizationRules by name with
    // Type.GetMethod(name, Static | Public | NonPublic | FlattenHierarchy) on
    // the concrete type. That doesn't return private static methods of a base
    // type, throws when there are overloads, and the method is then called
    // with no arguments or with the context.
    private static bool IsCalledObjectAuthorizationRulesMethod(IMethodSymbol method)
    {
      var type = method.ContainingType;

      if (!method.IsStatic || !type.IsStereotype())
      {
        return false;
      }

      if (IsPublicObjectAuthorizationRulesMethod(method))
      {
        return true;
      }

      return method.Name == CslaMemberConstants.Operations.AddObjectAuthorizationRules &&
        !(type.IsAbstract && method.DeclaredAccessibility == Accessibility.Private) &&
        HasObjectAuthorizationRulesSignature(method) &&
        type.GetMembers(method.Name).OfType<IMethodSymbol>().Count(_ => _.IsStatic) == 1 &&
        !type.GetMembers().OfType<IMethodSymbol>().Any(IsPublicObjectAuthorizationRulesMethod);
    }

    private static bool HasObjectAuthorizationRulesSignature(IMethodSymbol method) =>
      method.Parameters.Length == 0 ||
      (method.Parameters.Length == 1 &&
        method.Parameters[0].Type is { Name: CslaMemberConstants.Types.IAddObjectAuthorizationRulesContext } parameterType &&
        parameterType.ContainingAssembly.Name == CslaMemberConstants.AssemblyName);

    private static bool IsPublicObjectAuthorizationRulesMethod(IMethodSymbol method) =>
      method.IsStatic &&
      method.DeclaredAccessibility == Accessibility.Public &&
      method.GetAttributes().Any(_ =>
        _.AttributeClass is { Name: CslaMemberConstants.Types.ObjectAuthorizationRulesAttribute } attribute &&
        attribute.ContainingAssembly.Name == CslaMemberConstants.AssemblyName);
  }
}
