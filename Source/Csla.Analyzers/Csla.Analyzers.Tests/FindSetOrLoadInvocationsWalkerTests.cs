using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindSetOrLoadInvocationsWalkerTests
  {
    private static async Task<FindSetOrLoadInvocationsWalker> GetWalker(string path)
    {
      var code = File.ReadAllText(path);
      var document = TestHelpers.Create(code);
      var root = await document.GetSyntaxRootAsync();
      var model = await document.GetSemanticModelAsync();

      return new FindSetOrLoadInvocationsWalker(root, model);
    }

    [TestMethod]

    public async Task WalkWhenContainingTypeIsNotBusinessBase()
    {
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindSetOrLoadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsNotBusinessBase))}.cs");
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod()
    {
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindSetOrLoadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod))}.cs");
      Assert.IsNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetProperty()
    {
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindSetOrLoadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetProperty))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetPropertyConvert()
    {
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindSetOrLoadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetPropertyConvert))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadProperty()
    {
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindSetOrLoadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadProperty))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyAsync()
    {
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindSetOrLoadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyAsync))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyConvert()
    {
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindSetOrLoadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyConvert))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }

    [TestMethod]
    public async Task WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyMarkDirty()
    {
      var walker = await FindSetOrLoadInvocationsWalkerTests.GetWalker(
        $@"Targets\{nameof(FindSetOrLoadInvocationsWalkerTests)}\{(nameof(this.WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyMarkDirty))}.cs");
      Assert.IsNotNull(walker.Invocation);
    }
  }
}
