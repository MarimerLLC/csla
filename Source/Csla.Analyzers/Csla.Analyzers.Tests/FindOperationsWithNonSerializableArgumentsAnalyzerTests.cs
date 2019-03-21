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
    }

    [TestMethod]
    public async Task AnalyzeWithNotMobileObject()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithNotMobileObject { }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsNotOperation()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsNotOperation
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsNotOperation>
  {
    public void Foo() { }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNoArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsRootOperationWithNoArguments
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsRootOperationWithNoArguments>
  {
    private void DataPortal_Fetch() { }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument>
  {
    private void DataPortal_Fetch(int x) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class NonSerializableClass { }

  public class AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument>
  {
    private void DataPortal_Fetch(NonSerializableClass x) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments });
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgumentCustomType()
    {
      var code =
@"using System;

namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  [Serializable]
  public class SerializableClass { }

  public class AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument>
  {
    private void DataPortal_Fetch(SerializableClass x) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithNoArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsChildOperationWithNoArguments
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsChildOperationWithNoArguments>
  {
    private void Child_Fetch() { }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument>
  {
    private void Child_Fetch(int x) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class NonSerializableClass { }

  public class AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument>
  {
    private void Child_Fetch(NonSerializableClass x) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, Array.Empty<string>());
    }
  }
}