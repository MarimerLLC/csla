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
using Csla.Serialization.Mobile;
using Example.Business;

namespace cslalighttest.Serialization
{
  [TestClass]
  public class SerializationTests
  {
    [TestMethod]
    public void SerializeCriteriaSuccess()
    {
      var criteria = new SingleCriteria<string>(typeof(SerializationTests), "success");
      var actual = MobileFormatter.Serialize(criteria);

      Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void DeserializeCriteriaSuccess()
    {
      var expected = new SingleCriteria<string>(typeof(SerializationTests), "success");
      var buffer = MobileFormatter.Serialize(expected);
      
      var actual = MobileFormatter.Deserialize(buffer) as SingleCriteria<string>;

      Assert.AreEqual(expected.TypeName, actual.TypeName);
      Assert.AreEqual(expected.Value, actual.Value);
    }

    [TestMethod]
    public void BusinessObjectWithoutChildList()
    {
      DateTime birthdate = new DateTime(1980, 2, 3);

      Person expected = new Person();
      expected.Name = "test";
      expected.Unserialized = "should be null";
      expected.Birthdate = birthdate;

      var buffer = MobileFormatter.Serialize(expected);
      var actual = (Person)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expected.Name, actual.Name);
      Assert.AreEqual(expected.Birthdate, actual.Birthdate);
      Assert.AreEqual(expected.Age, actual.Age);

      Assert.AreEqual(actual.Unserialized, string.Empty);
      Assert.IsNull(actual.Addresses);
      Assert.IsNull(actual.PrimaryAddress);

      Assert.IsNotNull(expected.Unserialized);
      Assert.IsNull(expected.Addresses);
      Assert.IsNull(expected.PrimaryAddress);
    }

    [TestMethod]
    public void BusinessObjectWithChildList()
    {
      DateTime birthdate = new DateTime(1980, 2, 3);

      Person expectedPerson = new Person();
      expectedPerson.Name = "test";
      expectedPerson.Unserialized = "should be null";
      expectedPerson.Birthdate = birthdate;

      AddressList expectedAddressList = new AddressList();
      expectedPerson.Addresses = expectedAddressList;

      Address expectedA1 = (Address)expectedAddressList.AddNew();
      expectedA1.City = "Minneapolis";
      expectedA1.ZipCode = "55414";

      Address expectedA2 = (Address)expectedAddressList.AddNew();
      expectedA2.City = "Eden Prairie";
      expectedA2.ZipCode = "55403";

      expectedPerson.PrimaryAddress = expectedAddressList[1];

      var buffer = MobileFormatter.Serialize(expectedPerson);
      var actualPerson = (Person)MobileFormatter.Deserialize(buffer);

      Assert.AreEqual(expectedPerson.Name, actualPerson.Name);
      Assert.AreEqual(expectedPerson.Birthdate, actualPerson.Birthdate);
      Assert.AreEqual(expectedPerson.Age, actualPerson.Age);
      Assert.AreEqual(actualPerson.Unserialized, string.Empty);
      Assert.IsNotNull(expectedPerson.Unserialized);
      Assert.AreSame(expectedPerson.PrimaryAddress, expectedAddressList[1]);
      
      var actualAddressList = actualPerson.Addresses;
      Assert.IsNotNull(actualAddressList);
      Assert.AreEqual(expectedAddressList.Count, actualAddressList.Count);

      Assert.AreEqual(expectedAddressList[0].City, actualAddressList[0].City);
      Assert.AreEqual(expectedAddressList[0].ZipCode, actualAddressList[0].ZipCode);

      Assert.AreEqual(expectedAddressList[1].City, actualAddressList[1].City);
      Assert.AreEqual(expectedAddressList[1].ZipCode, actualAddressList[1].ZipCode);

      Assert.AreSame(actualPerson.PrimaryAddress, actualAddressList[1]);
    }
  }
}
