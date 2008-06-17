using System;
using System.Threading;
using Csla.Testing.Business.EditableRootTests;
using cslalighttest.Engine;

namespace cslalighttest.Stereotypes
{
  [TestClass]
  public class EditableRootTests
  {
    [TestMethod]
    public void CanConstructTest()
    {
      MockEditableRoot root = new MockEditableRoot();
    }

    [TestMethod]
    public void TestCreateNew(AsyncTestContext context)
    {
      Guid expectedId = new Guid("{7E7127CF-F22C-4903-BDCE-1714C81A9E89}");

      MockEditableRoot.CreateNew((o, e) =>
      {
        MockEditableRoot actual = o;
        Exception error = e;

        context.Assert.IsNull(error);
        context.Assert.IsNotNull(actual);
        context.Assert.AreEqual(expectedId, actual.Id);
      });
    }
  }
}
