using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class EvaluateManagedBackingFieldsAnalayzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new EvaluateManagedBackingFieldsAnalayzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics.Single(_ => _.Id == EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId);
      Assert.AreEqual(diagnostic.Title.ToString(), EvaluateManagedBackingFieldsAnalayzerConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(diagnostic.MessageFormat.ToString(), EvaluateManagedBackingFieldsAnalayzerConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(diagnostic.Category, EvaluateManagedBackingFieldsAnalayzerConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(diagnostic.DefaultSeverity, DiagnosticSeverity.Error,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldNotUsedByProperty()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassHasManagedBackingFieldNotUsedByProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedProperty()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassHasManagedBackingFieldUsedProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotPublic()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotPublic))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId, EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId, EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotReadonly))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId, EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId });
    }
  }
}