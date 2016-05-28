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

      var diagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.IsOperationMethodPublic);
      Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Design, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));

      var diagnosticForInterface = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.IsOperationMethodPublic);
      Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.Title, diagnosticForInterface.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.Message, diagnosticForInterface.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Design, diagnosticForInterface.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnosticForInterface.DefaultSeverity,
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
        new[] { Constants.AnalyzerIdentifiers.IsOperationMethodPublic },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed))}.cs",
        new[] { Constants.AnalyzerIdentifiers.IsOperationMethodPublic },
        diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndTypeIsInterface()
    {
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        $@"Targets\{nameof(IsOperationMethodPublicAnalyzerTests)}\{(nameof(this.AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndTypeIsInterface))}.cs",
        new[] { Constants.AnalyzerIdentifiers.IsOperationMethodPublicForInterface });
    }
  }
}