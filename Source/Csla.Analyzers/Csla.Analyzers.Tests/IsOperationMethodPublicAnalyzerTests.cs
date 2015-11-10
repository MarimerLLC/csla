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

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndMethodIsNotADataPortalOperation()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndMethodIsNotADataPortalOperation))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed))}.cs",
        new[] { IsOperationMethodPublicAnalyzerConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed))}.cs",
        new[] { IsOperationMethodPublicAnalyzerConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }
  }
}