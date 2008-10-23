using Csla.Core;
using Csla.Serialization.Mobile;
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


namespace cslalighttest.Serialization
{
  [TestClass]
  public class BindingListTests
  {  
    [TestMethod]
    public void VerifyAllowEdit()
    {
      var expected = new MobileBindingList<MockReadOnly>();
      expected.AllowEdit = true;
      byte[] buffer = MobileFormatter.Serialize(expected);

      var actual = (MobileBindingList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.AllowEdit, actual.AllowEdit);
    }

    [TestMethod]
    public void VerifyAllowRemove()
    {
      var expected = new MobileBindingList<MockReadOnly>();
      expected.AllowRemove = true;
      byte[] buffer = MobileFormatter.Serialize(expected);

      var actual = (MobileBindingList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.AllowRemove, actual.AllowRemove);
    }

    [TestMethod]
    public void VerifyAllowNew()
    {
      var expected = new MobileBindingList<MockReadOnly>();
      expected.AllowNew = true;
      byte[] buffer = MobileFormatter.Serialize(expected);

      var actual = (MobileBindingList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.AllowNew, actual.AllowNew);
    }

    [TestMethod]
    public void VerifyRaiseListChangedEvents()
    {
      var expected = new MobileBindingList<MockReadOnly>();
      expected.RaiseListChangedEvents = false;
      byte[] buffer = MobileFormatter.Serialize(expected);

      var actual = (MobileBindingList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.RaiseListChangedEvents, actual.RaiseListChangedEvents);
    }
  }
}
