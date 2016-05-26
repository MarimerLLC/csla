using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class EvaluateManagedBackingFieldsCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds
    {
      get
      {
        return ImmutableArray.Create(Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields);
      }
    }

    public sealed override FixAllProvider GetFixAllProvider()
    {
      return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var fieldNode = root.FindNode(diagnostic.Location.SourceSpan) as FieldDeclarationSyntax;

      context.CancellationToken.ThrowIfCancellationRequested();

      var newFieldNode = fieldNode;

      newFieldNode = newFieldNode.WithModifiers(SyntaxFactory.TokenList(
          SyntaxFactory.Token(SyntaxKind.PublicKeyword),
          SyntaxFactory.Token(SyntaxKind.StaticKeyword),
          SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)));

      var newRoot = root.ReplaceNode(fieldNode, newFieldNode);

      context.RegisterCodeFix(
        CodeAction.Create(
          EvaluateManagedBackingFieldsCodeFixConstants.FixManagedBackingFieldDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          EvaluateManagedBackingFieldsCodeFixConstants.FixManagedBackingFieldDescription), diagnostic);
    }
  }
}