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
using Csla.TestHelpers;

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
    private TestDIContext _testDIContext;

    [TestInitialize]
    public void TestInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }
    
    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void CreateNoCriteria()
    {
      IDataPortal<TestBizObj> dataPortal = _testDIContext.CreateDataPortal<TestBizObj>();

      TestResults.Reinitialise();

      TestBizObj obj;

      obj = TestBizObj.NewNoCriteria(dataPortal);
      Assert.IsNotNull(obj, "Failed to get object with no criteria");
      Assert.AreEqual("No criteria", TestResults.GetResult("Create"), "No criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void CreateWithCriteria()
    {
      IDataPortal<TestBizObj> dataPortal = _testDIContext.CreateDataPortal<TestBizObj>();

      TestResults.Reinitialise();

      TestBizObj obj;

      obj = TestBizObj.NewCriteria(dataPortal);
      Assert.IsNotNull(obj, "Failed to get object with criteria");
      Assert.AreEqual("Criteria", TestResults.GetResult("Create"), "Criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void CreateWithOtherCriteria()
    {
      IDataPortal<TestBizObj> dataPortal = _testDIContext.CreateDataPortal<TestBizObj>();

      TestResults.Reinitialise();

      TestBizObj obj;

      obj = TestBizObj.NewOtherCriteria(dataPortal);
      Assert.IsNotNull(obj, "Failed to get object with other criteria");
      Assert.AreEqual("Other criteria", TestResults.GetResult("Create"), "Other criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void FetchNullCriteria()
    {
      IDataPortal<TestBizObj> dataPortal = _testDIContext.CreateDataPortal<TestBizObj>();

      TestResults.Reinitialise();

      TestBizObj obj;

      obj = TestBizObj.GetNullCriteria(dataPortal);
      Assert.IsNotNull(obj, "Failed to get object with null criteria");
      Assert.AreEqual("Null criteria", TestResults.GetResult("Fetch"), "Null criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void FetchNoCriteria()
    {
      IDataPortal<TestBizObj> dataPortal = _testDIContext.CreateDataPortal<TestBizObj>();

      TestResults.Reinitialise();

      TestBizObj obj;

      obj = TestBizObj.GetNoCriteria(dataPortal);
      Assert.IsNotNull(obj, "Failed to get object with no criteria");
      Assert.AreEqual("No criteria", TestResults.GetResult("Fetch"), "No criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void FetchWithCriteria()
    {
      IDataPortal<TestBizObj> dataPortal = _testDIContext.CreateDataPortal<TestBizObj>();

      TestResults.Reinitialise();

      TestBizObj obj;

      obj = TestBizObj.GetCriteria(dataPortal);
      Assert.IsNotNull(obj, "Failed to get object with criteria");
      Assert.AreEqual("Criteria", TestResults.GetResult("Fetch"), "Criteria expected");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void FetchWithOtherCriteria()
    {
      IDataPortal<TestBizObj> dataPortal = _testDIContext.CreateDataPortal<TestBizObj>();

      TestResults.Reinitialise();

      TestBizObj obj;

      obj = TestBizObj.GetOtherCriteria(dataPortal);
      Assert.IsNotNull(obj, "Failed to get object with other criteria");
      Assert.AreEqual("Other criteria", TestResults.GetResult("Fetch"), "Other criteria expected");
    }
  }

  [Serializable]
  public class TestBizObj : Csla.BusinessBase<TestBizObj>
  {
    protected override object GetIdValue()
    {
      return 0;
    }

    public static TestBizObj NewNoCriteria(IDataPortal<TestBizObj> dataPortal)
    {
      return dataPortal.Create();
    }

    public static TestBizObj NewCriteria(IDataPortal<TestBizObj> dataPortal)
    {
      return dataPortal.Create(new Criteria());
    }

    public static TestBizObj NewOtherCriteria(IDataPortal<TestBizObj> dataPortal)
    {
      return dataPortal.Create(new OtherCriteria());
    }

    public static TestBizObj GetNoCriteria(IDataPortal<TestBizObj> dataPortal)
    {
      return dataPortal.Fetch();
    }

    public static TestBizObj GetNullCriteria(IDataPortal<TestBizObj> dataPortal)
    {
      return dataPortal.Fetch(null);
    }

    public static TestBizObj GetCriteria(IDataPortal<TestBizObj> dataPortal)
    {
      return dataPortal.Fetch(new Criteria());
    }

    public static TestBizObj GetOtherCriteria(IDataPortal<TestBizObj> dataPortal)
    {
      return dataPortal.Fetch(new OtherCriteria());
    }

    [Serializable]
    private class Criteria
    {
    }

    [Serializable]
    private class OtherCriteria
    {
    }

    [Create]
		protected void DataPortal_Create()
    {
      TestResults.Add("Create", "No criteria");
      BusinessRules.CheckRules();
    }

    [Create]
    private void DataPortal_Create(object criteria)
    {
      if (criteria == null)
        TestResults.Add("Create", "null criteria");
      else
        TestResults.Add("Create", "Other criteria");

      BusinessRules.CheckRules();
    }

    [Create]
    private void DataPortal_Create(Criteria criteria)
    {
      TestResults.Add("Create", "Criteria");
      BusinessRules.CheckRules();
    }

    private void DataPortal_Fetch()
    {
      TestResults.Add("Fetch", "No criteria");
    }

    protected void DataPortal_Fetch(object criteria)
    {
      if (criteria == null)
        TestResults.Add("Fetch", "Null criteria");
      else
        TestResults.Add("Fetch", "Other criteria");
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      TestResults.Add("Fetch", "Criteria");
    }
  }
}