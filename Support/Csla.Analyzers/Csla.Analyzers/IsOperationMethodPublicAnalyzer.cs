using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using static Csla.Analyzers.Extensions.IMethodSymbolExtensions;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class IsOperationMethodPublicAnalyzer 
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor makeNonPublicRule = new DiagnosticDescriptor(
			IsOperationMethodPublicAnalyzerConstants.DiagnosticId, IsOperationMethodPublicAnalyzerConstants.Title,
			IsOperationMethodPublicAnalyzerConstants.Message, IsOperationMethodPublicAnalyzerConstants.Category, 
			DiagnosticSeverity.Warning, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(IsOperationMethodPublicAnalyzer.makeNonPublicRule);
			}
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction<SyntaxKind>(
				IsOperationMethodPublicAnalyzer.AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
		}

		private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
		{
			var methodNode = (MethodDeclarationSyntax)context.Node;
			var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);
			var classSymbol = methodSymbol.ContainingType;

			if (classSymbol.IsStereotype() && methodSymbol.IsDataPortalOperation() && 
				methodSymbol.DeclaredAccessibility == Accessibility.Public)
			{
				var properties = new Dictionary<string, string>()
				{
					[IsOperationMethodPublicAnalyzerConstants.IsSealed] = classSymbol.IsSealed.ToString()
				};

            context.ReportDiagnostic(Diagnostic.Create(IsOperationMethodPublicAnalyzer.makeNonPublicRule,
					methodNode.Identifier.GetLocation(), 
					properties.ToImmutableDictionary()));
				return;
			}
		}
	}
}
