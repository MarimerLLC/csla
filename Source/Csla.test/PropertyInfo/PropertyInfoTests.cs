//-----------------------------------------------------------------------
// <copyright file="PropertyInfoTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
    private System.Diagnostics.Stopwatch _watch;

    [TestInitialize]
    public void Setup()
    {
      _watch = new Stopwatch();
      _watch.Start();
    }

    [TestCleanup]
    public void Cleanup()
    {
      _watch = null;
    }

    [TestMethod]
    public void TestName()
    {
      Assert.IsNotNull(PropertyInfoRoot._nameProperty.DefaultValue);
      Assert.IsTrue(PropertyInfoRoot._nameProperty.Name == PropertyInfoRoot._nameProperty.FriendlyName);

      _watch.Stop();
      Console.WriteLine("Test took {0}ms to complete.", _watch.ElapsedMilliseconds);
    }
    
    [TestMethod]
    public void TestNameDataAnnotations()
    {
      _watch.Stop();
      Console.WriteLine("Test took {0}ms to complete.", _watch.ElapsedMilliseconds);
    }

    [TestMethod]
    public void TestNameComponentModel()
    {
      Assert.IsNotNull(PropertyInfoRoot._nameComponentModelProperty.DefaultValue);
      Assert.IsTrue(PropertyInfoRoot._nameComponentModelProperty.FriendlyName == "Name: ComponentModel");

      _watch.Stop();
      Console.WriteLine("Test took {0}ms to complete.", _watch.ElapsedMilliseconds);

    }

    [TestMethod]
    public void TestNameFriendlyName()
    {
      Assert.IsNotNull(PropertyInfoRoot._nameFriendlyNameProperty.DefaultValue);
      Assert.IsTrue(PropertyInfoRoot._nameFriendlyNameProperty.FriendlyName == "Name: Friendly Name");

      _watch.Stop();
      Console.WriteLine("Test took {0}ms to complete.", _watch.ElapsedMilliseconds);
    }

    [TestMethod]
    public void TestNameDefaultValue()
    {
      Assert.AreEqual("x", PropertyInfoRoot.NameDefaultValueProperty.DefaultValue);
      Assert.IsTrue(PropertyInfoRoot.NameDefaultValueProperty.Name == PropertyInfoRoot.NameDefaultValueProperty.FriendlyName);

      _watch.Stop();
      Console.WriteLine("Test took {0}ms to complete.", _watch.ElapsedMilliseconds);
    }
  }
}