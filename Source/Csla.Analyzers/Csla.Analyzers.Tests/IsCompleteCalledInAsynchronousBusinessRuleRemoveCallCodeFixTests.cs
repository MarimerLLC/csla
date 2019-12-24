using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class IsCompleteCalledInAsynchronousBusinessRuleRemoveCallCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new IsCompleteCalledInAsynchronousBusinessRuleRemoveCallCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.CompleteInExecuteAsync,
        nameof(Constants.AnalyzerIdentifiers.CompleteInExecuteAsync));
    }

    [TestMethod]
    public async Task VerifyGetFixes()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public sealed class TestRule : BusinessRuleAsync
{
  protected override async Task ExecuteAsync(IRuleContext context)
  {
    context.Complete();
  }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsCompleteCalledInAsynchronousBusinessRuleAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsCompleteCalledInAsynchronousBusinessRuleRemoveCallCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyChangesAsync(actions,
        IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants.RemoveCompleteCalls, document,
        (model, newRoot) =>
        {
          Assert.AreEqual(0, newRoot.DescendantNodes(_ => true).OfType<InvocationExpressionSyntax>().Count());
        });
    }

    [TestMethod]
    public async Task VerifyGetFixesWithNameofCall()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public sealed class TestRule : BusinessRuleAsync
{
  protected override async Task ExecuteAsync(IRuleContext context)
  {
    var c = nameof(context);
    context.Complete();
  }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsCompleteCalledInAsynchronousBusinessRuleAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsCompleteCalledInAsynchronousBusinessRuleRemoveCallCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyChangesAsync(actions,
        IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants.RemoveCompleteCalls, document,
        (model, newRoot) =>
        {
          Assert.AreEqual(1, newRoot.DescendantNodes(_ => true).OfType<InvocationExpressionSyntax>().Count());
        });
    }
  }
}