using System.Collections.Immutable;
using System.Composition;
using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue);

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
      var invocationNode = root.FindNode(diagnostic.Location.SourceSpan) as InvocationExpressionSyntax;

      var awaitExpressionNode = invocationNode.FindParent<AwaitExpressionSyntax>();

      if (awaitExpressionNode != null)
      {
        var awaitKeyword = awaitExpressionNode.AwaitKeyword;
        var leadingTrivia = awaitKeyword.HasLeadingTrivia ?
          awaitKeyword.LeadingTrivia : new SyntaxTriviaList();

        var newAwaitExpressionNode = awaitExpressionNode.WithAwaitKeyword(
          awaitKeyword.WithLeadingTrivia(new SyntaxTriviaList()));
        var invocationIdentifier = ((invocationNode.Expression as MemberAccessExpressionSyntax)
          .Expression as IdentifierNameSyntax).Identifier;
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
}