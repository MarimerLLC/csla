//-----------------------------------------------------------------------
// <copyright file="EditableGetSetValidationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
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

    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
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
      IDataPortal<EditableGetSetRuleValidation> dataPortal = _testHost.GetDataPortal<EditableGetSetRuleValidation>();

      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation(dataPortal);

      // The property has a private backing field but isn't registered with RelationshipTypes.PrivateField.
      var ex = Assert.ThrowsExactly<InvalidOperationException>(() => egsv.MemberBackedIdWithNoRelationshipTypes);
      Assert.Contains(nameof(RelationshipTypes.PrivateField), ex.Message);
    }

    [TestMethod]
    public void InvalidSetValidationRulesOnProperties()
    {
      IDataPortal<EditableGetSetRuleValidation> dataPortal = _testHost.GetDataPortal<EditableGetSetRuleValidation>();

      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation(dataPortal);

      // The property has a private backing field but isn't registered with RelationshipTypes.PrivateField.
      // SetProperty wraps the InvalidOperationException in a PropertyLoadException.
      var ex = Assert.ThrowsExactly<PropertyLoadException>(() => egsv.MemberBackedIdWithNoRelationshipTypes = ID);
      Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public void CheckValidationRulesOnProperties()
    {
      IDataPortal<EditableGetSetRuleValidation> dataPortal = _testHost.GetDataPortal<EditableGetSetRuleValidation>();

      EditableGetSetRuleValidation egsv = EditableGetSetRuleValidation.NewEditableGetSetValidation(dataPortal);

      // Verify that the id properties are not set.
      Assert.IsTrue(egsv.Id.Equals(string.Empty));
      Assert.IsTrue(egsv.MemberBackedId.Equals(string.Empty));

      // A private backing field without RelationshipTypes.PrivateField can't be read.
      Assert.ThrowsExactly<InvalidOperationException>(() => egsv.MemberBackedIdWithNoRelationshipTypes);

      // Verify that the object is not valid.
      Assert.IsFalse(egsv.IsValid, egsv.BrokenRulesCollection.ToString());

      // Set the required property and verify that it has the value that it was set to.
      egsv.Id = ID;
      Assert.IsTrue(ID.Equals(egsv.Id));

      // Set the required property and verify that it has the value that it was set to.
      egsv.MemberBackedId = ID;
      Assert.IsTrue(ID.Equals(egsv.MemberBackedId));

      // It can't be set either, so its required rule stays broken.
      var ex = Assert.ThrowsExactly<PropertyLoadException>(() => egsv.MemberBackedIdWithNoRelationshipTypes = ID);
      Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);

      // Verify that the object is not valid.
      Assert.IsFalse(egsv.IsValid, egsv.BrokenRulesCollection.ToString());
    }

    #endregion
  }
}
