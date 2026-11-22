using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class NotNullPropertyEmptyFriendlyNameAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var diagnostics = new NotNullPropertyEmptyFriendlyNameAnalyzer().SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);
      var diagnostic = diagnostics[0];
      Assert.AreEqual("CSLA0035", diagnostic.Id);
      Assert.AreEqual(Constants.AnalyzerIdentifiers.NotNullPropertyEmptyFriendlyName, diagnostic.Id);
      Assert.AreEqual("An empty friendly name registers no default value", diagnostic.Title.ToString());
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category);
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity);
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.NotNullPropertyEmptyFriendlyName, nameof(NotNullPropertyEmptyFriendlyNameAnalyzer)),
        diagnostic.HelpLinkUri);
    }

    private static string BusinessObjectWith(string registration) =>
      $$"""
      #nullable enable
      using Csla;

      public class A : BusinessBase<A>
      {
        {{registration}}
        public string Name => GetProperty(NameProperty);
      }
      """;

    [DataTestMethod]
    [DataRow("RegisterNotNullProperty<string>(nameof(Name), \"\")", "\"\"")]
    [DataRow("RegisterNotNullProperty<string>(nameof(Name), \"  \")", "\"  \"")]
    [DataRow("RegisterNotNullProperty<string>(_ => _.Name, \"\")", "\"\"")]
    [DataRow("RegisterNotNullProperty<string>(nameof(Name), friendlyName: \"\")", "friendlyName: \"\"")]
    public async Task ReportsEmptyOrWhitespaceFriendlyName(string registration, string locationText)
    {
      var code = BusinessObjectWith($"public static readonly NotNullPropertyInfo<string> NameProperty = {registration};");
      var diagnostics = await TestHelpers.GetDiagnosticsAsync<NotNullPropertyEmptyFriendlyNameAnalyzer>(code);
      Assert.AreEqual(1, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
      var location = diagnostics[0].Location;
      Assert.AreEqual(locationText, location.SourceTree.ToString().Substring(location.SourceSpan.Start, location.SourceSpan.Length));
      StringAssert.Contains(diagnostics[0].GetMessage(), "'Name'");
    }

    [DataTestMethod]
    [DataRow("RegisterNotNullProperty<string>(nameof(Name), \"Name\")")]
    [DataRow("RegisterNotNullProperty<string>(nameof(Name), null)")]
    [DataRow("RegisterNotNullProperty<string>(nameof(Name), null, string.Empty)")]
    [DataRow("RegisterNotNullProperty<string>(nameof(Name), \"\", string.Empty)")]
    [DataRow("RegisterNotNullProperty<string>(nameof(Name))")]
    public async Task SilentForOtherRegistrations(string registration)
    {
      var code = BusinessObjectWith($"public static readonly NotNullPropertyInfo<string> NameProperty = {registration};");
      var diagnostics = await TestHelpers.GetDiagnosticsAsync<NotNullPropertyEmptyFriendlyNameAnalyzer>(code);
      Assert.AreEqual(0, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
    }

    [TestMethod]
    public async Task SilentForOtherPropertyTypes()
    {
      var code =
        """
        #nullable enable
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly NotNullPropertyInfo<A> ChildProperty = RegisterNotNullProperty<A>(nameof(Child), "");
          public A Child => GetProperty(ChildProperty);
          public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), "");
          public string Name => GetProperty(NameProperty)!;
        }
        """;
      var diagnostics = await TestHelpers.GetDiagnosticsAsync<NotNullPropertyEmptyFriendlyNameAnalyzer>(code);
      Assert.AreEqual(0, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
    }
  }
}
