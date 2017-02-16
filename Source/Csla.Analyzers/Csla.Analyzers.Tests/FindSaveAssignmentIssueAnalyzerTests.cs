using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

      var saveDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.FindSaveAssignmentIssue);
      Assert.AreEqual(FindSaveAssignmentIssueAnalyzerConstants.Title, saveDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindSaveAssignmentIssueAnalyzerConstants.Message, saveDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, saveDiagnostic.Category, 
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, saveDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));

      var saveAsyncDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue);
      Assert.AreEqual(FindSaveAsyncAssignmentIssueAnalyzerConstants.Title, saveAsyncDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindSaveAsyncAssignmentIssueAnalyzerConstants.Message, saveAsyncDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, saveAsyncDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, saveAsyncDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned))}.cs",
        new[] { Constants.AnalyzerIdentifiers.FindSaveAssignmentIssue });
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned))}.cs",
        new[] { Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue });
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectWithinItself()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnABusinessObjectWithinItself))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectWithinItself()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectWithinItself))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectAsThis()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnABusinessObjectAsThis))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsThis()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsThis))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectAsBase()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveIsCalledOnABusinessObjectAsBase))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsBase()
    {
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        $@"Targets\{nameof(FindSaveAssignmentIssueAnalyzerTests)}\{(nameof(this.AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsBase))}.cs",
        Array.Empty<string>());
    }
  }
}
