using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class IsOperationMethodPublicAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new IsOperationMethodPublicAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(diagnostic.Id, IsOperationMethodPublicAnalyzerConstants.DiagnosticId,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(diagnostic.Title.ToString(), IsOperationMethodPublicAnalyzerConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(diagnostic.MessageFormat.ToString(), IsOperationMethodPublicAnalyzerConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(diagnostic.Category, IsOperationMethodPublicAnalyzerConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(diagnostic.DefaultSeverity, DiagnosticSeverity.Warning,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    private static async Task RunAnalysisAsync(string path, int expectedDiagnosticCount)
    {
      await IsOperationMethodPublicAnalyzerTests.RunAnalysis(path, expectedDiagnosticCount, null);
    }

    private static async Task RunAnalysis(string path, int expectedDiagnosticCount,
      Action<List<Diagnostic>> diagnosticInspector)
    {
      var code = File.ReadAllText(path);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(
        code, new IsOperationMethodPublicAnalyzer());
      Assert.AreEqual(expectedDiagnosticCount, diagnostics.Count, nameof(diagnostics.Count));
      diagnosticInspector?.Invoke(diagnostics);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await IsOperationMethodPublicAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        0);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndMethodIsNotADataPortalOperation()
    {
      await IsOperationMethodPublicAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndMethodIsNotADataPortalOperation))}.cs",
        0);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic()
    {
      await IsOperationMethodPublicAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic))}.cs",
        0);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed()
    {
      await IsOperationMethodPublicAnalyzerTests.RunAnalysis(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed))}.cs",
        1, diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed()
    {
      await IsOperationMethodPublicAnalyzerTests.RunAnalysis(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed))}.cs",
        1, diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }
  }
}