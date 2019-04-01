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
  public sealed class FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.AnalyzerIdentifiers.FindSaveAssignmentIssue);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var invocationNode = root.FindNode(diagnostic.Location.SourceSpan) as InvocationExpressionSyntax;

      var invocationIdentifier = ((invocationNode.Expression as MemberAccessExpressionSyntax)
        .Expression as IdentifierNameSyntax).Identifier;
      var leadingTrivia = invocationIdentifier.HasLeadingTrivia ? 
        invocationIdentifier.LeadingTrivia : new SyntaxTriviaList();

      var newInvocationIdentifier = invocationIdentifier.WithLeadingTrivia(new SyntaxTriviaList());
      var newInvocationNode = invocationNode.ReplaceToken(invocationIdentifier, newInvocationIdentifier);

      context.CancellationToken.ThrowIfCancellationRequested();

      var simpleAssignmentExpressionNode = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
        SyntaxFactory.IdentifierName(newInvocationIdentifier), newInvocationNode)
        .WithLeadingTrivia(leadingTrivia);

      var newRoot = root.ReplaceNode(invocationNode, simpleAssignmentExpressionNode);

      context.RegisterCodeFix(
        CodeAction.Create(
          FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixConstants.AddAssignmentDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixConstants.AddAssignmentDescription), diagnostic);
    }
  }
}