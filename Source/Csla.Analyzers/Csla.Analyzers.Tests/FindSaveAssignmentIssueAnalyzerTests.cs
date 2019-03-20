using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindSaveAssignmentIssueAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new FindSaveAssignmentIssueAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);

      var saveDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.FindSaveAssignmentIssue);
      Assert.AreEqual(FindSaveAssignmentIssueAnalyzerConstants.Title, saveDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindSaveAssignmentIssueAnalyzerConstants.Message, saveDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, saveDiagnostic.Category, 
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, saveDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));

      var saveAsyncDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue);
      Assert.AreEqual(FindSaveAsyncAssignmentIssueAnalyzerConstants.Title, saveAsyncDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindSaveAsyncAssignmentIssueAnalyzerConstants.Message, saveAsyncDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, saveAsyncDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, saveAsyncDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase
  {
    public AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase Save() { return null; }
  }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBaseCaller
  {
    public void Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase();
      x.Save();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase()
    {
      var code =
@"using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase
  {
    public async Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase> SaveAsync()
    {
      return await Task.FromResult<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase>(null);
    }
  }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBaseCaller
  {
    public async Task Call()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase();
      await x.SaveAsync();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssignedCaller
  {
    public void Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned();
      x = x.Save();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned()
    {
      var code =
@"using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned>
  { }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssignedCaller
  {
    public async Task Call()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned();
      x = await x.SaveAsync();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssignedCaller
  {
    public void Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned();
      x.Save();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.FindSaveAssignmentIssue });
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned()
    {
      var code =
@"using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned>
  { }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssignedCaller
  {
    public async Task Call()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned();
      await x.SaveAsync();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue });
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedCaller
  {
    public AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned();
      return x.Save();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned()
    {
      var code =
@"using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned>
  { }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedCaller
  {
    public async Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned> AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedCall()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned();
      return await x.SaveAsync();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda()
    {
      var code =
@"using System;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaCaller
  {
    public void Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda();

      this.Run(() => x.Save());
    }

    private AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda Run(
      Func<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda> code)
    {
      return code();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda()
    {
      var code =
@"using System;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda>
  { }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaCaller
  {
    public async Task Call()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda();

      await this.Run(async () => await x.SaveAsync());
    }

    private async Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda> Run(
      Func<Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda>> code)
    {
      return await code();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock()
    {
      var code =
@"using System;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlockCaller
  {
    public void Call()
    {
      this.Run(() =>
      {
        var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock();
        return x.Save();
      });
    }

    private AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock Run(
      Func<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock> code)
    {
      return code();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock()
    {
      var code =
@"using System;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock>
  { }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlockCaller
  {
    public async Task Call()
    {
      await this.Run(async () =>
      {
        var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock();
        return await x.SaveAsync();
      });
    }

    private async Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock> Run(
      Func<Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock>> code)
    {
      return await code();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectWithinItself()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnABusinessObjectWithinItself
    : BusinessBase<AnalyzeWhenSaveIsCalledOnABusinessObjectWithinItself>
  {
    public void Foo()
    {
      Save();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectWithinItself()
    {
      var code =
@"using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectWithinItself
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectWithinItself>
  {
    public async Task Foo()
    {
      await SaveAsync();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectAsThis()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnABusinessObjectAsThis
    : BusinessBase<AnalyzeWhenSaveIsCalledOnABusinessObjectAsThis>
  {
    public void Foo()
    {
      this.Save();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsThis()
    {
      var code =
@"using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsThis
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsThis>
  {
    public async Task Foo()
    {
      await this.SaveAsync();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectAsBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnABusinessObjectAsBase
    : BusinessBase<AnalyzeWhenSaveIsCalledOnABusinessObjectAsBase>
  {
    public void Foo()
    {
      base.Save();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsBase()
    {
      var code =
@"using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsBase
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsBase>
  {
    public async Task Foo()
    {
      await base.SaveAsync();
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }
  }
}
