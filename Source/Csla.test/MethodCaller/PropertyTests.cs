//-----------------------------------------------------------------------
// <copyright file="PropertyTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitDriven;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Csla.Reflection;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.MethodCaller
{
  [TestClass]
  public class PropertyTests
  {
    private string one;
    private string three;

    public string Test1 { get { return "one"; } }

    public string Test2 { get { return "two"; } set { one = value; } }

    public string Test3 { set { three = value; } }

    [TestMethod]
    public void PropertyGetNoSetSuccess()
    {
      var expected = "one";
      var actual = Csla.Reflection.MethodCaller.CallPropertyGetter(this, "Test1");
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void PropertyGetWithSetSuccess()
    {
      var expected = "two";
      var actual = Csla.Reflection.MethodCaller.CallPropertyGetter(this, "Test2");
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PropertyGetOnlySetFail()
    {
      Csla.Reflection.MethodCaller.CallPropertyGetter(this, "Test3");
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void PropertySetOnlyGetFail()
    {
      Csla.Reflection.MethodCaller.CallPropertySetter(this, "Test1", "fail");
    }

    [TestMethod]
    public void PropertySetWithGetSuccess()
    {
      var expected = "two";
      Csla.Reflection.MethodCaller.CallPropertySetter(this, "Test2", expected);
      Assert.AreEqual(expected, this.one);
    }

    [TestMethod]
    public void PropertySetNoGetSuccess()
    {
      var expected = "three";
      Csla.Reflection.MethodCaller.CallPropertySetter(this, "Test3", expected);
      Assert.AreEqual(expected, this.three);
    }

  }
}