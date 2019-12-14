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
  public sealed class AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.AsynchronousBusinessRuleInheritance,
        nameof(Constants.AnalyzerIdentifiers.AsynchronousBusinessRuleInheritance));
    }

    [TestMethod]
    public async Task VerifyGetFixesWithNamespace()
    {
      var code =
@"using Csla.Rules;
using System.Threading.Tasks;

public sealed class TestRule : BusinessRule 
{
  protected override async void Execute(IRuleContext context)
  {
    await Task.Yield();
  }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));
      await TestHelpers.VerifyChangesAsync(actions,
        AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants.UpdateToAsyncEquivalentsDescription, document,
        (model, newRoot) =>
        {
          var classNode = newRoot.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>().Single();
          var classSymbol = model.GetDeclaredSymbol(classNode) as INamedTypeSymbol;
          Assert.AreEqual("BusinessRuleAsync", classSymbol.BaseType.Name);

          var methodSymbol = classSymbol.GetMembers().OfType<IMethodSymbol>().Single(_ => _.Name == "ExecuteAsync");
          Assert.AreEqual("Task", methodSymbol.ReturnType.Name);
          Assert.IsTrue(methodSymbol.IsAsync);
        });
    }

    [TestMethod]
    public async Task VerifyGetFixesWithoutNamespace()
    {
      var code =
@"using Csla.Rules;

public sealed class TestRule : BusinessRule 
{
  protected override async void Execute(IRuleContext context)
  {
    await Task.Yield();
  }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0], codeActionRegistration, default);
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyChangesAsync(actions,
        AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants.UpdateToAsyncEquivalentsDescription, document,
        (model, newRoot) =>
        {
          Assert.IsTrue(newRoot.DescendantNodes(_ => true).OfType<UsingDirectiveSyntax>().Any(
            _ => _.Name.GetText().ToString() == "System.Threading.Tasks"));
          var classNode = newRoot.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>().Single();
          var classSymbol = model.GetDeclaredSymbol(classNode) as INamedTypeSymbol;
          Assert.AreEqual("BusinessRuleAsync", classSymbol.BaseType.Name);

          var methodSymbol = classSymbol.GetMembers().OfType<IMethodSymbol>().Single(_ => _.Name == "ExecuteAsync");
          Assert.AreEqual("Task", methodSymbol.ReturnType.Name);
          Assert.IsTrue(methodSymbol.IsAsync);
        });
    }
  }
}