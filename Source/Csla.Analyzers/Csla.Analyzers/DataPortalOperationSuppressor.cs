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
          Constants.SuppressedDiagnosticIdentifiers.UnusedPrivateMember => unusedOperationRule,
          Constants.SuppressedDiagnosticIdentifiers.MarkMembersAsStatic => operationCanBeStaticRule,
          Constants.SuppressedDiagnosticIdentifiers.UnusedParameter => unusedOperationParameterRule,
          _ => null
        };

        if (descriptor is null)
        {
          continue;
        }

        var method = GetMethod(context, diagnostic);

        if (method is not null && IsCalledByCsla(method))
        {
          context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
        }
      }
    }

    private static IMethodSymbol? GetMethod(SuppressionAnalysisContext context, Diagnostic diagnostic)
    {
      var tree = diagnostic.Location.SourceTree;

      if (tree is null)
      {
        return null;
      }

      var node = tree.GetRoot(context.CancellationToken)
        .FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
      var model = context.GetSemanticModel(tree);

      if (diagnostic.Id == Constants.SuppressedDiagnosticIdentifiers.UnusedParameter)
      {
        // Lambdas and local functions inside an operation are ordinary code,
        // so only parameters of the operation itself qualify.
        var parameter = node.AncestorsAndSelf().OfType<ParameterSyntax>().FirstOrDefault();
        return parameter is not null &&
          model.GetDeclaredSymbol(parameter, context.CancellationToken) is IParameterSymbol { ContainingSymbol: IMethodSymbol { MethodKind: MethodKind.Ordinary } method }
            ? method : null;
      }

      // The nearest member declaration must be the method itself, so a
      // diagnostic on some other member never matches an enclosing method.
      return node.AncestorsAndSelf().OfType<MemberDeclarationSyntax>().FirstOrDefault() is MethodDeclarationSyntax methodNode
        ? model.GetDeclaredSymbol(methodNode, context.CancellationToken) as IMethodSymbol
        : null;
    }

    private static bool IsCalledByCsla(IMethodSymbol method)
    {
      if (method.IsDataPortalOperation())
      {
        return true;
      }

      // AuthorizationRuleManager calls a public static method with
      // [ObjectAuthorizationRules], or else a static method of any
      // accessibility named AddObjectAuthorizationRules.
      var (byNamingConvention, byAttribute) = method.IsAddObjectAuthorizationRulesOperation();
      return method.IsStatic &&
        method.ContainingType.IsStereotype() &&
        (byNamingConvention || (byAttribute && method.DeclaredAccessibility == Accessibility.Public));
    }
  }
}
