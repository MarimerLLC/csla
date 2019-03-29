using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]

  public sealed class FindBusinessObjectCreationAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new FindBusinessObjectCreationAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.FindBusinessObjectCreation, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(FindBusinessObjectCreationConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindBusinessObjectCreationConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.FindBusinessObjectCreation, nameof(FindBusinessObjectCreationAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenConstructorIsNotOnBusinessObject()
    {
      var code = 
@"public class A { }

  public class B
  {
    void Foo()
    {
      var a = new A();
    }
  }";
      await TestHelpers.RunAnalysisAsync<FindBusinessObjectCreationAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenConstructorIsOnBusinessObjectWithinObjectFactory()
    {
      var code =
@"using Csla;
using Csla.Server;

public class A : BusinessBase<A> { }

public class B : ObjectFactory
{
  void Foo()
  {
    var a = new A();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindBusinessObjectCreationAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenConstructorIsOnBusinessObjectOutsideOfObjectFactory()
    {
      var code =
@"using Csla;
using Csla.Server;

public class A : BusinessBase<A> { }

public class B
{
  void Foo()
  {
    var a = new A();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindBusinessObjectCreationAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.FindBusinessObjectCreation });
    }
  }
}
