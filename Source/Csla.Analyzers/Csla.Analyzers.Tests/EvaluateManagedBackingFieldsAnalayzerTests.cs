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

    private static async Task RunAnalysisAsync(string path, string[] diagnosticIds,
      Action<List<Diagnostic>> diagnosticInspector = null)
    {
      var code = File.ReadAllText(path);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(
        code, new EvaluateManagedBackingFieldsAnalayzer());
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
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldNotUsedByProperty()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldNotUsedByProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedProperty()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedProperty))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotPublic()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotPublic))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId }, 
        diagnostics =>
        {
          var diagnostic = diagnostics[0];
          Assert.IsFalse(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsPublic]));
          Assert.IsTrue(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsReadonly]));
          Assert.IsTrue(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsStatic]));
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotStatic()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotStatic))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId },
        diagnostics =>
        {
          var diagnostic = diagnostics[0];
          Assert.IsTrue(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsPublic]));
          Assert.IsTrue(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsReadonly]));
          Assert.IsFalse(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsStatic]));
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      await EvaluateManagedBackingFieldsAnalayzerTests.RunAnalysisAsync(
        $@"Targets\{nameof(EvaluateManagedBackingFieldsAnalayzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotReadonly))}.cs",
        new[] { EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId },
        diagnostics =>
        {
          var diagnostic = diagnostics[0];
          Assert.IsTrue(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsPublic]));
          Assert.IsFalse(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsReadonly]));
          Assert.IsTrue(bool.Parse(diagnostic.Properties[EvaluateManagedBackingFieldsAnalayzerConstants.IsStatic]));
        });
    }
  }
}