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
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.FindSaveAssignmentIssue, nameof(FindSaveAssignmentIssueAnalyzer)),
        saveDiagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));

      var saveAsyncDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue);
      Assert.AreEqual(FindSaveAsyncAssignmentIssueAnalyzerConstants.Title, saveAsyncDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(FindSaveAsyncAssignmentIssueAnalyzerConstants.Message, saveAsyncDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, saveAsyncDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, saveAsyncDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue, nameof(FindSaveAssignmentIssueAnalyzer)),
        saveAsyncDiagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase()
    {
      var code =
@"public class A
{
  public A Save() => null;
}

public class B
{
  public void Call()
  {
    var x = new A();
    x.Save();
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

public class A
{
  public async Task<A> SaveAsync() => await Task.FromResult<A>(null);
}

public class B
{
  public async Task Call()
  {
    var x = new A();
    await x.SaveAsync();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A> { }

public class B
{
  public void Call()
  {
    var x = new A();
    x = x.Save();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A> { }

public class B
{
  public async Task Call()
  {
    var x = new A();
    x = await x.SaveAsync();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A> { }

public class B
{
  public void Call()
  {
    var x = new A();
    x.Save();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.FindSaveAssignmentIssue });
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A> { }

public class B
{
  public async Task Call()
  {
    var x = new A();
    await x.SaveAsync();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue });
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>{ }

public class B
{
  public A Call()
  {
    var x = new A();
    return x.Save();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturned()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A>{ }

public class B
{
  public async Task<A> Call()
  {
    var x = new A();
    return await x.SaveAsync();
  }
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda()
    {
      var code =
@"using Csla;
using System;

public class A : BusinessBase<A>{ }

public class B
{
  public void Call()
  {
    var x = new A();

    this.Run(() => x.Save());
  }

  private B Run(Func<B> code) => code();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda()
    {
      var code =
@"using Csla;
using System;
using System.Threading.Tasks;

public class A : BusinessBase<A>{ }

public class B
{
  public async Task Call()
  {
    var x = new A();

    await this.Run(async () => await x.SaveAsync());
  }

  private async Task<A> Run(Func<Task<A>> code) => await code();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock()
    {
      var code =
@"using Csla;
using System;

public class A : BusinessBase<A>{ }

public class B
{
  public void Call()
  {
    this.Run(() =>
    {
      var x = new A();
      return x.Save();
    });
  }

  private A Run(Func<A> code) => code();
}
";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock()
    {
      var code =
@"using Csla;
using System;
using System.Threading.Tasks;

public class A : BusinessBase<A>{ }

public class B
{
  public async Task Call()
  {
    await this.Run(async () =>
    {
      var x = new A();
      return await x.SaveAsync();
    });
  }

  private async Task<A> Run(Func<Task<A>> code) => await code();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectWithinItself()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Foo() => Save();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectWithinItself()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A>
{
  public async Task Foo() => await SaveAsync();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectAsThis()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Foo() => this.Save();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsThis()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A>
{
  public async Task Foo() => this.SaveAsync();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveIsCalledOnABusinessObjectAsBase()
    {
      var code =
@"using Csla;

public class A
  : BusinessBase<A>
{
  public void Foo() => base.Save();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsBase()
    {
      var code =
@"using Csla;
using System.Threading.Tasks;

public class A : BusinessBase<A>
{
  public async Task Foo() => await base.SaveAsync();
}";
      await TestHelpers.RunAnalysisAsync<FindSaveAssignmentIssueAnalyzer>(
        code, Array.Empty<string>());
    }
  }
}