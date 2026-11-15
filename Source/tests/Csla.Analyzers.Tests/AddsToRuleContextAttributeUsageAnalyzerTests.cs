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
      Assert.AreEqual(2, diagnostics.Length);

      foreach (var diagnostic in diagnostics)
      {
        Assert.AreEqual(Constants.AnalyzerIdentifiers.AddsToRuleContextUsage, diagnostic.Id,
          nameof(DiagnosticDescriptor.Id));
        Assert.AreEqual("Methods with [AddsToRuleContext] should add to the rule context", diagnostic.Title.ToString(),
          nameof(DiagnosticDescriptor.Title));
        Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
          nameof(DiagnosticDescriptor.Category));
        Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
          nameof(DiagnosticDescriptor.DefaultSeverity));
        Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.AddsToRuleContextUsage, nameof(AddsToRuleContextAttributeUsageAnalyzer)),
          diagnostic.HelpLinkUri,
          nameof(DiagnosticDescriptor.HelpLinkUri));
      }
      Assert.AreEqual("Method '{0}' has [AddsToRuleContext] but {1}", diagnostics[0].MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual("'{0}' implements the [AddsToRuleContext] method '{1}' with the inherited method '{2}', which must have [AddsToRuleContext] so that it is checked",
        diagnostics[1].MessageFormat.ToString(), nameof(DiagnosticDescriptor.MessageFormat));
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
    public async Task AnalyzeWhenParameterIsDerivedRuleContextInterface()
    {
      var code =
        """
        using Csla.Rules;

        public interface IMyRuleContext : IRuleContext { }

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IMyRuleContext context) => context.AddErrorResult("failed");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenParameterIsConstrainedTypeParameter()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail<TContext>(TContext context)
            where TContext : IRuleContext => context.AddErrorResult("failed");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenAddMethodIsCalledOnAnotherContext()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          private static IRuleContext Other { get; set; }

          [AddsToRuleContext]
          public static void FailOther(IRuleContext context) => Other.AddErrorResult("failed");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage]);
    }

    [TestMethod]
    public async Task AnalyzeWhenContextIsUsedWithNullConditionalOperator()
    {
      var code =
        """
        using Csla.Rules;

        public static class RuleContextExtensions
        {
          [AddsToRuleContext]
          public static void Fail(this IRuleContext context) => context?.AddErrorResult("failed");

          [AddsToRuleContext]
          public static void FailAgain(this IRuleContext context) => context?.Fail();
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenOverrideOfAttributedMethodDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public abstract class MyRuleBase
          : BusinessRule
        {
          [AddsToRuleContext]
          protected abstract void Fail(IRuleContext context);
        }

        public class MyRule
          : MyRuleBase
        {
          protected override void Fail(IRuleContext context) { }

          protected override void Execute(IRuleContext context) => Fail(context);
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => Assert.AreEqual(12, diagnostics[0].Location.GetLineSpan().StartLinePosition.Line));
    }

    [TestMethod]
    public async Task AnalyzeWhenOverrideOfAttributedVirtualMethodCallsAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public class MyRuleBase
          : BusinessRule
        {
          [AddsToRuleContext]
          protected virtual void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }

        public class MyRule
          : MyRuleBase
        {
          protected override void Fail(IRuleContext context) => context.AddWarningResult("warning");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenImplicitImplementationOfAttributedInterfaceMethodDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          void Fail(IRuleContext context);
        }

        public class Helper : IHelper
        {
          public void Fail(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage]);
    }

    [TestMethod]
    public async Task AnalyzeWhenExplicitImplementationOfAttributedInterfaceMethodDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          void Fail(IRuleContext context);
        }

        public class Helper : IHelper
        {
          void IHelper.Fail(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage]);
    }

    [TestMethod]
    public async Task AnalyzeWhenStaticImplementationOfAttributedStaticAbstractInterfaceMethodDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          static abstract void Fail(IRuleContext context);
        }

        public class Helper : IHelper
        {
          public static void Fail(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => Assert.AreEqual(10, diagnostics[0].Location.GetLineSpan().StartLinePosition.Line));
    }

    [TestMethod]
    public async Task AnalyzeWhenStaticImplementationOfAttributedStaticAbstractInterfaceMethodCallsAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          static abstract void Fail(IRuleContext context);
        }

        public class Helper : IHelper
        {
          public static void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenInheritedMethodImplementsAttributedInterfaceMethod()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          void Fail(IRuleContext context);
        }

        public class Base
        {
          public void Fail(IRuleContext context) { }
        }

        public class Derived : Base, IHelper { }

        public class MoreDerived : Derived { }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => Assert.AreEqual("'Derived' implements the [AddsToRuleContext] method 'IHelper.Fail(IRuleContext)' with the inherited method 'Base.Fail(IRuleContext)', which must have [AddsToRuleContext] so that it is checked",
          diagnostics[0].GetMessage()));
    }

    [TestMethod]
    public async Task AnalyzeWhenAttributedInheritedMethodImplementsAttributedInterfaceMethod()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          void Fail(IRuleContext context);
        }

        public class Base
        {
          [AddsToRuleContext]
          public void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }

        public class Derived : Base, IHelper { }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenMethodInheritedFromReferencedAssemblyImplementsAttributedInterfaceMethod()
    {
      // object.ToString() comes from a referenced assembly, so its body can't be checked
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          string ToString();
        }

        public class Helper : IHelper { }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => StringAssert.StartsWith(diagnostics[0].GetMessage(), "'Helper' implements the [AddsToRuleContext] method 'IHelper.ToString()'"));
    }

    [TestMethod]
    public async Task AnalyzeWhenOverrideOfInterfaceImplementationDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          [AddsToRuleContext]
          void Fail(IRuleContext context);
        }

        public class Base : IHelper
        {
          public virtual void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }

        public class Derived : Base
        {
          public override void Fail(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => Assert.AreEqual(15, diagnostics[0].Location.GetLineSpan().StartLinePosition.Line));
    }

    [TestMethod]
    public async Task AnalyzeWhenAddMethodIsCalledThroughCastToUnrelatedType()
    {
      var code =
        """
        using System.Collections.Generic;
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Trace(IRuleContext context) => ((ICollection<string>)context).Add("trace");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage]);
    }

    [TestMethod]
    public async Task AnalyzeWhenAddMethodIsCalledThroughCastToRuleContextClass()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IRuleContext context) => ((RuleContext)context).AddErrorResult("failed");
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenImplementationOfInterfaceMethodWithoutAttributeDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public interface IHelper
        {
          void Fail(IRuleContext context);
        }

        public class Helper : IHelper
        {
          public void Fail(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenAttributedLocalFunctionDoesNotCallAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context)
          {
            Fail(context);

            [AddsToRuleContext]
            static void Fail(IRuleContext c) { }
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage],
        diagnostics => Assert.AreEqual("Method 'Fail' has [AddsToRuleContext] but it doesn't call an Add method on its rule context, or another [AddsToRuleContext] method that it passes the rule context to",
          diagnostics[0].GetMessage()));
    }

    [TestMethod]
    public async Task AnalyzeWhenAttributedLocalFunctionCallsAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context)
          {
            Fail(context);

            [AddsToRuleContext]
            static void Fail(IRuleContext c) => c.AddErrorResult("failed");
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenAttributedMethodIsInGeneratedCode()
    {
      var code =
        """
        // <auto-generated/>
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.AddsToRuleContextUsage]);
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
