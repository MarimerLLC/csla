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
  public sealed class IsOperationMethodPublicMakeNonPublicCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.IsOperationMethodPublic,
        nameof(Constants.AnalyzerIdentifiers.IsOperationMethodPublic));
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenClassIsNotSealed()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public class A : BusinessBase<A>
{
  [Fetch]
  public void Fetch() { }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsOperationMethodPublicAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(3, actions.Count);
      await TestHelpers.VerifyActionAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.PrivateDescription, document,
        tree, new[] { "rivate" });
      await TestHelpers.VerifyActionAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.ProtectedDescription, document,
        tree, new[] { "rotected" });
      await TestHelpers.VerifyActionAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.InternalDescription, document,
        tree, new[] { "internal" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenClassIsSealed()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public sealed class A : BusinessBase<A>
{
  [Fetch]
  public void Fetch() { }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsOperationMethodPublicAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(2, actions.Count);
      await TestHelpers.VerifyActionAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.PrivateDescription, document,
        tree, new[] { "rivate" });
      await TestHelpers.VerifyActionAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.InternalDescription, document,
        tree, new[] { "internal" });
    }
  }
}