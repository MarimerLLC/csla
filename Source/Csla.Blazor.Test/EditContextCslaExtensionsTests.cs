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
      Assert.AreEqual(1, messages.Count(), "Incorrect number of validation messages returned!");

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
      Assert.AreEqual(2, messages.Count(), "Incorrect number of validation messages returned!");

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
      Assert.AreEqual(2, messages.Count(), "Incorrect number of validation messages returned!");

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
      editContext.Validate();
      IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(person, "FirstName"));

      // Assert
      Assert.AreEqual(1, messages.Count(), "Incorrect number of validation messages returned!");

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
      editContext.Validate();
      IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(person, "HomeTelephone"));

      // Assert
      Assert.AreEqual(1, messages.Count(), "Incorrect number of validation messages returned!");

    }

    #region Helper Methods

    FakePerson GetValidFakePerson()
    {
      FakePerson person = FakePerson.NewFakePerson();

      person.LastName = "Smith";
      person.HomeTelephone = "01234 567890";

      return person;
    }

    #endregion

  }
}
