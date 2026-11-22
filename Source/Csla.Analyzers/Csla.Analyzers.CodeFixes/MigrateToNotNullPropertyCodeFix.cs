using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Csla.Analyzers
{
  /// <summary>
  /// Migrates a <c>PropertyInfo&lt;T&gt;</c> registration that can migrate with
  /// compile-time consequences only (a value type, a <c>string</c>, or an explicit
  /// non-null default) to <c>RegisterNotNullProperty</c>. Supports Fix All; its
  /// first scope is the containing type.
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class MigrateToNotNullPropertyCodeFix
    : CodeFixProvider
  {
    private static readonly FixAllProvider fixAllProvider = FixAllProvider.Create(
      async (context, document, diagnostics) =>
        await MigrateToNotNullPropertyRewriter.MigrateAsync(document, diagnostics, context.CancellationToken).ConfigureAwait(false),
      [FixAllScope.ContainingType, FixAllScope.Document, FixAllScope.Project, FixAllScope.Solution]);

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.MigrateToNotNullProperty];

    /// <summary>
    ///
    /// </summary>
    public override FixAllProvider GetFixAllProvider() => fixAllProvider;

    /// <summary>
    ///
    /// </summary>
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      foreach (var diagnostic in context.Diagnostics)
      {
        context.RegisterCodeFix(
          CodeAction.Create(MigrateToNotNullPropertyCodeFixConstants.Title,
            cancellationToken => MigrateToNotNullPropertyRewriter.MigrateAsync(context.Document, [diagnostic], cancellationToken),
            MigrateToNotNullPropertyCodeFixConstants.Title),
          diagnostic);
      }
      return Task.CompletedTask;
    }
  }

  /// <summary>
  /// Migrates a <c>PropertyInfo&lt;T&gt;</c> registration of a non-nullable reference
  /// type without a default to <c>RegisterNotNullProperty</c>. The property must then be
  /// loaded on every Create and Fetch path, so there is no Fix All: each one is a decision.
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class MigrateToNotNullPropertyWithReviewCodeFix
    : CodeFixProvider
  {
    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.MigrateToNotNullPropertyWithReview];

    /// <summary>
    /// No Fix All: each property must be checked against its data portal operations.
    /// </summary>
    public override FixAllProvider? GetFixAllProvider() => null;

    /// <summary>
    ///
    /// </summary>
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      foreach (var diagnostic in context.Diagnostics)
      {
        context.RegisterCodeFix(
          CodeAction.Create(MigrateToNotNullPropertyCodeFixConstants.ReviewTitle,
            cancellationToken => MigrateToNotNullPropertyRewriter.MigrateAsync(context.Document, [diagnostic], cancellationToken),
            MigrateToNotNullPropertyCodeFixConstants.ReviewTitle),
          diagnostic);
      }
      return Task.CompletedTask;
    }
  }
}
