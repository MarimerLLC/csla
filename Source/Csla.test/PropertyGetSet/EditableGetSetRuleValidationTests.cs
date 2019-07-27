//-----------------------------------------------------------------------
// <copyright file="EditableGetSetValidationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using UnitDriven;
using Csla.Serialization.Mobile;
using Csla.Core;
using Csla.Serialization;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
namespace Csla.Test.PropertyGetSet
{
#if TESTING
  //[System.Diagnostics.DebuggerNonUserCode]
#endif
  [TestClass]
  public class EditableGetSetRuleValidationTests
  {
    #region Constant(s)

    private const string ID = "CSLA_TEST";

    #endregion

    #region Tests

    [TestMethod]
    public void InvalidGetValidationRulesOnProperties()
    {
      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation();

      try
      {
        // get the required property and verify that it has the value that it was set to.
        var result = egsv.MemberBackedIdWithNoRelationshipTypes;
        Assert.Fail("This property has a backing feild and an exception should of been thrown.");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(true, ex.Message);
      }
    }

    [TestMethod]
    public void InvalidSetValidationRulesOnProperties()
    {
      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation();

      try
      {
        // Set the required property and verify that it has the value that it was set to.
        egsv.MemberBackedIdWithNoRelationshipTypes = ID;
        Assert.Fail("This property has a backing feild and an exception should of been thrown.");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(true, ex.Message);
      }
    }

    [TestMethod]
    public void CheckValidationRulesOnProperties()
    {
      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation();

      // Verify that the id properties are not set.
      Assert.IsTrue(egsv.Id.Equals(string.Empty));
      Assert.IsTrue(egsv.MemberBackedId.Equals(string.Empty));

      try
      {
        Assert.IsTrue(egsv.MemberBackedIdWithNoRelationshipTypes.Equals(string.Empty));
        Assert.Fail("This property has a backing feild and an exception should of been thrown.");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(true, ex.Message);
      }

      // Verify that the object is not valid.
      Assert.IsFalse(egsv.IsValid, egsv.BrokenRulesCollection.ToString());

      // Set the required property and verify that it has the value that it was set to.
      egsv.Id = ID;
      Assert.IsTrue(ID.Equals(egsv.Id));

      // Set the required property and verify that it has the value that it was set to.
      egsv.MemberBackedId = ID;
      Assert.IsTrue(ID.Equals(egsv.MemberBackedId));

      try
      {
        // Set the required property and verify that it has the value that it was set to.
        egsv.MemberBackedIdWithNoRelationshipTypes = ID;
        Assert.IsTrue(ID.Equals(egsv.MemberBackedIdWithNoRelationshipTypes));
      }
      catch (Exception ex)
      {
        Assert.IsTrue(true, ex.Message);
      }

      // Verify that the object is not valid.
      Assert.IsFalse(egsv.IsValid, egsv.BrokenRulesCollection.ToString());
    }

    #endregion
  }
}
