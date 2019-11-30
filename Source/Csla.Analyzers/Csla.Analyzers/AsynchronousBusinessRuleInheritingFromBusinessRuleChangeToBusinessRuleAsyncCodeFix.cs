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
using static Csla.Analyzers.Constants;

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

      var newRoot = root;

      foreach (var typeSymbolReference in typeSymbol.DeclaringSyntaxReferences)
      {
        var typeNode = typeSymbolReference.GetSyntax() as TypeDeclarationSyntax;

        var newTypeNode = typeNode.WithBaseList(GetBaseTypes(typeNode))
          .WithTriviaFrom(typeNode);

        var currentMethodNode = newTypeNode.DescendantNodes(_ => true).OfType<MethodDeclarationSyntax>()
          .Single(_ => _.Identifier.ValueText == "Execute");
        var newReturnType = SyntaxFactory.IdentifierName(nameof(Task))
          .WithTriviaFrom(currentMethodNode.ReturnType);
        var newIdentifier = SyntaxFactory.Identifier("ExecuteAsync")
          .WithTriviaFrom(currentMethodNode.Identifier);

        var newMethodNode = currentMethodNode.WithReturnType(newReturnType)
          .WithIdentifier(newIdentifier)
          .WithTriviaFrom(currentMethodNode);

        newTypeNode = newTypeNode.ReplaceNode(currentMethodNode, newMethodNode);
        newRoot = newRoot.ReplaceNode(typeNode, newTypeNode);

        if (!newRoot.HasUsing(Namespaces.SystemThreadingTasks))
        {
          newRoot = (newRoot as CompilationUnitSyntax).AddUsings(
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(
              Namespaces.SystemThreadingTasks)));
        }
      }

      context.RegisterCodeFix(
        CodeAction.Create(
          AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants.UpdateToAsyncEquivalentsDescription,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants.UpdateToAsyncEquivalentsDescription), diagnostic);
    }

    private static BaseListSyntax GetBaseTypes(TypeDeclarationSyntax typeNode)
    {
      var currentBaseList = typeNode.BaseList;

      var list = new SeparatedSyntaxList<BaseTypeSyntax>();

      foreach (var baseTypeNode in typeNode.BaseList.DescendantNodes(_ => true).OfType<SimpleBaseTypeSyntax>())
      {
        var baseTypeNodeIdentifier = baseTypeNode.DescendantNodes().OfType<IdentifierNameSyntax>().Single();

        if (baseTypeNodeIdentifier.Identifier.ValueText == "BusinessRule")
        {
          list = list.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("BusinessRuleAsync"))
            .WithTriviaFrom(baseTypeNode));
        }
        else if (baseTypeNodeIdentifier.Identifier.ValueText == "IBusinessRule")
        {
          list = list.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("IBusinessRuleAsync"))
            .WithTriviaFrom(baseTypeNode));
        }
        else
        {
          list = list.Add(baseTypeNode);
        }
      }

      return SyntaxFactory.BaseList(list).WithTriviaFrom(currentBaseList);
    }
  }
}