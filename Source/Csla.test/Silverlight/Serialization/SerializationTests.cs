//-----------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Same test as above only uses one of the mobile formatter overloads to verify that</summary>
//-----------------------------------------------------------------------
using System;
using System.Text;
using System.IO;
using Csla;
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.Test.Serialization;
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

    [Serializable]
    public class StringCriteria : CriteriaBase<StringCriteria>
    {
      public static readonly PropertyInfo<string> ValueProperty = RegisterProperty<string>(c => c.Value);
      public string Value
      {
        get { return ReadProperty(ValueProperty); }
        set { LoadProperty(ValueProperty, value); }
      }
    }

    [TestMethod]
    public void SerializeCriteriaSuccess()
    {
      UnitTestContext context = GetContext();
      var criteria = new StringCriteria { Value = "success" };
      var buffer = MobileFormatter.Serialize(criteria);

      var actual = (StringCriteria)MobileFormatter.Deserialize(buffer);

      context.Assert.AreEqual(criteria.Value, actual.Value);
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
    public void MobileDictionary_PrimitiveKey_PrimitiveValue()
    {
      UnitTestContext context = GetContext();
      var d = new MobileDictionary<string, int>();
      d.Add("a", 12343);
      d.Add("z", 943204);
      d.Add("b", 77878);
      d.Add("x", 42343);
      d.Add("r", 45345);

      byte[] buffer = MobileFormatter.Serialize(d);
      var r = (MobileDictionary<string, int>)MobileFormatter.Deserialize(buffer);

      context.Assert.IsTrue(r.ContainsKey("a"));
      context.Assert.IsTrue(r.ContainsKey("z"));
      context.Assert.IsTrue(r.ContainsKey("b"));
      context.Assert.IsTrue(r.ContainsKey("x"));
      context.Assert.IsTrue(r.ContainsKey("r"));

      context.Assert.AreEqual(d["a"], r["a"]);
      context.Assert.AreEqual(d["z"], r["z"]);
      context.Assert.AreEqual(d["b"], r["b"]);
      context.Assert.AreEqual(d["x"], r["x"]);
      context.Assert.AreEqual(d["r"], r["r"]);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MobileDictionary_PrimitiveKey_PrimitiveValue_BF()
    {
      UnitTestContext context = GetContext();
      var d = new MobileDictionary<string, int>();
      d.Add("a", 12343);
      d.Add("z", 943204);
      d.Add("b", 77878);
      d.Add("x", 42343);
      d.Add("r", 45345);

      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      var buffer = new System.IO.MemoryStream();
      formatter.Serialize(buffer, d);
      buffer.Position = 0;
      var r = (MobileDictionary<string, int>)formatter.Deserialize(buffer);

      context.Assert.IsTrue(r.ContainsKey("a"));
      context.Assert.IsTrue(r.ContainsKey("z"));
      context.Assert.IsTrue(r.ContainsKey("b"));
      context.Assert.IsTrue(r.ContainsKey("x"));
      context.Assert.IsTrue(r.ContainsKey("r"));

      context.Assert.AreEqual(d["a"], r["a"]);
      context.Assert.AreEqual(d["z"], r["z"]);
      context.Assert.AreEqual(d["b"], r["b"]);
      context.Assert.AreEqual(d["x"], r["x"]);
      context.Assert.AreEqual(d["r"], r["r"]);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MobileDictionary_PrimitiveKey_MobileValue()
    {
      UnitTestContext context = GetContext();
      var d = new MobileDictionary<string, MockReadOnly>();
      d.Add("a", new MockReadOnly(1));
      d.Add("z", new MockReadOnly(2));
      d.Add("b", new MockReadOnly(3));
      d.Add("x", new MockReadOnly(4));
      d.Add("r", new MockReadOnly(5));

      byte[] buffer = MobileFormatter.Serialize(d);
      var r = (MobileDictionary<string, MockReadOnly>)MobileFormatter.Deserialize(buffer);

      context.Assert.IsTrue(r.ContainsKey("a"));
      context.Assert.IsTrue(r.ContainsKey("z"));
      context.Assert.IsTrue(r.ContainsKey("b"));
      context.Assert.IsTrue(r.ContainsKey("x"));
      context.Assert.IsTrue(r.ContainsKey("r"));

      context.Assert.AreEqual(d["a"].Id, r["a"].Id);
      context.Assert.AreEqual(d["z"].Id, r["z"].Id);
      context.Assert.AreEqual(d["b"].Id, r["b"].Id);
      context.Assert.AreEqual(d["x"].Id, r["x"].Id);
      context.Assert.AreEqual(d["r"].Id, r["r"].Id);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MobileDictionary_MobileKey_PrimitiveValue()
    {
      UnitTestContext context = GetContext();
      var d = new MobileDictionary<MockReadOnly, string>();
      d.Add(new MockReadOnly(1), "v1");
      d.Add(new MockReadOnly(2), "v2");
      d.Add(new MockReadOnly(3), "v3");
      d.Add(new MockReadOnly(4), "v4");
      d.Add(new MockReadOnly(5), "v5");

      byte[] buffer = MobileFormatter.Serialize(d);
      var r = (MobileDictionary<MockReadOnly, string>)MobileFormatter.Deserialize(buffer);

      foreach (var key in r.Keys)
        context.Assert.AreEqual(key.Id, Convert.ToInt32(r[key].Substring(1)));
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MobileDictionary_MobileKey_MobileValue()
    {
      UnitTestContext context = GetContext();
      var d = new MobileDictionary<MockReadOnly, MockReadOnly>();
      d.Add(new MockReadOnly(21), new MockReadOnly(1));
      d.Add(new MockReadOnly(22), new MockReadOnly(2));
      d.Add(new MockReadOnly(23), new MockReadOnly(3));
      d.Add(new MockReadOnly(24), new MockReadOnly(4));
      d.Add(new MockReadOnly(25), new MockReadOnly(5));

      byte[] buffer = MobileFormatter.Serialize(d);
      var r = (MobileDictionary<MockReadOnly, MockReadOnly>)MobileFormatter.Deserialize(buffer);

      foreach (var key in r.Keys)
        context.Assert.AreEqual(key.Id, r[key].Id + 20);
      context.Assert.Success();
      context.Complete();
    }

    /// <summary>
    /// Verifies that serializing an object graph with sibling complex objects that
    /// implement Equals and are logically identical, produces an identical object graph upon
    /// deserialization (eg. the two siblings are separate instances post-deserialization).
    /// It should also be noted that the tested objects implement custom serialization methods.
    /// </summary>
    [TestMethod]
    public void LogicallyIdenticalChildObjects()
    {
      UnitTestContext context = GetContext();

      // Setup the customer w/ logically identical contacts
      Customer customer = new Customer();

      customer.Name = "Test Customer";
      customer.PrimaryContact.FirstName = "John";
      customer.PrimaryContact.LastName = "Smith";
      customer.AccountsPayableContact.FirstName = "John";
      customer.AccountsPayableContact.LastName = "Smith";

      // Serialize and deserialize the customer
      var buffer = MobileFormatter.Serialize(customer);
      var deserializedCustomer = (Customer)MobileFormatter.Deserialize(buffer);

      // Verify the deserialized customer is identical to the original object
      context.Assert.AreEqual(customer.Name, deserializedCustomer.Name);
      context.Assert.AreEqual(customer.PrimaryContact.FirstName, deserializedCustomer.PrimaryContact.FirstName);
      context.Assert.AreEqual(customer.PrimaryContact.LastName, deserializedCustomer.PrimaryContact.LastName);
      context.Assert.AreEqual(customer.AccountsPayableContact.FirstName, deserializedCustomer.AccountsPayableContact.FirstName);
      context.Assert.AreEqual(customer.AccountsPayableContact.LastName, deserializedCustomer.AccountsPayableContact.LastName);

      //
      // The two CustomerContact objects (PrimaryContact and AccountsPayableContact) should not
      // point to the same CustomerContact instance after deserialization, even though they
      // are logically identical. They start as different objects prior to serialization and 
      // they should end as different objects after.
      //
      context.Assert.IsFalse(Object.ReferenceEquals(deserializedCustomer.PrimaryContact, deserializedCustomer.AccountsPayableContact));

      context.Assert.Success();
      context.Complete();
    }

    /// <summary>
    /// Verifies that serializing/deserializing an object graph with null references of a
    /// type that is complex (eg. is a separate child object) works. It should also be noted
    /// that the tested objects implement custom serialization methods.
    /// </summary>
    [TestMethod]
    public void NullChildObject()
    {
      UnitTestContext context = GetContext();

      // Setup the customer w/ a null child object
      Customer customer = new Customer();

      customer.Name = "Test Customer";
      customer.PrimaryContact.FirstName = "John";
      customer.PrimaryContact.LastName = "Smith";
      customer.AccountsPayableContact = null;

      // Serialize and deserialize the customer
      var buffer = MobileFormatter.Serialize(customer);
      var deserializedCustomer = (Customer)MobileFormatter.Deserialize(buffer);

      // Verify the deserialized customer is identical to the original object
      context.Assert.AreEqual(customer.Name, deserializedCustomer.Name);
      context.Assert.AreEqual(customer.PrimaryContact.FirstName, deserializedCustomer.PrimaryContact.FirstName);
      context.Assert.AreEqual(customer.PrimaryContact.LastName, deserializedCustomer.PrimaryContact.LastName);
      context.Assert.IsNull(deserializedCustomer.AccountsPayableContact);

      context.Assert.Success();
      context.Complete();
    }

    /// <summary>
    /// Verifies that serialization/deserialization works for business objects that have
    /// an property storing an enum.
    /// </summary>
    [TestMethod]
    public void BusinessObjectWithEnum()
    {
      UnitTestContext context = GetContext();

      // Setup the customer w/ logically identical contacts
      CustomerWithEnum customer = new CustomerWithEnum();

      customer.Name = "Test Customer";
      customer.Quality = CustomerQuality.Good;

      // Serialize and deserialize the customer
      var buffer = MobileFormatter.Serialize(customer);
      var deserializedCustomer = (CustomerWithEnum)MobileFormatter.Deserialize(buffer);

      // Verify the deserialized customer is identical to the original object
      context.Assert.AreEqual(customer.Name, deserializedCustomer.Name);
      context.Assert.AreEqual(customer.Quality, deserializedCustomer.Quality);
      context.Assert.Success();
      context.Complete();
    }

		[TestMethod()]
		public void TestSerializationCslaBinaryReaderWriterList()
		{
			UnitTestContext context = GetContext();

			var test = new BinaryReaderWriterTestClassList();
			BinaryReaderWriterTestClassList result;
			test.Setup();
			var serialized = MobileFormatter.SerializeToDTO(test);
			CslaBinaryWriter writer = new CslaBinaryWriter();
			byte[] data;
			using (var stream = new MemoryStream())
			{
				writer.Write(stream, serialized);
				data = stream.ToArray();
			}

			CslaBinaryReader reader = new CslaBinaryReader();
			using (var stream = new MemoryStream(data))
			{
				var deserialized = reader.Read(stream);
				result = (BinaryReaderWriterTestClassList)MobileFormatter.DeserializeFromDTO(deserialized);
			}

			context.Assert.AreEqual(test.Count, result.Count);
			for (int i = 0; i < test.Count; i++)
			{
				context.Assert.AreEqual(test[i].CharTest, result[i].CharTest);
				context.Assert.AreEqual(test[i].DateTimeOffsetTest, result[i].DateTimeOffsetTest);
				context.Assert.AreEqual(test[i].DateTimeTest, result[i].DateTimeTest);
				context.Assert.AreEqual(test[i].DecimalTest, result[i].DecimalTest);
				context.Assert.AreEqual(test[i].DoubleTest, result[i].DoubleTest);
				context.Assert.AreEqual(test[i].EnumTest, result[i].EnumTest);
				context.Assert.AreEqual(test[i].GuidTest, result[i].GuidTest);
				context.Assert.AreEqual(test[i].Int16Test, result[i].Int16Test);
				context.Assert.AreEqual(test[i].Int32Test, result[i].Int32Test);
				context.Assert.AreEqual(test[i].Int64Test, result[i].Int64Test);
				context.Assert.AreEqual(test[i].SByteTest, result[i].SByteTest);
				context.Assert.AreEqual(test[i].SingleTest, result[i].SingleTest);
				context.Assert.AreEqual(test[i].StringTest, result[i].StringTest);
				context.Assert.AreEqual(test[i].TimeSpanTest, result[i].TimeSpanTest);
				context.Assert.AreEqual(test[i].UInt16Test, result[i].UInt16Test);
				context.Assert.AreEqual(test[i].UInt32Test, result[i].UInt32Test);
				context.Assert.AreEqual(test[i].UInt64Test, result[i].UInt64Test);
				context.Assert.AreEqual(test[i].NullableButSetInt, result[i].NullableButSetInt);
				context.Assert.IsNull(test[i].NullableInt);
				context.Assert.IsNull(result[i].NullableInt);

				context.Assert.AreEqual(test[i].EmptySmartDateTest, result[i].EmptySmartDateTest);
				context.Assert.AreEqual(test[i].EmptySmartDateTest.FormatString, result[i].EmptySmartDateTest.FormatString);
				context.Assert.AreEqual(test[i].EmptySmartDateTest.EmptyIsMin, result[i].EmptySmartDateTest.EmptyIsMin);
				context.Assert.AreEqual(test[i].EmptySmartDateTest.IsEmpty, result[i].EmptySmartDateTest.IsEmpty);
				context.Assert.AreEqual(test[i].EmptySmartDateTest.Date, result[i].EmptySmartDateTest.Date);
				context.Assert.AreEqual(test[i].FilledSmartDateTest, result[i].FilledSmartDateTest);
				context.Assert.AreEqual(test[i].FilledSmartDateTest.FormatString, result[i].FilledSmartDateTest.FormatString);
				context.Assert.AreEqual(test[i].FilledSmartDateTest.EmptyIsMin, result[i].FilledSmartDateTest.EmptyIsMin);
				context.Assert.AreEqual(test[i].FilledSmartDateTest.IsEmpty, result[i].FilledSmartDateTest.IsEmpty);
				context.Assert.AreEqual(test[i].FilledSmartDateTest.Date, result[i].FilledSmartDateTest.Date);

			}
			context.Assert.Success();
			context.Complete();
		}


		[TestMethod()]
		public void TestSerializationCslaBinaryReaderWriter()
		{
			UnitTestContext context = GetContext();

			var test = new BinaryReaderWriterTestClass();
			BinaryReaderWriterTestClass result;
			test.Setup();
			var serialized = MobileFormatter.SerializeToDTO(test);
			CslaBinaryWriter writer = new CslaBinaryWriter();
			byte[] data;
			using (var stream = new MemoryStream())
			{
				writer.Write(stream, serialized);
				data = stream.ToArray();
			}

			CslaBinaryReader reader = new CslaBinaryReader();
			using (var stream = new MemoryStream(data))
			{
				var deserialized = reader.Read(stream);
				result = (BinaryReaderWriterTestClass)MobileFormatter.DeserializeFromDTO(deserialized);
			}
			context.Assert.AreEqual(test.BoolTest, result.BoolTest);
			context.Assert.AreEqual(test.ByteArrayTest.Length, result.ByteArrayTest.Length);
			for (int i = 0; i < test.ByteArrayTest.Length; i++)
			{
				context.Assert.AreEqual(test.ByteArrayTest[i], result.ByteArrayTest[i]);
			}

			context.Assert.AreEqual(test.ByteTest, result.ByteTest);
			context.Assert.AreEqual(test.CharArrayTest.Length, result.CharArrayTest.Length);
			for (int i = 0; i < test.CharArrayTest.Length; i++)
			{
				context.Assert.AreEqual(test.CharArrayTest[i], result.CharArrayTest[i]);
			}

			context.Assert.AreEqual(test.CharTest, result.CharTest);
			context.Assert.AreEqual(test.DateTimeOffsetTest, result.DateTimeOffsetTest);
			context.Assert.AreEqual(test.DateTimeTest, result.DateTimeTest);
			context.Assert.AreEqual(test.DecimalTest, result.DecimalTest);
			context.Assert.AreEqual(test.DoubleTest, result.DoubleTest);
			context.Assert.AreEqual(test.EnumTest, result.EnumTest);
			context.Assert.AreEqual(test.GuidTest, result.GuidTest);
			context.Assert.AreEqual(test.Int16Test, result.Int16Test);
			context.Assert.AreEqual(test.Int32Test, result.Int32Test);
			context.Assert.AreEqual(test.Int64Test, result.Int64Test);
			context.Assert.AreEqual(test.SByteTest, result.SByteTest);
			context.Assert.AreEqual(test.SingleTest, result.SingleTest);
			context.Assert.AreEqual(test.StringTest, result.StringTest);
			context.Assert.AreEqual(test.TimeSpanTest, result.TimeSpanTest);
			context.Assert.AreEqual(test.UInt16Test, result.UInt16Test);
			context.Assert.AreEqual(test.UInt32Test, result.UInt32Test);
			context.Assert.AreEqual(test.UInt64Test, result.UInt64Test);
			context.Assert.AreEqual(test.NullableButSetInt, result.NullableButSetInt);
			context.Assert.IsNull(test.NullableInt);
			context.Assert.IsNull(result.NullableInt);

			context.Assert.AreEqual(test.EmptySmartDateTest, result.EmptySmartDateTest);
			context.Assert.AreEqual(test.EmptySmartDateTest.FormatString, result.EmptySmartDateTest.FormatString);
			context.Assert.AreEqual(test.EmptySmartDateTest.EmptyIsMin, result.EmptySmartDateTest.EmptyIsMin);
			context.Assert.AreEqual(test.EmptySmartDateTest.IsEmpty, result.EmptySmartDateTest.IsEmpty);
			context.Assert.AreEqual(test.EmptySmartDateTest.Date, result.EmptySmartDateTest.Date);

			context.Assert.AreEqual(test.FilledSmartDateTest, result.FilledSmartDateTest);
			context.Assert.AreEqual(test.FilledSmartDateTest.FormatString, result.FilledSmartDateTest.FormatString);
			context.Assert.AreEqual(test.FilledSmartDateTest.EmptyIsMin, result.FilledSmartDateTest.EmptyIsMin);
			context.Assert.AreEqual(test.FilledSmartDateTest.IsEmpty, result.FilledSmartDateTest.IsEmpty);
			context.Assert.AreEqual(test.FilledSmartDateTest.Date, result.FilledSmartDateTest.Date);

			context.Assert.Success();
			context.Complete();
		}
  }
}