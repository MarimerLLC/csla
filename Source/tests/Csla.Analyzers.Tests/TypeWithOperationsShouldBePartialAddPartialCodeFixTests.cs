using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class TypeWithOperationsShouldBePartialAddPartialCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new TypeWithOperationsShouldBePartialAddPartialCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial,
        nameof(Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial));
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenClassIsNotPartial()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public sealed class A : BusinessBase<A>
        {
          [Fetch]
          private void Fetch(int id) { }
        }
        """;
      var expected =
        """
        using Csla;
        using System;

        [Serializable]
        public sealed partial class A : BusinessBase<A>
        {
          [Fetch]
          private void Fetch(int id) { }
        }
        """;
      await VerifyFixAsync(code, expected);
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenNestedClassAndContainingTypesAreNotPartial()
    {
      var code =
        """
        using Csla;
        using System;

        class Outer
        {
          public static partial class Middle
          {
            internal static class Inner
            {
              /// <summary>
              /// Business class
              /// </summary>
              [Serializable]
              public abstract class A<T> : BusinessBase<T>
                where T : A<T>
              {
                [Fetch]
                protected void Fetch(int id) { }
              }
            }
          }
        }
        """;
      var expected =
        """
        using Csla;
        using System;

        partial class Outer
        {
          public static partial class Middle
          {
            internal static partial class Inner
            {
              /// <summary>
              /// Business class
              /// </summary>
              [Serializable]
              public abstract partial class A<T> : BusinessBase<T>
                where T : A<T>
              {
                [Fetch]
                protected void Fetch(int id) { }
              }
            }
          }
        }
        """;
      await VerifyFixAsync(code, expected);
    }

    [TestMethod]
    public async Task VerifyGetFixesWhenClassHasNoModifiers()
    {
      var code =
        """
        using Csla;
        using System;

        namespace N
        {
          /// <summary>
          /// Business class
          /// </summary>
          class A : BusinessBase<A>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;
      var expected =
        """
        using Csla;
        using System;

        namespace N
        {
          /// <summary>
          /// Business class
          /// </summary>
          partial class A : BusinessBase<A>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;
      await VerifyFixAsync(code, expected);
    }

    private static async Task VerifyFixAsync(string code, string expected)
    {
      var document = TestHelpers.Create(code);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new TypeWithOperationsShouldBePartialAnalyzer());
      Assert.AreEqual(1, diagnostics.Count, nameof(diagnostics.Count));

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new TypeWithOperationsShouldBePartialAddPartialCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count);

      await TestHelpers.VerifyChangesAsync(actions,
        TypeWithOperationsShouldBePartialAddPartialCodeFixConstants.AddPartialDescription, document,
        (model, newRoot) =>
        {
          Assert.AreEqual(expected, newRoot.ToFullString());
          foreach (var typeNode in newRoot.DescendantNodes().OfType<TypeDeclarationSyntax>())
          {
            Assert.IsTrue(typeNode.Modifiers.Any(SyntaxKind.PartialKeyword), typeNode.Identifier.Text);
          }
        });

      var fixedDiagnostics = await TestHelpers.GetDiagnosticsAsync(expected, new TypeWithOperationsShouldBePartialAnalyzer());
      Assert.AreEqual(0, fixedDiagnostics.Count, nameof(fixedDiagnostics.Count));
    }
  }
}
