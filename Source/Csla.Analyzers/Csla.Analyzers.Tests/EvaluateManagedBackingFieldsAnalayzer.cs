using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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

    private static async Task RunAnalysisAsync(string path, int expectedDiagnosticCount)
    {
      var code = File.ReadAllText(path);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(
        code, new EvaluateManagedBackingFieldsAnalayzer());
      Assert.AreEqual(expectedDiagnosticCount, diagnostics.Count, nameof(diagnostics.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        0);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldNotUsedByProperty()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldNotUsedByProperty))}.cs",
        0);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedProperty()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedProperty))}.cs",
        0);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotPublic()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotPublic))}.cs",
        1);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotStatic()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotStatic))}.cs",
        1);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotReadonly))}.cs",
        1);
    }
  }
}