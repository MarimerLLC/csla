using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds =>
      ImmutableArray.Create(Constants.AnalyzerIdentifiers.AsynchronousBusinessRuleInheritance);

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

      var returnType = methodNode.ReturnType;
      var methodName = methodNode.Identifier;

      // TODO: OK, do WithReturnType(), and WithIdentifier() on the methodNode.

      // Can also do WithBaseList on the type declaration. Do this by changing
      // GetAllRelevantBaseTypes() to GetBaseTypes(), which will return
      // IEnumerable<SimpleBaseTypeSyntax>. It will yield new base types for the
      // rule types, and leave others as-is.

      var newRoot = root.ReplaceNode(returnType, SyntaxFactory.IdentifierName(nameof(Task)));
      newRoot = newRoot.ReplaceToken(methodName, SyntaxFactory.Identifier("ExecuteAsync"));

      foreach(var (oldBase, newBase) in GetAllRelevantBaseTypes(typeSymbol, model))
      {
        newRoot = newRoot.ReplaceNode(oldBase, newBase);
      }

      context.RegisterCodeFix(
        CodeAction.Create(
          AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants.UpdateToAsyncEquivalentsDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants.UpdateToAsyncEquivalentsDescription), diagnostic);
    }

    private static IEnumerable<(SimpleBaseTypeSyntax, SimpleBaseTypeSyntax)> GetAllRelevantBaseTypes(
      INamedTypeSymbol typeSymbol, SemanticModel model)
    {
      foreach(var typeSymbolReference in typeSymbol.DeclaringSyntaxReferences)
      {
        var typeSymbolReferenceNode = typeSymbolReference.GetSyntax() as TypeDeclarationSyntax;

        foreach (var baseTypeNode in typeSymbolReferenceNode.BaseList.DescendantNodes(_ => true).OfType<SimpleBaseTypeSyntax>())
        {
          var baseTypeNodeIdentifier = baseTypeNode.DescendantNodes().OfType<IdentifierNameSyntax>().Single();

          if(baseTypeNodeIdentifier.Identifier.ValueText == "BusinessRule")
          {
            yield return (baseTypeNode, SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("BusinessRuleAsync")));
          }
          else if (baseTypeNodeIdentifier.Identifier.ValueText == "IBusinessRule")
          {
            yield return (baseTypeNode, SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("IBusinessRuleAsync")));
          }
        }
      }
    }
  }
}