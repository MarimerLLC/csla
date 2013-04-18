//-----------------------------------------------------------------------
// <copyright file="PropertyGetSetTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

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
  public class NullStringPropertyGetSetTests
  {
#if SILVERLIGHT
    [TestInitialize]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = "Local"; // "Csla.DataPortalClient.WcfProxy, Csla";
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows;
    }
#else
    [TestInitialize]
    public void Initialize()
    {
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows;
    }
#endif

    [TestCleanup]
    public void Cleanup()
    {
    }

    #region SetProperty and variations overrides - initialized EditableRoot

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringManaged_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesSetManaged, "Freshly initialized string property should be string.empty");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringBacking_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesSetBacking, "Freshly initialized string property should be string.empty");
    }

    [TestMethod]
    public void NewEditableRoot_SetConvertPropertyManagedSmartDate_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesSetConvertManagedSmartDate, "should be empty");
    }

    [TestMethod]
    public void NewEditableRoot_SetConvertPropertyBackingSmartDate_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesSetConvertBackingSmartDate, "should be empty");
    }

    [Ignore]
    [TestMethod]
    public void NewEditableRoot_SetConvertPropertyManagedObject_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesSetConvertManagedObject, "should be empty");
    }

    [Ignore]
    [TestMethod]
    public void NewEditableRoot_SetConvertPropertyBackingObject_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesSetConvertBackingObject, "should be empty");
    }

    #endregion

    #region SetProperty and variations overrides - manual set non-null values

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringManaged_AssignedValue_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetManaged = "Test String";
      Assert.AreEqual("Test String", item.UsesSetManaged, "Should have value");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringBacking_AssignedValue_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetBacking = "Test String";
      Assert.AreEqual("Test String", item.UsesSetBacking, "should have value");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertManagedSmartDate_AssignedValue_Equals_NonEmpty()
    {
      var testDate = new DateTime(2013, 4, 1);
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertManagedSmartDate = testDate.ToString();
      Assert.AreEqual(testDate, DateTime.Parse(item.UsesSetConvertManagedSmartDate));
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertBackingSmartDate_AssignedValue_Equals_NonEmpty()
    {
      var testDate = new DateTime(2013, 4, 1);
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertBackingSmartDate = testDate.ToString();
      Assert.AreEqual(testDate, DateTime.Parse(item.UsesSetConvertBackingSmartDate));
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertManagedObject_AssignedValue_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertManagedObject = "Test String";
      Assert.AreEqual("Test String", item.UsesSetConvertManagedObject);
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertBacking_AssignedValue_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertBackingObject = "Test String";
      Assert.AreEqual("Test String", item.UsesSetConvertBackingObject);
    }

    #endregion

    #region SetProperty and variations overrides - manual set null, returns empty string

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringManaged_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetManaged = null;
      Assert.AreEqual(string.Empty, item.UsesSetManaged, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringBacking_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetBacking = null;
      Assert.AreEqual(string.Empty, item.UsesSetBacking, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertManagedSmartDate_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertManagedSmartDate = null;
      Assert.AreEqual(string.Empty, item.UsesSetConvertManagedSmartDate);
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertBackingSmartDate_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertBackingSmartDate = null;
      Assert.AreEqual(string.Empty, item.UsesSetConvertBackingSmartDate);
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertManagedObject_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertManagedObject = null;
      Assert.AreEqual(string.Empty, item.UsesSetConvertManagedObject);
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertBackingObject_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertBackingObject = null;
      Assert.AreEqual(string.Empty, item.UsesSetConvertBackingObject);
    }

    #endregion

    #region SetProperty and variations overrides - manual set value then null, returns empty string

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringManaged_AssignedValueThenNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetManaged = "Initial String";
      item.UsesSetManaged = null;
      Assert.AreEqual(string.Empty, item.UsesSetManaged, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringBacking_AssignedValueThenNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetBacking = "Initial String";
      item.UsesSetBacking = null;
      Assert.AreEqual(string.Empty, item.UsesSetBacking, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertManagedSmartDate_AssignedValueThenNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertManagedSmartDate = new DateTime(2013, 4, 18).ToString();
      item.UsesSetConvertManagedSmartDate = null;
      Assert.AreEqual(string.Empty, item.UsesSetConvertManagedSmartDate);
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertBackingSmartDate_AssignedValueThenNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertBackingSmartDate = new DateTime(2013, 4, 18).ToString();
      item.UsesSetConvertBackingSmartDate = null;
      Assert.AreEqual(string.Empty, item.UsesSetConvertBackingSmartDate);
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertManagedObject_AssignedValueThenNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertManagedObject = "Initial String";
      item.UsesSetConvertManagedObject = null;
      Assert.AreEqual(string.Empty, item.UsesSetConvertManagedObject);
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertBackingObject_AssignedValueThenNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertBackingObject = "Initial String";
      item.UsesSetConvertBackingObject = null;
      Assert.AreEqual(string.Empty, item.UsesSetConvertBackingObject);
    }

    #endregion

    #region SetProperty and variations overrides - manual set null then value, returns value

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringManaged_AssignedNullThenValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetManaged = null;
      item.UsesSetManaged = "My String";
      Assert.AreEqual("My String", item.UsesSetManaged, "should have value");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyOfStringBacking_AssignedNullThenValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetBacking = null;
      item.UsesSetBacking = "My String";
      Assert.AreEqual("My String", item.UsesSetBacking, "should have value");
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertManagedSmartDate_AssignedNullThenValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertManagedSmartDate = null;
      item.UsesSetConvertManagedSmartDate = new DateTime(2013, 4, 18).ToString();
      Assert.AreEqual(new DateTime(2013, 4, 18), DateTime.Parse(item.UsesSetConvertManagedSmartDate));
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertBackingSmartDate_AssignedNullThenValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertBackingSmartDate = null;
      item.UsesSetConvertBackingSmartDate = new DateTime(2013, 4, 18).ToString();
      Assert.AreEqual(new DateTime(2013, 4, 18), DateTime.Parse(item.UsesSetConvertBackingSmartDate));
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertManagedObject_AssignedNullThenValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertManagedObject = null;
      item.UsesSetConvertManagedObject = "My String";
      Assert.AreEqual("My String", item.UsesSetConvertManagedObject);
    }

    [TestMethod]
    public void NewEditableRoot_SetPropertyConvertBackingObject_AssignedNullThenValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesSetConvertBackingObject = null;
      item.UsesSetConvertBackingObject = "My String";
      Assert.AreEqual("My String", item.UsesSetConvertBackingObject);
    }

    #endregion

    #region LoadProperty and variations overrides - initialized EditableRoot

    [TestMethod]
    public void NewEditableRoot_LoadProperty_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesLoad, "Freshly initialized string property should be string.empty");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyMarkDirty_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesLoadMarkDirty, "Freshly initialized string property should be string.empty");
    }

    [Ignore]
    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertSmartDate_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesLoadConvertSmartDate, "Freshly initialized string property should be string.empty");
    }

    [Ignore]
    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertObject_Initially_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      Assert.AreEqual(string.Empty, item.UsesLoadConvertObject, "Freshly initialized string property should be string.empty");
    }

    #endregion

    #region LoadProperty and variations override - manual set non-null values

    [TestMethod]
    public void NewEditableRoot_LoadProperty_AssignedValue_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoad = "Test String";
      Assert.AreEqual("Test String", item.UsesLoad, "Should have value");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyMarkDirty_AssignedValue_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadMarkDirty = "Test String";
      Assert.AreEqual("Test String", item.UsesLoadMarkDirty, "Should have value");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertSmartDate_AssignedValue_Equals_NonEmpty()
    {
      var testDate = new DateTime(2013, 4, 1);
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadConvertSmartDate = testDate.ToString();
      Assert.AreEqual(testDate, DateTime.Parse(item.UsesLoadConvertSmartDate));
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertObject_AssignedValue_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadConvertObject = "Test String";
      Assert.AreEqual("Test String", item.UsesLoadConvertObject, "Should have value");
    }

    #endregion

    #region LoadProperty and variations overrides - manual set null, returns empty string

    [TestMethod]
    public void NewEditableRoot_LoadProperty_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoad = (string)null;
      Assert.AreEqual(string.Empty, item.UsesLoad, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyMarkDirty_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadMarkDirty = (string)null;
      Assert.AreEqual(string.Empty, item.UsesLoadMarkDirty, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertSmartDate_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadConvertSmartDate = (string)null;
      Assert.AreEqual(string.Empty, item.UsesLoadConvertSmartDate, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertObject_AssignedNull_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadConvertObject = (string)null;
      Assert.AreEqual(string.Empty, item.UsesLoadConvertObject, "Should be string.Empty");
    }

    #endregion

    #region LoadProperty and variations overrides - manual set value then null, returns empty string

    [TestMethod]
    public void NewEditableRoot_LoadProperty_AssignedValueThanNull_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoad = "Test String";
      item.UsesLoad = (string)null;
      Assert.AreEqual(string.Empty, item.UsesLoad, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyMarkDirty_AssignedValueThanNull_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadMarkDirty = "Test String";
      item.UsesLoadMarkDirty = (string)null;
      Assert.AreEqual(string.Empty, item.UsesLoadMarkDirty, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertSmartDate_AssignedValueThanNull_Equals_NonEmpty()
    {
      var testDate = new DateTime(2013, 4, 1);
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadConvertSmartDate = testDate.ToString();
      item.UsesLoadConvertSmartDate = (string)null;
      Assert.AreEqual(string.Empty, item.UsesLoadConvertSmartDate, "Should be string.Empty");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertObject_AssignedValueThanNull_Equals_NonEmpty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadConvertObject = "Test String";
      item.UsesLoadConvertObject = (string)null;
      Assert.AreEqual(string.Empty, item.UsesLoadConvertObject, "Should be string.Empty");
    }
    #endregion

    #region LoadProperty and variations override - manual set null then value, returns value

    [TestMethod]
    public void NewEditableRoot_LoadProperty_AssignedNullThanValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoad = (string)null;
      item.UsesLoad = "Test String";
      Assert.AreEqual("Test String", item.UsesLoad, "Should have value");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyMarkDirty_AssignedNullThanValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadMarkDirty = (string)null;
      item.UsesLoadMarkDirty = "Test String";
      Assert.AreEqual("Test String", item.UsesLoadMarkDirty, "Should have value");
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertSmartDate_AssignedNullThanValue_Equals_Empty()
    {
      var testDate = new DateTime(2013, 4, 1);
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadConvertSmartDate = (string)null;
      item.UsesLoadConvertSmartDate = testDate.ToString();
      Assert.AreEqual(testDate, DateTime.Parse(item.UsesLoadConvertSmartDate));
    }

    [TestMethod]
    public void NewEditableRoot_LoadPropertyConvertObject_AssignedNullThanValue_Equals_Empty()
    {
      var item = NullStringEditableRoot.NewEditableRoot();
      item.UsesLoadConvertObject = (string)null;
      item.UsesLoadConvertObject = "Test String";
      Assert.AreEqual("Test String", item.UsesLoadConvertObject, "Should have value");
    }
    #endregion

  }
}
