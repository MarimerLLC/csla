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
	[ExportCodeFixProvider(IsBusinessObjectSerializableConstants.DiagnosticId, LanguageNames.CSharp)]
	[Shared]
	public sealed class IsBusinessObjectSerializableMakeSerializableCodeFix
		: CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
		{
			get
			{
				return ImmutableArray.Create(IsBusinessObjectSerializableConstants.DiagnosticId);
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

			var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Serializable"));
			var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList<AttributeSyntax>().Add(attribute));
			var newClassNode = classNode.AddAttributeLists(attributeList);
			var newRoot = root.ReplaceNode(classNode, newClassNode);
			
			context.RegisterCodeFix(
				CodeAction.Create(
					IsBusinessObjectSerializableMakeSerializableCodeFixConstants.Description,
					_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot))), diagnostic);
		}
	}
}