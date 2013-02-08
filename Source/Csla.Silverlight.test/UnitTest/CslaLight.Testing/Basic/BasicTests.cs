using System;
using UnitDriven;


#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 

namespace cslalighttest.Basic
{
  [TestClass]
  public class BasicTests
  {
    [TestMethod]
    public void CancelEditRoundTripsIsDirty()
    {
      var obj = new Root();
      obj.Clean();
      Assert.IsFalse(obj.IsDirty, "Before edit");
      obj.BeginEdit();
      Assert.IsFalse(obj.IsDirty, "After begin edit");
      obj.CancelEdit();
      Assert.IsFalse(obj.IsDirty, "After cancel edit");
    }


  }
}
