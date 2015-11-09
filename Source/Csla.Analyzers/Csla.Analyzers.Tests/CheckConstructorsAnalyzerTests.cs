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

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructor()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructor))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateNoArgumentConstructor()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPrivateNoArgumentConstructor))}.cs",
        new[] { PublicNoArgumentConstructorIsMissingConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPrivateConstructorWithArguments))}.cs",
        new[] { PublicNoArgumentConstructorIsMissingConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments))}.cs",
        new[] { ConstructorHasParametersConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments))}.cs",
        new[] { ConstructorHasParametersConstants.DiagnosticId, PublicNoArgumentConstructorIsMissingConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasStaticConstructor()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasStaticConstructor))}.cs",
        new[] { PublicNoArgumentConstructorIsMissingConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments))}.cs",
        new[] { ConstructorHasParametersConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments))}.cs",
        new[] { ConstructorHasParametersConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments))}.cs",
        new[] { ConstructorHasParametersConstants.DiagnosticId },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments()
    {
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(
        $@"Targets\{nameof(CheckConstructorsAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments))}.cs",
        new string[0]);
    }
  }
}