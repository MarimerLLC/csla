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
  [ExportCodeFixProvider(PublicNoArgumentConstructorIsMissingConstants.DiagnosticId, LanguageNames.CSharp)]
  [Shared]
  public sealed class CheckConstructorsAnalyzerAddConstructorCodeFix
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

      CheckConstructorsAnalyzerAddConstructorCodeFix.AddCodeFix(
        context, root, diagnostic, classNode);
    }

    private static SyntaxNode AddAttribute(SyntaxNode root, ClassDeclarationSyntax classNode,
      string name)
    {
      var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(name));
      var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList<AttributeSyntax>().Add(attribute));
      var newClassNode = classNode.AddAttributeLists(attributeList);
      return root.ReplaceNode(classNode, newClassNode);
    }

    private static void AddCodeFix(CodeFixContext context, SyntaxNode root,
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
              SyntaxFactory.TriviaList(),
              SyntaxKind.CloseParenToken,
              SyntaxFactory.TriviaList(SyntaxFactory.Space))))
        .WithBody(SyntaxFactory.Block()
          .WithOpenBraceToken(
            SyntaxFactory.Token(
              SyntaxFactory.TriviaList(),
              SyntaxKind.OpenBraceToken,
              SyntaxFactory.TriviaList(SyntaxFactory.Space)))
          .WithCloseBraceToken(
            SyntaxFactory.Token(
              SyntaxFactory.TriviaList(),
              SyntaxKind.CloseBraceToken,
              SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturn, SyntaxFactory.LineFeed))));
      var newClassNode = classNode.AddMembers(constructor);
      var newRoot = root.ReplaceNode(classNode, newClassNode);

      context.RegisterCodeFix(
        CodeAction.Create(
          CheckConstructorsAnalyzerAddConstructorCodeFixConstants.AddConstructorDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          CheckConstructorsAnalyzerAddConstructorCodeFixConstants.AddConstructorDescription), diagnostic);
    }
  }
}