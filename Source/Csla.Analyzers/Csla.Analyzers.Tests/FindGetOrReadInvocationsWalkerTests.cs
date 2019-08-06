using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindGetOrReadInvocationsWalkerTests
  {
    private static async Task<FindGetOrReadInvocationsWalker> GetWalker(string code)
    {
      var document = TestHelpers.Create(code);
      var root = await document.GetSyntaxRootAsync();
      var model = await document.GetSemanticModelAsync();

      return new FindGetOrReadInvocationsWalker(root, model);
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
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.GetProperty(null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetPropertyConvert()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.GetPropertyConvert<int, int>(null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.ReadProperty(null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadPropertyConvert()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.ReadPropertyConvert<int, int>(null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.LazyGetProperty<int>(null, null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetPropertyAsync()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.LazyGetPropertyAsync<int>(null, null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.LazyReadProperty<int>(null, null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadPropertyAsync()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public void Go() => this.LazyReadPropertyAsync<int>(null, null);
}";
      var walker = await GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }
  }
}