using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
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
@"using Csla;

public class A : BusinessBase<A>
{
  private A(int a) { }
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

      await TestHelpers.VerifyChangesAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.AddPublicConstructorDescription, document,
        (model, newRoot) =>
        {
          var classNode = newRoot.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>().Single();
          var classSymbol = model.GetDeclaredSymbol(classNode) as INamedTypeSymbol;
          var methodSymbol = classSymbol.GetMembers().OfType<IMethodSymbol>()
            .Single(_ => _.MethodKind == MethodKind.Constructor && _.Name == ".ctor" && _.DeclaredAccessibility == Accessibility.Public);
          Assert.IsTrue(methodSymbol.Parameters.Length == 0);
        });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExists()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  private A() { }
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

      await TestHelpers.VerifyChangesAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription, document,
        (model, newRoot) =>
        {
          var classNode = newRoot.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>().Single();
          var classSymbol = model.GetDeclaredSymbol(classNode) as INamedTypeSymbol;
          var methodSymbol = classSymbol.GetMembers().OfType<IMethodSymbol>()
            .Single(_ => _.MethodKind == MethodKind.Constructor && _.Name == ".ctor" && _.DeclaredAccessibility == Accessibility.Public);
          Assert.IsTrue(methodSymbol.Parameters.Length == 0);
        });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndLeadingTriviaExists()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  // Hey! Don't loose me! 
  private A() { }
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
      await TestHelpers.VerifyChangesAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription, document,
        (model, newRoot) =>
        {
          var classNode = newRoot.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>().Single();
          var classSymbol = model.GetDeclaredSymbol(classNode) as INamedTypeSymbol;
          var methodSymbol = classSymbol.GetMembers().OfType<IMethodSymbol>()
            .Single(_ => _.MethodKind == MethodKind.Constructor && _.Name == ".ctor" && _.DeclaredAccessibility == Accessibility.Public);
          Assert.IsTrue(methodSymbol.Parameters.Length == 0);

          Assert.IsTrue(newRoot.DescendantTrivia(_ => true, true)
            .Any(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia) && _.ToFullString().Contains("Hey! Don't loose me!")));
        });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndTrailingTriviaExists()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  private A()/* And not this either */ { }
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
      await TestHelpers.VerifyChangesAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription, document,
        (model, newRoot) =>
        {
          var classNode = newRoot.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>().Single();
          var classSymbol = model.GetDeclaredSymbol(classNode) as INamedTypeSymbol;
          var methodSymbol = classSymbol.GetMembers().OfType<IMethodSymbol>()
            .Single(_ => _.MethodKind == MethodKind.Constructor && _.Name == ".ctor" && _.DeclaredAccessibility == Accessibility.Public);
          Assert.IsTrue(methodSymbol.Parameters.Length == 0);

          Assert.IsTrue(newRoot.DescendantTrivia(_ => true, true)
            .Any(_ => _.IsKind(SyntaxKind.MultiLineCommentTrivia) && _.ToFullString().Contains("And not this either")));
        });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsWithNestedClasses()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public A() { }

  public class B
    : BusinessBase<B>
  {
    private B() { }
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
      await TestHelpers.VerifyChangesAsync(actions,
        CheckConstructorsAnalyzerPublicConstructorCodeFixConstants.UpdateNonPublicConstructorToPublicDescription, document,
        (model, newRoot) =>
        {
          var classBNode = newRoot.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>().Single(_ => _.Identifier.Text == "B");
          var classBSymbol = model.GetDeclaredSymbol(classBNode) as INamedTypeSymbol;
          var methodBSymbol = classBSymbol.GetMembers().OfType<IMethodSymbol>()
            .Single(_ => _.MethodKind == MethodKind.Constructor && _.Name == ".ctor" && _.DeclaredAccessibility == Accessibility.Public);
          Assert.IsTrue(methodBSymbol.Parameters.Length == 0);
        });
    }
  }
}