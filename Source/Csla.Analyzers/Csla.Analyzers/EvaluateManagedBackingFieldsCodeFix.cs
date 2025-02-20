using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Editing;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class EvaluateManagedBackingFieldsCodeFix
    : CodeFixProvider
  {
    /// <summary>
    /// 
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields];

    /// <summary>
    /// 
    /// </summary>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <summary>
    /// 
    /// </summary>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var fieldNode = root.FindNode(diagnostic.Location.SourceSpan);

      context.CancellationToken.ThrowIfCancellationRequested();

      var newFieldNode = fieldNode;

      var generator = SyntaxGenerator.GetGenerator(context.Document);
      newFieldNode = generator.WithModifiers(newFieldNode, DeclarationModifiers.Static + DeclarationModifiers.ReadOnly);
      newFieldNode = generator.WithAccessibility(newFieldNode, Accessibility.Public);

      var newRoot = root.ReplaceNode(fieldNode, newFieldNode);

      context.RegisterCodeFix(
        CodeAction.Create(
          EvaluateManagedBackingFieldsCodeFixConstants.FixManagedBackingFieldDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          EvaluateManagedBackingFieldsCodeFixConstants.FixManagedBackingFieldDescription), diagnostic);
    }
  }
}