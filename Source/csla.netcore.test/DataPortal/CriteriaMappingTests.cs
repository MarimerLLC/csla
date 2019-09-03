//-----------------------------------------------------------------------
// <copyright file="CriteriaMappingTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class CriteriaMappingTests
  {
    [TestMethod]
    public void EmptyToEmptyArray()
    {
      Assert.IsTrue(GetCriteriaObject() is Server.EmptyCriteria);
      var array = GetCriteriaArray(GetCriteriaObject());
      Assert.AreEqual(0, array.Length);
    }

    [TestMethod]
    public void EmptyCriteriaToEmptyArray()
    {
      //Assert.IsTrue(GetCriteriaObject(Server.EmptyCriteria.Instance)[0] is Server.EmptyCriteria);
      var array = GetCriteriaArray(Server.EmptyCriteria.Instance);
      Assert.AreEqual(0, array.Length);
    }

    [TestMethod]
    public void EmptyArrayToEmptyArray()
    {
      var start = Array.Empty<object>();
      Assert.IsTrue(GetCriteriaObject(start) is Server.EmptyCriteria);
      var array = GetCriteriaArray(GetCriteriaObject(start));
      Assert.AreEqual(0, array.Length);
    }

    [TestMethod]
    public void NullToNullInArray()
    {
      object start = null;
      Assert.IsTrue(GetCriteriaObject(start) is Server.NullCriteria);
      var array = GetCriteriaArray(GetCriteriaObject(start));
      Assert.AreEqual(1, array.Length);
      Assert.IsNull(array[0]);
    }

    [TestMethod]
    public void IntToInt()
    {
      object start = 123;
      Assert.AreEqual(start, GetCriteriaArray(start)[0]);
      Assert.AreEqual(start, GetCriteriaObject(start));
    }

    private object GetCriteriaObject(params object[] parameters)
    {
      return Csla.Server.DataPortal.GetCriteriaFromArray(parameters);
    }

    private object[] GetCriteriaArray()
    {
      return Csla.Server.DataPortal.GetCriteriaArray(Server.EmptyCriteria.Instance);
    }

    private object[] GetCriteriaArray(object criteria)
    {
      return Csla.Server.DataPortal.GetCriteriaArray(criteria);
    }
  }
}
