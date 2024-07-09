using Csla.Analyzers.ManagedBackingFieldUsesNameof;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class EvaluateManagedBackingFieldsNameofCodeFixTests
  {

    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      var fix = new EvaluateManagedBackingFieldsNameofCodeFix();
      var ids = fix.FixableDiagnosticIds.ToList();

      Assert.AreEqual(1, ids.Count, nameof(ids.Count));
      Assert.AreEqual(ids[0], Constants.AnalyzerIdentifiers.EvaluateManagedBackingFieldsNameof,
        nameof(Constants.AnalyzerIdentifiers.EvaluateManagedBackingFieldsNameof));
    }
    [TestMethod]
    public async Task VerifyCodeFixReplacesLambdaWithNameof()
    {
      var testCode = """

        using Csla;

        public class TestClass : BusinessBase<TestClass>
        {
          private static readonly PropertyInfo<string> TestProperty = RegisterProperty<string>(c => c.TestName, "Test Name");
          public string TestName
          {
            get { return GetProperty(TestProperty); }
            set { SetProperty(TestProperty, value); }
          }
        }
        """;

      var document = TestHelpers.Create(testCode);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(testCode, new EvaluateManagedBackingFieldsNameofAnalyzer());

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (action, _) => actions.Add(action));

      var fix = new EvaluateManagedBackingFieldsNameofCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0], codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyChangesAsync(actions,
        "Use nameof", document,
        (model, newRoot) =>
        {
          var fieldNode = newRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().First();
          var fieldInitializer = fieldNode.Declaration.Variables.First().Initializer.Value.ToString();
          Assert.IsTrue(fieldInitializer.Contains("nameof(TestName)"), "Field initializer should contain nameof expression.");
        });
    }
    [TestMethod]
    public async Task VerifyCodeFixReplacesConstantWithNameof()
    {
      var testCode = """

        using Csla;

        public class TestClass : BusinessBase<TestClass>
        {
          public static string TestPropertyName = "TypeName";
          private static readonly PropertyInfo<string> TestProperty = RegisterProperty<string>(TestPropertyName, "Test Name");
          public string TestName
          {
            get { return GetProperty(TestProperty); }
            set { SetProperty(TestProperty, value); }
          }
        }
        """;

      var document = TestHelpers.Create(testCode);
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(testCode, new EvaluateManagedBackingFieldsNameofAnalyzer());

      var actions = new List<CodeAction>();
      var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
        (action, _) => actions.Add(action));

      var fix = new EvaluateManagedBackingFieldsNameofCodeFix();
      var codeFixContext = new CodeFixContext(document, diagnostics[0], codeActionRegistration, new CancellationToken(false));
      await fix.RegisterCodeFixesAsync(codeFixContext);

      Assert.AreEqual(1, actions.Count, nameof(actions.Count));

      await TestHelpers.VerifyChangesAsync(actions,
        "Use nameof", document,
        (model, newRoot) =>
        {
          var fieldNode = newRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().Last();
          var fieldInitializer = fieldNode.Declaration.Variables.First().Initializer.Value.ToString();
          Assert.IsTrue(fieldInitializer.Contains("nameof(TestPropertyName)"), "Field initializer should contain nameof expression.");
        });
    }
  }
}
