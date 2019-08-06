using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

      var ctorHasParametersDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.ConstructorHasParameters);
      Assert.AreEqual(ConstructorHasParametersConstants.Title, ctorHasParametersDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(ConstructorHasParametersConstants.Message, ctorHasParametersDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, ctorHasParametersDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, ctorHasParametersDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));

      var publicNoArgsCtorDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing);
      Assert.AreEqual(PublicNoArgumentConstructorIsMissingConstants.Title, publicNoArgsCtorDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(PublicNoArgumentConstructorIsMissingConstants.Message, publicNoArgsCtorDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, publicNoArgsCtorDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, publicNoArgsCtorDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructor()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructor))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateNoArgumentConstructor()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPrivateNoArgumentConstructor))}.cs",
        new[] { Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing },
        diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPrivateConstructorWithArguments))}.cs",
        new[] { Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments))}.cs",
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments))}.cs",
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters, Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasStaticConstructor()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasStaticConstructor))}.cs",
        new[] { Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments))}.cs",
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments))}.cs",
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments))}.cs",
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments))}.cs",
        Array.Empty<string>());
    }
  }
}