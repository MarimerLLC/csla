﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
  public sealed class FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue, ids[0],
        nameof(Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue));
    }

    [TestMethod]
    public async Task VerifyGetFixes()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A> { }

public class VerifyGetFixes
{
  private IDataPortal<A> _dataPortal;

  public VerifyGetFixes(IDataPortal<A> dataPortal)
  {
    _dataPortal = dataPortal;
  }

  public async Task Use()
  {
    var x = _dataPortal.Fetch<A>();
    await x.SaveAsync();
  }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new FindSaveAssignmentIssueAnalyzer());

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyChangesAsync(actions,
        FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixConstants.AddAssignmentDescription, document,
        (_, newRoot) =>
        {
          Assert.IsTrue(newRoot.DescendantNodes(_ => true).OfType<AssignmentExpressionSyntax>().Any());
        });
    }
  }
}