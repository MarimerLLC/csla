﻿using Csla.Analyzers;
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

namespace Csla.Analyzers.Tests
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
    public async Task VerifyGetFixesWhenUsingSystemExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(IsBusinessObjectSerializableMakeSerializableCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenUsingSystemExists))}.cs");
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
        tree, new[] { $"  [Serializable]{Environment.NewLine}    " });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenUsingSystemDoesNotExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(IsBusinessObjectSerializableMakeSerializableCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenUsingSystemDoesNotExists))}.cs");
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
        tree, new[] { $"using System;{Environment.NewLine}", $"  [Serializable]{Environment.NewLine}    " });
    }
  }
}
