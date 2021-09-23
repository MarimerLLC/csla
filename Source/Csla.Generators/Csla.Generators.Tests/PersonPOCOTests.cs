using Csla.Serialization.Mobile;
using Csla.Serialization;
using CslaSerialization.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Csla.Generators.TestObjects;

namespace Csla.Generators.Tests
{

  /// <summary>
  /// Tests of serialization on the PersonPOCO class and its children
  /// </summary>
	[TestClass]
  public class PersonPOCOTests
  {

    #region GetState

    [TestMethod]
    public void GetState_WithPersonId5_ReturnsInfoContaining5()
    {

      // Arrange
      SerializationInfo serializationInfo = new SerializationInfo();
      int actual;
      int expected = 5;
      IMobileObject mobileObject;
      PersonPOCO person = new PersonPOCO();
      person.PersonId = 5;

      // Act
      mobileObject = (IMobileObject)person;
      mobileObject.GetState(serializationInfo);
      actual = serializationInfo.GetValue<int>("PersonId");

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void GetState_WithFirstNameJoe_ReturnsInfoContainingJoe()
    {

      // Arrange
      SerializationInfo serializationInfo = new SerializationInfo();
      string actual;
      string expected = "Joe";
      IMobileObject mobileObject;
      PersonPOCO person = new PersonPOCO();
      person.FirstName = "Joe";

      // Act
      mobileObject = (IMobileObject)person;
      mobileObject.GetState(serializationInfo);
      actual = serializationInfo.GetValue<string>("FirstName");

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void GetState_WithLastNameSmith_ReturnsInfoContainingSmith()
    {

      // Arrange
      SerializationInfo serializationInfo = new SerializationInfo();
      string actual;
      string expected = "Smith";
      IMobileObject mobileObject;
      PersonPOCO person = new PersonPOCO();
      person.LastName = "Smith";

      // Act
      mobileObject = (IMobileObject)person;
      mobileObject.GetState(serializationInfo);
      actual = serializationInfo.GetValue<string>("LastName");

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void GetState_WithInternalDateOfBirth210412_ReturnsInfoWithoutDateOfBirth()
    {

      // Arrange
      SerializationInfo serializationInfo = new SerializationInfo();
      bool actual;
      bool expected = false;
      IMobileObject mobileObject;
      PersonPOCO person = new PersonPOCO();
      person.SetDateOfBirth(new DateTime(2021, 04, 12, 16, 57, 53));

      // Act
      mobileObject = (IMobileObject)person;
      mobileObject.GetState(serializationInfo);
      actual = serializationInfo.Values.ContainsKey("DateOfBirth");

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void GetState_WithMiddleNameMid_ReturnsInfoWithoutMiddleName()
    {

      // Arrange
      SerializationInfo serializationInfo = new SerializationInfo();
      bool actual;
      bool expected = false;
      IMobileObject mobileObject;
      PersonPOCO person = new PersonPOCO();
      person.SetMiddleName("Mid");

      // Act
      mobileObject = (IMobileObject)person;
      mobileObject.GetState(serializationInfo);
      actual = serializationInfo.Values.ContainsKey("MiddleName");

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void GetState_WithFieldMiddleNameMid_ReturnsInfoContainingMid()
    {

      // Arrange
      SerializationInfo serializationInfo = new SerializationInfo();
      string actual;
      string expected = "Mid";
      IMobileObject mobileObject;
      PersonPOCO person = new PersonPOCO();
      person.SetMiddleName("Mid");

      // Act
      mobileObject = (IMobileObject)person;
      mobileObject.GetState(serializationInfo);
      actual = serializationInfo.GetValue<string>("_middleName");

      // Assert
      Assert.AreEqual(expected, actual);

    }

    #endregion

    #region SetState

    [TestMethod]
    public void SetState_WithPersonId5_ReturnsPersonWithId5()
    {

      // Arrange
      SerializationInfo serializationInfo = PersonSerializationInfoFactory.GetDefaultSerializationInfo();
      int actual;
      int expected = 5;
      PersonPOCO person = new PersonPOCO();
      IMobileObject mobileObject;

      // Act
      serializationInfo.Values["PersonId"].Value = 5;
      mobileObject = (IMobileObject)person;
      mobileObject.SetState(serializationInfo);
      actual = person.PersonId;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SetState_WithFirstNameJoe_ReturnsPersonWithFirstNameJoe()
    {

      // Arrange
      SerializationInfo serializationInfo = PersonSerializationInfoFactory.GetDefaultSerializationInfo();
      string actual;
      string expected = "Joe";
      PersonPOCO person = new PersonPOCO();
      IMobileObject mobileObject;

      // Act
      serializationInfo.Values["FirstName"].Value = "Joe";
      mobileObject = (IMobileObject)person;
      mobileObject.SetState(serializationInfo);
      actual = person.FirstName;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SetState_WithLastNameSmith_ReturnsPersonWithLastNameSmith()
    {

      // Arrange
      SerializationInfo serializationInfo = PersonSerializationInfoFactory.GetDefaultSerializationInfo();
      string actual;
      string expected = "Smith";
      PersonPOCO person = new PersonPOCO();
      IMobileObject mobileObject;

      // Act
      serializationInfo.Values["LastName"].Value = "Smith";
      mobileObject = (IMobileObject)person;
      mobileObject.SetState(serializationInfo);
      actual = person.LastName;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SetState_WithInternalDateOfBirth210412_ReturnsPersonWithNoDateOfBirth()
    {

      // Arrange
      SerializationInfo serializationInfo = PersonSerializationInfoFactory.GetDefaultSerializationInfo();
      DateTime actual;
      DateTime expected = DateTime.MinValue;
      PersonPOCO person = new PersonPOCO();
      IMobileObject mobileObject;

      // Act
      serializationInfo.Values["DateOfBirth"].Value = new DateTime(2021, 04, 12, 18, 27, 43);
      mobileObject = (IMobileObject)person;
      mobileObject.SetState(serializationInfo);
      actual = person.GetDateOfBirth();

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SetState_WithNonSerializedTextFred_ReturnsEmptyString()
    {

      // Arrange
      SerializationInfo serializationInfo = PersonSerializationInfoFactory.GetDefaultSerializationInfo();
      string actual;
      string expected = string.Empty;
      PersonPOCO person = new PersonPOCO();
      IMobileObject mobileObject;

      // Act
      serializationInfo.Values["NonSerializedText"].Value = "Fred";
      mobileObject = (IMobileObject)person;
      mobileObject.SetState(serializationInfo);
      actual = person.NonSerializedText;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SetState_WithPrivateTextFred_ReturnsEmptyString()
    {

      // Arrange
      SerializationInfo serializationInfo = PersonSerializationInfoFactory.GetDefaultSerializationInfo();
      string actual;
      string expected = string.Empty;
      PersonPOCO person = new PersonPOCO();
      IMobileObject mobileObject;

      // Act
      serializationInfo.Values["PrivateText"].Value = "Fred";
      mobileObject = (IMobileObject)person;
      mobileObject.SetState(serializationInfo);
      actual = person.GetUnderlyingPrivateText();

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SetState_WithPrivateSerializedTextFred_ReturnsFred()
    {

      // Arrange
      SerializationInfo serializationInfo = PersonSerializationInfoFactory.GetDefaultSerializationInfo();
      string actual;
      string expected = "Fred";
      PersonPOCO person = new PersonPOCO();
      IMobileObject mobileObject;

      // Act
      serializationInfo.Values["PrivateSerializedText"].Value = "Fred";
      mobileObject = (IMobileObject)person;
      mobileObject.SetState(serializationInfo);
      actual = person.GetPrivateSerializedText();

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SetState_WithIncludedMiddleNameFieldMid_ReturnsMiddleNamePropertyMid()
    {

      // Arrange
      SerializationInfo serializationInfo = PersonSerializationInfoFactory.GetDefaultSerializationInfo();
      string actual;
      string expected = "Mid";
      PersonPOCO person = new PersonPOCO();
      IMobileObject mobileObject;

      // Act
      serializationInfo.Values["_middleName"].Value = "Mid";
      mobileObject = (IMobileObject)person;
      mobileObject.SetState(serializationInfo);
      actual = person.MiddleName;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    #endregion

    #region GetChildren

    [TestMethod]
    public void GetChildren_WithAddress1HighStreet_IncludesAddressKey()
    {

      // Arrange
      SerializationInfo serializationInfo = new SerializationInfo();
      bool expected = true;
      bool actual;
      IMobileObject mobileObject;
      PersonPOCO person = new PersonPOCO();
      person.Address = new AddressPOCO() { AddressLine1 = "1 High Street" };
      MobileFormatter formatter = new MobileFormatter();

      // Act
      mobileObject = (IMobileObject)person;
      mobileObject.GetChildren(serializationInfo, formatter);
      actual = serializationInfo.Children.ContainsKey("Address");

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void GetChildren_WithEmailAddress_IncludesEmailAddressKey()
    {

      // Arrange
      SerializationInfo serializationInfo = new SerializationInfo();
      bool expected = true;
      bool actual;
      IMobileObject mobileObject;
      PersonPOCO person = new PersonPOCO();
      person.EmailAddress = new EmailAddress() { Email = "a@b.com" };
      MobileFormatter formatter = new MobileFormatter();

      // Act
      mobileObject = (IMobileObject)person;
      mobileObject.GetChildren(serializationInfo, formatter);
      actual = serializationInfo.Children.ContainsKey("EmailAddress");

      // Assert
      Assert.AreEqual(expected, actual);

    }

    #endregion

    #region SetChildren

    #endregion

    #region Serialize Then Deserialize

    [TestMethod]
    public void SerializeThenDeserialize_WithPublicAutoImpPropertyPersonId5_HasPersonId5()
    {

      // Arrange
      int actual;
      int expected = 5;
      PersonPOCO person = new PersonPOCO();
      person.PersonId = 5;
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.PersonId;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithPublicAutoImpPropertyFirstNameJoe_HasFirstNameJoe()
    {

      // Arrange
      string actual;
      string expected = "Joe";
      PersonPOCO person = new PersonPOCO();
      person.FirstName = "Joe";
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.FirstName;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithPublicPropertyLastNameSmith_HasLastNameSmith()
    {

      // Arrange
      string actual;
      string expected = "Smith";
      PersonPOCO person = new PersonPOCO();
      person.LastName = "Smith";
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.LastName;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithIncludedPrivateFieldMiddleNameMid_HasMiddleNameMid()
    {

      // Arrange
      string actual;
      string expected = "Mid";
      PersonPOCO person = new PersonPOCO();
      person.SetMiddleName("Mid");
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.MiddleName;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithExcludedPublicAutoImpPropertyNonSerializedTextNon_HasEmptyNonSerializedText()
    {

      // Arrange
      string actual;
      string expected = "";
      PersonPOCO person = new PersonPOCO();
      person.NonSerializedText = "Non";
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.NonSerializedText;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithIncludedPrivateAutoImpPropertyPrivateSerializedTextPri_HasPrivateSerializedTextPri()
    {

      // Arrange
      string actual;
      string expected = "Pri";
      PersonPOCO person = new PersonPOCO();
      person.SetPrivateSerializedText("Pri");
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.GetPrivateSerializedText();

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithPrivateAutoImpPropertyPrivateTextPriv_HasEmptyPrivateText()
    {

      // Arrange
      string actual;
      string expected = "";
      PersonPOCO person = new PersonPOCO();
      person.SetUnderlyingPrivateText("Priv");
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.GetUnderlyingPrivateText();

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithInternalAutoImpPropertyDateOfBirth20210412165753_HasMinDateOfBirth()
    {

      // Arrange
      DateTime actual;
      DateTime expected = DateTime.MinValue;
      PersonPOCO person = new PersonPOCO();
      person.SetDateOfBirth(new DateTime(2021, 04, 12, 16, 57, 53));
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.GetDateOfBirth();

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithAutoSerializableAddress1HighStreet_HasAddressOf1HighStreet()
    {

      // Arrange
      string actual;
      string expected = "1 High Street";
      PersonPOCO person = new PersonPOCO();
      person.Address = new AddressPOCO() { AddressLine1 = "1 High Street" };
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson?.Address?.AddressLine1;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithAutoSerializableAddressNull_HasNullAddress()
    {

      // Arrange
      AddressPOCO actual;
      PersonPOCO person = new PersonPOCO();
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.Address;

      // Assert
      Assert.IsNull(actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithAutoSerializableAddressTownsville_HasAddressOfTownsville()
    {

      // Arrange
      string actual;
      string expected = "Townsville";
      PersonPOCO person = new PersonPOCO();
      person.Address = new AddressPOCO() { Town = "Townsville" };
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson?.Address?.Town;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithIMobileObjectEmailAddressNull_HasEmailAddressNull()
    {

      // Arrange
      EmailAddress actual;
      PersonPOCO person = new PersonPOCO();
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson.EmailAddress;

      // Assert
      Assert.IsNull(actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_WithIMobileObjectEmailAddressAatBdotCom_HasEmailAddressAatBdotCom()
    {

      // Arrange
      string actual;
      string expected = "a@b.com";
      PersonPOCO person = new PersonPOCO();
      person.EmailAddress = new EmailAddress() { Email = "a@b.com" };
      PersonPOCO deserializedPerson;

      // Act
      deserializedPerson = SerializeThenDeserialisePersonPOCO(person);
      actual = deserializedPerson?.EmailAddress?.Email;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Serialize and then deserialize a PersonPOCO object, exercising the generated code
    /// associated with these two operations on the PersonPOCO test object
    /// </summary>
    /// <param name="valueToSerialize">The object to be serialized</param>
    /// <returns>The PersonPOCO that results from serialization then deserialization</returns>
    private PersonPOCO SerializeThenDeserialisePersonPOCO(PersonPOCO valueToSerialize)
    {
      System.IO.MemoryStream serializationStream;
      PersonPOCO deserializedValue;
      MobileFormatter formatter = new MobileFormatter();

      // Act
      using (serializationStream = new System.IO.MemoryStream())
      {
        formatter.Serialize(serializationStream, valueToSerialize);
        serializationStream.Seek(0, System.IO.SeekOrigin.Begin);
        deserializedValue = formatter.Deserialize(serializationStream) as PersonPOCO;
      }

      return deserializedValue;
    }

    #endregion

  }
}
