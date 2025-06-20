using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Csla.Analyzers.ManagedBackingFieldUsesNameof
{
  /// <summary>
  /// 
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class EvaluateManagedBackingFieldsNameofCodeFix
    : CodeFixProvider
  {
    /// <summary>
    /// 
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.EvaluateManagedBackingFieldsNameof];


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
      var diagnostic = context.Diagnostics.First();
      var diagnosticSpan = diagnostic.Location.SourceSpan;

      var parentToken = root.FindToken(diagnosticSpan.Start).Parent;
      if (parentToken is null)
      {
        return;
      }
      var argumentSyntax = parentToken.AncestorsAndSelf().OfType<ArgumentSyntax>().First();

      context.RegisterCodeFix(
        CodeAction.Create(
          title: "Use nameof",
          createChangedDocument: c => UseNameofAsync(context.Document, argumentSyntax, c),
          equivalenceKey: "Use nameof"),
        diagnostic);
    }

    private async Task<Document> UseNameofAsync(Document document, ArgumentSyntax argumentSyntax, CancellationToken cancellationToken)
    {
      var propertyName = "";
      if (argumentSyntax.Expression is SimpleLambdaExpressionSyntax lambdaExpression)
      {
        var memberAccessExpression = (MemberAccessExpressionSyntax)lambdaExpression.Body;
        propertyName = memberAccessExpression.Name.Identifier.ValueText;
      }
      var nameofExpression = SyntaxFactory.ParseExpression($"nameof({propertyName})");

      var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
      if (root is null)
      {
        return document;
      }

      var newRoot = root.ReplaceNode(argumentSyntax.Expression, nameofExpression);

      return document.WithSyntaxRoot(newRoot);
    }
  }
}