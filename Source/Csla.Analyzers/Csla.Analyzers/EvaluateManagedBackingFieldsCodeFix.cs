using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Editing;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class EvaluateManagedBackingFieldsCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

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