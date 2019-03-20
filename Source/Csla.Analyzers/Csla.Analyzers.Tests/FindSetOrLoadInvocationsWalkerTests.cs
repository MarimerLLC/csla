using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindSetOrLoadInvocationsWalkerTests
  {
    private static async Task<FindSetOrLoadInvocationsWalker> GetWalker(string code)
    {
      var document = TestHelpers.Create(code);
      var root = await document.GetSyntaxRootAsync();
      var model = await document.GetSemanticModelAsync();

      return new FindSetOrLoadInvocationsWalker(root, model);
    }

    [TestMethod]

    public async Task WalkWhenContainingTypeIsNotBusinessBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalker
{
  public class WalkWhenContainingTypeIsNotBusinessBase { }
}";
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod>
  {
    public void Go()
    {
      this.GetHashCode();
    }
  }
}";
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetProperty>
  {
    public void Go()
    {
      this.SetProperty(null, null);
    }
  }
}";
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetPropertyConvert()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetPropertyConvert
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetPropertyConvert>
  {
    public void Go()
    {
      this.SetPropertyConvert<int, int>(null, 0);
    }
  }
}";
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadProperty>
  {
    public void Go()
    {
      this.LoadProperty(null, null);
    }
  }
}";
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyAsync()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyAsync
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyAsync>
  {
    public void Go()
    {
      this.LoadPropertyAsync<int>(null, null);
    }
  }
}";
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyConvert()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyConvert
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyConvert>
  {
    public void Go()
    {
      this.LoadPropertyConvert<int, int>(null, 0);
    }
  }
}";
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyMarkDirty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyMarkDirty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyMarkDirty>
  {
    public void Go()
    {
      this.LoadPropertyMarkDirty(null, null);
    }
  }
}";
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }
  }
}