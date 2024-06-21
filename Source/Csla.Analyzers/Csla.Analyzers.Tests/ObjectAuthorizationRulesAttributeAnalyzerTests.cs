using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class ObjectAuthorizationRulesAttributeAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new ObjectAuthorizationRulesAttributeAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(3, diagnostics.Length);

      var attributeMissingDiagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesAttributeMissing, attributeMissingDiagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(ObjectAuthorizationRulesAttributeAnalyzerConstants.AttributeMissingTitle, attributeMissingDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(ObjectAuthorizationRulesAttributeAnalyzerConstants.AttributeMissingMessage, attributeMissingDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, attributeMissingDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, attributeMissingDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesAttributeMissing, nameof(ObjectAuthorizationRulesAttributeAnalyzer)),
        attributeMissingDiagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));

      var rulesConfigurationPublicDiagnostic = diagnostics[1];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesPublic, rulesConfigurationPublicDiagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(ObjectAuthorizationRulesAttributeAnalyzerConstants.RulesPublicTitle, rulesConfigurationPublicDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(ObjectAuthorizationRulesAttributeAnalyzerConstants.RulesPublicMessage, rulesConfigurationPublicDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, rulesConfigurationPublicDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Info, rulesConfigurationPublicDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesPublic, nameof(ObjectAuthorizationRulesAttributeAnalyzer)),
        rulesConfigurationPublicDiagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));

      var rulesConfigurationStaticDiagnostic = diagnostics[2];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesStatic, rulesConfigurationStaticDiagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(ObjectAuthorizationRulesAttributeAnalyzerConstants.RulesStaticTitle, rulesConfigurationStaticDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(ObjectAuthorizationRulesAttributeAnalyzerConstants.RulesStaticMessage, rulesConfigurationStaticDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, rulesConfigurationStaticDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, rulesConfigurationStaticDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesStatic, nameof(ObjectAuthorizationRulesAttributeAnalyzer)),
        rulesConfigurationStaticDiagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotMobileObject()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<ObjectAuthorizationRulesAttributeAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndObjectAuthorizationRulesHasAttribute()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          [ObjectAuthorizationRules]
          public static void ConfigureObjectAuthorizationRules()
          {
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<ObjectAuthorizationRulesAttributeAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndDoesNotHaveObjectAuthorizationRules()
    {
      var code = 
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          [Fetch]
          private void Fetch() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<ObjectAuthorizationRulesAttributeAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsMobileObjectAndOperationHasNamingConvention()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static void AddObjectAuthorizationRules()
          {
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<ObjectAuthorizationRulesAttributeAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesAttributeMissing]);
    }

    [TestMethod]
    public async Task ObjectAuthorizationRulesMethodShouldBePublic()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          [ObjectAuthorizationRules]
          private static void AddObjectAuthorizationRules()
          {
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<ObjectAuthorizationRulesAttributeAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesPublic]);
    }

    [TestMethod]
    public async Task ObjectAuthorizationRulesMethodShouldBeStatic()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          [ObjectAuthorizationRules]
          public void AddObjectAuthorizationRules()
          {
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<ObjectAuthorizationRulesAttributeAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.ObjectAuthorizationRulesStatic]);
    }
  }
}