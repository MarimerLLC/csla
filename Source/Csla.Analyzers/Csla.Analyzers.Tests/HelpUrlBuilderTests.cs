using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class HelpUrlBuilderTests
  {
    [TestMethod]
    public void Build()
    {
      var url = HelpUrlBuilder.Build("a", "b");
      Assert.AreEqual("https://github.com/MarimerLLC/csla/tree/master/docs/analyzers/a-b.md", url);
    }
  }
}
