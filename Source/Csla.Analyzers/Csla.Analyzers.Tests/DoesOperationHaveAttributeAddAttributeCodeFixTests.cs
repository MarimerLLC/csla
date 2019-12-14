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

    [DataRow(CslaMemberConstants.Operations.DataPortalCreate,
        CslaMemberConstants.OperationAttributes.Create, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription, 
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalCreateAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalCreate,
        CslaMemberConstants.OperationAttributes.Create, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalCreateAndDoesNotHaveUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalFetch,
        CslaMemberConstants.OperationAttributes.Fetch, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalFetchAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalFetch,
        CslaMemberConstants.OperationAttributes.Fetch, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalFetchAndDoesNotHaveUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalInsert,
        CslaMemberConstants.OperationAttributes.Insert, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalInsertAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalInsert,
        CslaMemberConstants.OperationAttributes.Insert, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalInsertAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalUpdate,
        CslaMemberConstants.OperationAttributes.Update, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalUpdateAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalUpdate,
        CslaMemberConstants.OperationAttributes.Update, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalUpdateAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalDelete,
        CslaMemberConstants.OperationAttributes.Delete, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalDeleteAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalDelete,
        CslaMemberConstants.OperationAttributes.Delete, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalDeleteAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelf, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalDeleteSelfAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelf, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalDeleteSelfAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalExecute,
        CslaMemberConstants.OperationAttributes.Execute, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalExecuteAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalExecute,
        CslaMemberConstants.OperationAttributes.Execute, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalExecuteAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildCreate,
        CslaMemberConstants.OperationAttributes.CreateChild, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsCreateChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildCreate,
        CslaMemberConstants.OperationAttributes.CreateChild, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsCreateChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildFetch,
        CslaMemberConstants.OperationAttributes.FetchChild, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsFetchChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildFetch,
        CslaMemberConstants.OperationAttributes.FetchChild, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsFetchChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildInsert,
        CslaMemberConstants.OperationAttributes.InsertChild, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsInsertChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildInsert,
        CslaMemberConstants.OperationAttributes.InsertChild, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsInsertChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildUpdate,
        CslaMemberConstants.OperationAttributes.UpdateChild, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsUpdateChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildUpdate,
        CslaMemberConstants.OperationAttributes.UpdateChild, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsUpdateChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelfChild, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDeleteSelfChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelfChild, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsDeleteSelfChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildExecute,
        CslaMemberConstants.OperationAttributes.ExecuteChild, true,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsExecuteChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildExecute,
        CslaMemberConstants.OperationAttributes.ExecuteChild, false,
        DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants.AddAttributeAndUsingDescription,
      DisplayName = "VerifyGetFixesWhenOperationIsExecuteChildAndDoesNotHasUsing")]
    [DataTestMethod]
    public async Task VerifyGetFixes(string operationName, string attributeName, bool includeUsingCsla,
      string expectedDescription)
    {
      var code =
$@"{(includeUsingCsla ? "using Csla;" : string.Empty)}

public class A : Csla.BusinessBase<A>
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

      await TestHelpers.VerifyChangesAsync(actions,
        expectedDescription, document,
        (model, newRoot) =>
        {
          Assert.IsTrue(newRoot.DescendantNodes(_ => true).OfType<AttributeSyntax>().Any(_ => _.Name.ToString() == attributeName));

          if(includeUsingCsla)
          {
            Assert.IsTrue(newRoot.DescendantNodes(_ => true).OfType<UsingDirectiveSyntax>().Any(
              _ => _.Name.GetText().ToString() == "Csla"));
          }
        });
    }
  }
}