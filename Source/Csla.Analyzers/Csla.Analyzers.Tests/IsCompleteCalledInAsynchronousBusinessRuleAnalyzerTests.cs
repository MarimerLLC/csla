using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class IsCompleteCalledInAsynchronousBusinessRuleAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new IsCompleteCalledInAsynchronousBusinessRuleAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.CompleteInExecuteAsync, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(IsCompleteCalledInAsynchronousBusinessRuleConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(IsCompleteCalledInAsynchronousBusinessRuleConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.CompleteInExecuteAsync, nameof(IsCompleteCalledInAsynchronousBusinessRuleAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotABusinessRule()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<IsCompleteCalledInAsynchronousBusinessRuleAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndDoesNotCallComplete()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public class A : BusinessRuleAsync
{ 
  protected override Task ExecuteAsync(IRuleContext context) { } 
}";
      await TestHelpers.RunAnalysisAsync<IsCompleteCalledInAsynchronousBusinessRuleAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsComplete()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public class A : BusinessRuleAsync
{ 
  protected override Task ExecuteAsync(IRuleContext context) 
  { 
    context.Complete();
  } 
}";
      await TestHelpers.RunAnalysisAsync<IsCompleteCalledInAsynchronousBusinessRuleAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.CompleteInExecuteAsync });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsABusinessRuleAndCallsCompleteAndNameof()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public class A : BusinessRuleAsync
{ 
  protected override Task ExecuteAsync(IRuleContext context) 
  { 
    var c = nameof(context);
    context.Complete();
  } 
}";
      await TestHelpers.RunAnalysisAsync<IsCompleteCalledInAsynchronousBusinessRuleAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.CompleteInExecuteAsync });
    }
  }
}