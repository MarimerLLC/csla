using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class CheckConstructorsAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new CheckConstructorsAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);

      var ctorHasParametersDiagnostic = diagnostics.Single(_ => _.Id == ConstructorHasParametersConstants.DiagnosticId);
      Assert.AreEqual(ctorHasParametersDiagnostic.Title.ToString(), ConstructorHasParametersConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(ctorHasParametersDiagnostic.MessageFormat.ToString(), ConstructorHasParametersConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(ctorHasParametersDiagnostic.Category, ConstructorHasParametersConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(ctorHasParametersDiagnostic.DefaultSeverity, DiagnosticSeverity.Warning,
        nameof(DiagnosticDescriptor.DefaultSeverity));

      var publicNoArgsCtorDiagnostic = diagnostics.Single(_ => _.Id == PublicNoArgumentConstructorIsMissingConstants.DiagnosticId);
      Assert.AreEqual(publicNoArgsCtorDiagnostic.Title.ToString(), PublicNoArgumentConstructorIsMissingConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(publicNoArgsCtorDiagnostic.MessageFormat.ToString(), PublicNoArgumentConstructorIsMissingConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(publicNoArgsCtorDiagnostic.Category, PublicNoArgumentConstructorIsMissingConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(publicNoArgsCtorDiagnostic.DefaultSeverity, DiagnosticSeverity.Error,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    private static async Task RunAnalysisAsync(string path, string[] diagnosticIds)
    {
      var code = File.ReadAllText(path);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(
        code, new CheckConstructorsAnalyzer());
      Assert.AreEqual(diagnosticIds.Length, diagnostics.Count, nameof(diagnostics.Count));

      foreach (var diagnosticId in diagnosticIds)
      {
        Assert.IsTrue(diagnostics.Any(_ => _.Id == diagnosticId), diagnosticId);
      }
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await CheckConstructorsAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructor()
    {
      await CheckConstructorsAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructor))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateNoArgumentConstructor()
    {
      await CheckConstructorsAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPrivateNoArgumentConstructor))}.cs",
        new[] { PublicNoArgumentConstructorIsMissingConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateConstructorWithArguments()
    {
      await CheckConstructorsAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPrivateConstructorWithArguments))}.cs",
        new[] { PublicNoArgumentConstructorIsMissingConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      await CheckConstructorsAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments))}.cs",
        new[] { ConstructorHasParametersConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      await CheckConstructorsAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments))}.cs",
        new[] { ConstructorHasParametersConstants.DiagnosticId, PublicNoArgumentConstructorIsMissingConstants.DiagnosticId });
    }
  }
}