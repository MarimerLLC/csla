using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using Csla.Analyzers.Extensions;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var invocationNode = root.FindNode(diagnostic.Location.SourceSpan) as InvocationExpressionSyntax;

      var awaitExpressionNode = invocationNode.FindParent<AwaitExpressionSyntax>();

      if(awaitExpressionNode != null)
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
            FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFixConstants.AddAssignmentDescription,
            _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
            FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFixConstants.AddAssignmentDescription), diagnostic);
      }
    }
  }
}