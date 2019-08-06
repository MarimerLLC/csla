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
using System.Collections.Generic;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class DoesChildOperationHaveRunLocalRemoveAttributeCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.AnalyzerIdentifiers.DoesChildOperationHaveRunLocal);

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
      var newRoot = root;

      var model = await context.Document.GetSemanticModelAsync(context.CancellationToken);
      var methodSymbol = model.GetDeclaredSymbol(methodNode);
      var runLocalAttributeSymbol = methodSymbol.GetAttributes().First(_ => _.AttributeClass.IsRunLocalAttribute());

      foreach(var attribute in methodSymbol.GetAttributes().Where(_ => _.AttributeClass.IsRunLocalAttribute()))
      {
        newRoot = newRoot.RemoveNode(attribute.ApplicationSyntaxReference.GetSyntax(), SyntaxRemoveOptions.KeepNoTrivia);
      }

      var attributeListsToRemove = new List<AttributeListSyntax>();

      foreach (var attributeList in newRoot.DescendantNodes(_ => true).OfType<AttributeListSyntax>())
      {
        if (attributeList.Attributes.Count == 0)
        {
          attributeListsToRemove.Add(attributeList);
        }
      }

      foreach(var attributeListToRemove in attributeListsToRemove)
      {
        newRoot = newRoot.RemoveNode(attributeListToRemove, SyntaxRemoveOptions.KeepEndOfLine);
      }

      context.RegisterCodeFix(
        CodeAction.Create(
          DoesChildOperationHaveRunLocalRemoveAttributeCodeFixConstants.RemoveRunLocalDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          DoesChildOperationHaveRunLocalRemoveAttributeCodeFixConstants.RemoveRunLocalDescription), diagnostic);
    }
  }
}