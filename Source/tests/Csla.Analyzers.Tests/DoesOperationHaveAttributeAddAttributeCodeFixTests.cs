using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;

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
        "Add attribute", 
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalCreateAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalCreate,
        CslaMemberConstants.OperationAttributes.Create, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalCreateAndDoesNotHaveUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalFetch,
        CslaMemberConstants.OperationAttributes.Fetch, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalFetchAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalFetch,
        CslaMemberConstants.OperationAttributes.Fetch, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalFetchAndDoesNotHaveUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalInsert,
        CslaMemberConstants.OperationAttributes.Insert, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalInsertAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalInsert,
        CslaMemberConstants.OperationAttributes.Insert, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalInsertAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalUpdate,
        CslaMemberConstants.OperationAttributes.Update, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalUpdateAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalUpdate,
        CslaMemberConstants.OperationAttributes.Update, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalUpdateAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalDelete,
        CslaMemberConstants.OperationAttributes.Delete, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalDeleteAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalDelete,
        CslaMemberConstants.OperationAttributes.Delete, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalDeleteAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelf, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalDeleteSelfAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelf, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalDeleteSelfAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalExecute,
        CslaMemberConstants.OperationAttributes.Execute, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalExecuteAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.DataPortalExecute,
        CslaMemberConstants.OperationAttributes.Execute, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsDataPortalExecuteAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildCreate,
        CslaMemberConstants.OperationAttributes.CreateChild, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsCreateChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildCreate,
        CslaMemberConstants.OperationAttributes.CreateChild, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsCreateChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildFetch,
        CslaMemberConstants.OperationAttributes.FetchChild, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsFetchChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildFetch,
        CslaMemberConstants.OperationAttributes.FetchChild, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsFetchChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildInsert,
        CslaMemberConstants.OperationAttributes.InsertChild, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsInsertChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildInsert,
        CslaMemberConstants.OperationAttributes.InsertChild, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsInsertChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildUpdate,
        CslaMemberConstants.OperationAttributes.UpdateChild, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsUpdateChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildUpdate,
        CslaMemberConstants.OperationAttributes.UpdateChild, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsUpdateChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelfChild, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsDeleteSelfChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildDeleteSelf,
        CslaMemberConstants.OperationAttributes.DeleteSelfChild, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsDeleteSelfChildAndDoesNotHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildExecute,
        CslaMemberConstants.OperationAttributes.ExecuteChild, true,
        "Add attribute",
      DisplayName = "VerifyGetFixesWhenOperationIsExecuteChildAndHasUsing")]
    [DataRow(CslaMemberConstants.Operations.ChildExecute,
        CslaMemberConstants.OperationAttributes.ExecuteChild, false,
        "Add attribute and using statement",
      DisplayName = "VerifyGetFixesWhenOperationIsExecuteChildAndDoesNotHasUsing")]
    [DataTestMethod]
    public async Task VerifyGetFixes(string operationName, string attributeName, bool includeUsingCsla,
      string expectedDescription)
    {
      var usingCsla = includeUsingCsla ? "using Csla;" : string.Empty;
      var code =
        $$"""
          {{usingCsla}}

          public class A : Csla.BusinessBase<A>
          {
            private void {{operationName}}() { }
          }
          """;
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new DoesOperationHaveAttributeAnalyzer());

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