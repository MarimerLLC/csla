using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using static Csla.Analyzers.Extensions.SyntaxNodeExtensions;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFix
    : CodeFixProvider
  {
    /// <summary>
    /// 
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.FindOperationsWithIncorrectReturnTypes];

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

      if(methodSymbol.IsAsync)
      {
        var newRoot = root.ReplaceNode(methodNode.ReturnType, SyntaxFactory.IdentifierName(typeof(Task).Name));

        if (!root.HasUsing(FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.SystemThreadingTasksNamespace))
        {
          if (newRoot is not CompilationUnitSyntax compilationUnitSyntax)
          {
            return;
          }
          newRoot = compilationUnitSyntax.AddUsings(
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(
              FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.SystemThreadingTasksNamespace)));
        }

        context.RegisterCodeFix(
          CodeAction.Create(
            FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.ChangeReturnTypeToTaskDescription,
            _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
            FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.ChangeReturnTypeToTaskDescription), diagnostic);
      }
      else
      {
        var newRoot = root.ReplaceNode(methodNode.ReturnType, 
          SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)));
        context.RegisterCodeFix(
          CodeAction.Create(
            FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.ChangeReturnTypeToVoidDescription,
            _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
            FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.ChangeReturnTypeToVoidDescription), diagnostic);
      }
    }
  }
}