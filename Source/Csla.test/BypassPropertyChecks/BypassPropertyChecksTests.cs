//-----------------------------------------------------------------------
// <copyright file="BypassPropertyChecksTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.BypassPropertyChecks
{
  [TestClass]
  public class BypassPropertyChecksTests
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

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassReadWriteWithRightsTurnNotificationBackOn()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadIdByPass(1);
      int actual = testObj.ReadIdByPass();
      Assert.AreEqual(1, actual);
      Assert.AreEqual(false, propertyChangedFired);
      Assert.AreEqual(false, testObj.IsDirty);

      testObj.LoadIdByNestedPass(3);
      int actual1 = testObj.ReadIdByPass();
      Assert.AreEqual(3, actual1);
      Assert.AreEqual(false, propertyChangedFired);
      Assert.AreEqual(false, testObj.IsDirty);

      testObj.LoadId(2);
      Assert.AreEqual(true, propertyChangedFired);
      int actual2 = testObj.ReadId();
      Assert.AreEqual(2, actual2);
      Assert.AreEqual(true, testObj.IsDirty);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassReadWriteNoRightsTurnNotificationBackOn()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      bool propertyChangedFired = false;

      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId2ByPass(1);
      int actual = testObj.ReadId2ByPass();
      Assert.AreEqual(1, actual);
      Assert.AreEqual(false, propertyChangedFired);
      Assert.AreEqual(false, testObj.IsDirty);
      testObj.LoadId2(2);
      int actual1 = testObj.ReadId2ByPass();
      Assert.AreEqual(1, actual1); // still one becuase set failed
      Assert.AreEqual(true, testObj.IsDirty);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassReadWriteNoRights()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      bool propertyChangedFired = false;

      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (_, _) =>
        {
          propertyChangedFired = true;
        };
      testObj.LoadId2ByPass(1);
      int actual = testObj.ReadId2ByPass();
      Assert.AreEqual(1, actual);
      Assert.AreEqual(false, propertyChangedFired);
      Assert.AreEqual(false, testObj.IsDirty);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassReadWriteWithRights()
    {
      TestDIContext customDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = customDIContext.CreateDataPortal<BypassBusinessBase>();

      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId(1);
      int actual = testObj.ReadId();
      Assert.AreEqual(1, actual);
      Assert.AreEqual(true, propertyChangedFired);
      Assert.AreEqual(true, testObj.IsDirty);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassWriteNoRightsDoNotBypass()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      BypassBusinessBase testObj = dataPortal.Fetch();
      bool propertyChangedFired = false;
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId2(1);
      int actual = testObj.ReadId2ByPass();
      Assert.AreEqual(1, actual);
      Assert.AreEqual(true, propertyChangedFired);
      Assert.AreEqual(true, testObj.IsDirty);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassReadNoRightsDoNotBypass()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId2ByPass(1);
      Assert.AreEqual(false, testObj.IsDirty);
      int actual = testObj.ReadId2ByPass();
      Assert.AreEqual(1, actual);
      int actual1 = testObj.ReadId2();
      Assert.AreEqual(0, actual1); // 0 becuase we cannot read
      Assert.AreEqual(false, propertyChangedFired);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassReadWriteNoRightsBackingField()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      bool propertyChangedFired = false;

      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId4ByPass(1);
      Assert.AreEqual(false, testObj.IsDirty);
      int actual = testObj.ReadId4ByPass();
      Assert.AreEqual(1, actual);
      Assert.AreEqual(false, propertyChangedFired);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassReadWriteWithRightsBackingField()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId3(1);
      Assert.AreEqual(true, testObj.IsDirty);
      int actual = testObj.ReadId3();
      Assert.AreEqual(1, actual);
      Assert.AreEqual(true, propertyChangedFired);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassWriteNoRightsDoNotBypassBackingField()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();

      BypassBusinessBase testObj = dataPortal.Fetch();
      bool propertyChangedFired = false;
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId4(1);
       Assert.AreEqual(true, testObj.IsDirty);
       int actual = testObj.ReadId4ByPass();
       Assert.AreEqual(1, actual);
      Assert.AreEqual(true, propertyChangedFired);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassReadNoRightsDoNotBypassBackingField()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBase> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBase>();
      
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = dataPortal.Fetch();
      testObj.PropertyChanged += (_, _) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId4ByPass(1);
      Assert.AreEqual(false, testObj.IsDirty);
      int actual = testObj.ReadId4ByPass();
      Assert.AreEqual(1, actual);
      int actual1 = testObj.ReadId4();
      Assert.AreEqual(0, actual1); // 0 becuase we cannot read
      Assert.AreEqual(false, propertyChangedFired);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassFactory()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<BypassBusinessBaseUsingFactory> dataPortal = testDIContext.CreateDataPortal<BypassBusinessBaseUsingFactory>();

      BypassBusinessBaseUsingFactory obj = BypassBusinessBaseUsingFactory.GetObject(dataPortal);
      Assert.AreEqual(false, obj.IsDirty);
      int actual = obj.ReadId2ByPass();
      Assert.AreEqual(7, actual);
    }
  }
}