using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

      var ctorHasParametersDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule);
      Assert.AreEqual(OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Title, ctorHasParametersDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Message, ctorHasParametersDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, ctorHasParametersDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, ctorHasParametersDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasAbstractProperty()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasAbstractProperty))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasStaticProperty()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasStaticProperty))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithNoMethodCall()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithNoMethodCall))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements))}.cs",
        new[] { Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression))}.cs",
        new[] { Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule, Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithNoMethodCall()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasSetterWithNoMethodCall))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements))}.cs",
        new[] { Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndDirectInvocationExpression()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasSetterWithMethodCallAndDirectInvocationExpression))}.cs",
        Array.Empty<string>());
    }
  }
}
