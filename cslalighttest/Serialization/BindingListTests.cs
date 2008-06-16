using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Core;
using Csla.Serialization.Mobile;

namespace cslalighttest.Serialization
{
  [TestClass]
  public class BindingListTests
  {  
    [TestMethod]
    public void VerifyAllowEdit()
    {
      var expected = new MobileList<MockReadOnly>();
      expected.AllowEdit = true;
      byte[] buffer = MobileFormatter.Serialize(expected);

      var actual = (MobileList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.AllowEdit, actual.AllowEdit);
    }

    [TestMethod]
    public void VerifyAllowRemove()
    {
      var expected = new MobileList<MockReadOnly>();
      expected.AllowRemove = true;
      byte[] buffer = MobileFormatter.Serialize(expected);

      var actual = (MobileList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.AllowRemove, actual.AllowRemove);
    }

    [TestMethod]
    public void VerifyAllowNew()
    {
      var expected = new MobileList<MockReadOnly>();
      expected.AllowNew = true;
      byte[] buffer = MobileFormatter.Serialize(expected);

      var actual = (MobileList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.AllowNew, actual.AllowNew);
    }

    [TestMethod]
    public void VerifyRaiseListChangedEvents()
    {
      var expected = new MobileList<MockReadOnly>();
      expected.RaiseListChangedEvents = false;
      byte[] buffer = MobileFormatter.Serialize(expected);

      var actual = (MobileList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.RaiseListChangedEvents, actual.RaiseListChangedEvents);
    }
  }
}
