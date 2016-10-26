using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindOperationsWithNonSerializableArgumentsAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new FindOperationsWithNonSerializableArgumentsAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(FindOperationsWithNonSerializableArgumentsConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindOperationsWithNonSerializableArgumentsConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Design, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWithNotMobileObject()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWithNotMobileObject))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsNotOperation()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWithMobileObjectAndMethodIsNotOperation))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNoArguments()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWithMobileObjectAndMethodIsRootOperationWithNoArguments))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument))}.cs",
        new[] { Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments });
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithNoArguments()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWithMobileObjectAndMethodIsChildOperationWithNoArguments))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument))}.cs",
        Array.Empty<string>());
    }
  }
}
