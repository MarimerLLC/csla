//-----------------------------------------------------------------------
// <copyright file="BypassPropertyChecksTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using System;
using UnitDriven;
using System.Security.Claims;
using Csla.TestHelpers;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


namespace Csla.Test.BypassPropertyChecks
{
  [TestClass]
  public class BypassPropertyChecksTests : TestBase
  {
    private static ClaimsPrincipal GetPrincipal(params string[] roles)
    {
      var identity = new ClaimsIdentity();
      foreach (var item in roles)
        identity.AddClaim(new Claim(ClaimTypes.Role, item));
      return new ClaimsPrincipal(identity);
    }

    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void TestBypassReadWriteWithRightsTurnNotificationBackOn()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadIdByPass(1);
      context.Assert.AreEqual(1, testObj.ReadIdByPass());
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.AreEqual(false, testObj.IsDirty);

      testObj.LoadIdByNestedPass(3);
      context.Assert.AreEqual(3, testObj.ReadIdByPass());
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.AreEqual(false, testObj.IsDirty);

      testObj.LoadId(2);
      context.Assert.AreEqual(true, propertyChangedFired);
      context.Assert.AreEqual(2, testObj.ReadId());
      context.Assert.AreEqual(true, testObj.IsDirty);

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassReadWriteNoRightsTurnNotificationBackOn()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      bool propertyChangedFired = false;

      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId2ByPass(1);
      context.Assert.AreEqual(1, testObj.ReadId2ByPass());
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.AreEqual(false, testObj.IsDirty);
      testObj.LoadId2(2);
      context.Assert.AreEqual(1, testObj.ReadId2ByPass()); // still one becuase set failed
      context.Assert.AreEqual(true, testObj.IsDirty);
      context.Assert.Success();
      context.Complete();
    }


    [TestMethod]
    public void TestBypassReadWriteNoRights()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      bool propertyChangedFired = false;

      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (o, e) =>
        {
          propertyChangedFired = true;
        };
      testObj.LoadId2ByPass(1);
      context.Assert.AreEqual(1, testObj.ReadId2ByPass());
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.AreEqual(false, testObj.IsDirty);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadWriteWithRights()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId(1);
      context.Assert.AreEqual(1, testObj.ReadId());
      context.Assert.AreEqual(true, propertyChangedFired);
      context.Assert.AreEqual(true, testObj.IsDirty);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassWriteNoRightsDoNotBypass()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      BypassBusinessBase testObj = dataPortal.Fetch();
      bool propertyChangedFired = false;
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId2(1);
      context.Assert.AreEqual(1, testObj.ReadId2ByPass());
      context.Assert.AreEqual(true, propertyChangedFired);
      context.Assert.AreEqual(true, testObj.IsDirty);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadNoRightsDoNotBypass()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId2ByPass(1);
      context.Assert.AreEqual(false, testObj.IsDirty);
      context.Assert.AreEqual(1, testObj.ReadId2ByPass());
      context.Assert.AreEqual(0, testObj.ReadId2()); // 0 becuase we cannot read
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.Success();
      context.Complete();
    }


    
    [TestMethod]
    public void TestBypassReadWriteNoRightsBackingField()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      bool propertyChangedFired = false;

      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId4ByPass(1);
      context.Assert.AreEqual(false, testObj.IsDirty);
      context.Assert.AreEqual(1, testObj.ReadId4ByPass());
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadWriteWithRightsBackingField()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId3(1);
      context.Assert.AreEqual(true, testObj.IsDirty);
      context.Assert.AreEqual(1, testObj.ReadId3());
      context.Assert.AreEqual(true, propertyChangedFired);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassWriteNoRightsDoNotBypassBackingField()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      UnitTestContext context = GetContext();
      BypassBusinessBase testObj = dataPortal.Fetch();
      bool propertyChangedFired = false;
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId4(1);
       context.Assert.AreEqual(true, testObj.IsDirty);
      context.Assert.AreEqual(1, testObj.ReadId4ByPass());
      context.Assert.AreEqual(true, propertyChangedFired);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadNoRightsDoNotBypassBackingField()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();
      
      UnitTestContext context = GetContext();
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId4ByPass(1);
      context.Assert.AreEqual(false, testObj.IsDirty);
      context.Assert.AreEqual(1, testObj.ReadId4ByPass());
      context.Assert.AreEqual(0, testObj.ReadId4()); // 0 becuase we cannot read
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassFactory()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBaseUsingFactory> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBaseUsingFactory>();

      UnitTestContext context = GetContext();

      BypassBusinessBaseUsingFactory obj = BypassBusinessBaseUsingFactory.GetObject(dataPortal);
      context.Assert.AreEqual(false, obj.IsDirty);
      context.Assert.AreEqual(7, obj.ReadId2ByPass());
      context.Assert.Success();
      context.Complete();
    }
  }
}