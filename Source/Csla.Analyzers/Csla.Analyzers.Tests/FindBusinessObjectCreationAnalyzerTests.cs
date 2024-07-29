using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
      Assert.AreEqual("Find CSLA Business Objects That Are Created Outside of a ObjectFactory", diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("CSLA business objects should not be created outside of a ObjectFactory instance", diagnostic.MessageFormat.ToString(),
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
        """
        public class A { }
        
          public class B
          {
            void Foo()
            {
              var a = new A();
            }
          }
        """;
      await TestHelpers.RunAnalysisAsync<FindBusinessObjectCreationAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenConstructorIsOnBusinessObjectWithinObjectFactory()
    {
      var code =
        """
        using Csla;
        using Csla.Server;

        public class A : BusinessBase<A> { }

        public class B : ObjectFactory
        {
          void Foo()
          {
            var a = new A();
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindBusinessObjectCreationAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenConstructorIsOnBusinessObjectOutsideOfObjectFactory()
    {
      var code =
        """
        using Csla;
        using Csla.Server;

        public class A : BusinessBase<A> { }

        public class B
        {
          void Foo()
          {
            var a = new A();
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindBusinessObjectCreationAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.FindBusinessObjectCreation]);
    }
  }
}
