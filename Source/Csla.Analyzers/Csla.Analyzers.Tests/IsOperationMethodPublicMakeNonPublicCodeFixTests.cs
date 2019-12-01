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
  public sealed class IsOperationMethodPublicMakeNonPublicCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.IsOperationMethodPublic,
        nameof(Constants.AnalyzerIdentifiers.IsOperationMethodPublic));
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenClassIsNotSealed()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public class A : BusinessBase<A>
{
  [Fetch]
  public void Fetch() { }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsOperationMethodPublicAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(3, actions.Count);

      await TestHelpers.VerifyChangesAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.PrivateDescription, document,
        (model, newRoot) =>
        {
          var methodNode = newRoot.DescendantNodes(_ => true).OfType<MethodDeclarationSyntax>().Single();
          var methodSymbol = model.GetDeclaredSymbol(methodNode) as IMethodSymbol;
          Assert.AreEqual(Accessibility.Private, methodSymbol.DeclaredAccessibility);
        });
      await TestHelpers.VerifyChangesAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.ProtectedDescription, document,
        (model, newRoot) =>
        {
          var methodNode = newRoot.DescendantNodes(_ => true).OfType<MethodDeclarationSyntax>().Single();
          var methodSymbol = model.GetDeclaredSymbol(methodNode) as IMethodSymbol;
          Assert.AreEqual(Accessibility.Protected, methodSymbol.DeclaredAccessibility);
        });
      await TestHelpers.VerifyChangesAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.InternalDescription, document,
        (model, newRoot) =>
        {
          var methodNode = newRoot.DescendantNodes(_ => true).OfType<MethodDeclarationSyntax>().Single();
          var methodSymbol = model.GetDeclaredSymbol(methodNode) as IMethodSymbol;
          Assert.AreEqual(Accessibility.Internal, methodSymbol.DeclaredAccessibility);
        });
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenClassIsSealed()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public sealed class A : BusinessBase<A>
{
  [Fetch]
  public void Fetch() { }
}";
      var document = TestHelpers.Create(code);
      var tree = await document.GetSyntaxTreeAsync();
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsOperationMethodPublicAnalyzer());
      var sourceSpan = diagnostics[0].Location.SourceSpan;

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      await TestHelpers.VerifyChangesAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.PrivateDescription, document,
        (model, newRoot) =>
        {
          var methodNode = newRoot.DescendantNodes(_ => true).OfType<MethodDeclarationSyntax>().Single();
          var methodSymbol = model.GetDeclaredSymbol(methodNode) as IMethodSymbol;
          Assert.AreEqual(Accessibility.Private, methodSymbol.DeclaredAccessibility);
        });
      await TestHelpers.VerifyChangesAsync(actions,
        IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.InternalDescription, document,
        (model, newRoot) =>
        {
          var methodNode = newRoot.DescendantNodes(_ => true).OfType<MethodDeclarationSyntax>().Single();
          var methodSymbol = model.GetDeclaredSymbol(methodNode) as IMethodSymbol;
          Assert.AreEqual(Accessibility.Internal, methodSymbol.DeclaredAccessibility);
        });
    }
  }
}