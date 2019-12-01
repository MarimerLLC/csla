using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindRefAndOutParametersInOperationsAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new FindRefAndOutParametersInOperationsAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.RefOrOutParameterInOperation, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(FindRefAndOutParametersInOperationsAnalyzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindRefAndOutParametersInOperationsAnalyzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.RefOrOutParameterInOperation, nameof(FindRefAndOutParametersInOperationsAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotABusinessBase()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<FindRefAndOutParametersInOperationsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessBaseAndHasNoParameters()
    {
      var code = 
@"using Csla;

public class A 
  : BusinessBase<A>
{ 
  public A() { }

  [Fetch]
  private void Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<FindRefAndOutParametersInOperationsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessBaseAndHasParameterNotRefOrOut()
    {
      var code =
@"using Csla;

public class A 
  : BusinessBase<A>
{ 
  public A() { }

  [Fetch]
  private void Fetch(string a) { }
}";
      await TestHelpers.RunAnalysisAsync<FindRefAndOutParametersInOperationsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessBaseAndHasRefParameter()
    {
      var code =
@"using Csla;

public class A 
  : BusinessBase<A>
{ 
  public A() { }

  [Fetch]
  private void Fetch(ref string a) { }
}";
      await TestHelpers.RunAnalysisAsync<FindRefAndOutParametersInOperationsAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.RefOrOutParameterInOperation });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessBaseAndHasOutParameter()
    {
      var code =
@"using Csla;

public class A 
  : BusinessBase<A>
{ 
  public A() { }

  [Fetch]
  private void Fetch(out string a) { a = string.Empty; }
}";
      await TestHelpers.RunAnalysisAsync<FindRefAndOutParametersInOperationsAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.RefOrOutParameterInOperation });
    }
  }
}