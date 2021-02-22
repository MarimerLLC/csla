using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindOperationsWithNonSerializableArgumentsAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new FindOperationsWithNonSerializableArgumentsAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual(FindOperationsWithNonSerializableArgumentsConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindOperationsWithNonSerializableArgumentsConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Design, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, nameof(FindOperationsWithNonSerializableArgumentsAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWithNotMobileObject()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsNotOperation()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Foo() { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNoArguments()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [Fetch]
  private void Fetch() { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [Fetch]
  private void Fetch(int x) { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument()
    {
      var code =
@"using Csla;

public class A { }

public class B : BusinessBase<B>
{
  [Fetch]
  private void Fetch(A x) { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments });
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithMobileObjectArgument()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A> { }

public class B : BusinessBase<B>
{
  [Fetch]
  private void Fetch(A x) { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgumentThatIsInjectable()
    {
      var code =
@"using Csla;

public class A { }

public class B : BusinessBase<B>
{
  [Fetch]
  private void Fetch([Inject] A x) { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgumentCustomType()
    {
      var code =
@"using Csla;
using System;

[Serializable]
public class A { }

public class B : BusinessBase<B>
{
  [Fetch]
  private void Fetch(A x) { }
}
";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithNoArguments()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [FetchChild]
  private void FetchChild() { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  [FetchChild]
  private void FetchChild(int x) { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument()
    {
      var code =
@"using Csla;

public class A { }

public class B : BusinessBase<B>
{
  [FetchChild]
  private void FetchChild(A x) { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }
  }
}