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
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsNotBusinessBase { }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
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
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetProperty>
  {
    public void Go()
    {
      this.GetProperty(null);
    }
  }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetPropertyConvert()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetPropertyConvert
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetPropertyConvert>
  {
    public void Go()
    {
      this.GetPropertyConvert<int, int>(null);
    }
  }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadProperty>
  {
    public void Go()
    {
      this.ReadProperty(null);
    }
  }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadPropertyConvert()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadPropertyConvert
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadPropertyConvert>
  {
    public void Go()
    {
      this.ReadPropertyConvert<int, int>(null);
    }
  }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetProperty>
  {
    public void Go()
    {
      this.LazyGetProperty<int>(null, null);
    }
  }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetPropertyAsync()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetPropertyAsync
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetPropertyAsync>
  {
    public void Go()
    {
      this.LazyGetPropertyAsync<int>(null, null);
    }
  }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadProperty>
  {
    public void Go()
    {
      this.LazyReadProperty<int>(null, null);
    }
  }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadPropertyAsync()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadPropertyAsync
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadPropertyAsync>
  {
    public void Go()
    {
      this.LazyReadPropertyAsync<int>(null, null);
    }
  }
}";
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(code);
      Assert.IsNotNull(walker.Invocation);
    }
  }
}
