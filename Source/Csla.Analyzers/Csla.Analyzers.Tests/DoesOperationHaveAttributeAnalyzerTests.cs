using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class DoesOperationHaveAttributeAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new DoesOperationHaveAttributeAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.DoesOperationHaveAttribute, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(DoesOperationHaveAttributeAnalyzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(DoesOperationHaveAttributeAnalyzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Info, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.DoesOperationHaveAttribute, nameof(DoesOperationHaveAttributeAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotMobileObject()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<DoesOperationHaveAttributeAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndOperationHasNamingConventionAndAttribute()
    {
      var code = 
@"using Csla;

public class A : BusinessBase<A>
{ 
  [Fetch]
  private void DataPortal_Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<DoesOperationHaveAttributeAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndOperationHasAttribute()
    {
      var code = 
@"using Csla;

public class A : BusinessBase<A>
{ 
  [Fetch]
  private void Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<DoesOperationHaveAttributeAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndOperationHasNamingConvention()
    {
      var code = 
@"using Csla;

public class A : BusinessBase<A>
{ 
  private void DataPortal_Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<DoesOperationHaveAttributeAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.DoesOperationHaveAttribute });
    }
  }
}