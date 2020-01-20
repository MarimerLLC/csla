using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class MultipleDataAccessTest
  {
    [TestMethod]
    public void TestDpFetch()
    {
      var result = MultipleDataAccess.GetMultiple(1);
      Assert.AreEqual(1, result.Id);
      Assert.AreEqual("abc", result.Name);

      result = MultipleDataAccess.GetMultiple();
      Assert.AreEqual(int.MaxValue, result.Id);
      Assert.AreEqual(string.Empty, result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(AmbiguousMatchException), "Should throw 'AmbiguousMatchException'")]
    public void TestDpFetchNullable()
    {
      try
      {
        var result = MultipleDataAccess.GetMultiple(1, default(bool?));
      }
      catch (DataPortalException ex)
      {
        throw ex.GetBaseException();
      }
    }
  }
}
