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
  public sealed class DoesOperationHaveAttributeAddAttributeCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new DoesOperationHaveAttributeAddAttributeCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.DoesOperationHaveAttribute,
        nameof(Constants.AnalyzerIdentifiers.DoesOperationHaveAttribute));
    }

    private static async Task VerifyGetFixes(string operationName, string attributeName)
    {
      var code =
$@"using Csla;

public class A : BusinessBase<A>
{{
  private void {operationName}() {{ }}
}}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new DoesOperationHaveAttributeAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new DoesOperationHaveAttributeAddAttributeCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription, document,
        tree, new[] { $"[{attributeName}]" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsDataPortalCreate() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.DataPortalCreate,
        CslaMemberConstants.OperationAttributes.Create);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsDataPortalFetch() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.DataPortalFetch,
        CslaMemberConstants.OperationAttributes.Fetch);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsDataPortalInsert() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.DataPortalInsert,
        CslaMemberConstants.OperationAttributes.Insert);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsDataPortalUpdate() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.DataPortalUpdate,
        CslaMemberConstants.OperationAttributes.Update);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsDataPortalDelete() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.DataPortalDelete,
        CslaMemberConstants.OperationAttributes.Delete);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsDataPortalDeleteSelf() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.DataPortalDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelf);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsDataPortalExecute() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.DataPortalExecute,
        CslaMemberConstants.OperationAttributes.Execute);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsChildCreate() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.ChildCreate,
        CslaMemberConstants.OperationAttributes.CreateChild);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsChildFetch() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.ChildFetch,
        CslaMemberConstants.OperationAttributes.FetchChild);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsChildInsert() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.ChildInsert,
        CslaMemberConstants.OperationAttributes.InsertChild);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsChildUpdate() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.ChildUpdate,
        CslaMemberConstants.OperationAttributes.UpdateChild);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsChildDeleteSelf() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.ChildDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelfChild);

    [TestMethod]
    public async Task VerifyGetFixesWhenOperationIsChildExecute() =>
      await VerifyGetFixes(CslaMemberConstants.Operations.ChildExecute,
        CslaMemberConstants.OperationAttributes.ExecuteChild);
  }
}