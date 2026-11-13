using System.Collections.Immutable;
using Csla.Analyzers.Extensions;
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
    // Every CSLA assembly is signed with CslaKey.snk. Checking the key, not
    // just the name, keeps other assemblies named Csla.* out.
    private static readonly ImmutableArray<byte> cslaPublicKeyToken =
      [0x93, 0xbe, 0x5f, 0xdc, 0x09, 0x3e, 0x4c, 0x30];

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
      if (!context.TryFindNode(diagnostic, out var node, out var model) ||
        node.AncestorsAndSelf().OfType<ExpressionStatementSyntax>().FirstOrDefault() is not { } statement)
      {
        return null;
      }

      // The statement's value is the outermost call, which for a?.B() is the
      // call after the ?. operator.
      var expression = statement.Expression;

      while (true)
      {
        if (expression is ParenthesizedExpressionSyntax parenthesized)
        {
          expression = parenthesized.Expression;
        }
        else if (expression is ConditionalAccessExpressionSyntax conditionalAccess)
        {
          expression = conditionalAccess.WhenNotNull;
        }
        else
        {
          break;
        }
      }

      return expression is InvocationExpressionSyntax invocation
        ? model.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol
        : null;
    }

    private static bool IsCslaFluentMethod(IMethodSymbol method)
    {
      if (method.ContainingAssembly?.Identity.PublicKeyToken.SequenceEqual(cslaPublicKeyToken) != true)
      {
        return false;
      }

      // A matching return type alone doesn't make a method fluent: Clone() and
      // IRuleContext.GetChainedContext() return new objects of their own type.
      // So only CSLA's configuration and builder APIs qualify, when they return
      // the type they were called on:
      // - extension methods (AddCsla, DataPortal, UseHttpProxy, ...), which in
      //   CSLA are all configuration and registration methods;
      // - instance methods of the builder types, named *Options (CslaOptions,
      //   RuleOptions, ...) or *Tester (BusinessRuleTester, ...).
      // Static factory methods and Save() (which returns T) don't qualify.
      // Value types are excluded, because ignoring the result of a method on
      // an immutable struct loses the change.
      ITypeSymbol? receiverType;

      if (method.MethodKind == MethodKind.ReducedExtension)
      {
        receiverType = method.ReceiverType;
      }
      else if (method.IsExtensionMethod)
      {
        receiverType = method.Parameters.FirstOrDefault()?.Type;
      }
      else if (!method.IsStatic && IsBuilderType(method.ContainingType))
      {
        receiverType = method.ContainingType;
      }
      else
      {
        return false;
      }

      return receiverType is not null &&
        method.ReturnType.IsReferenceType &&
        SymbolEqualityComparer.Default.Equals(method.ReturnType, receiverType);
    }

    private static bool IsBuilderType(INamedTypeSymbol type) =>
      type.Name.EndsWith("Options", StringComparison.Ordinal) ||
      type.Name.EndsWith("Tester", StringComparison.Ordinal);
  }
}
