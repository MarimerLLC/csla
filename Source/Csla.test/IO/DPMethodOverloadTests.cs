//-----------------------------------------------------------------------
// <copyright file="DPMethodOverloadTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.IO
{
  [TestClass]
  public class DPMethodOverloadTests
  {
    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void CreateNoCriteria()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      TestBizObj obj;

      obj = TestBizObj.NewNoCriteria();
      Assert.IsNotNull(obj, "Failed to get object with no criteria");
      Assert.AreEqual("No criteria", Csla.ApplicationContext.GlobalContext["Create"], "No criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void CreateWithCriteria()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      TestBizObj obj;

      obj = TestBizObj.NewCriteria();
      Assert.IsNotNull(obj, "Failed to get object with criteria");
      Assert.AreEqual("Criteria", Csla.ApplicationContext.GlobalContext["Create"], "Criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void CreateWithOtherCriteria()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      TestBizObj obj;

      obj = TestBizObj.NewOtherCriteria();
      Assert.IsNotNull(obj, "Failed to get object with other criteria");
      Assert.AreEqual("Other criteria", Csla.ApplicationContext.GlobalContext["Create"], "Other criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void FetchNullCriteria()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      TestBizObj obj;

      obj = TestBizObj.GetNullCriteria();
      Assert.IsNotNull(obj, "Failed to get object with null criteria");
      Assert.AreEqual("Null criteria", Csla.ApplicationContext.GlobalContext["Fetch"], "Null criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void FetchNoCriteria()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      TestBizObj obj;

      obj = TestBizObj.GetNoCriteria();
      Assert.IsNotNull(obj, "Failed to get object with no criteria");
      Assert.AreEqual("No criteria", Csla.ApplicationContext.GlobalContext["Fetch"], "No criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void FetchWithCriteria()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      TestBizObj obj;

      obj = TestBizObj.GetCriteria();
      Assert.IsNotNull(obj, "Failed to get object with criteria");
      Assert.AreEqual("Criteria", Csla.ApplicationContext.GlobalContext["Fetch"], "Criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void FetchWithOtherCriteria()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      TestBizObj obj;

      obj = TestBizObj.GetOtherCriteria();
      Assert.IsNotNull(obj, "Failed to get object with other criteria");
      Assert.AreEqual("Other criteria", Csla.ApplicationContext.GlobalContext["Fetch"], "Other criteria expected");
    }
  }

  [Serializable]
  public class TestBizObj : Csla.BusinessBase<TestBizObj>
  {
    protected override object GetIdValue()
    {
      return 0;
    }

    public static TestBizObj NewNoCriteria()
    {
      return Csla.DataPortal.Create<TestBizObj>();
    }

    public static TestBizObj NewCriteria()
    {
      return Csla.DataPortal.Create<TestBizObj>(new Criteria());
    }

    public static TestBizObj NewOtherCriteria()
    {
      return Csla.DataPortal.Create<TestBizObj>(new OtherCriteria());
    }

    public static TestBizObj GetNoCriteria()
    {
      return Csla.DataPortal.Fetch<TestBizObj>();
    }

    public static TestBizObj GetNullCriteria()
    {
      return Csla.DataPortal.Fetch<TestBizObj>(null);
    }

    public static TestBizObj GetCriteria()
    {
      return Csla.DataPortal.Fetch<TestBizObj>(new Criteria());
    }

    public static TestBizObj GetOtherCriteria()
    {
      return Csla.DataPortal.Fetch<TestBizObj>(new OtherCriteria());
    }

    [Serializable]
    private class Criteria
    {
    }

    [Serializable]
    private class OtherCriteria
    {
    }

    protected override void DataPortal_Create()
    {
      Csla.ApplicationContext.GlobalContext["Create"] = "No criteria";
    }

    private void DataPortal_Create(object criteria)
    {
      if (criteria == null)
        Csla.ApplicationContext.GlobalContext["Create"] = "null criteria";
      else
        Csla.ApplicationContext.GlobalContext["Create"] = "Other criteria";
    }

    private void DataPortal_Create(Criteria criteria)
    {
      Csla.ApplicationContext.GlobalContext["Create"] = "Criteria";
    }

    private void DataPortal_Fetch()
    {
      Csla.ApplicationContext.GlobalContext["Fetch"] = "No criteria";
    }

    protected void DataPortal_Fetch(object criteria)
    {
      if (criteria == null)
        Csla.ApplicationContext.GlobalContext["Fetch"] = "Null criteria";
      else
        Csla.ApplicationContext.GlobalContext["Fetch"] = "Other criteria";
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      Csla.ApplicationContext.GlobalContext["Fetch"] = "Criteria";
    }
  }
}