using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class MultipleDataAccessTest
  {
    private TestDIContext _testDIContext;

    [TestInitialize]
    public void TestInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void TestDpFetch()
    {
      IDataPortal<MultipleDataAccess> dataPortal = _testDIContext.CreateDataPortal<MultipleDataAccess>();

      var result = dataPortal.Fetch(1);
      Assert.AreEqual(1, result.Id);
      Assert.AreEqual("abc", result.Name);

      result = dataPortal.Fetch();
      Assert.AreEqual(int.MaxValue, result.Id);
      Assert.AreEqual(string.Empty, result.Name);

      result = dataPortal.Fetch(new List<int?>());
      Assert.AreEqual("Fetch(List<int?> values)", TestResults.GetResult("Method"));

      result = dataPortal.Fetch(new List<DateTime?>());
      Assert.AreEqual("Fetch(List<DateTime?> values)", TestResults.GetResult("Method"));
    }

    [TestMethod]
    [ExpectedException(typeof(AmbiguousMatchException), "Should throw 'AmbiguousMatchException'")]
    public void TestDpFetchNullable()
    {
      IDataPortal<MultipleDataAccess> dataPortal = _testDIContext.CreateDataPortal<MultipleDataAccess>();

      try
      {
        var result = dataPortal.Fetch(1, default(bool?));
      }
      catch (DataPortalException ex)
      {
        throw ex.GetBaseException();
      }
    }
  }
}
