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
  public sealed class CheckConstructorsAnalyzerPublicConstructorCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new CheckConstructorsAnalyzerPublicConstructorCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(PublicNoArgumentConstructorIsMissingConstants.DiagnosticId, ids[0],
        nameof(PublicNoArgumentConstructorIsMissingConstants.DiagnosticId));
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenConstructorNoArgumentsDoesNotExist()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(CheckConstructorsAnalyzerPublicConstructorCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenConstructorNoArgumentsDoesNotExist))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new CheckConstructorsAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new CheckConstructorsAnalyzerPublicConstructorCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.AddPublicConstructorDescription, document,
        tree, new[] { $@"      public VerifyGetFixesWhenConstructorNoArgumentsDoesNotExist(){Environment.NewLine}        {{{Environment.NewLine}        }}{Environment.NewLine}    " });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(CheckConstructorsAnalyzerPublicConstructorCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenPrivateConstructorNoArgumentsExists))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new CheckConstructorsAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new CheckConstructorsAnalyzerPublicConstructorCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription, document,
        tree, new[] { "    public" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndLeadingTriviaExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(CheckConstructorsAnalyzerPublicConstructorCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndLeadingTriviaExists))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new CheckConstructorsAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new CheckConstructorsAnalyzerPublicConstructorCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription, document,
        tree, new[] { @"    // Hey! Don't loose me! 
        public" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndTrailingTriviaExists()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(CheckConstructorsAnalyzerPublicConstructorCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndTrailingTriviaExists))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new CheckConstructorsAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new CheckConstructorsAnalyzerPublicConstructorCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription, document,
        tree, new[] { @"    public" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndClassHasNestedClasses()
    {
      var code = File.ReadAllText(
        $@"Targets\{nameof(CheckConstructorsAnalyzerPublicConstructorCodeFixTests)}\{(nameof(this.VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndClassHasNestedClasses))}.cs");
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostic = (await TestHelpers.GetDiagnosticsAsync(code, new CheckConstructorsAnalyzer()))
        .Single(_ => _.Location.SourceSpan.End == 191 &&
          _.Location.SourceSpan.Start == 114);
      var sourceSpan = diagnostic.Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new CheckConstructorsAnalyzerPublicConstructorCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostic,
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyActionAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription, document,
        tree, new[] { @"    public" });
    }
  }
}
