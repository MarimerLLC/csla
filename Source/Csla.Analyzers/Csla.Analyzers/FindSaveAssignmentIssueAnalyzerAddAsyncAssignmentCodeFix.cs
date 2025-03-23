using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using Csla.Analyzers.Extensions;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFix
    : CodeFixProvider
  {
    /// <summary>
    /// 
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue];

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
      if (root.FindNode(diagnostic.Location.SourceSpan) is not InvocationExpressionSyntax invocationNode)
      {
        return;
      }

      if (invocationNode.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax || memberAccessExpressionSyntax.Expression is not IdentifierNameSyntax identifierNameSyntax)
      {
        return;
      }

      var awaitExpressionNode = invocationNode.FindParent<AwaitExpressionSyntax>();
      if (awaitExpressionNode == null)
      {
        return;
      }

      var awaitKeyword = awaitExpressionNode.AwaitKeyword;
      var leadingTrivia = awaitKeyword.HasLeadingTrivia ? awaitKeyword.LeadingTrivia : new SyntaxTriviaList();

      var newAwaitExpressionNode = awaitExpressionNode.WithAwaitKeyword(awaitKeyword.WithLeadingTrivia(new SyntaxTriviaList()));
      var invocationIdentifier = identifierNameSyntax.Identifier;
      var newInvocationIdentifier = invocationIdentifier.WithLeadingTrivia(new SyntaxTriviaList());

      context.CancellationToken.ThrowIfCancellationRequested();

      var simpleAssignmentExpressionNode = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
        SyntaxFactory.IdentifierName(newInvocationIdentifier), newAwaitExpressionNode)
        .WithLeadingTrivia(leadingTrivia);

      var newRoot = root.ReplaceNode(awaitExpressionNode, simpleAssignmentExpressionNode);

      context.RegisterCodeFix(
        CodeAction.Create(
          FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixConstants.AddAssignmentDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixConstants.AddAssignmentDescription), diagnostic);
    }
  }
}