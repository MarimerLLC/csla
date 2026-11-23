using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class BusinessRuleDoesNotUseAddMethodsOnContextAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.BusinessRuleContextUsage, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual("Find Business Rules That Do Not Use Add() Methods on the Context", diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("Business rules should use at least one Add() method on the context", diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.BusinessRuleContextUsage, nameof(BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotABusinessRule()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndDoesNotCallAnAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.BusinessRuleContextUsage]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAddMethod()
    {
      var code =
        """
        using Csla.Rules;

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context)
          {
            context.AddDirtyProperty(null);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsTypedNotNullAddOutValue()
    {
      var code =
        """
        using Csla;
        using Csla.Rules;

        public class A
          : BusinessRule
        {
          public static NotNullPropertyInfo<string> NameProperty { get; set; }

          protected override void Execute(IRuleContext context)
          {
            context.AddOutValue(NameProperty, "x");
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAsyncAndDoesNotCallAnAddMethod()
    {
      var code =
        """
        using Csla.Rules;
        using System.Threading.Tasks;

        public class A
          : BusinessRuleAsync
        {
          protected override async Task ExecuteAsync(IRuleContext context) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.BusinessRuleContextUsage]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAsyncAndCallsAddMethod()
    {
      var code =
        """
        using Csla.Rules;
        using System.Threading.Tasks;

        public class A
          : BusinessRuleAsync
        {
          protected override async Task ExecuteAsync(IRuleContext context)
          {
            context.AddDirtyProperty(null);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAddMethodAndUsesNameof()
    {
      var code =
        """
        using Csla.Rules;

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context)
          {
            var c = nameof(context);
            context.AddDirtyProperty(null);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAttributedExtensionMethod()
    {
      var code =
        """
        using Csla.Core;
        using Csla.Rules;

        public static class RuleContextExtensions
        {
          [AddsToRuleContext]
          public static void SetOutValue(this IRuleContext context, IPropertyInfo property, object value) =>
            context.AddOutValue(property, value);
        }

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context)
          {
            context.SetOutValue(null, 42);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeDocumentationSampleWithAttributedHelper()
    {
      // Same code as the "Helper methods" sample in
      // docs/analyzers/CSLA0017-BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer.md
      var code =
        """
        using Csla;
        using Csla.Rules;

        public static class RuleContextExtensions
        {
          [AddsToRuleContext]
          public static void SetOutValue<T>(this IRuleContext context, PropertyInfo<T> property, T value)
          {
            // normalization, logging, ...
            context.AddOutValue(property, value);
          }
        }

        public class SetToFortyTwo
          : BusinessRule
        {
          private readonly PropertyInfo<int> _target;

          public SetToFortyTwo(PropertyInfo<int> target)
            : base(target)
          {
            _target = target;
            AffectedProperties.Add(target);
          }

          protected override void Execute(IRuleContext context)
          {
            // No CSLA0017: SetOutValue has [AddsToRuleContext].
            context.SetOutValue(_target, 42);
          }
        }
        """;
      var compilation = await TestHelpers.Create(code).Project.GetCompilationAsync();
      var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
      Assert.AreEqual(0, errors.Count, string.Join(Environment.NewLine, errors));
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
      await TestHelpers.RunAnalysisAsync<AddsToRuleContextAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAttributedGenericExtensionMethod()
    {
      var code =
        """
        using Csla;
        using Csla.Rules;

        public static class RuleContextExtensions
        {
          [AddsToRuleContext]
          public static void SetOutValue<T>(this IRuleContext context, PropertyInfo<T> property, T value) =>
            context.AddOutValue(property, value);
        }

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context)
          {
            context.SetOutValue<int>(null, 42);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAttributedStaticMethod()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          [AddsToRuleContext]
          public static void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context)
          {
            Helper.Fail(context);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAttributedPrivateHelper()
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
          }

          [AddsToRuleContext]
          private void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAttributedBaseClassHelper()
    {
      var code =
        """
        using Csla.Rules;

        public abstract class MyRuleBase
          : BusinessRule
        {
          [AddsToRuleContext]
          protected void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }

        public class A
          : MyRuleBase
        {
          protected override void Execute(IRuleContext context)
          {
            Fail(context);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsHelperWithoutAttribute()
    {
      var code =
        """
        using Csla.Rules;

        public static class Helper
        {
          public static void Fail(IRuleContext context) => context.AddErrorResult("failed");
        }

        public class A
          : BusinessRule
        {
          protected override void Execute(IRuleContext context)
          {
            Helper.Fail(context);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.BusinessRuleContextUsage]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAsyncAndCallsAttributedExtensionMethod()
    {
      var code =
        """
        using Csla.Rules;
        using System.Threading.Tasks;

        public static class RuleContextExtensions
        {
          [AddsToRuleContext]
          public static void Fail(this IRuleContext context) => context.AddErrorResult("failed");
        }

        public class A
          : BusinessRuleAsync
        {
          protected override async Task ExecuteAsync(IRuleContext context)
          {
            await Task.Yield();
            context.Fail();
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(code, []);
    }
  }
}