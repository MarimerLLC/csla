using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.PerfTest;

[TestClass]
public class TestToProfilableLargeFetchAndClone
{
  private static TestDIContext _testDIContext;
  private IDataPortal<TestItem> _portal;
  private TestItem _baseItem;

  [ClassInitialize]
  public static void ClassInitialize(TestContext context)
  {
    _testDIContext = TestDIContextFactory.CreateDefaultContext();
  }

  [TestInitialize]
  public async Task TestInitialize()
  {
    _portal = _testDIContext.CreateDataPortal<TestItem>();
    _baseItem = await _portal.FetchAsync();
  }

  [TestMethod]
  public async Task FetchAsync_Test()
  {
    var result = await _portal.FetchAsync();
    Assert.IsNotNull(result);
  }

  [TestMethod]
  public void Clone_Test()
  {
    var clone = _baseItem.Clone();
    Assert.IsNotNull(clone);
  }
}
