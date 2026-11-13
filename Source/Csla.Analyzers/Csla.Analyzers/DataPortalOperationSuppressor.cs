using System.Collections.Immutable;
using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
        (IsCalledDataPortalOperation(method) || IsCalledObjectAuthorizationRulesMethod(method));

    private static bool IsOperationCanBeStaticFalsePositive(SuppressionAnalysisContext context, Diagnostic diagnostic) =>
      GetMethod(context, diagnostic) is { } method && IsCalledDataPortalOperation(method);

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
        IsCalledDataPortalOperation(method) &&
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

    // Private operations in a base type stay candidates: when the derived type
    // has no matching operation, the data portal looks again from the base type.
    private static bool IsCalledDataPortalOperation(IMethodSymbol method)
    {
      var type = method.ContainingType;

      // The data portal and the generated mappings only call instance methods.
      if (method.IsStatic || !type.IsStereotype())
      {
        return false;
      }

      var isRoot = method.IsRootDataPortalOperation().ByAttribute;
      var isChild = method.IsChildDataPortalOperation().ByAttribute;

      if (!isRoot && !isChild)
      {
        return false;
      }

      if (!type.HasObjectFactoryAttribute())
      {
        return true;
      }

      // With [ObjectFactory], the factory handles root operations. Child
      // operations run on the business type through its generated mapping;
      // without one, reflection sends them to the factory too, except
      // [CreateChild].
      return isChild && (HasGeneratedOperationMapping(type) || HasCreateChildAttribute(method));
    }

    // The operations generator emits a mapping for a non-abstract type that is
    // declared partial, along with every type that contains it.
    private static bool HasGeneratedOperationMapping(INamedTypeSymbol type)
    {
      if (type.IsAbstract)
      {
        return false;
      }

      for (var current = type; current is not null; current = current.ContainingType)
      {
        if (!current.DeclaringSyntaxReferences.Any(_ =>
          _.GetSyntax() is TypeDeclarationSyntax declaration && declaration.Modifiers.Any(SyntaxKind.PartialKeyword)))
        {
          return false;
        }
      }

      return true;
    }

    // The data portal reads operation attributes with inherit: true, so an
    // override inherits [CreateChild] from the method it overrides.
    private static bool HasCreateChildAttribute(IMethodSymbol method)
    {
      for (var current = method; current is not null; current = current.OverriddenMethod)
      {
        if (current.GetAttributes().Any(_ =>
          _.AttributeClass is { Name: CslaMemberConstants.Types.CreateChildAttribute } attribute &&
          attribute.ContainingAssembly.Name == CslaMemberConstants.AssemblyName))
        {
          return true;
        }
      }

      return false;
    }

    // AuthorizationRuleManager calls the public static method marked with
    // exactly [ObjectAuthorizationRules]. Only when there is none does it call
    // a static method of any accessibility named AddObjectAuthorizationRules.
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
        !type.GetMembers().OfType<IMethodSymbol>().Any(IsPublicObjectAuthorizationRulesMethod);
    }

    private static bool IsPublicObjectAuthorizationRulesMethod(IMethodSymbol method) =>
      method.IsStatic &&
      method.DeclaredAccessibility == Accessibility.Public &&
      method.GetAttributes().Any(_ =>
        _.AttributeClass is { Name: CslaMemberConstants.Types.ObjectAuthorizationRulesAttribute } attribute &&
        attribute.ContainingAssembly.Name == CslaMemberConstants.AssemblyName);
  }
}
