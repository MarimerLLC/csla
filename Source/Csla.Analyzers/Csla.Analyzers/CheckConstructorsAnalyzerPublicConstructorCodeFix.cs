using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Formatting;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(PublicNoArgumentConstructorIsMissingConstants.DiagnosticId, LanguageNames.CSharp)]
  [Shared]
  public sealed class CheckConstructorsAnalyzerPublicConstructorCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds
    {
      get
      {
        return ImmutableArray.Create(PublicNoArgumentConstructorIsMissingConstants.DiagnosticId);
      }
    }

    public sealed override FixAllProvider GetFixAllProvider()
    {
      return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      if (context.CancellationToken.IsCancellationRequested)
      {
        return;
      }

      var diagnostic = context.Diagnostics.First();
      var classNode = root.FindNode(diagnostic.Location.SourceSpan) as ClassDeclarationSyntax;

      if (context.CancellationToken.IsCancellationRequested)
      {
        return;
      }

      var hasNonPublicNoArgumentConstructor = bool.Parse(
        diagnostic.Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]);

      if (hasNonPublicNoArgumentConstructor)
      {
        if (context.Document.SupportsSemanticModel)
        {
          var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
          CheckConstructorsAnalyzerPublicConstructorCodeFix.AddCodeFixWithUpdatingNonPublicConstructor(
            context, root, diagnostic, classNode, model);
        }
      }
      else
      {
        CheckConstructorsAnalyzerPublicConstructorCodeFix.AddCodeFixWithNewPublicConstructor(
          context, root, diagnostic, classNode);
      }
    }

    private static void AddCodeFixWithNewPublicConstructor(CodeFixContext context, SyntaxNode root,
      Diagnostic diagnostic, ClassDeclarationSyntax classNode)
    {
      // Generated from http://roslynquoter.azurewebsites.net/
      var constructor = SyntaxFactory.ConstructorDeclaration(classNode.Identifier)
        .WithModifiers(
          SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
        .WithParameterList(SyntaxFactory.ParameterList()
          .WithOpenParenToken(
            SyntaxFactory.Token(SyntaxKind.OpenParenToken))
          .WithCloseParenToken(
            SyntaxFactory.Token(
              SyntaxKind.CloseParenToken)))
        .WithBody(SyntaxFactory.Block()
          .WithOpenBraceToken(
            SyntaxFactory.Token(
              SyntaxKind.OpenBraceToken))
          .WithCloseBraceToken(
            SyntaxFactory.Token(
              SyntaxKind.CloseBraceToken))).NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation);
      var newClassNode = classNode.AddMembers(constructor);
      var newRoot = root.ReplaceNode(classNode, newClassNode);

      context.RegisterCodeFix(
        CodeAction.Create(
          CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.AddPublicConstructorDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.AddPublicConstructorDescription), diagnostic);
    }

    private static void AddCodeFixWithUpdatingNonPublicConstructor(CodeFixContext context, SyntaxNode root,
      Diagnostic diagnostic, ClassDeclarationSyntax classNode, SemanticModel model)
    {
      var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
      var classSymbol = model.GetDeclaredSymbol(classNode);

      if (classSymbol != null)
      {
        var constructor = classNode.DescendantNodesAndSelf()
          .Where(_ => _.IsKind(SyntaxKind.ConstructorDeclaration))
          .Cast<ConstructorDeclarationSyntax>()
          .Single(c => model.GetDeclaredSymbol(c).ContainingType == classSymbol &&
            c.ParameterList.Parameters.Count == 0 &&
            !c.Modifiers.Contains(publicModifier));

        var newConstructor = constructor.WithModifiers(SyntaxFactory.TokenList(publicModifier));

        if (constructor.HasLeadingTrivia)
        {
          newConstructor = newConstructor.WithLeadingTrivia(constructor.GetLeadingTrivia());
        }

        if (constructor.HasTrailingTrivia)
        {
          newConstructor = newConstructor.WithTrailingTrivia(constructor.GetTrailingTrivia());
        }

        var newRoot = root.ReplaceNode(constructor, newConstructor);

        context.RegisterCodeFix(
          CodeAction.Create(
            CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription,
            _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
            CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription), diagnostic);
      }
    }
  }
}