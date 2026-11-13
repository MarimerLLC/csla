// Test-only stubs: they reuse other analyzers' IDs, are created directly by the tests and aren't shipped.
#pragma warning disable RS1001, RS1017, RS2008

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Analyzers.Tests
{
  // The real IDE and CA analyzers aren't available to the tests, so these stubs
  // report the same diagnostic IDs at the same locations, on every candidate.
  // The suppressor tests then check which of those diagnostics are suppressed.

  internal static class StubDescriptors
  {
    internal static DiagnosticDescriptor Create(string id) =>
      new(id, id, id, "Stub", DiagnosticSeverity.Warning, true);
  }

  /// <summary>
  /// Reports a diagnostic on the identifier of every method, like IDE0051 and CA1822 do.
  /// </summary>
  internal abstract class MethodIdentifierStubAnalyzer(string id)
    : DiagnosticAnalyzer
  {
    private readonly DiagnosticDescriptor _rule = StubDescriptors.Create(id);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_rule];

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(
        _ => _.ReportDiagnostic(Diagnostic.Create(_rule, ((MethodDeclarationSyntax)_.Node).Identifier.GetLocation())),
        SyntaxKind.MethodDeclaration);
    }
  }

  internal sealed class UnusedPrivateMemberStubAnalyzer()
    : MethodIdentifierStubAnalyzer(Constants.SuppressedDiagnosticIdentifiers.UnusedPrivateMember);

  internal sealed class MarkMembersAsStaticStubAnalyzer()
    : MethodIdentifierStubAnalyzer(Constants.SuppressedDiagnosticIdentifiers.MarkMembersAsStatic);

  /// <summary>
  /// Reports a diagnostic with an unrelated ID on every method identifier.
  /// </summary>
  internal sealed class UnrelatedStubAnalyzer()
    : MethodIdentifierStubAnalyzer("XYZ0001");

  /// <summary>
  /// Reports IDE0060 on the identifier of every parameter, including lambda and local function parameters.
  /// </summary>
  internal sealed class UnusedParameterStubAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor rule =
      StubDescriptors.Create(Constants.SuppressedDiagnosticIdentifiers.UnusedParameter);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [rule];

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(
        _ => _.ReportDiagnostic(Diagnostic.Create(rule, ((ParameterSyntax)_.Node).Identifier.GetLocation())),
        SyntaxKind.Parameter);
    }
  }

  /// <summary>
  /// Reports IDE0058 on the expression of every expression statement whose value isn't void.
  /// </summary>
  internal sealed class ExpressionValueIsUnusedStubAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor rule =
      StubDescriptors.Create(Constants.SuppressedDiagnosticIdentifiers.ExpressionValueIsUnused);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [rule];

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(_ =>
      {
        var expression = ((ExpressionStatementSyntax)_.Node).Expression;
        var type = _.SemanticModel.GetTypeInfo(expression, _.CancellationToken).Type;

        if (type is not null && type.SpecialType != SpecialType.System_Void &&
          expression is not AssignmentExpressionSyntax)
        {
          _.ReportDiagnostic(Diagnostic.Create(rule, expression.GetLocation()));
        }
      }, SyntaxKind.ExpressionStatement);
    }
  }
}
