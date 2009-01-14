using System;
using System.Text;
using System.IO;
using Csla;
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
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [TestClass]
  public class SerializationTests : TestBase
  {
    [TestMethod]
    public void SerializeWithNullArgumentShouldNotThrowException()
    {
      UnitTestContext context = GetContext();

      var actual = MobileFormatter.Serialize(null);
      object result = MobileFormatter.Deserialize(actual);

      context.Assert.IsNotNull(actual);
      context.Assert.IsNull(result);
      context.Assert.Success();
      context.Complete();      
    }

    [TestMethod]
    public void SerializeCriteriaSuccess()
    {
      UnitTestContext context = GetContext();
      var criteria = new SingleCriteria<SerializationTests, string>("success");
      var actual = MobileFormatter.Serialize(criteria);

      context.Assert.IsNotNull(actual);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void DeserializeCriteriaSuccess()
    {
      UnitTestContext context = GetContext();
      var expected = new SingleCriteria<SerializationTests, string>("success");
      var buffer = MobileFormatter.Serialize(expected);

      var actual = (SingleCriteria<SerializationTests, string>)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(expected.TypeName, actual.TypeName);
      context.Assert.AreEqual(expected.Value, actual.Value);
      context.Assert.Success();
      context.Complete();
    }

    /// <summary>
    /// Same test as above only uses one of the mobile formatter overloads to verify that
    /// using a TextReader/Writer works as well.
    /// </summary>
    [TestMethod]
    public void SerializeWithTextWriter()
    {
      UnitTestContext context = GetContext();

      MobileFormatter formatter = new MobileFormatter();
      StringBuilder sb = new StringBuilder();
      SingleCriteria<SerializationTests, string> expected = new SingleCriteria<SerializationTests, string>("success");
      SingleCriteria<SerializationTests, string> actual = null;

      using (TextWriter tw = new StringWriter(sb))
        formatter.Serialize(tw, expected);
      
      string buffer = sb.ToString();

      using(TextReader tr = new StringReader(buffer))
        actual = (SingleCriteria<SerializationTests, string>)formatter.Deserialize(tr);

      context.Assert.AreEqual(expected.TypeName, actual.TypeName);
      context.Assert.AreEqual(expected.Value, actual.Value);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void BusinessObjectWithoutChildList()
    {
      UnitTestContext context = GetContext();
      DateTime birthdate = new DateTime(1980, 2, 3);

      Person expected = new Person();
      expected.Name = "test";
      expected.Unserialized = "should be null";
      expected.Birthdate = birthdate;
      expected.DtoDate = DateTimeOffset.Parse("1/1/2000");

      var buffer = MobileFormatter.Serialize(expected);
      var actual = (Person)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(expected.Name, actual.Name);
      context.Assert.AreEqual(expected.Birthdate, actual.Birthdate);
      context.Assert.AreEqual(expected.DtoDate, actual.DtoDate);
      context.Assert.AreEqual(expected.Age, actual.Age);

      context.Assert.AreEqual(actual.Unserialized, string.Empty);
      context.Assert.IsNull(actual.Addresses);
      context.Assert.IsNull(actual.PrimaryAddress);

      context.Assert.IsNotNull(expected.Unserialized);
      context.Assert.IsNull(expected.Addresses);
      context.Assert.IsNull(expected.PrimaryAddress);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void BusinessObjectWithChildList()
    {
      UnitTestContext context = GetContext();
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

      context.Assert.AreEqual(expectedPerson.Name, actualPerson.Name);
      context.Assert.AreEqual(expectedPerson.Birthdate, actualPerson.Birthdate);
      context.Assert.AreEqual(expectedPerson.Age, actualPerson.Age);
      context.Assert.AreEqual(actualPerson.Unserialized, string.Empty);
      context.Assert.IsNotNull(expectedPerson.Unserialized);
      context.Assert.AreSame(expectedPerson.PrimaryAddress, expectedAddressList[1]);

      var actualAddressList = actualPerson.Addresses;
      context.Assert.IsNotNull(actualAddressList);
      context.Assert.AreEqual(expectedAddressList.Count, actualAddressList.Count);

      context.Assert.AreEqual(expectedAddressList[0].City, actualAddressList[0].City);
      context.Assert.AreEqual(expectedAddressList[0].ZipCode, actualAddressList[0].ZipCode);

      context.Assert.AreEqual(expectedAddressList[1].City, actualAddressList[1].City);
      context.Assert.AreEqual(expectedAddressList[1].ZipCode, actualAddressList[1].ZipCode);

      context.Assert.AreSame(actualPerson.PrimaryAddress, actualAddressList[1]);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void SerializeAndDeserializeReadOnly()
    {
      UnitTestContext context = GetContext();
      MockReadOnly ro = new MockReadOnly(1);
      MockReadOnlyList expected = new MockReadOnlyList(ro);

      byte[] serialized = MobileFormatter.Serialize(expected);

      // Deserialization should not throw an exception when adding
      // deserialized items back into the list.
      MockReadOnlyList actual = (MockReadOnlyList)MobileFormatter.Deserialize(serialized);

      context.Assert.AreEqual(expected.Count, actual.Count);
      context.Assert.AreEqual(expected[0].Id, actual[0].Id);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void SerializeMobileListWithPrimitiveTypes()
    {
      UnitTestContext context = GetContext();

      var expected = new MobileBindingList<int>();
      expected.Add(1);
      context.Assert.Try(() => MobileFormatter.Serialize(expected));
      context.Assert.Fail();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void MobileListWithNonBusinessObjectReferenceType()
    {
      UnitTestContext context = GetContext();
      var expected = new MobileBindingList<MockNonBusinessObject>();
      expected.Add(new MockNonBusinessObject { Member = "xyz" });

      context.Assert.Try((Func<object, byte[]>)MobileFormatter.Serialize, expected );
      context.Assert.Fail();
      context.Complete();
    }

    [TestMethod]
    public void ReadOnlyBaseTest()
    {
      UnitTestContext context = GetContext();
      ReadOnlyPerson expected = ReadOnlyPerson.GetReadOnlyPerson("John Does", 1980);
      byte[] buffer = MobileFormatter.Serialize(expected);
      ReadOnlyPerson actual = (ReadOnlyPerson)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(expected.Birthdate, actual.Birthdate);
      context.Assert.AreEqual(expected.Name, actual.Name);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ReadOnlyListBaseTest()
    {
      UnitTestContext context = GetContext();
      ReadOnlyPersonList expected = ReadOnlyPersonList.GetReadOnlyPersonList();
      byte[] buffer = MobileFormatter.Serialize(expected);
      ReadOnlyPersonList actual = (ReadOnlyPersonList)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(expected.Count, actual.Count);
      context.Assert.AreEqual(expected[1].Name, actual[1].Name);
      context.Assert.AreEqual(expected[0].Birthdate, actual[0].Birthdate);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MobileListSerializationSuccess()
    {
      UnitTestContext context = GetContext();
      MobileList<string> expected = new MobileList<string>();
      expected.Add("one");
      expected.Add("two");

      byte[] buffer = MobileFormatter.Serialize(expected);
      MobileList<string> actual = (MobileList<string>)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(expected.Count, actual.Count);
      context.Assert.AreEqual(expected[0], actual[0]);
      context.Assert.AreEqual(expected[1], actual[1]);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MobileListEmptySerializationSuccess()
    {
      UnitTestContext context = GetContext();
      MobileList<string> expected = new MobileList<string>();

      byte[] buffer = MobileFormatter.Serialize(expected);
      MobileList<string> actual = (MobileList<string>)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(expected.Count, actual.Count);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MobileListOfMobileObjectsSerializationSuccess()
    {
      UnitTestContext context = GetContext();
      MobileList<MockReadOnly> expected = new MobileList<MockReadOnly>();
      expected.Add(new MockReadOnly(1));
      expected.Add(new MockReadOnly(2));

      byte[] buffer = MobileFormatter.Serialize(expected);
      MobileList<MockReadOnly> actual = (MobileList<MockReadOnly>)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(expected.Count, actual.Count);
      context.Assert.AreEqual(expected[0].Id, actual[0].Id);
      context.Assert.AreEqual(expected[1].Id, actual[1].Id);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MobileListOfNonMobileObjectsSerializationSuccess()
    {
      UnitTestContext context = GetContext();
      MobileList<MockNonBusinessObject> expected = new MobileList<MockNonBusinessObject>();
      expected.Add(new MockNonBusinessObject { Member = "one", Child = new MockNonBusinessObject2 { Id = 1 } });
      expected.Add(new MockNonBusinessObject { Member = "two", Child = new MockNonBusinessObject2 { Id = 2 } });

      byte[] buffer = MobileFormatter.Serialize(expected);
      MobileList<MockNonBusinessObject> actual = (MobileList<MockNonBusinessObject>)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(expected.Count, actual.Count);
      context.Assert.AreEqual(expected[0].Member, actual[0].Member);
      context.Assert.AreEqual(expected[0].Child.Id, actual[0].Child.Id);
      context.Assert.AreEqual(expected[1].Member, actual[1].Member);
      context.Assert.AreEqual(expected[1].Child.Id, actual[1].Child.Id);
      context.Assert.Success();
      context.Complete();
    }
  }
}
