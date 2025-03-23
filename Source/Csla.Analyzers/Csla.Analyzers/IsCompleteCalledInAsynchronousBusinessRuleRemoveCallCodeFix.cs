using System.Collections.Immutable;
using System.Composition;
using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class IsCompleteCalledInAsynchronousBusinessRuleRemoveCallCodeFix
    : CodeFixProvider
  {
    /// <summary>
    /// 
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.CompleteInExecuteAsync];

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
      var methodNode = root.FindNode(diagnostic.Location.SourceSpan) as MethodDeclarationSyntax;
      if (methodNode is null)
      {
        return;
      }

      context.CancellationToken.ThrowIfCancellationRequested();
      await AddCodeFixAsync(context, root, diagnostic, methodNode);
    }

    private static async Task AddCodeFixAsync(CodeFixContext context, SyntaxNode root, Diagnostic diagnostic, MethodDeclarationSyntax methodNode)
    {
      var model = await context.Document.GetSemanticModelAsync(context.CancellationToken);
      var methodSymbol = model.GetDeclaredSymbol(methodNode);
      if (methodSymbol is null)
      {
        return;
      }
      var contextParameter = methodSymbol.Parameters[0];

      var newRoot = root;

      var completeInvocations = methodNode.DescendantNodes(_ => true).OfType<InvocationExpressionSyntax>()
        .Where(invocation =>
        {
          return model.GetSymbolInfo(invocation.Expression).Symbol is IMethodSymbol invocationSymbol &&
            invocationSymbol.Name == "Complete" && SymbolEqualityComparer.Default.Equals(invocationSymbol.ContainingType, contextParameter.Type);
        })
        .Select(invocation => invocation.FindParent<ExpressionStatementSyntax>())
        .Where(i => i is not null)
        .Select(s => s!)
        ;

      newRoot = newRoot.RemoveNodes(completeInvocations, SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepDirectives);
      if (newRoot is null)
      {
        return;
      }
      
      newRoot = newRoot.WithAdditionalAnnotations(Formatter.Annotation);

      context.RegisterCodeFix(
        CodeAction.Create(
          IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants.RemoveCompleteCalls,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants.RemoveCompleteCalls), diagnostic);
    }
  }
}
