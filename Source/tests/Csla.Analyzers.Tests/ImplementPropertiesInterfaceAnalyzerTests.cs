using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class ImplementPropertiesInterfaceAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new ImplementPropertiesInterfaceAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.InvalidImplementPropertiesInterface, diagnostic.Id);
      Assert.AreEqual("Interface properties cannot be generated", diagnostic.Title.ToString());
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category);
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity);
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.InvalidImplementPropertiesInterface, nameof(ImplementPropertiesInterfaceAnalyzer)),
        diagnostic.HelpLinkUri);
    }

    [TestMethod]
    [DataRow("CslaImplementPropertiesInterface<ITestClass>")]
    [DataRow("Csla.CslaImplementPropertiesInterface<ITestClass>")]
    [DataRow("global::Csla.CslaImplementPropertiesInterfaceAttribute<ITestClass>")]
    public async Task InterfaceDeclaredInSource(string attribute)
    {
      var code = $$"""
        using Csla;

        [{{attribute}}]
        public partial class TestClass : BusinessBase<TestClass>
        {
          private interface ITestClass
          {
            string Name { get; set; }
          }
        }
        """;

      await TestHelpers.RunAnalysisAsync<ImplementPropertiesInterfaceAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AliasedAttributeWithInterfaceDeclaredInSource()
    {
      var code = """
        using ImplementProperties = Csla.CslaImplementPropertiesInterfaceAttribute<ITestClass>;

        [ImplementProperties]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
        }

        public interface ITestClass
        {
          string Name { get; set; }
        }
        """;

      await TestHelpers.RunAnalysisAsync<ImplementPropertiesInterfaceAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task TypeArgumentIsClass()
    {
      var code = """
        [Csla.CslaImplementPropertiesInterface<NotAnInterface>]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
        }

        public class NotAnInterface
        {
          public string Name { get; set; }
        }
        """;

      await AssertDiagnosticAsync(code, "Properties are not generated for 'TestClass' because 'NotAnInterface' is not an interface");
    }

    [TestMethod]
    public async Task InterfaceNotDeclaredInSource()
    {
      var code = """
        [Csla.CslaImplementPropertiesInterface<System.IServiceProvider>]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
        }
        """;

      await AssertDiagnosticAsync(code, "Properties are not generated for 'TestClass' because the interface 'System.IServiceProvider' is not declared in this project");
    }

    [TestMethod]
    public async Task UnresolvedTypeArgument()
    {
      var code = """
        [Csla.CslaImplementPropertiesInterface<IMissing>]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
        }
        """;

      await TestHelpers.RunAnalysisAsync<ImplementPropertiesInterfaceAnalyzer>(code, []);
    }

    private static async Task AssertDiagnosticAsync(string code, string message)
    {
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new ImplementPropertiesInterfaceAnalyzer());
      Assert.AreEqual(1, diagnostics.Count);
      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.InvalidImplementPropertiesInterface, diagnostic.Id);
      Assert.AreEqual(message, diagnostic.GetMessage());
      Assert.IsTrue(diagnostic.Location.SourceTree!.GetText().ToString(diagnostic.Location.SourceSpan).StartsWith("Csla.CslaImplementPropertiesInterface<"),
        "the diagnostic should be reported on the attribute");
    }
  }
}
