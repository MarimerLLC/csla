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
        private void New() { }

        [Fetch]
        private void GetById(int id) { }

        [Delete]
        private void Remove(int id) { }

        [FetchChild]
        private void LoadChild(int id) { }

        [CreateChild]
        private void NewChild() { }
      }

      [DataPortalExtensions]
      [Serializable]
      public partial class C : CommandBase<C>
      {
        [Execute]
        private void Run(int id) { }
      }

      [Serializable]
      public partial class B : BusinessBase<B>
      {
        [Fetch]
        private void Fetch(int id) { }
      }
      """;

    private static string CreateCode(string body, string header = "", string additionalTypes = "", string assemblyAttributes = "") =>
      $$"""
      {{header}}
      using Csla;
      using System;
      using System.Threading.Tasks;

      {{assemblyAttributes}}

      {{BusinessTypes}}

      {{additionalTypes}}

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
    public async Task AnalyzeWhenCriteriaArrayIsPassed()
    {
      var code = CreateCode(
        """
        object[] criteria = [1];
        await portal.FetchAsync(criteria);
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenCriteriaCountDoesNotMatchAnyExtension()
    {
      var code = CreateCode(
        """
        await portal.FetchAsync(1, 2);
        await portal.CreateAsync(1);
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenCriteriaTypesDoNotMatchAnyExtension()
    {
      var code = CreateCode(
        """
        await portal.FetchAsync("x");
        childPortal.FetchChild(1.5);
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenCriteriaConvertToExtensionParameters()
    {
      var code = CreateCode(
        """
        short id = 1;
        await portal.FetchAsync(id);
        IDataPortal<D> dPortal = null;
        await dPortal.FetchAsync(null, "a", "b");
        """,
        additionalTypes:
        """
        [DataPortalExtensions]
        [Serializable]
        public partial class D : BusinessBase<D>
        {
          [Fetch]
          private void GetByNames(string category, params string[] names) { }
        }
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension, Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenPreferredOverloadHasNoDataPortalExtension()
    {
      var code = CreateCode(
        """
        IDataPortal<D> dPortal = null;
        await dPortal.FetchAsync(1);
        """,
        additionalTypes:
        """
        public interface IDal { }

        [DataPortalExtensions]
        [Serializable]
        public partial class D : BusinessBase<D>
        {
          [Fetch]
          private void GetById(int id) { }

          [NoDataPortalExtension]
          [Fetch]
          private void GetById(int id, [Inject] IDal dal) { }
        }
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenNoExtensionIsGeneratedForTheOperation()
    {
      var code = CreateCode(
        """
        IDataPortal<D> dPortal = null;
        IChildDataPortal<D> dChildPortal = null;
        await dPortal.FetchAsync(1);
        dPortal.Delete(1);
        await dPortal.CreateAsync("a");
        await dPortal.ExecuteAsync(1);
        dChildPortal.CreateChild(1);
        """,
        additionalTypes:
        """
        [DataPortalExtensions]
        [Serializable]
        public partial class D : BusinessBase<D>
        {
          private class Secret { }

          [NoDataPortalExtension]
          [Fetch]
          private void GetById(int id) { }

          [Delete]
          private void Delete(int id) { }

          [Create]
          private void NewWith(Secret secret) { }

          [Create]
          private void NewByRef(ref string name) { }

          [FetchChild]
          private void LoadChild(int id) { }
        }
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenRootOperationNameIsOnlyAChildPortalMember()
    {
      var code = CreateCode(
        """
        IDataPortal<D> dPortal = null;
        await dPortal.CreateAsync(1);
        """,
        additionalTypes:
        """
        [DataPortalExtensions]
        [Serializable]
        public partial class D : BusinessBase<D>
        {
          [Create]
          private void CreateChild(int id) { }
        }
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension]);
    }

    [TestMethod]
    public async Task AnalyzeWhenPrefixIsInvalid()
    {
      var code = CreateCode(
        """
        IDataPortal<D> dPortal = null;
        await dPortal.FetchAsync(1);
        """,
        additionalTypes:
        """
        [DataPortalExtensions(Prefix = "My-")]
        [Serializable]
        public partial class D : BusinessBase<D>
        {
          [Fetch]
          private void GetById(int id) { }
        }
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenSyncExtensionsAreNotGenerated()
    {
      var code = CreateCode(
        """
        portal.Fetch(1);
        childPortal.FetchChild(1);
        commandPortal.Execute(1);
        await portal.FetchAsync(1);
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension],
        diagnostics => StringAssert.Contains(diagnostics[0].GetMessage(), "'FetchAsync'"),
        new Dictionary<string, string> { ["build_property.CslaGenerateSyncDataPortalExtensions"] = "false" });
    }

    [TestMethod]
    public async Task AnalyzeWhenAssemblyHasDataPortalExtensionsAttribute()
    {
      var code = CreateCode(
        """
        IDataPortal<D> dPortal = null;
        IDataPortal<E> ePortal = null;
        IDataPortal<F> fPortal = null;
        await dPortal.FetchAsync(1);
        await ePortal.FetchAsync(1);
        await fPortal.FetchAsync(1);
        """,
        assemblyAttributes: """[assembly: DataPortalExtensions(Prefix = "Portal")]""",
        additionalTypes:
        """
        [Serializable]
        public partial class D : BusinessBase<D>
        {
          [Fetch]
          private void Fetch(int id) { }
        }

        [NoDataPortalExtension]
        [Serializable]
        public partial class E : BusinessBase<E>
        {
          [Fetch]
          private void Fetch(int id) { }
        }

        [DataPortalExtensions(Prefix = "My")]
        [Serializable]
        public partial class F : BusinessBase<F>
        {
          [Fetch]
          private void Fetch(int id) { }
        }
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension, Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension],
        diagnostics =>
        {
          var names = diagnostics
            .Select(d => d.Properties[UseGeneratedDataPortalExtensionAnalyzer.ExtensionNamesProperty])
            .OrderBy(n => n)
            .ToList();
          CollectionAssert.AreEqual(new[] { "MyFetchAsync", "PortalFetchAsync" }, names);
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenAsyncSuffixIsConfigured()
    {
      var code = CreateCode(
        """
        await portal.FetchAsync(1);
        portal.Fetch(1);
        """);
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension],
        diagnostics => Assert.AreEqual("GetById", diagnostics[0].Properties[UseGeneratedDataPortalExtensionAnalyzer.ExtensionNamesProperty]),
        new Dictionary<string, string> { ["build_property.CslaDataPortalExtensionsAsyncSuffix"] = "none" });
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

    [TestMethod]
    [DataRow("Pages/EditCustomer.razor")]
    [DataRow("Pages/EditCustomer.cshtml")]
    public async Task AnalyzeWhenGeneratedCodeMapsToRazorFile(string razorFile)
    {
      var code = CreateCode(
        $"""
        #line (12,5)-(12,40) "{razorFile}"
        await portal.FetchAsync(1);
        #line default
        """,
        "// <auto-generated/>");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension],
        diagnostics =>
        {
          var span = diagnostics[0].Location.GetMappedLineSpan();
          Assert.AreEqual(razorFile, span.Path);
          Assert.AreEqual(11, span.StartLinePosition.Line);
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenGeneratedCodeMapsToRazorFileWithLineDirective()
    {
      var code = CreateCode(
        """
        #line 12 "Pages/EditCustomer.razor"
        await portal.FetchAsync(1);
        #line default
        """,
        "// <auto-generated/>");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension],
        diagnostics =>
        {
          var span = diagnostics[0].Location.GetMappedLineSpan();
          Assert.AreEqual("Pages/EditCustomer.razor", span.Path);
          Assert.AreEqual(11, span.StartLinePosition.Line);
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenRazorGeneratedCodeIsHidden()
    {
      var code = CreateCode(
        """
        #line (12,5)-(12,20) "Pages/EditCustomer.razor"
        var id = 1;
        #line hidden
        await portal.FetchAsync(id);
        #line default
        """,
        "// <auto-generated/>");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }

    // A business library as the generator leaves it: private operation methods,
    // which a referencing assembly cannot see, and marked extension methods.
    private const string ReferencedLibrary =
      """
      using Csla;
      using System;
      using System.Threading.Tasks;

      [assembly: DataPortalExtensions(Prefix = "Portal")]

      namespace Library
      {
        [Serializable]
        public partial class Person : BusinessBase<Person>
        {
          [Create]
          private void Create() { }

          [Fetch]
          private void Fetch(int id) { }

          [Delete]
          private void Delete(int id) { }

          [FetchChild]
          private void FetchChild(int id) { }

          [Serializable]
          public partial class Address : BusinessBase<Address>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }

        public static partial class PersonDataPortalExtensions
        {
          [GeneratedDataPortalExtension("CreateAsync")]
          public static Task<Person> PortalCreateAsync(this IDataPortal<Person> portal) => portal.CreateAsync();

          [GeneratedDataPortalExtension("FetchAsync")]
          public static Task<Person> PortalFetchAsync(this IDataPortal<Person> portal, int id) => portal.FetchAsync(id);

          [GeneratedDataPortalExtension("Fetch")]
          public static Person PortalFetch(this IDataPortal<Person> portal, int id) => portal.Fetch(id);

          [GeneratedDataPortalExtension("FetchChild")]
          internal static Person PortalFetchChild(this IChildDataPortal<Person> portal, int id) => portal.FetchChild(id);

          // not marked, as if generated by an earlier version of the generator
          public static Task PortalDeleteAsync(this IDataPortal<Person> portal, int id) => portal.DeleteAsync(id);
        }

        public static partial class Person_AddressDataPortalExtensions
        {
          [GeneratedDataPortalExtension("FetchAsync")]
          public static Task<Person.Address> PortalFetchAsync(this IDataPortal<Person.Address> portal, int id) => portal.FetchAsync(id);
        }
      }
      """;

    private static async Task RunReferencedAnalysisAsync(string body, string[] diagnosticIds, string extensionNames = null)
    {
      var library = await TestHelpers.CreateLibraryReferenceAsync(ReferencedLibrary, "Library");
      var code =
        $$"""
        using Csla;
        using Library;
        using System.Threading.Tasks;

        public class Consumer
        {
          public async Task Run(IDataPortal<Person> portal, IChildDataPortal<Person> childPortal, IDataPortal<Person.Address> addressPortal)
          {
            {{body}}
            await Task.CompletedTask;
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, diagnosticIds,
        diagnostics =>
        {
          if (extensionNames is not null)
          {
            Assert.AreEqual(extensionNames, diagnostics[0].Properties[UseGeneratedDataPortalExtensionAnalyzer.ExtensionNamesProperty]);
          }
        },
        additionalReferences: [library]);
    }

    [TestMethod]
    [DataRow("await portal.FetchAsync(1);", "PortalFetchAsync")]
    [DataRow("portal.Fetch(1);", "PortalFetch")]
    [DataRow("await portal.CreateAsync();", "PortalCreateAsync")]
    [DataRow("await addressPortal.FetchAsync(1);", "PortalFetchAsync")]
    public async Task AnalyzeWhenBusinessTypeIsInReferencedAssembly(string body, string extensionNames) =>
      await RunReferencedAnalysisAsync(body, [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension], extensionNames);

    [TestMethod]
    public async Task AnalyzeWhenBusinessTypeIsInReferencedAssemblyAndArgumentsDoNotMatch() =>
      await RunReferencedAnalysisAsync("await portal.FetchAsync(\"x\");", []);

    [TestMethod]
    public async Task AnalyzeWhenBusinessTypeIsInReferencedAssemblyAndExtensionIsNotMarked() =>
      await RunReferencedAnalysisAsync("await portal.DeleteAsync(1);", []);

    [TestMethod]
    public async Task AnalyzeWhenBusinessTypeIsInReferencedAssemblyAndExtensionIsNotAccessible() =>
      await RunReferencedAnalysisAsync("childPortal.FetchChild(1);", []);

    [TestMethod]
    public async Task AnalyzeWhenGeneratedCodeMapsToOtherFile()
    {
      var code = CreateCode(
        """
        #line 12 "Customer.tt"
        await portal.FetchAsync(1);
        #line default
        """,
        "// <auto-generated/>");
      await TestHelpers.RunAnalysisAsync<UseGeneratedDataPortalExtensionAnalyzer>(code, []);
    }
  }
}
