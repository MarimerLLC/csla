using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindSaveAssignmentIssueAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new FindSaveAssignmentIssueAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);

      var saveDiagnostic = diagnostics.Single(_ => _.Id == FindSaveAssignmentIssueAnalyzerConstants.DiagnosticId);
      Assert.AreEqual(saveDiagnostic.Title.ToString(), FindSaveAssignmentIssueAnalyzerConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(saveDiagnostic.MessageFormat.ToString(), FindSaveAssignmentIssueAnalyzerConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(saveDiagnostic.Category, FindSaveAssignmentIssueAnalyzerConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(saveDiagnostic.DefaultSeverity, DiagnosticSeverity.Error,
        nameof(DiagnosticDescriptor.DefaultSeverity));

      var saveAsyncDiagnostic = diagnostics.Single(_ => _.Id == FindSaveAsyncAssignmentIssueAnalyzerConstants.DiagnosticId);
      Assert.AreEqual(saveAsyncDiagnostic.Title.ToString(), FindSaveAsyncAssignmentIssueAnalyzerConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(saveAsyncDiagnostic.MessageFormat.ToString(), FindSaveAsyncAssignmentIssueAnalyzerConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(saveAsyncDiagnostic.Category, FindSaveAsyncAssignmentIssueAnalyzerConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(saveAsyncDiagnostic.DefaultSeverity, DiagnosticSeverity.Error,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    private static async Task RunAnalysisAsync(string path, string[] diagnosticIds)
    {
      await FindSaveAssignmentIssueAnalyzerTests.RunAnalysisAsync(path, diagnosticIds, null);
    }

    private static async Task RunAnalysisAsync(string path, string[] diagnosticIds,
      Action<List<Diagnostic>> diagnosticInspector)
    {
      var code = File.ReadAllText(path);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(
        code, new FindSaveAssignmentIssueAnalyzer());
      Assert.AreEqual(diagnosticIds.Length, diagnostics.Count, nameof(diagnostics.Count));

      foreach (var diagnosticId in diagnosticIds)
      {
        Assert.IsTrue(diagnostics.Any(_ => _.Id == diagnosticId), diagnosticId);
      }

      diagnosticInspector?.Invoke(diagnostics);
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase()
    {
      await FindSaveAssignmentIssueAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase()
    {
      await FindSaveAssignmentIssueAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned()
    {
      await FindSaveAssignmentIssueAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned()
    {
      await FindSaveAssignmentIssueAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned()
    {
      await FindSaveAssignmentIssueAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned))}.cs",
        new[] { FindSaveAssignmentIssueAnalyzerConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned()
    {
      await FindSaveAssignmentIssueAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned))}.cs",
        new[] { FindSaveAsyncAssignmentIssueAnalyzerConstants.DiagnosticId });
    }
  }
}
