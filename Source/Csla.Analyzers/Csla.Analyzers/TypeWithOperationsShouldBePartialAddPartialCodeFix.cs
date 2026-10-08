using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Analyzers
{
  /// <summary>
  /// Adds the <c>partial</c> modifier to a type with data portal
  /// operation methods and to all of its containing types.
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class TypeWithOperationsShouldBePartialAddPartialCodeFix
    : CodeFixProvider
  {
    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial];

    /// <summary>
    ///
    /// </summary>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <summary>
    ///
    /// </summary>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
      if (root is null)
      {
        return;
      }

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var classNode = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ClassDeclarationSyntax>();
      if (classNode is null)
      {
        return;
      }

      var nonPartialNodes = TypeWithOperationsShouldBePartialAnalyzer.GetNonPartialTypeDeclarations(classNode);
      if (nonPartialNodes.Count == 0)
      {
        return;
      }

      context.CancellationToken.ThrowIfCancellationRequested();

      var newRoot = root.ReplaceNodes(nonPartialNodes, (_, rewritten) => AddPartialModifier(rewritten));
      var description = TypeWithOperationsShouldBePartialAddPartialCodeFixConstants.AddPartialDescription;

      context.RegisterCodeFix(
        CodeAction.Create(description,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)), description), diagnostic);
    }

    private static TypeDeclarationSyntax AddPartialModifier(TypeDeclarationSyntax node)
    {
      if (node.Modifiers.Count == 0)
      {
        // No modifiers: the partial keyword takes over the leading trivia
        // (indentation, comments) of the type keyword.
        var keyword = node.Keyword;
        var partialToken = SyntaxFactory.Token(
          keyword.LeadingTrivia, SyntaxKind.PartialKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space));
        return node
          .WithKeyword(keyword.WithLeadingTrivia(SyntaxFactory.TriviaList()))
          .WithModifiers(SyntaxFactory.TokenList(partialToken));
      }

      // partial must be the last modifier, immediately before the type keyword.
      var partialModifier = SyntaxFactory.Token(SyntaxKind.PartialKeyword)
        .WithTrailingTrivia(SyntaxFactory.Space);
      return node.WithModifiers(node.Modifiers.Add(partialModifier));
    }
  }
}
