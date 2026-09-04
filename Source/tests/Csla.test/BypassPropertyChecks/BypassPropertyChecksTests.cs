//-----------------------------------------------------------------------
// <copyright file="BypassPropertyChecksTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Testing;
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

    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestBypassReadWriteWithRightsTurnNotificationBackOn()
    {
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();

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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();

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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();

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
      using var customHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = customHost.GetDataPortal<BypassBusinessBase>();

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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();

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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();

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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();

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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();

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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();

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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBase> dataPortal = testHost.GetDataPortal<BypassBusinessBase>();
      
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
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(GetPrincipal("Admin")));
      IDataPortal<BypassBusinessBaseUsingFactory> dataPortal = testHost.GetDataPortal<BypassBusinessBaseUsingFactory>();

      BypassBusinessBaseUsingFactory obj = BypassBusinessBaseUsingFactory.GetObject(dataPortal);
      Assert.AreEqual(false, obj.IsDirty);
      int actual = obj.ReadId2ByPass();
      Assert.AreEqual(7, actual);
    }
  }
}