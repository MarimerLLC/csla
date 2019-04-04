//-----------------------------------------------------------------------
// <copyright file="BindingListTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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

    [TestMethod]
    public void VerifyDeserializingListDoesNotRaiseListChangedEvent()
    {
      var original = new MockMobileList();
      original.RaiseListChangedEvents = true;
      original.Add(new MockReadOnly(0));
      original.Add(new MockReadOnly(1));
      original.Add(new MockReadOnly(2));

      Assert.IsTrue(original.HasRaisedOnListChanged, "ListChanged event should have been raised.");

      byte[] buffer = MobileFormatter.Serialize(original);
      var deserialized = (MockMobileList)MobileFormatter.Deserialize(buffer);

      Assert.IsTrue(deserialized.Count > 0);
      Assert.IsTrue(deserialized.RaiseListChangedEvents, "Deserializing list should leave RaiseListChangedEvents unchanged.");
      Assert.IsFalse(deserialized.HasRaisedOnListChanged, "Deserializing list should not have raised ListChanged event.");

      original.RaiseListChangedEvents = false;
      byte[] buffer2 = MobileFormatter.Serialize(original);
      var deserialized2 = (MockMobileList)MobileFormatter.Deserialize(buffer2);

      Assert.IsTrue(deserialized2.Count > 0);
      Assert.IsFalse(deserialized2.RaiseListChangedEvents, "Deserializing list should leave RaiseListChangedEvents unchanged 2.");
      Assert.IsFalse(deserialized2.HasRaisedOnListChanged, "Deserializing list should not have raised ListChanged event 2.");
    }
  }
}