//-----------------------------------------------------------------------
// <copyright file="PropertyInfoTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.PropertyInfo
{
  [TestClass]
  public class PropertyInfoTests
  {
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
      IDataPortal<PropertyInfoRoot> dataPortal = _testDIContext.CreateDataPortal<PropertyInfoRoot>();

      Assert.AreEqual("x", PropertyInfoRoot.NameDefaultValueProperty.DefaultValue);
      Assert.AreEqual("x", PropertyInfoRoot.NewPropertyInfoRoot(dataPortal).NameDefaultValue);
    }
    
    [TestMethod]
    public void TestStringNullDefaultValue()
    {
      IDataPortal<PropertyInfoRoot> dataPortal = _testDIContext.CreateDataPortal<PropertyInfoRoot>();

      Assert.AreEqual(null, PropertyInfoRoot.StringNullDefaultValueProperty.DefaultValue);
      Assert.AreEqual(null, PropertyInfoRoot.NewPropertyInfoRoot(dataPortal).StringNullDefaultValue);
    }
  }
}