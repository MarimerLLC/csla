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
  public sealed class CheckConstructorsAnalyzerPublicConstructorCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new CheckConstructorsAnalyzerPublicConstructorCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing, ids[0],
        nameof(Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing));
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenConstructorNoArgumentsDoesNotExist()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerPublicConstructorCodeFixTestss
{
  public class VerifyGetFixesWhenConstructorNoArgumentsDoesNotExist : BusinessBase<VerifyGetFixesWhenConstructorNoArgumentsDoesNotExist>
  {
    private VerifyGetFixesWhenConstructorNoArgumentsDoesNotExist(int a) { }
  }
}";
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
        tree, new[] { "public VerifyGetFixesWhenConstructorNoArgumentsDoesNotExist()" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExists()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerPublicConstructorCodeFixTestss
{
  public class VerifyGetFixesWhenPrivateConstructorNoArgumentsExists : BusinessBase<VerifyGetFixesWhenPrivateConstructorNoArgumentsExists>
  {
    private VerifyGetFixesWhenPrivateConstructorNoArgumentsExists() { }
  }
}";
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
        tree, new[] { "public" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndLeadingTriviaExists()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerPublicConstructorCodeFixTests
{
  public class VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndLeadingTriviaExists
    : BusinessBase<VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndLeadingTriviaExists>
  {
    // Hey! Don't loose me! 
    private VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndLeadingTriviaExists() { }
  }
}";
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
        tree, new[] { "// Hey! Don't loose me!", "public" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndTrailingTriviaExists()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerPublicConstructorCodeFixTests
{
  public class VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndTrailingTriviaExists
    : BusinessBase<VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndTrailingTriviaExists>
  {
    private VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndTrailingTriviaExists()/* And not this either */ { }
  }
}";
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
        tree, new[] { "public" });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsWithNestedClasses()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerPublicConstructorCodeFixTestss
{
  public class VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsWithNestedClasses
    : BusinessBase<VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsWithNestedClasses>
  {
    private VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsWithNestedClasses() { }

    public class NestedClass
      : BusinessBase<NestedClass>
    {
      private NestedClass() { }
    }
  }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new CheckConstructorsAnalyzer());

      foreach(var diagnostic in diagnostics)
      {
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
          tree, new[] { "public" });
      }
    }
  }
}
