using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.FindOperationsWithIncorrectReturnTypes,
        nameof(Constants.AnalyzerIdentifiers.FindOperationsWithIncorrectReturnTypes));
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenChangingToVoid()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [Fetch]
  public string Fetch() { }
}";

      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new FindOperationsWithIncorrectReturnTypesAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.ChangeReturnTypeToVoidDescription, document,
        tree, new[] { "void" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenChangingToTask()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A>
{
  [Fetch]
  public async string FetchAsync() { }
}";

      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new FindOperationsWithIncorrectReturnTypesAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.ChangeReturnTypeToTaskDescription, document,
        tree, new[] { "Task" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenChangingToTaskAndUsingDoesNotExist()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [Fetch]
  public async string FetchAsync() { }
}";

      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new FindOperationsWithIncorrectReturnTypesAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        FindOperationsWithIncorrectReturnTypeResolveCorrectTypeCodeFixConstants.ChangeReturnTypeToTaskDescription, document,
        tree, new[] { "using System.Threading.Tasks;", "Task" });
    }
  }
}