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
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldNotUsedByProperty()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldNotUsedByProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedProperty()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotPublic()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotPublic))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotStatic()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotStatic))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotReadonly))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId });
    }
  }
}