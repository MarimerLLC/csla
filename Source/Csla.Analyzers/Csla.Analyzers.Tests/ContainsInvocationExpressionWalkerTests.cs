using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class ContainsInvocationExpressionWalkerTests
  {
    private static async Task<ContainsInvocationExpressionWalker> GetWalker(string code)
    {
      var document = TestHelpers.Create(code);
      var root = await document.GetSyntaxRootAsync();

      return new ContainsInvocationExpressionWalker(root);
    }

    [TestMethod]
    public async Task WalkWhenNodeHasNoInvocations()
    {
      var code = "public class WalkWhenNodeHasNoInvocations { }";
      var walker = await GetWalker(code);
      Assert.IsFalse(walker.HasIssue);
    }

    [TestMethod]
    public async Task WalkWhenNodeHasInvocation()
    {
      var code =
@"public class WalkWhenNodeHasInvocation
{
  public void Go() => this.GetHashCode();
}";
      var walker = await GetWalker(code);
      Assert.IsTrue(walker.HasIssue);
    }
  }
}
