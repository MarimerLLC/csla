using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  internal static class TestHelpers
  {
    internal static async Task VerifyChangesAsync(List<CodeAction> actions, string title, Document document,
      Action<SemanticModel, SyntaxNode> handleChanges)
    {
      var action = actions.Where(_ => _.Title == title).First();
      var operations = (await action.GetOperationsAsync(
        new CancellationToken(false))).ToArray();
      var operation = operations[0] as ApplyChangesOperation;
      var newDoc = operation.ChangedSolution.GetDocument(document.Id);
      var newTree = await newDoc.GetSyntaxTreeAsync();

      handleChanges(await newDoc.GetSemanticModelAsync(), await newTree.GetRootAsync());
    }

    internal static async Task RunAnalysisAsync<T>(string code, string[] diagnosticIds,
      Action<List<Diagnostic>> diagnosticInspector = null, Dictionary<string, string> globalOptions = null)
      where T : DiagnosticAnalyzer, new()
    {
      var diagnostics = await GetDiagnosticsAsync(code, new T(), globalOptions);
      Assert.AreEqual(diagnosticIds.Length, diagnostics.Count, nameof(diagnostics.Count));

      foreach (var diagnosticId in diagnosticIds)
      {
        Assert.IsTrue(diagnostics.Any(_ => _.Id == diagnosticId), diagnosticId);
      }

      diagnosticInspector?.Invoke(diagnostics);
    }

    internal static async Task<List<Diagnostic>> GetDiagnosticsAsync(string code, DiagnosticAnalyzer analyzer,
      Dictionary<string, string> globalOptions = null)
    {
      var document = Create(code);
      var options = new AnalyzerOptions([], new GlobalOptionsProvider(globalOptions ?? []));
      var compilation = (await document.Project.GetCompilationAsync())
        .WithAnalyzers([analyzer], options);
      return (await compilation.GetAnalyzerDiagnosticsAsync()).ToList();
    }

    /// <summary>
    /// Supplies global build properties, such as <c>build_property.*</c> values, to analyzers.
    /// </summary>
    private sealed class GlobalOptionsProvider(Dictionary<string, string> values) : AnalyzerConfigOptionsProvider
    {
      public override AnalyzerConfigOptions GlobalOptions { get; } = new Options(values);

      public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => GlobalOptions;

      public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => GlobalOptions;

      private sealed class Options(Dictionary<string, string> values) : AnalyzerConfigOptions
      {
        public override bool TryGetValue(string key, out string value) => values.TryGetValue(key, out value);
      }
    }

    internal static Document Create(string code)
    {
      var projectName = "Test";
      var projectId = ProjectId.CreateNewId(projectName);

      using var workspace = new AdhocWorkspace();
      var solution = workspace.CurrentSolution
        .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
        .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
        .AddMetadataReferences(projectId, AssemblyReferences.GetMetadataReferences(new[]
        {
          typeof(object).Assembly,
          typeof(Enumerable).Assembly,
          typeof(CSharpCompilation).Assembly,
          typeof(Compilation).Assembly,
          typeof(Attribute).Assembly,
          typeof(Task<>).Assembly,
          typeof(BusinessBase<>).Assembly
        }));

      var documentId = DocumentId.CreateNewId(projectId);
      solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(code));

      return solution.GetProject(projectId).Documents.First();
    }
  }
}