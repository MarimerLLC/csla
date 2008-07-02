using System;
using System.Threading;
using Csla.Testing.Business.EditableRootTests;
using cslalighttest.Engine;
using System.Diagnostics;
using Csla.Testing.Business.ReadOnlyTest;

namespace cslalighttest.Stereotypes
{
  [TestClass]
  public class ReadOnlyTests
  {
    [TestSetup]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      Csla.DataPortalClient.WcfProxy.DefaultUrl = ReadOnlyPerson.DataPortalUrl;
    }

    [TestMethod]
    public void TestReadOnlyFetch(AsyncTestContext context)
    {
      Guid id = Guid.NewGuid();
      ReadOnlyPerson.Fetch(
        id,
        (actual, error) =>
        {
          context.Assert.AreEqual(id, actual.Id);
          context.Assert.AreEqual(id.ToString(), actual.Name);
          context.Assert.AreEqual(new DateTime(1980, 1, 1), actual.Birthdate);
          context.Assert.Success();
        });
    }
  }
}
