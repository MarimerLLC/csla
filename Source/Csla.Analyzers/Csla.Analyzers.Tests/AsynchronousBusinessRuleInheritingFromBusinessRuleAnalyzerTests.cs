using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.AsynchronousBusinessRuleInheritance, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.AsynchronousBusinessRuleInheritance, nameof(AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotABusinessRule()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleButDoesNotOverrideExecute()
    {
      var code = 
@"using Csla.Rules;

public class A : BusinessRule { }";
      await TestHelpers.RunAnalysisAsync<AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndOverridesExecuteAndIsNotAsync()
    {
      var code =
@"using Csla.Rules;

public class A : BusinessRule 
{ 
  protected override void Execute(IRuleContext context)
  {
    base.Execute(context);
  }
}";
      await TestHelpers.RunAnalysisAsync<AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndOverridesExecuteAndIsAsync()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public class A : BusinessRule 
{ 
  protected override async void Execute(IRuleContext context)
  {
    await Task.Yield();
    base.Execute(context);
  }
}";
      await TestHelpers.RunAnalysisAsync<AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.AsynchronousBusinessRuleInheritance });
    }
  }
}