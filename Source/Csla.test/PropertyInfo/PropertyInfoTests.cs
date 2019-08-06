//-----------------------------------------------------------------------
// <copyright file="PropertyInfoTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
#if !NUNIT
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.PropertyInfo
{
  [TestClass]
  public class PropertyInfoTests
  {
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
      Assert.AreEqual("x", PropertyInfoRoot.NameDefaultValueProperty.DefaultValue);
      Assert.AreEqual("x", PropertyInfoRoot.NewPropertyInfoRoot().NameDefaultValue);
    }
    
    [TestMethod]
    public void TestStringNullDefaultValue()
    {
      Assert.AreEqual(null, PropertyInfoRoot.StringNullDefaultValueProperty.DefaultValue);
      Assert.AreEqual(null, PropertyInfoRoot.NewPropertyInfoRoot().StringNullDefaultValue);
    }
  }
}