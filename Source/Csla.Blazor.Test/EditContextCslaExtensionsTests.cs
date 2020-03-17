using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Blazor.Test.Fakes;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Csla.Blazor.Test
{
	[TestClass]
	public class EditContextCslaExtensionsTests
	{

		[TestMethod]
		public void ValidateModel_EmptyLastName_OneValidationMessage()
		{
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set last name to an invalid value
      person.LastName = string.Empty;

      // Act
      editContext.Validate();
      IEnumerable<string> messages = editContext.GetValidationMessages();

      // Assert
      Assert.AreEqual(1, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

		}

    [TestMethod]
    public void ValidateModel_ShortFirstName_NoValidationMessages()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set last name to an invalid value
      person.LastName = "A";

      // Act
      editContext.Validate();
      IEnumerable<string> messages = editContext.GetValidationMessages();

      // Assert
      Assert.AreEqual(0, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateModel_ShortLastName_NoValidationMessages()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set last name to an invalid value
      person.LastName = "A";

      // Act
      editContext.Validate();
      IEnumerable<string> messages = editContext.GetValidationMessages();

      // Assert
      Assert.AreEqual(0, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateModel_ExcessiveFirstNameEmptyLastName_TwoValidationMessages()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set first and last names to invalid values
      person.FirstName = "This text is more than twenty five characters long";
      person.LastName = string.Empty;

      // Act
      editContext.Validate();
      IEnumerable<string> messages = editContext.GetValidationMessages();

      // Assert
      Assert.AreEqual(2, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateModel_NeitherTelephoneProvided_TwoValidationMessages()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set both phone numbers to invalid values
      person.HomeTelephone = "";
      person.MobileTelephone = "";

      // Act
      editContext.Validate();
      IEnumerable<string> messages = editContext.GetValidationMessages();

      // Assert
      Assert.AreEqual(2, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateModel_EmptyChildEmailAddress_OneValidationMessage()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Create a new child object (which is immediately invalid)
      person.EmailAddresses.NewEmailAddress();

      // Act
      editContext.Validate();
      IEnumerable<string> messages = editContext.GetValidationMessages();

      // Assert
      Assert.AreEqual(1, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateField_ExcessiveFirstNameEmptyLastName_OneValidationMessageForFirstName()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set first and last names to invalid values
      person.FirstName = "This text is more than twenty five characters long";
      person.LastName = string.Empty;

      // Act
      editContext.NotifyFieldChanged(new FieldIdentifier(person, nameof(person.FirstName)));
      IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(person, nameof(person.FirstName)));

      // Assert
      Assert.AreEqual(1, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateField_NeitherTelephoneProvided_OneValidationMessageForHomeTelephone()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set both phone numbers to invalid values
      person.HomeTelephone = "";
      person.MobileTelephone = "";

      // Act
      editContext.NotifyFieldChanged(new FieldIdentifier(person, nameof(person.HomeTelephone)));
      IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(person, nameof(person.HomeTelephone)));

      // Assert
      Assert.AreEqual(1, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateField_ShortFirstName_NoValidationMessagesForFirstName()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set first name to an invalid value
      person.FirstName = "A";

      // Act
      editContext.NotifyFieldChanged(new FieldIdentifier(person, nameof(person.FirstName)));
      IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(person, nameof(person.FirstName)));

      // Assert
      Assert.AreEqual(0, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateField_ShortLastName_NoValidationMessagesForLastName()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Set both last name to an invalid value
      person.LastName = "A";

      // Act
      editContext.NotifyFieldChanged(new FieldIdentifier(person, nameof(person.LastName)));
      IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(person, nameof(person.LastName)));

      // Assert
      Assert.AreEqual(0, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateField_MissingEmailAddress1_OneValidationMessageForEmailAddress1()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Add a new, empty email address
      FakePersonEmailAddress address = person.EmailAddresses.NewEmailAddress();
      address.EmailAddress = "";

      // Act
      editContext.NotifyFieldChanged(new FieldIdentifier(address, nameof(address.EmailAddress)));
      IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(address, nameof(address.EmailAddress)));

      // Assert
      Assert.AreEqual(1, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    [TestMethod]
    public void ValidateField_ValidEmailAddress1_NoValidationMessagesForEmailAddress1()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      EditContext editContext = new EditContext(person);
      editContext.AddCslaValidation();

      // Add a new, valid email address
      FakePersonEmailAddress address = person.EmailAddresses.NewEmailAddress();
      address.EmailAddress = "name@domain.com";

      // Act
      editContext.NotifyFieldChanged(new FieldIdentifier(address, nameof(address.EmailAddress)));
      IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(address, nameof(address.EmailAddress)));

      // Assert
      Assert.AreEqual(0, messages.Count(), "Incorrect number of validation messages returned! " + ConcatenateMessages(messages));

    }

    #region Helper Methods

    FakePerson GetValidFakePerson()
    {
      FakePerson person = FakePerson.NewFakePerson();

      person.FirstName = "John";
      person.LastName = "Smith";
      person.HomeTelephone = "01234 567890";

      return person;
    }

    private string ConcatenateMessages(IEnumerable<string> messages)
    {
      return string.Join("; ", messages);
    }

    #endregion

  }
}
