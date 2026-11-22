using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class NotNullPropertyNotLoadedAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var diagnostics = new NotNullPropertyNotLoadedAnalyzer().SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);
      foreach (var diagnostic in diagnostics)
      {
        Assert.AreEqual("CSLA0033", diagnostic.Id);
        Assert.AreEqual(Constants.AnalyzerIdentifiers.NotNullPropertyNotLoaded, diagnostic.Id);
        Assert.AreEqual("Data portal operations should load not-null properties", diagnostic.Title.ToString());
        Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category);
        Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity);
        Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.NotNullPropertyNotLoaded, nameof(NotNullPropertyNotLoadedAnalyzer)),
          diagnostic.HelpLinkUri);
      }
    }

    private const string Usings =
      """
      #nullable enable
      using System;
      using Csla;

      public class Child : BusinessBase<Child> { }
      public interface IDal { Child GetChild(); string GetName(); }

      """;

    private static string RootWith(string operations, string registrations =
      """
      public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
      public Child Item { get => GetProperty(ItemProperty); set => SetProperty(ItemProperty, value); }
      """) =>
      Usings +
      $$"""
      public partial class A : BusinessBase<A>
      {
        {{registrations}}
        {{operations}}
      }
      """;

    private static Task<List<Diagnostic>> GetDiagnosticsAsync(string code, params string[] additionalSources) =>
      TestHelpers.GetDiagnosticsAsync<NotNullPropertyNotLoadedAnalyzer>(code, additionalSources);

    private static async Task AssertReportedAsync(string code, string locationText, string propertyNames, params string[] additionalSources)
    {
      var diagnostics = await GetDiagnosticsAsync(code, additionalSources);
      Assert.AreEqual(1, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
      var location = diagnostics[0].Location;
      Assert.AreEqual(locationText, location.SourceTree.ToString().Substring(location.SourceSpan.Start, location.SourceSpan.Length));
      StringAssert.EndsWith(diagnostics[0].GetMessage(), propertyNames);
    }

    private static async Task AssertSilentAsync(string code, params string[] additionalSources)
    {
      var diagnostics = await GetDiagnosticsAsync(code, additionalSources);
      Assert.AreEqual(0, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
    }

    [TestMethod]
    public async Task ReportsFetchThatDoesNotLoad()
    {
      var code = RootWith("[Fetch] private void Fetch(int id) { }");
      await AssertReportedAsync(code, "Fetch", ": Item");
    }

    [DataTestMethod]
    [DataRow("[Create] private void Create() { }", "Create")]
    [DataRow("[CreateChild] private void CreateChild() { }", "CreateChild")]
    [DataRow("[FetchChild] private void FetchChild() { } [CreateChild] private void CreateChild(Child c) => LoadProperty(ItemProperty, c);", "FetchChild")]
    public async Task ReportsEachCompletingOperation(string operations, string locationText)
    {
      await AssertReportedAsync(RootWith(operations), locationText, ": Item");
    }

    [TestMethod]
    public async Task ReportsEveryMissingPropertyInOneDiagnostic()
    {
      var code = RootWith("[Fetch] private void Fetch() { }",
        """
        public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
        public Child Item => GetProperty(ItemProperty);
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
        public string Name => GetProperty(NameProperty);
        """);
      await AssertReportedAsync(code, "Fetch", ": Item, Name");
    }

    [TestMethod]
    public async Task ReportsOnlyTheMissingProperty()
    {
      var code = RootWith("[Fetch] private void Fetch(IDal dal) { LoadProperty(ItemProperty, dal.GetChild()); }",
        """
        public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
        public Child Item => GetProperty(ItemProperty);
        public static readonly NotNullPropertyInfo<Child> OtherProperty = RegisterNotNullProperty<Child>(nameof(Other));
        public Child Other => GetProperty(OtherProperty);
        """);
      await AssertReportedAsync(code, "Fetch", ": Other");
    }

    [TestMethod]
    public async Task ReportsInheritedRegistration()
    {
      var code = Usings +
        """
        public abstract class Base<T> : BusinessBase<T> where T : Base<T>
        {
          public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
          public Child Item => GetProperty(ItemProperty);
        }

        public class A : Base<A>
        {
          [Fetch] private void Fetch() { }
        }
        """;
      await AssertReportedAsync(code, "Fetch", ": Item");
    }

    [TestMethod]
    public async Task ReportsGeneratedRegistration()
    {
      var code = RootWith("[Fetch] private void Fetch() { }", "public Child Item => GetProperty(ItemProperty);");
      var generated =
        """
        // <auto-generated/>
        #nullable enable
        using Csla;
        partial class A
        {
          public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
        }
        """;
      await AssertReportedAsync(code, "Fetch", ": Item", generated);
    }

    [TestMethod]
    public async Task ReportsCommandExecute()
    {
      var code = Usings +
        """
        public class Cmd : CommandBase<Cmd>
        {
          public static readonly NotNullPropertyInfo<Child> ResultProperty = RegisterNotNullProperty<Child>(nameof(Result));
          public Child Result { get => ReadProperty(ResultProperty); private set => LoadProperty(ResultProperty, value); }
          [Execute] private void Execute() { }
        }
        """;
      await AssertReportedAsync(code, "Execute", ": Result");
    }

    [TestMethod]
    public async Task ReportsChildWithoutOwnCreateChild()
    {
      var code = RootWith("[FetchChild] private void FetchChild(IDal dal) => LoadProperty(ItemProperty, dal.GetChild());");
      var diagnostics = await GetDiagnosticsAsync(code);
      Assert.AreEqual(1, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
      var location = diagnostics[0].Location;
      Assert.AreEqual("A", location.SourceTree.ToString().Substring(location.SourceSpan.Start, location.SourceSpan.Length));
      StringAssert.Contains(diagnostics[0].GetMessage(), "[CreateChild]");
      StringAssert.EndsWith(diagnostics[0].GetMessage(), ": Item");
    }

    [TestMethod]
    public async Task ReportsChildWithoutOwnCreateChildOnTheUserDeclaration()
    {
      // the generated declaration comes first, so it is the type's first location
      var generated =
        """
        // <auto-generated/>
        #nullable enable
        using Csla;
        partial class A
        {
          public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
        }
        """;
      var user = Usings +
        """
        public partial class A : BusinessBase<A>
        {
          public Child Item => GetProperty(ItemProperty);
          [FetchChild] private void FetchChild(IDal dal) => LoadProperty(ItemProperty, dal.GetChild());
        }
        """;
      var diagnostics = await GetDiagnosticsAsync(generated, user);
      Assert.AreEqual(1, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
      var location = diagnostics[0].Location;
      Assert.AreEqual(user, location.SourceTree.ToString());
      Assert.AreEqual("A", user.Substring(location.SourceSpan.Start, location.SourceSpan.Length));
    }

    [DataTestMethod]
    [DataRow("LoadProperty(ItemProperty, dal.GetChild());")]
    [DataRow("LoadProperty((Csla.Core.IPropertyInfo)ItemProperty, dal.GetChild());")]
    [DataRow("LoadPropertyMarkDirty(ItemProperty, dal.GetChild());")]
    [DataRow("SetProperty(ItemProperty, dal.GetChild());")]
    [DataRow("SetProperty((Csla.Core.IPropertyInfo)ItemProperty, dal.GetChild());")]
    [DataRow("LoadPropertyConvert<Child, Child>(ItemProperty, dal.GetChild());")]
    [DataRow("SetPropertyConvert<Child, Child>(ItemProperty, dal.GetChild());")]
    [DataRow("Item = dal.GetChild();")]
    [DataRow("this.Item = dal.GetChild();")]
    [DataRow("using (BypassPropertyChecks) { Item = dal.GetChild(); }")]
    [DataRow("Action load = () => LoadProperty(ItemProperty, dal.GetChild()); load();")]
    [DataRow("void Load() => LoadProperty(ItemProperty, dal.GetChild()); Load();")]
    [DataRow("if (dal is null) return; LoadProperty(ItemProperty, dal.GetChild());")]
    public async Task SilentWhenTheOperationLoads(string body)
    {
      var code = RootWith($"[Fetch] private void Fetch(IDal dal) {{ {body} }}");
      await AssertSilentAsync(code);
    }

    [DataTestMethod]
    [DataRow("Mapper.Map(dal, this);")]
    [DataRow("new Loader(this);")]
    [DataRow("LoadFrom(dal);")]
    [DataRow("LoadCommon(dal);")]
    [DataRow("foreach (var p in FieldManager.GetRegisteredProperties()) LoadProperty(p, null);")]
    public async Task SilentWhenTheOperationMayLoadElsewhere(string body)
    {
      var code = Usings +
        """
        public static class Mapper { public static void Map(IDal dal, object target) { } }
        public class Loader { public Loader(object target) { } }

        public abstract class Base<T> : BusinessBase<T> where T : Base<T>
        {
          protected void LoadCommon(IDal dal) { }
        }

        public class A : Base<A>
        {
          public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
          public Child Item => GetProperty(ItemProperty);
          private void LoadFrom(IDal dal) { }
          [Fetch] private void Fetch(IDal dal) { BODY }
        }
        """.Replace("BODY", body);
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForObjectFactory()
    {
      var code = Usings +
        """
        [Csla.Server.ObjectFactory("Factory")]
        public class A : BusinessBase<A>
        {
          public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
          public Child Item => GetProperty(ItemProperty);
          [Fetch] private void Fetch() { }
        }
        """;
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForCommandCreate()
    {
      var code = Usings +
        """
        public class Cmd : CommandBase<Cmd>
        {
          public static readonly NotNullPropertyInfo<Child> ResultProperty = RegisterNotNullProperty<Child>(nameof(Result));
          public Child Result { get => ReadProperty(ResultProperty); private set => LoadProperty(ResultProperty, value); }
          [Create] private void Create() { }
          [Execute] private void Execute(IDal dal) => Result = dal.GetChild();
        }
        """;
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForCommandInputLoadedByCreate()
    {
      var code = Usings +
        """
        public class Cmd : CommandBase<Cmd>
        {
          public static readonly NotNullPropertyInfo<Child> InputProperty = RegisterNotNullProperty<Child>(nameof(Input));
          public Child Input { get => ReadProperty(InputProperty); private set => LoadProperty(InputProperty, value); }
          [Create] private void Create(Child input) => Input = input;
          [Execute] private void Execute() { }
        }
        """;
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForCommandInputWithPublicSetter()
    {
      var code = Usings +
        """
        public class Cmd : CommandBase<Cmd>
        {
          public static readonly NotNullPropertyInfo<Child> InputProperty = RegisterNotNullProperty<Child>(nameof(Input));
          public Child Input { get => ReadProperty(InputProperty); set => LoadProperty(InputProperty, value); }
          [Execute] private void Execute() { }
        }
        """;
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentWithDefault()
    {
      var code = RootWith("[Fetch] private void Fetch() { }",
        """
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);
        public string Name => GetProperty(NameProperty);
        """);
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForValueType()
    {
      var code = RootWith("[Fetch] private void Fetch() { }",
        """
        public static readonly NotNullPropertyInfo<int> CountProperty = RegisterNotNullProperty<int>(nameof(Count));
        public int Count => GetProperty(CountProperty);
        """);
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForPropertyInfo()
    {
      var code = RootWith("[Fetch] private void Fetch() { }",
        """
        public static readonly PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(nameof(Item));
        public Child? Item => GetProperty(ItemProperty);
        """);
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForOperationsThatDoNotComplete()
    {
      var code = RootWith(
        """
        [Insert] private void Insert() { }
        [Update] private void Update() { }
        [DeleteSelf] private void DeleteSelf() { }
        private void Fetch() { }
        """);
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForMetadataBaseRegistration()
    {
      var library = await TestHelpers.CreateLibraryReferenceAsync(
        """
        #nullable enable
        using Csla;
        public class Child : BusinessBase<Child> { }
        public abstract class Base<T> : BusinessBase<T> where T : Base<T>
        {
          public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
          public Child Item => GetProperty(ItemProperty);
        }
        """, "Library");
      var code =
        """
        #nullable enable
        using Csla;
        public class A : Base<A>
        {
          [Fetch] private void Fetch() { }
        }
        """;
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new NotNullPropertyNotLoadedAnalyzer(),
        additionalReferences: [library]);
      Assert.AreEqual(0, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
    }

    [TestMethod]
    public async Task SilentForChildWithOwnCreateChild()
    {
      var code = RootWith(
        """
        [CreateChild] private void CreateChild(Child item) => LoadProperty(ItemProperty, item);
        [FetchChild] private void FetchChild(IDal dal) => LoadProperty(ItemProperty, dal.GetChild());
        """);
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForAbstractChildWithoutCreateChild()
    {
      var code = Usings +
        """
        public abstract class A : BusinessBase<A>
        {
          public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item));
          public Child Item => GetProperty(ItemProperty);
          [FetchChild] private void FetchChild(IDal dal) => LoadProperty(ItemProperty, dal.GetChild());
        }
        """;
      await AssertSilentAsync(code);
    }
  }
}
