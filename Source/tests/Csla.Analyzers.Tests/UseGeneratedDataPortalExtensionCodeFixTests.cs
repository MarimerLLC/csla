using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class UseGeneratedDataPortalExtensionCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new UseGeneratedDataPortalExtensionCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension, ids[0]);
    }

    [TestMethod]
    public async Task VerifyGetFixesReplacesCall()
    {
      var code =
        """
        using Csla;
        using System;
        using System.Threading.Tasks;

        [DataPortalExtensions]
        [Serializable]
        public partial class A : BusinessBase<A>
        {
          [Fetch]
          private void GetById(int id) { }
        }

        public class Consumer
        {
          public Task<A> Run(IDataPortal<A> portal) => portal.FetchAsync(1);
        }
        """;
      var expected = code.Replace("portal.FetchAsync(1)", "portal.GetByIdAsync(1)");
      await VerifyFixAsync(code, "GetByIdAsync", expected, expectedActionCount: 1);
    }

    [TestMethod]
    public async Task VerifyGetFixesAddsUsingForBusinessNamespace()
    {
      var code =
        """
        using Csla;
        using System;
        using System.Threading.Tasks;

        namespace Models
        {
          [DataPortalExtensions(Prefix = "Portal")]
          [Serializable]
          public partial class A : BusinessBase<A>
          {
            [FetchChild]
            private void Load(int id, string name) { }
          }
        }

        namespace Consumers
        {
          public class Consumer
          {
            public Models.A Run(IChildDataPortal<Models.A> portal) => portal.FetchChild(1, "x");
          }
        }
        """;
      var fixedCode = await ApplyFixAsync(code, "PortalLoad", expectedActionCount: 1);

      StringAssert.Contains(fixedCode, "portal.PortalLoad(1, \"x\")");
      StringAssert.Contains(fixedCode, "using Models;");
    }

    [TestMethod]
    public async Task VerifyGetFixesOffersEachMatchingExtension()
    {
      var code =
        """
        using Csla;
        using System;
        using System.Threading.Tasks;

        [DataPortalExtensions]
        [Serializable]
        public partial class A : BusinessBase<A>
        {
          [Fetch]
          private void GetById(int id) { }

          [Fetch]
          private void GetByNumber(long number) { }
        }

        public class Consumer
        {
          public Task<A> Run(IDataPortal<A> portal) => portal.FetchAsync(1);
        }
        """;
      var fixedCode = await ApplyFixAsync(code, "GetByNumberAsync", expectedActionCount: 2);

      StringAssert.Contains(fixedCode, "portal.GetByNumberAsync(1)");
    }

    private static async Task VerifyFixAsync(string code, string extensionName, string expected, int expectedActionCount)
    {
      var fixedCode = await ApplyFixAsync(code, extensionName, expectedActionCount);
      Assert.AreEqual(expected, fixedCode);
    }

    private static async Task<string> ApplyFixAsync(string code, string extensionName, int expectedActionCount)
    {
      var document = TestHelpers.Create(code);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new UseGeneratedDataPortalExtensionAnalyzer());
      Assert.AreEqual(1, diagnostics.Count, nameof(diagnostics.Count));

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (a, _) => { actions.Add(a); });

      var fix = new UseGeneratedDataPortalExtensionCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0],
        codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(expectedActionCount, actions.Count, nameof(actions.Count));

      string result = null;
      await TestHelpers.VerifyChangesAsync(actions,
        string.Format(UseGeneratedDataPortalExtensionCodeFixConstants.UseExtensionDescription, extensionName), document,
        (_, newRoot) => result = newRoot.ToFullString());
      return result;
    }
  }
}
