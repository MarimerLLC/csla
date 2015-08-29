using Csla.Analyzers;
using Csla.Analyzers.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FixingIsOneWay.Tests
{
  [TestClass]
  public sealed class IsBusinessObjectSerializableMakeSerializableCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new IsBusinessObjectSerializableMakeSerializableCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(IsBusinessObjectSerializableConstants.DiagnosticId, ids[0],
        nameof(IsBusinessObjectSerializableConstants.DiagnosticId));
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenOnlyUsingSystemExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(IsBusinessObjectSerializableMakeSerializableCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenOnlyUsingSystemExists))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsBusinessObjectSerializableAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsBusinessObjectSerializableMakeSerializableCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSerializableAndUsingDescription, document,
        tree, $"using Csla.Serialization;{Environment.NewLine}{Environment.NewLine}[Serializable]");
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenOnlyUsingCslaSerializationExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(IsBusinessObjectSerializableMakeSerializableCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenOnlyUsingCslaSerializationExists))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsBusinessObjectSerializableAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsBusinessObjectSerializableMakeSerializableCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSerializableAndUsingDescription, document,
        tree, $"using System;{Environment.NewLine}{Environment.NewLine}[Serializable]");
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenBothUsingsExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(IsBusinessObjectSerializableMakeSerializableCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenBothUsingsExists))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsBusinessObjectSerializableAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsBusinessObjectSerializableMakeSerializableCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSerializableAndUsingDescription, document,
        tree, $"{Environment.NewLine}[Serializable]");
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenNeitherUsingsExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(IsBusinessObjectSerializableMakeSerializableCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenNeitherUsingsExists))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsBusinessObjectSerializableAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsBusinessObjectSerializableMakeSerializableCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        IsBusinessObjectSerializableMakeSerializableCodeFixConstants.AddSerializableAndUsingDescription, document,
        tree, $"using System;{Environment.NewLine}using Csla.Serialization;{Environment.NewLine}{Environment.NewLine}[Serializable]");
    }
  }
}
