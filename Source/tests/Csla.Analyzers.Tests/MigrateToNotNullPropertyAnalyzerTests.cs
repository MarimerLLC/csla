using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class MigrateToNotNullPropertyAnalyzerTests
  {
    private const string FixAll = "CSLA0037";
    private const string Review = "CSLA0038";

    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var diagnostics = new MigrateToNotNullPropertyAnalyzer().SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);

      Assert.AreEqual(FixAll, diagnostics[0].Id);
      Assert.AreEqual(Constants.AnalyzerIdentifiers.MigrateToNotNullProperty, diagnostics[0].Id);
      Assert.AreEqual("Register the property as a not-null property", diagnostics[0].Title.ToString());
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.MigrateToNotNullProperty, nameof(MigrateToNotNullPropertyAnalyzer)),
        diagnostics[0].HelpLinkUri);

      Assert.AreEqual(Review, diagnostics[1].Id);
      Assert.AreEqual(Constants.AnalyzerIdentifiers.MigrateToNotNullPropertyWithReview, diagnostics[1].Id);
      Assert.AreEqual("Register the property as a not-null property after review", diagnostics[1].Title.ToString());
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.MigrateToNotNullPropertyWithReview, nameof(MigrateToNotNullPropertyAnalyzer)),
        diagnostics[1].HelpLinkUri);

      foreach (var diagnostic in diagnostics)
      {
        Assert.AreEqual(Constants.Categories.Refactoring, diagnostic.Category);
        Assert.AreEqual(DiagnosticSeverity.Info, diagnostic.DefaultSeverity);
      }
    }

    private static string BusinessObjectWith(string members, string baseType = "BusinessBase<A>", string nullable = "#nullable enable") =>
      nullable +
      $$"""

      using System;
      using System.Diagnostics.CodeAnalysis;
      using Csla;

      public class Child : BusinessBase<Child> { }

      public partial class A : {{baseType}}
      {
        public static string? MaybeName => null;
        public static Child? MaybeChild => null;
        {{members}}
      }
      """;

    private static async Task AssertDiagnosticAsync(string code, string expectedId, string fieldName = "ItemProperty",
      params string[] additionalSources)
    {
      var diagnostics = await TestHelpers.GetDiagnosticsAsync<MigrateToNotNullPropertyAnalyzer>(code, additionalSources);
      Assert.AreEqual(1, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
      Assert.AreEqual(expectedId, diagnostics[0].Id, diagnostics[0].ToString());
      var location = diagnostics[0].Location;
      Assert.AreEqual(fieldName, location.SourceTree.ToString().Substring(location.SourceSpan.Start, location.SourceSpan.Length));
    }

    private static async Task AssertSilentAsync(string code, params string[] additionalSources)
    {
      var diagnostics = await TestHelpers.GetDiagnosticsAsync<MigrateToNotNullPropertyAnalyzer>(code, additionalSources);
      Assert.AreEqual(0, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
    }

    [DataTestMethod]
    [DataRow("int", "RegisterProperty<int>(nameof(Item))")]
    [DataRow("DateTime", "RegisterProperty<DateTime>(nameof(Item))")]
    [DataRow("int", "RegisterProperty<int>(nameof(Item), \"Item\", 42)")]
    [DataRow("int", "RegisterProperty<int>(nameof(Item), RelationshipTypes.None)")]
    [DataRow("string", "RegisterProperty<string>(nameof(Item))")]
    [DataRow("string", "RegisterProperty<string>(nameof(Item), \"Full name\")")]
    [DataRow("string", "RegisterProperty<string>(nameof(Item), null, null)")]
    [DataRow("string", "RegisterProperty<string>(nameof(Item), null, \"none\")")]
    [DataRow("string", "RegisterProperty<string>(_ => _.Item)")]
    [DataRow("Child", "RegisterProperty<Child>(nameof(Item), null, new Child())")]
    public async Task ReportsFixAllTier(string type, string registration)
    {
      var code = BusinessObjectWith(
        $"public static readonly PropertyInfo<{type}> ItemProperty = {registration};" +
        $"public {type} Item => GetProperty(ItemProperty)!;");
      await AssertDiagnosticAsync(code, FixAll);
    }

    [DataTestMethod]
    [DataRow("RegisterProperty<Child>(nameof(Item))")]
    [DataRow("RegisterProperty<Child>(nameof(Item), \"Item\")")]
    [DataRow("RegisterProperty<Child>(nameof(Item), \"Item\", null)")]
    [DataRow("RegisterProperty<Child>(nameof(Item), \"Item\", MaybeChild)")]
    [DataRow("RegisterProperty<Child>(nameof(Item), null, null, RelationshipTypes.None)")]
    public async Task ReportsReviewTier(string registration)
    {
      var code = BusinessObjectWith(
        $"public static readonly PropertyInfo<Child> ItemProperty = {registration};" +
        "public Child Item => GetProperty(ItemProperty)!;");
      await AssertDiagnosticAsync(code, Review);
    }

    [DataTestMethod]
    [DataRow("ReadOnlyBase<A>")]
    [DataRow("CommandBase<A>")]
    [DataRow("BusinessDocumentBase<A, Child>")]
    public async Task ReportsOnEachBaseClass(string baseType)
    {
      var code = BusinessObjectWith(
        "public static readonly PropertyInfo<int> ItemProperty = RegisterProperty<int>(_ => _.Item);" +
        "public int Item => ReadProperty(ItemProperty);", baseType);
      await AssertDiagnosticAsync(code, FixAll);
    }

    [TestMethod]
    public async Task ReportsWithoutClrProperty()
    {
      var code = BusinessObjectWith("public static readonly PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(\"Item\");");
      await AssertDiagnosticAsync(code, Review);
    }

    [DataTestMethod]
    [DataRow("int?", "RegisterProperty<int?>(nameof(Item))", "int?")]
    [DataRow("string?", "RegisterProperty<string?>(nameof(Item))", "string?")]
    [DataRow("Child", "RegisterProperty<Child>(nameof(Item))", "Child?")]
    [DataRow("string", "RegisterProperty<string>(nameof(Item))", "string?")]
    [DataRow("string", "RegisterProperty<string>(nameof(Item), null, MaybeName)", "string")]
    [DataRow("Child", "RegisterProperty<Child>(nameof(Item), RelationshipTypes.LazyLoad)", "Child?")]
    [DataRow("Child", "RegisterProperty<Child>(nameof(Item), RelationshipTypes.LazyLoad)", "Child")]
    [DataRow("int", "RegisterProperty<int>(nameof(Item), RelationshipTypes.PrivateField)", "int")]
    [DataRow("int", "RegisterProperty(new PropertyInfo<int>(nameof(Item)))", "int")]
    public async Task SilentForRegistrationsThatDoNotMigrate(string type, string registration, string propertyType)
    {
      var code = BusinessObjectWith(
        $"public static readonly PropertyInfo<{type}> ItemProperty = {registration};" +
        $"public {propertyType} Item => GetProperty(ItemProperty)!;");
      await AssertSilentAsync(code);
    }

    [DataTestMethod]
    [DataRow("[AllowNull] public Child Item { get => GetProperty(ItemProperty)!; set => SetProperty(ItemProperty, value); }")]
    [DataRow("[MaybeNull] public Child Item => GetProperty(ItemProperty);")]
    public async Task SilentForNullabilityAttributes(string property)
    {
      var code = BusinessObjectWith(
        "public static readonly PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(nameof(Item));" + property);
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForUnconstrainedTypeParameter()
    {
      var code =
        """
        #nullable enable
        using Csla;
        public class A<T> : BusinessBase<A<T>>
        {
          public static readonly PropertyInfo<T> ItemProperty = RegisterProperty<T>(nameof(Item));
          public T Item => GetProperty(ItemProperty)!;
        }
        """;
      await AssertSilentAsync(code);
    }

    [DataTestMethod]
    [DataRow("string")]
    [DataRow("int")]
    public async Task SilentWithNullableAnnotationsDisabled(string type)
    {
      // a legacy project: value types qualify by type alone, so the context check matters
      var code = BusinessObjectWith(
        $"public static readonly PropertyInfo<{type}> ItemProperty = RegisterProperty<{type}>(nameof(Item));" +
        $"public {type} Item => GetProperty(ItemProperty);", nullable: "#nullable disable");
      await AssertSilentAsync(code);
    }

    [DataTestMethod]
    [DataRow("public Child Item => LazyGetProperty(ItemProperty, () => new Child())!;")]
    [DataRow("public Child Item => GetProperty(ItemProperty)!; public void Load() => LoadPropertyAsync(ItemProperty, System.Threading.Tasks.Task.FromResult(new Child()));")]
    public async Task SilentWhenUsedWithDelayedLoad(string member)
    {
      var code = BusinessObjectWith(
        "public static readonly PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(nameof(Item));" + member);
      await AssertSilentAsync(code);
    }

    [DataTestMethod]
    [DataRow("[DoNotSerialize]")]
    [DataRow("[CalculatedProperty]")]
    public async Task SilentForNotSerializedReferenceWithoutDefault(string attribute)
    {
      var code = BusinessObjectWith(
        "public static readonly PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(nameof(Item));" +
        $"{attribute} public Child Item => GetProperty(ItemProperty)!;");
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task ReportsNotSerializedString()
    {
      var code = BusinessObjectWith(
        "public static readonly PropertyInfo<string> ItemProperty = RegisterProperty<string>(nameof(Item));" +
        "[DoNotSerialize] public string Item => GetProperty(ItemProperty)!;");
      await AssertDiagnosticAsync(code, FixAll);
    }

    [TestMethod]
    public async Task SilentForMultipleDeclarators()
    {
      var code = BusinessObjectWith(
        "public static readonly PropertyInfo<int> ItemProperty = RegisterProperty<int>(nameof(Item)), OtherProperty = RegisterProperty<int>(nameof(Other));" +
        "public int Item => GetProperty(ItemProperty); public int Other => GetProperty(OtherProperty);");
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForNotNullPropertyInfo()
    {
      var code = BusinessObjectWith(
        "public static readonly NotNullPropertyInfo<int> ItemProperty = RegisterNotNullProperty<int>(nameof(Item));" +
        "public int Item => GetProperty(ItemProperty);");
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForManagedObjectBase()
    {
      var code =
        """
        #nullable enable
        using Csla;
        using Csla.Core;
        public class Criteria : ManagedObjectBase
        {
          public static readonly PropertyInfo<int> ItemProperty = RegisterProperty(typeof(Criteria), new PropertyInfo<int>("Item"));
          public int Item => ReadProperty(ItemProperty);
        }
        """;
      await AssertSilentAsync(code);
    }

    [TestMethod]
    public async Task SilentForGeneratedRegistration()
    {
      var code = BusinessObjectWith("public int Item => GetProperty(ItemProperty);");
      var generated =
        """
        // <auto-generated/>
        #nullable enable
        using Csla;
        partial class A
        {
          public static readonly PropertyInfo<int> ItemProperty = RegisterProperty<int>(nameof(Item));
        }
        """;
      await AssertSilentAsync(code, generated);
    }
  }
}
