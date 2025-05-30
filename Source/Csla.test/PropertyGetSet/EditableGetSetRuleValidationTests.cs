//-----------------------------------------------------------------------
// <copyright file="EditableGetSetValidationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    #region Tests

    [TestMethod]
    public void InvalidGetValidationRulesOnProperties()
    {
      IDataPortal<EditableGetSetRuleValidation> dataPortal = _testDIContext.CreateDataPortal<EditableGetSetRuleValidation>();

      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation(dataPortal);

      var ex = Assert.ThrowsException<Exception>(() => 
      {
        // get the required property and verify that it has the value that it was set to.
        var result = egsv.MemberBackedIdWithNoRelationshipTypes;
      });
      Assert.IsNotNull(ex.Message);
    }

    [TestMethod]
    public void InvalidSetValidationRulesOnProperties()
    {
      IDataPortal<EditableGetSetRuleValidation> dataPortal = _testDIContext.CreateDataPortal<EditableGetSetRuleValidation>();

      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation(dataPortal);

      var ex = Assert.ThrowsException<Exception>(() => 
      {
        // Set the required property and verify that it has the value that it was set to.
        egsv.MemberBackedIdWithNoRelationshipTypes = ID;
      });
      Assert.IsNotNull(ex.Message);
    }

    [TestMethod]
    public void CheckValidationRulesOnProperties()
    {
      IDataPortal<EditableGetSetRuleValidation> dataPortal = _testDIContext.CreateDataPortal<EditableGetSetRuleValidation>();

      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation(dataPortal);

      // Verify that the id properties are not set.
      Assert.IsTrue(egsv.Id.Equals(string.Empty));
      Assert.IsTrue(egsv.MemberBackedId.Equals(string.Empty));

      var ex = Assert.ThrowsException<Exception>(() => 
      {
        Assert.IsTrue(egsv.MemberBackedIdWithNoRelationshipTypes.Equals(string.Empty));
      });
      Assert.IsNotNull(ex.Message);

      // Verify that the object is not valid.
      Assert.IsFalse(egsv.IsValid, egsv.BrokenRulesCollection.ToString());

      // Set the required property and verify that it has the value that it was set to.
      egsv.Id = ID;
      Assert.IsTrue(ID.Equals(egsv.Id));

      // Set the required property and verify that it has the value that it was set to.
      egsv.MemberBackedId = ID;
      Assert.IsTrue(ID.Equals(egsv.MemberBackedId));

      var ex = Assert.ThrowsException<Exception>(() => 
      {
        // Set the required property and verify that it has the value that it was set to.
        egsv.MemberBackedIdWithNoRelationshipTypes = ID;
        Assert.IsTrue(ID.Equals(egsv.MemberBackedIdWithNoRelationshipTypes));
      });
      Assert.IsNotNull(ex.Message);

      // Verify that the object is not valid.
      Assert.IsFalse(egsv.IsValid, egsv.BrokenRulesCollection.ToString());
    }

    #endregion
  }
}
