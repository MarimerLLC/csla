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
  public sealed class EvaluatePropertiesForSimplicityAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new EvaluatePropertiesForSimplicityAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var ctorHasParametersDiagnostic = diagnostics.Single(_ => _.Id == OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId);
      Assert.AreEqual(ctorHasParametersDiagnostic.Title.ToString(), OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(ctorHasParametersDiagnostic.MessageFormat.ToString(), OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(ctorHasParametersDiagnostic.Category, OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(ctorHasParametersDiagnostic.DefaultSeverity, DiagnosticSeverity.Warning,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    private static async Task RunAnalysisAsync(string path, string[] diagnosticIds)
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(path, diagnosticIds, null);
    }

    private static async Task RunAnalysisAsync(string path, string[] diagnosticIds,
      Action<List<Diagnostic>> diagnosticInspector)
    {
      var code = File.ReadAllText(path);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(
        code, new EvaluatePropertiesForSimplicityAnalyzer());
      Assert.AreEqual(diagnosticIds.Length, diagnostics.Count, nameof(diagnostics.Count));

      foreach (var diagnosticId in diagnosticIds)
      {
        Assert.IsTrue(diagnostics.Any(_ => _.Id == diagnosticId), diagnosticId);
      }

      diagnosticInspector?.Invoke(diagnostics);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasAbstractProperty()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasAbstractProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasStaticProperty()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasStaticProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasGetterWithNoMethodCall()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasGetterWithNoMethodCall))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallButIsNotCslaPropertyMethod))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndMultipleStatements()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndMultipleStatements))}.cs",
        new[] { OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression))}.cs",
        new[] { OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndReturnAndDirectInvocationExpression()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndReturnAndDirectInvocationExpression))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasSetterWithNoMethodCall()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasSetterWithNoMethodCall))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallButIsNotCslaPropertyMethod))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallAndMultipleStatements()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallAndMultipleStatements))}.cs",
        new[] { OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallAndDirectInvocationExpression()
    {
      await EvaluatePropertiesForSimplicityAnalyzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallAndDirectInvocationExpression))}.cs",
        new string[0]);
    }
  }
}
