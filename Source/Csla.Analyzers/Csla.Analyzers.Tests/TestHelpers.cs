using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  internal static class TestHelpers
  {
    internal static async Task VerifyActionAsync(List<CodeAction> actions, string title, Document document,
      SyntaxTree tree, string expectedNewText)
    {
      var action = actions.Where(_ => _.Title == title).First();

      var operation = (await action.GetOperationsAsync(
        new CancellationToken(false))).ToArray()[0] as ApplyChangesOperation;
      var newDoc = operation.ChangedSolution.GetDocument(document.Id);
      var newTree = await newDoc.GetSyntaxTreeAsync();
      var changes = newTree.GetChanges(tree);

      Assert.AreEqual(1, changes.Count, nameof(changes.Count));
      Assert.AreEqual(expectedNewText, changes[0].NewText, nameof(TextChange.NewText));
    }

    internal static async Task<List<Diagnostic>> GetDiagnosticsAsync(string code, DiagnosticAnalyzer analyzer)
    {
      var document = TestHelpers.Create(code);
      var root = await document.GetSyntaxRootAsync();
      var compilation = (await document.Project.GetCompilationAsync())
        .WithAnalyzers(ImmutableArray.Create(analyzer));
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
         .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
         .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location))
         .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location))
         .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location))
         .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(BusinessBase<>).Assembly.Location));

      var documentId = DocumentId.CreateNewId(projectId);
      solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(code));

      return solution.GetProject(projectId).Documents.First();
    }
  }
}