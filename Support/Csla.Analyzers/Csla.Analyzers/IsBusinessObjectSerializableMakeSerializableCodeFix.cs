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

			if(root.HasUsing(IsBusinessObjectSerializableMakeSerializableCodeFixConstants.SystemNamespace))
			{
				IsBusinessObjectSerializableMakeSerializableCodeFix.AddCodeFixWhenSystemUsingExists(
					context, root, diagnostic, classNode);
			}
			else
			{
				IsBusinessObjectSerializableMakeSerializableCodeFix.AddCodeFixWhenSystemUsingDoesNotExist(
					context, root, diagnostic, classNode);
			}
		}

		private static SyntaxNode AddAttribute(SyntaxNode root, ClassDeclarationSyntax classNode,
			string name)
		{
			var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(name));
			var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList<AttributeSyntax>().Add(attribute));
			var newClassNode = classNode.AddAttributeLists(attributeList);
			return root.ReplaceNode(classNode, newClassNode);
		}

		private static void AddCodeFixWhenSystemUsingExists(CodeFixContext context, SyntaxNode root, 
			Diagnostic diagnostic, ClassDeclarationSyntax classNode)
		{
			var newRoot = IsBusinessObjectSerializableMakeSerializableCodeFix.AddAttribute(
				root, classNode, IsBusinessObjectSerializableMakeSerializableCodeFixConstants.SerializableName);

			context.RegisterCodeFix(
				CodeAction.Create(
					IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSerializableDescription,
					_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot))), diagnostic);
		}

		private static void AddCodeFixWhenSystemUsingDoesNotExist(CodeFixContext context, SyntaxNode root,
			Diagnostic diagnostic, ClassDeclarationSyntax classNode)
		{
			var qualifiedRoot = IsBusinessObjectSerializableMakeSerializableCodeFix.AddAttribute(
				root, classNode, IsBusinessObjectSerializableMakeSerializableCodeFixConstants.SerializableFullName);

			context.RegisterCodeFix(
				CodeAction.Create(
					IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSystemSerializableDescription,
					_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(qualifiedRoot))), diagnostic);

			var unqualifiedRoot = IsBusinessObjectSerializableMakeSerializableCodeFix.AddAttribute(
				root, classNode, IsBusinessObjectSerializableMakeSerializableCodeFixConstants.SerializableName);

			unqualifiedRoot = (unqualifiedRoot as CompilationUnitSyntax).AddUsings(
				SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(
					IsBusinessObjectSerializableMakeSerializableCodeFixConstants.SystemNamespace)));

			context.RegisterCodeFix(
				CodeAction.Create(
					IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSerializableAndUsingDescription,
					_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(unqualifiedRoot))), diagnostic);
		}
	}
}