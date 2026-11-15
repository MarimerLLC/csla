using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class AddsToRuleContextAttributeUsageAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new AddsToRuleContextAttributeUsageAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.AddsToRuleContextUsage, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual("Methods with [AddsToRuleContext] should add to the rule context", diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("Method '{0}' has [AddsToRuleContext] but {1}", diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.AddsToRuleContextUsage, nameof(AddsToRuleContextAttributeUsageAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenExtensionMethodCallsAddMethod()
    {
      var code =
        """
        using Csla.Core;
        using Csla.Rules;

        public static class RuleContextExtensions
        {
          [AddsToRuleContext]
          public static void SetOutValue(this IRuleContext context, IPropertyInfo property, object value)
          {
            context.AddOutValue(property, value);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenStaticMethodCallsAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenPrivateInstanceMethodWithRuleContextClassCallsAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public class A
        {
          [AddsToRuleContext]
          private void Fail(RuleContext context) => context.AddErrorResult("failed");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodCallsAttributedExtensionMethodWithItsContext()
    {
      var code =
        """
        using Csla.Rules;

        public static class RuleContextExtensions
        {
          [AddsToRuleContext]
          public static void Fail(this IRuleContext context) => context.AddErrorResult("failed");

          [AddsToRuleContext]
          public static void FailTwice(this IRuleContext context)
          {
            context.Fail();
            RuleContextExtensions.Fail(context);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodCallsAttributedStaticMethodWithItsContext()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IRuleContext context) => context.AddErrorResult("failed");

          [AddsToRuleContext]
          public static void FailIf(bool condition, IRuleContext context)
          {
            if (condition)
              Fail(context);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodCallsAttributedMethodWithItsSecondContextParameter()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IRuleContext context) => context.AddErrorResult("failed");

          [AddsToRuleContext]
          public static void FailOther(IRuleContext context, IRuleContext other) => Fail(other);
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodCallsAttributedMethodWithAnotherContext()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          private static IRuleContext Other { get; set; }

          [AddsToRuleContext]
          public static void Fail(IRuleContext context) => context.AddErrorResult("failed");

          [AddsToRuleContext]
          public static void FailOther(IRuleContext context) => Fail(Other);
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => Assert.AreEqual("Method 'FailOther' has [AddsToRuleContext] but it doesn't call an Add method on its rule context, or another [AddsToRuleContext] method that it passes the rule context to",
          diagnostics[0].GetMessage()));
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodOnlyCallsItself()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IRuleContext context, int depth)
          {
            if (depth > 0)
              Fail(context, depth - 1);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage]);
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => Assert.AreEqual("Fail", diagnostics[0].Location.SourceTree.ToString()
          .Substring(diagnostics[0].Location.SourceSpan.Start, diagnostics[0].Location.SourceSpan.Length)));
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodHasNoRuleContextParameter()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(string message) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => Assert.AreEqual("Method 'Fail' has [AddsToRuleContext] but it has no IRuleContext parameter",
          diagnostics[0].GetMessage()));
    }

    [TestMethod]
    public async Task AnalyzeWhenAbstractMethodHasAttribute()
    {
      var code =
        """
        using Csla.Rules;

        public abstract class A
        {
          [AddsToRuleContext]
          protected abstract void Fail(IRuleContext context);
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenInterfaceMethodHasAttribute()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          void Fail(IRuleContext context);
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodWithoutAttributeDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          public static void Fail(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }
  }
}
