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
using Csla.Silverlight;

namespace cslalighttest.Serialization
{
  [TestClass]
  public class SerializationTests
  {
    [TestMethod]
    public void SerializeCriteriaSuccess()
    {
      SingleCriteria<string> criteria = new SingleCriteria<string>(typeof(SerializationTests), "success");
      var buffer = Csla.Serialization.Mobile.MobileFormatter.Serialize(criteria);

      Assert.IsNotNull(buffer);
    }

    [TestMethod]
    public void DeserializeCriteriaSuccess()
    {
      SingleCriteria<string> expected = new SingleCriteria<string>(typeof(SerializationTests), "success");
      var buffer = Csla.Serialization.Mobile.MobileFormatter.Serialize(expected);

      var actual = Csla.Serialization.Mobile.MobileFormatter.Deserialize(buffer) as SingleCriteria<string>;

      Assert.AreEqual(expected.TypeName, actual.TypeName);
      Assert.AreEqual(expected.Value, actual.Value);
    }
  }
}
