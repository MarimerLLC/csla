using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.IsBusinessObjectSerializable, nameof(IsBusinessObjectSerializableAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotMobileObject()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndIsSerializable()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public class A : BusinessBase<A>{ }";
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndIsNotSerializable()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>{ }";
      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.IsBusinessObjectSerializable]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsDerivedFromReadOnlyListBaseAndIsNotSerializable() {
      var code = @"
using Csla;
using System;

namespace Testnamespace 
{
  [Serializable]
  public class B : BusinessBase<B> {}

  public class A : ReadOnlyListBase<A,B> {}
}
";

      await TestHelpers.RunAnalysisAsync<IsBusinessObjectSerializableAnalyzer>(code, [Constants.AnalyzerIdentifiers.IsBusinessObjectSerializable]);
    }
  }
}