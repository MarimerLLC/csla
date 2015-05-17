using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class IsBusinessObjectSerializableAnalyzer 
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor makeSerializableRule = new DiagnosticDescriptor(
			IsBusinessObjectSerializableConstants.DiagnosticId, IsBusinessObjectSerializableConstants.Title,
			IsBusinessObjectSerializableConstants.Message, IsBusinessObjectSerializableConstants.Category, 
			DiagnosticSeverity.Error, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(IsBusinessObjectSerializableAnalyzer.makeSerializableRule);
			}
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction<SyntaxKind>(
				IsBusinessObjectSerializableAnalyzer.AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
		}

		private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
		{
			var classNode = (ClassDeclarationSyntax)context.Node;
			var classSymbol = context.SemanticModel.GetDeclaredSymbol(classNode);

         if (classSymbol.IsStereotype() && !classSymbol.IsSerializable())
			{
				context.ReportDiagnostic(Diagnostic.Create(IsBusinessObjectSerializableAnalyzer.makeSerializableRule,
					classNode.Identifier.GetLocation()));
				return;
			}
		}
	}
}
