using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindGetOrReadInvocationsWalkerTests
  {
    private static async Task<FindGetOrReadInvocationsWalker> GetWalker(string path)
    {
      var code = File.ReadAllText(path);
      var document = TestHelpers.Create(code);
      var root = await document.GetSyntaxRootAsync();
      var model = await document.GetSemanticModelAsync();

      return new FindGetOrReadInvocationsWalker(root, model);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsNotBusinessBase()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsNotBusinessBase))}.cs");
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod))}.cs");
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetProperty()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetProperty))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetPropertyConvert()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetPropertyConvert))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadProperty()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadProperty))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadPropertyConvert()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadPropertyConvert))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetProperty()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetProperty))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetPropertyAsync()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetPropertyAsync))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadProperty()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadProperty))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadPropertyAsync()
    {
      var walker = await FindGetOrReadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindGetOrReadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadPropertyAsync))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }
  }
}
