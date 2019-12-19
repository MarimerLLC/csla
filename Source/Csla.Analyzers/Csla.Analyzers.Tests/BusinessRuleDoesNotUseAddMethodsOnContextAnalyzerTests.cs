using System;
using System.Threading.Tasks;
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
      Assert.AreEqual(BusinessRuleDoesNotUseAddMethodsOnContextAnalyzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(BusinessRuleDoesNotUseAddMethodsOnContextAnalyzerConstants.Message, diagnostic.MessageFormat.ToString(),
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
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndDoesNotCallAnAddMethod()
    {
      var code =
@"using Csla.Rules;

public class A 
  : BusinessRule 
{ 
  protected override void Execute(IRuleContext context) { }
}";
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.BusinessRuleContextUsage });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAddMethod()
    {
      var code =
@"using Csla.Rules;

public class A 
  : BusinessRule 
{ 
  protected override void Execute(IRuleContext context) 
  { 
    context.AddDirtyProperty(null);
  }
}";
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAsyncAndDoesNotCallAnAddMethod()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public class A 
  : BusinessRuleAsync
{ 
  protected override async Task ExecuteAsync(IRuleContext context) { }
}";
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.BusinessRuleContextUsage });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAsyncAndCallsAddMethod()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public class A 
  : BusinessRuleAsync
{ 
  protected override async Task ExecuteAsync(IRuleContext context)
  { 
    context.AddDirtyProperty(null);
  }
}";
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsAddMethodAndUsesNameof()
    {
      var code =
@"using Csla.Rules;

public class A 
  : BusinessRule 
{ 
  protected override void Execute(IRuleContext context) 
  { 
    var c = nameof(context);
    context.AddDirtyProperty(null);
  }
}";
      await TestHelpers.RunAnalysisAsync<BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer>(
        code, Array.Empty<string>());
    }
  }
}