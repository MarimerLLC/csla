using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
      Assert.AreEqual(Constants.AnalyzerIdentifiers.IsBusinessObjectSerializable, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(IsBusinessObjectSerializableConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(IsBusinessObjectSerializableConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotMobileObject()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsNotMobileObject))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndIsSerializable()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsMobileObjectAndIsSerializable))}.cs",
        Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndIsNotSerializable()
    {
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        $@"Targets\{nameof(IsBusinessObjectSerializableAnalyzerTests)}\{(nameof(this.AnalyzeWhenClassIsMobileObjectAndIsNotSerializable))}.cs",
        new[] { Constants.AnalyzerIdentifiers.IsBusinessObjectSerializable });
    }
  }
}
