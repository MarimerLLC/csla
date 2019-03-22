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
  public sealed class EvaluateManagedBackingFieldsCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new EvaluateManagedBackingFieldsCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
        nameof(Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields));
    }

    [TestMethod]
    public async Task VerifyGetFixes()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  PropertyInfo<string> DataProperty =
    RegisterProperty<string>(_ => _.Data);
  public string Data
  {
    get { return GetProperty(DataProperty); }
    set { SetProperty(DataProperty, value); }
  }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new EvaluateManagedBackingFieldsAnalayzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new EvaluateManagedBackingFieldsCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        EvaluateManagedBackingFieldsCodeFixConstants.FixManagedBackingFieldDescription, document,
        tree, new[] { "public static readonly" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWithTrivia()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  #region Properties
  private static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
  #endregion

  public string Data => GetProperty(DataProperty);
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new EvaluateManagedBackingFieldsAnalayzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new EvaluateManagedBackingFieldsCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        EvaluateManagedBackingFieldsCodeFixConstants.FixManagedBackingFieldDescription, document,
        tree, new[] { "#region Properties", "public" });
    }
  }
}
