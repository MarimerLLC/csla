using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using static Csla.Analyzers.Extensions.SyntaxNodeExtensions;
using static Csla.Analyzers.Constants;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class IsBusinessObjectSerializableMakeSerializableCodeFix
    : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.AnalyzerIdentifiers.IsBusinessObjectSerializable);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var classNode = root.FindNode(diagnostic.Location.SourceSpan) as ClassDeclarationSyntax;

      context.CancellationToken.ThrowIfCancellationRequested();

      AddCodeFix(context, root, diagnostic, classNode);
    }

    private static SyntaxNode AddAttribute(SyntaxNode root, ClassDeclarationSyntax classNode, string name)
    {
      var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(name));
      var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList<AttributeSyntax>().Add(attribute));
      var newClassNode = classNode.AddAttributeLists(attributeList);
      return root.ReplaceNode(classNode, newClassNode);
    }

    private static void AddCodeFix(CodeFixContext context, SyntaxNode root,
      Diagnostic diagnostic, ClassDeclarationSyntax classNode)
    {
      var newRoot = AddAttribute(
        root, classNode, IsBusinessObjectSerializableMakeSerializableCodeFixConstants.SerializableName);

      var description = IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSerializableDescription;

      if (!root.HasUsing(Namespaces.System))
      {
        newRoot = (newRoot as CompilationUnitSyntax).AddUsings(
          SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Namespaces.System)));
        description = IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSerializableAndUsingDescription;
      }

      context.RegisterCodeFix(
        CodeAction.Create(
          description,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          description), diagnostic);
    }
  }
}