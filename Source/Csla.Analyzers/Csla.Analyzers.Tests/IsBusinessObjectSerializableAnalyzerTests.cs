using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class IsBusinessObjectSerializableAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new IsBusinessObjectSerializableAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(diagnostic.Id, IsBusinessObjectSerializableConstants.DiagnosticId,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(diagnostic.Title.ToString(), IsBusinessObjectSerializableConstants.Title,
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(diagnostic.MessageFormat.ToString(), IsBusinessObjectSerializableConstants.Message,
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(diagnostic.Category, IsBusinessObjectSerializableConstants.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(diagnostic.DefaultSeverity, DiagnosticSeverity.Error,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotMobileObject()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotMobileObject))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndIsSerializable()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsMobileObjectAndIsSerializable))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndIsNotSerializable()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsMobileObjectAndIsNotSerializable))}.cs",
        new[] { IsBusinessObjectSerializableConstants.DiagnosticId });
    }
  }
}
