using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using System.Collections.Generic;
using Csla.Analyzers.Extensions;

namespace Csla.Analyzers
{
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class DoesOperationHaveAttributeAddAttributeCodeFix
    : CodeFixProvider
  {
    private static readonly ImmutableDictionary<string, string> nameAttributeMap = new Dictionary<string, string>
    {
      { CslaMemberConstants.Operations.DataPortalCreate, CslaMemberConstants.OperationAttributes.Create },
      { CslaMemberConstants.Operations.DataPortalFetch, CslaMemberConstants.OperationAttributes.Fetch },
      { CslaMemberConstants.Operations.DataPortalInsert, CslaMemberConstants.OperationAttributes.Insert },
      { CslaMemberConstants.Operations.DataPortalUpdate, CslaMemberConstants.OperationAttributes.Update },
      { CslaMemberConstants.Operations.DataPortalDelete, CslaMemberConstants.OperationAttributes.Delete },
      { CslaMemberConstants.Operations.DataPortalDeleteSelf, CslaMemberConstants.OperationAttributes.DeleteSelf },
      { CslaMemberConstants.Operations.DataPortalExecute, CslaMemberConstants.OperationAttributes.Execute },
      { CslaMemberConstants.Operations.ChildCreate, CslaMemberConstants.OperationAttributes.CreateChild },
      { CslaMemberConstants.Operations.ChildFetch, CslaMemberConstants.OperationAttributes.FetchChild },
      { CslaMemberConstants.Operations.ChildInsert, CslaMemberConstants.OperationAttributes.InsertChild },
      { CslaMemberConstants.Operations.ChildUpdate, CslaMemberConstants.OperationAttributes.UpdateChild },
      { CslaMemberConstants.Operations.ChildDeleteSelf, CslaMemberConstants.OperationAttributes.DeleteSelfChild },
      { CslaMemberConstants.Operations.ChildExecute, CslaMemberConstants.OperationAttributes.ExecuteChild }
    }.ToImmutableDictionary();

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.AnalyzerIdentifiers.DoesOperationHaveAttribute);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      context.CancellationToken.ThrowIfCancellationRequested();

      var diagnostic = context.Diagnostics.First();
      var methodNode = root.FindNode(diagnostic.Location.SourceSpan) as MethodDeclarationSyntax;

      context.CancellationToken.ThrowIfCancellationRequested();

      AddCodeFix(context, root, diagnostic, methodNode);
    }

    private static SyntaxNode AddAttribute(SyntaxNode root, MethodDeclarationSyntax methodNode, string name)
    {
      var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(name));
      var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList<AttributeSyntax>().Add(attribute));
      var newClassNode = methodNode.AddAttributeLists(attributeList);
      return root.ReplaceNode(methodNode, newClassNode);
    }

    private static void AddCodeFix(CodeFixContext context, SyntaxNode root,
      Diagnostic diagnostic, MethodDeclarationSyntax methodNode)
    {
      var newRoot = AddAttribute(root, methodNode, nameAttributeMap[methodNode.Identifier.ValueText]);

      var description = DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription;

      if (!root.HasUsing(DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.CslaNamespace))
      {
        newRoot = (newRoot as CompilationUnitSyntax).AddUsings(
          SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(
            DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.CslaNamespace)));
        description = DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription;
      }

      context.RegisterCodeFix(
        CodeAction.Create(
          description,
          _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
          description), diagnostic);
    }
  }
}