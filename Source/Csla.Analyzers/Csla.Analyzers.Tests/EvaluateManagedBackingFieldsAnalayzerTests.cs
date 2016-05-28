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

      var diagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields);
      Assert.AreEqual(EvaluateManagedBackingFieldsAnalayzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(EvaluateManagedBackingFieldsAnalayzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category, 
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
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
        new[] { Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields, Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic))}.cs",
        new[] { Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields, Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotReadonly))}.cs",
        new[] { Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields, Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields });
    }
  }
}