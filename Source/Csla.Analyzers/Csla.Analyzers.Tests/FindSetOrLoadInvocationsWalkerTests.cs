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
      var code = "public class A { }";
      var walker = await GetWalker(code);
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.GetHashCode();
}";
      var walker = await GetWalker(code);
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.SetProperty(null, null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetPropertyConvert()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.SetPropertyConvert<int, int>(null, 0);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.LoadProperty(null, null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyAsync()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.LoadPropertyAsync<int>(null, null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyConvert()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.LoadPropertyConvert<int, int>(null, 0);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyMarkDirty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.LoadPropertyMarkDirty(null, null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }
  }
}