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
  public sealed class IsOperationMethodPublicMakeNonPublicCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.AnalyzerIdentifiers.IsOperationMethodPublic);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var methodNode = root.FindNode(diagnostic.Location.SourceSpan) as MethodDeclarationSyntax;

      context.CancellationToken.ThrowIfCancellationRequested();

      RegisterNewCodeFix(
        context, root, methodNode, SyntaxKind.PrivateKeyword,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.PrivateDescription, diagnostic);
      RegisterNewCodeFix(
        context, root, methodNode, SyntaxKind.InternalKeyword,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.InternalDescription, diagnostic);

      var isSealed = bool.Parse(diagnostic.Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]);

      if (!isSealed)
      {
        RegisterNewCodeFix(
          context, root, methodNode, SyntaxKind.ProtectedKeyword,
          IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.ProtectedDescription, diagnostic);
      }
    }

    private static void RegisterNewCodeFix(CodeFixContext context, SyntaxNode root, MethodDeclarationSyntax methodNode,
      SyntaxKind visibility, string description, Diagnostic diagnostic)
    {
      var publicModifier = methodNode.Modifiers.Where(_ => _.Kind() == SyntaxKind.PublicKeyword).First();
      var visibilityNode = SyntaxFactory.Token(publicModifier.LeadingTrivia, visibility,
        publicModifier.TrailingTrivia);
      var modifiers = methodNode.Modifiers.Replace(publicModifier, visibilityNode);
      var newwMethodNode = methodNode.WithModifiers(modifiers);
      var newRoot = root.ReplaceNode(methodNode, newwMethodNode);

      context.RegisterCodeFix(
        CodeAction.Create(description,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)), description), diagnostic);
    }
  }
}