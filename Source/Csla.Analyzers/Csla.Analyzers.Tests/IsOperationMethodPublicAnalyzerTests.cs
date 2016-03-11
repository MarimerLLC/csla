using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
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
      Assert.AreEqual(2, diagnostics.Length);

      var diagnostic = diagnostics.Single(_ => _.Id == IsOperationMethodPublicAnalyzerConstants.DiagnosticId);
      Assert.AreEqual(diagnostic.Title.ToString(), IsOperationMethodPublicAnalyzerConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(diagnostic.MessageFormat.ToString(), IsOperationMethodPublicAnalyzerConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(diagnostic.Category, IsOperationMethodPublicAnalyzerConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(diagnostic.DefaultSeverity, DiagnosticSeverity.Warning,
        nameof(DiagnosticDescriptor.DefaultSeverity));

      var diagnosticForInterface = diagnostics.Single(_ => _.Id == IsOperationMethodPublicAnalyzerConstants.DiagnosticForInterfaceId);
      Assert.AreEqual(diagnosticForInterface.Title.ToString(), IsOperationMethodPublicAnalyzerConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(diagnosticForInterface.MessageFormat.ToString(), IsOperationMethodPublicAnalyzerConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(diagnosticForInterface.Category, IsOperationMethodPublicAnalyzerConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(diagnosticForInterface.DefaultSeverity, DiagnosticSeverity.Warning,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsNotStereotype()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenTypeIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsNotADataPortalOperation()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenTypeIsStereotypeAndMethodIsNotADataPortalOperation))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed))}.cs",
        new[] { IsOperationMethodPublicAnalyzerConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed))}.cs",
        new[] { IsOperationMethodPublicAnalyzerConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndTypeIsInterface()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndTypeIsInterface))}.cs",
        new[] { IsOperationMethodPublicAnalyzerConstants.DiagnosticForInterfaceId });
    }
  }
}