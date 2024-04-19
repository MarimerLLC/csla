//-----------------------------------------------------------------------
// <copyright file="AppDomainTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.AppDomainTests
{
  [TestClass]
  public class AppDomainTestClass
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void AppDomainTestIsCalled()
    {
      IDataPortal<Basic.Root> dataPortal = _testDIContext.CreateDataPortal<Basic.Root>();

      TestResults.Reinitialise();
      int local = AppDomain.CurrentDomain.Id;
      Basic.Root r = dataPortal.Create(new Basic.Root.Criteria());
      int remote = r.CreatedDomain;

      if (System.Configuration.ConfigurationManager.AppSettings["CslaDataPortalProxy"] == null)
        Assert.AreEqual(local, remote, "Local and Remote AppDomains should be the same");
      else
        Assert.IsFalse((local == remote), "Local and Remote AppDomains should be different");

    }

    [TestCleanup]
    public void ClearContextsAfterEachTest()
    {
      TestResults.Reinitialise();
    }
  }
}