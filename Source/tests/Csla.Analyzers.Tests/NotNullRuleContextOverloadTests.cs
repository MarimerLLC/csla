using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  /// <summary>
  /// Pins how calls bind to the IRuleContext members that take a NotNullPropertyInfo,
  /// and which nullable warnings they give, as the docs describe.
  /// </summary>
  [TestClass]
  public sealed class NotNullRuleContextOverloadTests
  {
    private const string Code =
      """
      #nullable enable
      using Csla;
      using Csla.Core;
      using Csla.Rules;

      public class A : BusinessBase<A>
      {
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);
        public string Name => GetProperty(NameProperty);
        public static readonly PropertyInfo<string> OldProperty = RegisterProperty<string>(nameof(Old));
        public string? Old => GetProperty(OldProperty);
      }

      public static class Uses
      {
        public static void TypedGet(IRuleContext c) { string s = c.GetInputValue(A.NameProperty); }
        public static void OldGet(IRuleContext c) { string? s = c.GetInputValue(A.OldProperty); }
        public static void TypedOut(IRuleContext c, string v) { c.AddOutValue(A.NameProperty, v); }
        public static void TypedOutMaybeNull(IRuleContext c, string? v) { c.AddOutValue(A.NameProperty, v); }
        public static void CastOut(IRuleContext c, string? v) { c.AddOutValue((IPropertyInfo)A.NameProperty, v); }
        public static void TryOut(IRuleContext c) { if (c.TryGetInputValue(A.NameProperty, out var v)) { string s = v; } }
        public static void TryRef(IRuleContext c) { string? v = null; c.TryGetInputValue(A.NameProperty, ref v); }
      }
      """;

    private static async Task<(Compilation Compilation, SemanticModel Model, SyntaxNode Root)> CompileAsync()
    {
      var document = TestHelpers.Create(Code);
      var compilation = await document.Project.GetCompilationAsync();
      TestHelpers.AssertNoCompilerErrors(compilation);
      var tree = await document.GetSyntaxTreeAsync();
      return (compilation, compilation.GetSemanticModel(tree), await tree.GetRootAsync());
    }

    private static IMethodSymbol BoundMethod(SemanticModel model, SyntaxNode root, string useName)
    {
      var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(_ => _.Identifier.Text == useName);
      var invocation = method.DescendantNodes().OfType<InvocationExpressionSyntax>().Single();
      return (IMethodSymbol)model.GetSymbolInfo(invocation).Symbol;
    }

    private static List<string> NullableWarnings(Compilation compilation, SyntaxNode root, string useName)
    {
      var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(_ => _.Identifier.Text == useName);
      return compilation.GetDiagnostics()
        .Where(_ => _.Id.StartsWith("CS86", StringComparison.Ordinal) && method.Span.Contains(_.Location.SourceSpan))
        .Select(_ => _.Id)
        .ToList();
    }

    private static string FirstParameterType(IMethodSymbol method) => method.OriginalDefinition.Parameters[0].Type.Name;

    [DataTestMethod]
    [DataRow("TypedGet", "NotNullPropertyInfo")]
    [DataRow("OldGet", "PropertyInfo")]
    [DataRow("TypedOut", "NotNullPropertyInfo")]
    [DataRow("TypedOutMaybeNull", "NotNullPropertyInfo")]
    [DataRow("CastOut", "IPropertyInfo")]
    [DataRow("TryOut", "NotNullPropertyInfo")]
    [DataRow("TryRef", "PropertyInfo")]
    public async Task CallsBindToTheExpectedOverload(string useName, string expectedParameterType)
    {
      var (_, model, root) = await CompileAsync();

      Assert.AreEqual(expectedParameterType, FirstParameterType(BoundMethod(model, root, useName)));
    }

    [DataTestMethod]
    [DataRow("TypedGet")]
    [DataRow("OldGet")]
    [DataRow("TypedOut")]
    [DataRow("CastOut")]
    [DataRow("TryOut")]
    [DataRow("TryRef")]
    public async Task NoNullableWarning(string useName)
    {
      var (compilation, _, root) = await CompileAsync();

      var warnings = NullableWarnings(compilation, root, useName);
      Assert.AreEqual(0, warnings.Count, string.Join(", ", warnings));
    }

    [TestMethod]
    public async Task TypedAddOutValueWarnsForAMaybeNullValue()
    {
      var (compilation, _, root) = await CompileAsync();

      // T is inferred as string? from the value, so the warning is on the property argument
      Assert.AreEqual("CS8620", string.Join(", ", NullableWarnings(compilation, root, "TypedOutMaybeNull")));
    }
  }
}
