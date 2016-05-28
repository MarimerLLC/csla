using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public async Task AnalyzeWhenClassIsNotMobileObject()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotMobileObject))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndMethodIsNotOperation()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsMobileObjectAndMethodIsNotOperation))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithNoArguments()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithNoArguments))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithSerializableArgument()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithSerializableArgument))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithNonSerializableArgument()
    {
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        $@"Targets\{nameof(FindOperationsWithNonSerializableArgumentsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithNonSerializableArgument))}.cs",
        new[] { Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments });
    }
  }
}
