using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
	internal static class TestHelpers
	{
		internal static async Task<List<Diagnostic>> GetDiagnosticsAsync(string code)
		{
			var document = TestHelpers.Create(code);
			var root = await document.GetSyntaxRootAsync();
			var compilation = (await document.Project.GetCompilationAsync())
				.WithAnalyzers(ImmutableArray.Create(new IsBusinessObjectSerializableAnalyzer() as DiagnosticAnalyzer));
			return (await compilation.GetAnalyzerDiagnosticsAsync()).ToList();
		}

		internal static Document Create(string code)
		{
			var projectName = "Test";
			var projectId = ProjectId.CreateNewId(projectName);

			var solution = new AdhocWorkspace()
				 .CurrentSolution
				 .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
				 .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(object).Assembly))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(Enumerable).Assembly))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(CSharpCompilation).Assembly))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(Compilation).Assembly))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(BusinessBase<>).Assembly));

			var documentId = DocumentId.CreateNewId(projectId);
			solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(code));

			return solution.GetProject(projectId).Documents.First();
		}
	}
}
