using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindOperationsWithIncorrectReturnTypesAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new FindOperationsWithIncorrectReturnTypesAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.FindOperationsWithIncorrectReturnTypes, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(FindOperationsWithIncorrectReturnTypesAnalyzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindOperationsWithIncorrectReturnTypesAnalyzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Design, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.FindOperationsWithIncorrectReturnTypes, nameof(FindOperationsWithIncorrectReturnTypesAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWithNotMobileObject()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithIncorrectReturnTypesAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsNotOperation()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Foo() { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithIncorrectReturnTypesAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsOperationReturningVoid()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [Fetch]
  private void Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithIncorrectReturnTypesAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsOperationReturningTask()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A>
{
  [Fetch]
  private async Task FetchAsync() { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithIncorrectReturnTypesAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsOperationReturningIncorrectType()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [Fetch]
  private string Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithIncorrectReturnTypesAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.FindOperationsWithIncorrectReturnTypes });
    }
  }
}