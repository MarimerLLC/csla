using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class UseGeneratedDataPortalExtensionAnalyzerTests
  {
    private const string BusinessTypes =
      """
      [DataPortalExtensions]
      [Serializable]
      public partial class A : BusinessBase<A>
      {
        [Create]
        private void Create() { }

        [Fetch]
        private void Fetch(int id) { }

        [Delete]
        private void Delete(int id) { }

        [FetchChild]
        private void FetchChild(int id) { }

        [CreateChild]
        private void CreateChild() { }
      }

      [DataPortalExtensions]
      [Serializable]
      public partial class C : CommandBase<C>
      {
        [Execute]
        private void Execute(int id) { }
      }

      [Serializable]
      public partial class B : BusinessBase<B>
      {
        [Fetch]
        private void Fetch(int id) { }
      }
      """;

    private static string CreateCode(string body, string header = "") =>
      $$"""
      {{header}}
      using Csla;
      using System;
      using System.Threading.Tasks;

      {{BusinessTypes}}

      public class Consumer
      {
        public async Task Run(IDataPortal<A> portal, IChildDataPortal<A> childPortal,
          DataPortal<A> concretePortal, IDataPortal<B> otherPortal, IDataPortal<C> commandPortal, C command)
        {
          {{body}}
          await Task.CompletedTask;
        }
      }
      """;

    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new UseGeneratedDataPortalExtensionAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual("Use generated data portal extension method", diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("Use the generated data portal extension method for '{0}' instead of calling '{1}' with untyped criteria",
        diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension, nameof(UseGeneratedDataPortalExtensionAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenFetchAsyncIsCalledOnDataPortal()
    {
      var code = CreateCode("await portal.FetchAsync(1);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension],
        diagnostics =>
        {
          Assert.AreEqual("Use the generated data portal extension method for 'A' instead of calling 'FetchAsync' with untyped criteria",
            diagnostics[0].GetMessage());
          var location = diagnostics[0].Location;
          Assert.AreEqual("FetchAsync", location.SourceTree.GetText().ToString(location.SourceSpan));
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenFetchIsCalledOnDataPortal()
    {
      var code = CreateCode("portal.Fetch(1);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenCreateIsCalledOnDataPortal()
    {
      var code = CreateCode("portal.Create();");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenCreateAsyncIsCalledOnDataPortal()
    {
      var code = CreateCode("await portal.CreateAsync();");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenDeleteIsCalledOnDataPortal()
    {
      var code = CreateCode("portal.Delete(1);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenDeleteAsyncIsCalledOnDataPortal()
    {
      var code = CreateCode("await portal.DeleteAsync(1);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenExecuteWithCriteriaIsCalledOnDataPortal()
    {
      var code = CreateCode("commandPortal.Execute(1);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenExecuteAsyncWithCriteriaIsCalledOnDataPortal()
    {
      var code = CreateCode("await commandPortal.ExecuteAsync(1);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenExecuteWithCommandIsCalledOnDataPortal()
    {
      var code = CreateCode("commandPortal.Execute(command);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenExecuteAsyncWithCommandIsCalledOnDataPortal()
    {
      var code = CreateCode("await commandPortal.ExecuteAsync(command);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenFetchChildIsCalledOnChildDataPortal()
    {
      var code = CreateCode("childPortal.FetchChild(1);");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension],
        diagnostics => Assert.AreEqual(
          "Use the generated data portal extension method for 'A' instead of calling 'FetchChild' with untyped criteria",
          diagnostics[0].GetMessage()));
    }

    [TestMethod]
    public async Task AnalyzeWhenCreateChildAsyncIsCalledOnChildDataPortal()
    {
      var code = CreateCode("await childPortal.CreateChildAsync();");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodsAreCalledOnConcreteDataPortal()
    {
      var code = CreateCode(
        """
        concretePortal.Fetch(1);
        await concretePortal.FetchChildAsync(1);
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension, Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenBusinessTypeDoesNotHaveDataPortalExtensionsAttribute()
    {
      var code = CreateCode(
        """
        await otherPortal.FetchAsync(1);
        otherPortal.Fetch(1);
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenUpdateIsCalledOnDataPortal()
    {
      var code = CreateCode(
        """
        var item = await portal.CreateAsync();
        await portal.UpdateAsync(item);
        portal.Update(item);
        await concretePortal.UpdateAsync(item);
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension],
        diagnostics => Assert.IsTrue(diagnostics[0].GetMessage().Contains("'CreateAsync'")));
    }

    [TestMethod]
    public async Task AnalyzeWhenCodeIsGenerated()
    {
      var code = CreateCode(
        """
        await portal.FetchAsync(1);
        childPortal.FetchChild(1);
        commandPortal.Execute(1);
        """,
        "// <auto-generated/>");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }
  }
}
