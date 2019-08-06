using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class ContainsInvocationExpressionWalkerTests
  {
    private static async Task<ContainsInvocationExpressionWalker> GetWalker(string path)
    {
      var code = File.ReadAllText(path);
      var document = TestHelpers.Create(code);
      var root = await document.GetSyntaxRootAsync();

      return new ContainsInvocationExpressionWalker(root);
    }

    [TestMethod]
    public async Task WalkWhenNodeHasNoInvocations()
    {
      var walker = await ContainsInvocationExpressionWalkerTests.GetWalker(
        $@"Targets\{nameof(ContainsInvocationExpressionWalkerTests)}\{(nameof(this.WalkWhenNodeHasNoInvocations))}.cs");
      Assert.IsFalse(walker.HasIssue);
    }

    [TestMethod]
    public async Task WalkWhenNodeHasInvocation()
    {
      var walker = await ContainsInvocationExpressionWalkerTests.GetWalker(
        $@"Targets\{nameof(ContainsInvocationExpressionWalkerTests)}\{(nameof(this.WalkWhenNodeHasInvocation))}.cs");
      Assert.IsTrue(walker.HasIssue);
    }
  }
}
