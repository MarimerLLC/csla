using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class DoesChildOperationHaveRunLocalAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new DoesChildOperationHaveRunLocalAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.DoesChildOperationHaveRunLocal, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(DoesChildOperationHaveRunLocalAnalyzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(DoesChildOperationHaveRunLocalAnalyzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.DoesChildOperationHaveRunLocal, nameof(DoesChildOperationHaveRunLocalAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotMobileObject()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<DoesChildOperationHaveRunLocalAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndRootOperationHasRunLocal()
    {
      var code = 
@"using Csla;

public class A : BusinessBase<A>
{ 
  [RunLocal]
  [Fetch]
  private void Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<DoesChildOperationHaveRunLocalAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndChildOperationDoesNotHaveRunLocal()
    {
      var code = 
@"using Csla;

public class A : BusinessBase<A>
{ 
  [FetchChild]
  private void FetchChild() { }
}";
      await TestHelpers.RunAnalysisAsync<DoesChildOperationHaveRunLocalAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndChildOperationHasRunLocal()
    {
      var code = 
@"using Csla;

public class A : BusinessBase<A>
{ 
  [RunLocal]
  [FetchChild]
  private void FetchChild() { }
}";
      await TestHelpers.RunAnalysisAsync<DoesChildOperationHaveRunLocalAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.DoesChildOperationHaveRunLocal });
    }
  }
}