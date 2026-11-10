using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class CalculatedPropertyAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new CalculatedPropertyAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);

      var mustBeReadOnly = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.CalculatedPropertyMustBeReadOnly, mustBeReadOnly.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual("Calculated properties should be read-only", mustBeReadOnly.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("The calculated property '{0}' should have no setter or a private setter", mustBeReadOnly.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, mustBeReadOnly.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, mustBeReadOnly.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.CalculatedPropertyMustBeReadOnly, nameof(CalculatedPropertyAnalyzer)),
        mustBeReadOnly.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));

      var requiresBusinessBase = diagnostics[1];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.CalculatedPropertyRequiresBusinessBase, requiresBusinessBase.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual("Calculated properties are only supported in BusinessBase types", requiresBusinessBase.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("The calculated property '{0}' is not in a BusinessBase type, so it will not be serialized or recalculated", requiresBusinessBase.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, requiresBusinessBase.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, requiresBusinessBase.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.CalculatedPropertyRequiresBusinessBase, nameof(CalculatedPropertyAnalyzer)),
        requiresBusinessBase.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    private static string BusinessObjectWith(string property) =>
      $$"""
      using Csla;

      public class A : BusinessBase<A>
      {
        public static readonly PropertyInfo<int> TotalProperty = RegisterProperty<int>(nameof(Total));
        {{property}}
      }
      """;

    [TestMethod]
    public async Task AnalyzeWhenPropertyIsNotCalculated()
    {
      var code = BusinessObjectWith(
        "public int Total { get => GetProperty(TotalProperty); set => SetProperty(TotalProperty, value); }");
      await TestHelpers.RunAnalysisAsync<CalculatedPropertyAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenCalculatedPropertyIsGetterOnly()
    {
      var code = BusinessObjectWith(
        "[CalculatedProperty] public int Total => GetProperty(TotalProperty);");
      await TestHelpers.RunAnalysisAsync<CalculatedPropertyAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenCalculatedPropertyHasPrivateSetter()
    {
      var code = BusinessObjectWith(
        "[CalculatedProperty] public int Total { get => GetProperty(TotalProperty); private set => SetProperty(TotalProperty, value); }");
      await TestHelpers.RunAnalysisAsync<CalculatedPropertyAnalyzer>(code, []);
    }

    [TestMethod]
    [DataRow("public int Total { get => GetProperty(TotalProperty); set => SetProperty(TotalProperty, value); }", DisplayName = "public setter")]
    [DataRow("public int Total { get => GetProperty(TotalProperty); internal set => SetProperty(TotalProperty, value); }", DisplayName = "internal setter")]
    [DataRow("public int Total { get => GetProperty(TotalProperty); protected set => SetProperty(TotalProperty, value); }", DisplayName = "protected setter")]
    [DataRow("public int Total { get => GetProperty(TotalProperty); private protected set => SetProperty(TotalProperty, value); }", DisplayName = "private protected setter")]
    [DataRow("public int Total { get => GetProperty(TotalProperty); init => SetProperty(TotalProperty, value); }", DisplayName = "init accessor")]
    public async Task AnalyzeWhenCalculatedPropertyHasNonPrivateSetter(string property)
    {
      var code = BusinessObjectWith("[CalculatedProperty] " + property);
      await TestHelpers.RunAnalysisAsync<CalculatedPropertyAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.CalculatedPropertyMustBeReadOnly]);
    }

    [TestMethod]
    public async Task AnalyzeWhenCalculatedPropertyIsInReadOnlyBase()
    {
      var code =
        """
        using Csla;

        public class A : ReadOnlyBase<A>
        {
          public static readonly PropertyInfo<int> TotalProperty = RegisterProperty<int>(nameof(Total));
          [CalculatedProperty]
          public int Total => GetProperty(TotalProperty);
        }
        """;
      await TestHelpers.RunAnalysisAsync<CalculatedPropertyAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.CalculatedPropertyRequiresBusinessBase]);
    }

    [TestMethod]
    public async Task AnalyzeWhenCalculatedPropertyIsInNonCslaType()
    {
      var code =
        """
        using Csla;

        public class A
        {
          [CalculatedProperty]
          public int Total { get; set; }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CalculatedPropertyAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.CalculatedPropertyRequiresBusinessBase]);
    }
  }
}
