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
using Csla;
using Csla.Silverlight;
using Csla.Serialization.Mobile;

namespace cslalighttest.Serialization
{
  [TestClass]
  public class SerializationTests
  {
    [TestMethod]
    public void SerializeCriteriaSuccess()
    {
      var criteria = new SingleCriteria<SerializationTests, string>("success");
      var actual = MobileFormatter.Serialize(criteria);

      Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void DeserializeCriteriaSuccess()
    {
      var expected = new SingleCriteria<SerializationTests, string>("success");
      var buffer = MobileFormatter.Serialize(expected);

      var actual = (SingleCriteria<SerializationTests, string>)MobileFormatter.Deserialize(buffer);

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
      
      Address expectedA1 = new Address();
      expectedA1.City = "Minneapolis";
      expectedA1.ZipCode = "55414";
      
      Address expectedA2 = new Address();
      expectedA2.City = "Eden Prairie";
      expectedA2.ZipCode = "55403";

      expectedAddressList.Add(expectedA1);
      expectedAddressList.Add(expectedA2);
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

    [TestMethod]
    public void SerializeAndDeserializeReadOnly()
    {
      MockReadOnly ro = new MockReadOnly(1);
      MockReadOnlyList expected = new MockReadOnlyList(ro);

      byte[] serialized = MobileFormatter.Serialize(expected);

      // Deserialization should not throw an exception when adding
      // deserialized items back into the list.
      MockReadOnlyList actual = (MockReadOnlyList)MobileFormatter.Deserialize(serialized);

      Assert.AreEqual(expected.Count, actual.Count);
      Assert.AreEqual(expected[0].Id, actual[0].Id);
    }
  }
}
