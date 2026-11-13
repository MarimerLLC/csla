using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Analyzers
{
  /// <summary>
  /// Suppresses IDE0058 (expression value is never used) when the expression
  /// calls a CSLA fluent configuration method, such as <c>AddCsla()</c>,
  /// that returns its target only to allow chaining.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class FluentConfigurationSuppressor
    : DiagnosticSuppressor
  {
    private static readonly SuppressionDescriptor resultUnusedRule =
      new(
        Constants.SuppressorIdentifiers.FluentConfigurationResultUnused,
        Constants.SuppressedDiagnosticIdentifiers.ExpressionValueIsUnused,
        FluentConfigurationSuppressorConstants.ResultUnusedJustification);

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => [resultUnusedRule];

    /// <summary>
    ///
    /// </summary>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
      foreach (var diagnostic in context.ReportedDiagnostics)
      {
        if (diagnostic.Id == Constants.SuppressedDiagnosticIdentifiers.ExpressionValueIsUnused &&
          GetInvokedMethod(context, diagnostic) is { } method &&
          IsCslaFluentMethod(method))
        {
          context.ReportSuppression(Suppression.Create(resultUnusedRule, diagnostic));
        }
      }
    }

    private static IMethodSymbol? GetInvokedMethod(SuppressionAnalysisContext context, Diagnostic diagnostic)
    {
      var tree = diagnostic.Location.SourceTree;

      if (tree is null)
      {
        return null;
      }

      var node = tree.GetRoot(context.CancellationToken)
        .FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

      if (node.AncestorsAndSelf().OfType<ExpressionStatementSyntax>().FirstOrDefault() is not { } statement)
      {
        return null;
      }

      var expression = statement.Expression;

      while (expression is ParenthesizedExpressionSyntax parenthesized)
      {
        expression = parenthesized.Expression;
      }

      return expression is InvocationExpressionSyntax invocation
        ? context.GetSemanticModel(tree).GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol
        : null;
    }

    private static bool IsCslaFluentMethod(IMethodSymbol method)
    {
      var assemblyName = method.ContainingAssembly?.Name;

      if (assemblyName is null ||
        !(assemblyName == CslaMemberConstants.AssemblyName || assemblyName.StartsWith(CslaMemberConstants.AssemblyName + ".", StringComparison.Ordinal)))
      {
        return false;
      }

      // Ignoring the result of Save() is a real bug (see FindSaveAssignmentIssueAnalyzer).
      if (method.Name == Constants.SaveMethodNames.Save || method.Name == Constants.SaveMethodNames.SaveAsync)
      {
        return false;
      }

      // A fluent method returns the object it was called on. Value types are
      // excluded, because ignoring the result of a method on an immutable
      // struct loses the change.
      var receiverType = method.MethodKind == MethodKind.Ordinary && method.IsExtensionMethod
        ? method.Parameters.FirstOrDefault()?.Type
        : method.ReceiverType;

      return receiverType is not null &&
        method.ReturnType.IsReferenceType &&
        SymbolEqualityComparer.Default.Equals(method.ReturnType, receiverType);
    }
  }
}
