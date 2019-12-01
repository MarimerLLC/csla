using Microsoft.CodeAnalysis;
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
  public sealed class DoesChildOperationHaveRunLocalRemoveAttributeCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new DoesChildOperationHaveRunLocalRemoveAttributeCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.DoesChildOperationHaveRunLocal,
        nameof(Constants.AnalyzerIdentifiers.DoesChildOperationHaveRunLocal));
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenRunLocalIsStandalone()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [RunLocal]
  [FetchChild]
  private void FetchChild() { }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new DoesChildOperationHaveRunLocalAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new DoesChildOperationHaveRunLocalRemoveAttributeCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyChangesAsync(actions,
        DoesChildOperationHaveRunLocalRemoveAttributeCodeFixConstants.RemoveRunLocalDescription, document,
        (model, newRoot) =>
        {
          Assert.IsFalse(newRoot.DescendantNodes(_ => true).OfType<AttributeSyntax>().Any(_ => _.Name.ToString() == "RunLocal"));
        });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenRunLocalIsEmbeddedInList()
    {
      var code =
@"using Csla;
using System;

public sealed class FooAttribute : Attribute { }

public class A : BusinessBase<A>
{
  [RunLocal, Foo, FetchChild]
  private void FetchChild() { }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new DoesChildOperationHaveRunLocalAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new DoesChildOperationHaveRunLocalRemoveAttributeCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyChangesAsync(actions,
        DoesChildOperationHaveRunLocalRemoveAttributeCodeFixConstants.RemoveRunLocalDescription, document,
        (model, newRoot) =>
        {
          Assert.IsFalse(newRoot.DescendantNodes(_ => true).OfType<AttributeSyntax>().Any(_ => _.Name.ToString() == "RunLocal"));
        });
    }
  }
}