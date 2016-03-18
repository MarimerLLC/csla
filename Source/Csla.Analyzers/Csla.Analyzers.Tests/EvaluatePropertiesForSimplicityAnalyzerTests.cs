using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasAbstractProperty()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasAbstractProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasStaticProperty()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasStaticProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithNoMethodCall()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithNoMethodCall))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements))}.cs",
        new[] { OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression))}.cs",
        new[] { OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId, OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithNoMethodCall()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasSetterWithNoMethodCall))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements))}.cs",
        new[] { OnlyUseCslaPropertyMethodsInGetSetRuleConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndDirectInvocationExpression()
    {
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        $@"Targets\{nameof(EvaluatePropertiesForSimplicityAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassHasSetterWithMethodCallAndDirectInvocationExpression))}.cs",
        new string[0]);
    }
  }
}
