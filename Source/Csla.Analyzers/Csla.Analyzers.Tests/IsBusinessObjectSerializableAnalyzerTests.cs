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
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotStereotype))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndIsSerializable()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndIsSerializable))}.cs",
        new string[0]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndIsNotSerializable()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsStereotypeAndIsNotSerializable))}.cs",
        new[] { IsBusinessObjectSerializableConstants.DiagnosticId });
    }
  }
}
