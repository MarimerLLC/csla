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
using Microsoft.CodeAnalysis.Editing;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class CheckConstructorsAnalyzerPublicConstructorCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds => 
      ImmutableArray.Create(Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var classNode = root.FindNode(diagnostic.Location.SourceSpan) as ClassDeclarationSyntax;

      context.CancellationToken.ThrowIfCancellationRequested();

      var hasNonPublicNoArgumentConstructor = bool.Parse(
        diagnostic.Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]);

      if (hasNonPublicNoArgumentConstructor)
      {
        if (context.Document.SupportsSemanticModel)
        {
          var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
          AddCodeFixWithUpdatingNonPublicConstructor(
            context, root, diagnostic, classNode, model);
        }
      }
      else
      {
        AddCodeFixWithNewPublicConstructor(
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
        var constructorSymbol = classSymbol.Constructors
          .Single(_ => _.Parameters.Count() == 0 &&
            !_.DeclaredAccessibility.HasFlag(Accessibility.Public));

        var constructor = constructorSymbol.DeclaringSyntaxReferences[0].GetSyntax(context.CancellationToken);

        var newConstructor = constructor;

        var generator = SyntaxGenerator.GetGenerator(context.Document);
        newConstructor = generator.WithAccessibility(newConstructor, Accessibility.Public);

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