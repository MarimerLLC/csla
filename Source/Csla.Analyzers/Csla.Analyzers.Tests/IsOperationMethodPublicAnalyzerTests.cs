using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class IsOperationMethodPublicAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new IsOperationMethodPublicAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);

      var diagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.IsOperationMethodPublic);
      Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Design, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.IsOperationMethodPublic, nameof(IsOperationMethodPublicAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));

      var diagnosticForInterface = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.IsOperationMethodPublicForInterface);
      Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.Title, diagnosticForInterface.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.Message, diagnosticForInterface.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Design, diagnosticForInterface.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnosticForInterface.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.IsOperationMethodPublicForInterface, nameof(IsOperationMethodPublicAnalyzer)),
        diagnosticForInterface.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsNotStereotype()
    {
      var code =
@"public class A
{
  [Fetch]
  public void Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsNotADataPortalOperation()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public class A : BusinessBase<A>
{
  public void AMethod() { }
}";
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public class A : BusinessBase<A>
{
  private void DataPortal_Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public class A : BusinessBase<A>
{
  [Fetch]
  public void Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(code, 
        new[] { Constants.AnalyzerIdentifiers.IsOperationMethodPublic },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public sealed class A : BusinessBase<A>
{
  [Fetch]
  public void Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(code, 
        new[] { Constants.AnalyzerIdentifiers.IsOperationMethodPublic },
        diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[IsOperationMethodPublicAnalyzerConstants.IsSealed]));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndTypeIsInterface()
    {
      var code =
@"using Csla;
using Csla.Core;

public interface A
  : IBusinessObject
{
  [Fetch]
  void Fetch();
}";
      await TestHelpers.RunAnalysisAsync<IsOperationMethodPublicAnalyzer>(code, 
        new[] { Constants.AnalyzerIdentifiers.IsOperationMethodPublicForInterface });
    }
  }
}