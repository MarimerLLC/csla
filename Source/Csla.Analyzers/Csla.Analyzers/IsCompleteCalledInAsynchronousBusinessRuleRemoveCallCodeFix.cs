using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class IsCompleteCalledInAsynchronousBusinessRuleRemoveCallCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds =>
      ImmutableArray.Create(Constants.AnalyzerIdentifiers.CompleteInExecuteAsync);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var methodNode = root.FindNode(diagnostic.Location.SourceSpan) as MethodDeclarationSyntax;

      context.CancellationToken.ThrowIfCancellationRequested();
      await AddCodeFixAsync(context, root, diagnostic, methodNode);
    }

    private static async Task AddCodeFixAsync(CodeFixContext context, SyntaxNode root,
      Diagnostic diagnostic, MethodDeclarationSyntax methodNode)
    {
      var model = await context.Document.GetSemanticModelAsync(context.CancellationToken);
      var methodSymbol = model.GetDeclaredSymbol(methodNode);
      var typeSymbol = methodSymbol.ContainingType;
      var contextParameter = methodSymbol.Parameters[0];

      var newRoot = root;

      var completeInvocations = methodNode.DescendantNodes(_ => true).OfType<InvocationExpressionSyntax>()
        .Where(invocation =>
        {
          return model.GetSymbolInfo(invocation.Expression).Symbol is IMethodSymbol invocationSymbol &&
            invocationSymbol.Name == "Complete" && Equals(invocationSymbol.ContainingType, contextParameter.Type);
        })
        .Select(invocation => invocation.FindParent<ExpressionStatementSyntax>());

      newRoot = newRoot.RemoveNodes(completeInvocations, SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepDirectives)
        .WithAdditionalAnnotations(Formatter.Annotation);

      context.RegisterCodeFix(
        CodeAction.Create(
          IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants.RemoveCompleteCalls,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants.RemoveCompleteCalls), diagnostic);
    }
  }
}
