//-----------------------------------------------------------------------
// <copyright file="PropertyInfoTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.PropertyInfo
{
  [TestClass]
  public class PropertyInfoTests
  {
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

    [TestMethod]
    public void TestName()
    {
      Assert.AreEqual(PropertyInfoRoot._nameProperty.Name, PropertyInfoRoot._nameProperty.FriendlyName);
    }
    
    [TestMethod]
    public void TestNameDataAnnotations()
    {
      Assert.AreEqual("Name: DataAnnotations", PropertyInfoRoot._nameDataAnnotationsProperty.FriendlyName);
    }

    [TestMethod]
    public void TestNameComponentModel()
    {
      Assert.AreEqual("Name: ComponentModel", PropertyInfoRoot._nameComponentModelProperty.FriendlyName);
    }

    [TestMethod]
    public void TestNameFriendlyName()
    {
      Assert.AreEqual("Name: Friendly Name", PropertyInfoRoot._nameFriendlyNameProperty.FriendlyName);
    }

    [TestMethod]
    public void TestDefaultValue()
    {
      IDataPortal<PropertyInfoRoot> dataPortal = _testHost.GetDataPortal<PropertyInfoRoot>();

      Assert.AreEqual("x", PropertyInfoRoot.NameDefaultValueProperty.DefaultValue);
      Assert.AreEqual("x", PropertyInfoRoot.NewPropertyInfoRoot(dataPortal).NameDefaultValue);
    }

    [TestMethod]
    public void TestStringNullDefaultValue()
    {
      IDataPortal<PropertyInfoRoot> dataPortal = _testHost.GetDataPortal<PropertyInfoRoot>();

      Assert.AreEqual(null, PropertyInfoRoot.StringNullDefaultValueProperty.DefaultValue);
      Assert.AreEqual(null, PropertyInfoRoot.NewPropertyInfoRoot(dataPortal).StringNullDefaultValue);
    }

    [TestMethod]
    public void TestContainingType()
    {
      Assert.IsTrue(ReferenceEquals(typeof(PropertyInfoRoot).GetProperty(nameof(PropertyInfoRoot.ContainingType)), PropertyInfoRoot.ContainingTypeProperty.GetPropertyInfo()));      
      Assert.IsTrue(ReferenceEquals(null, PropertyInfoRoot.ContainingTypeNullProperty.GetPropertyInfo()));
    }
  }
}