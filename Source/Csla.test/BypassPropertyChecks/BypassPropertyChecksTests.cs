//-----------------------------------------------------------------------
// <copyright file="BypassPropertyChecksTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using UnitDriven;
using System.Security.Claims;

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

    [TestMethod]
    public void TestBypassReadWriteWithRightsTurnNotificationBackOn()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin", "Random");
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = new BypassBusinessBase();
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
      Csla.ApplicationContext.User = new System.Security.Claims.ClaimsPrincipal();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassReadWriteNoRightsTurnNotificationBackOn()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      bool propertyChangedFired = false;

      BypassBusinessBase testObj = new BypassBusinessBase();
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
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }


    [TestMethod]
    public void TestBypassReadWriteNoRights()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      bool propertyChangedFired = false;

      BypassBusinessBase testObj = new BypassBusinessBase();
      testObj.PropertyChanged += (o, e) =>
        {
          propertyChangedFired = true;
        };
      testObj.LoadId2ByPass(1);
      context.Assert.AreEqual(1, testObj.ReadId2ByPass());
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.AreEqual(false, testObj.IsDirty);
      context.Assert.Success();
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadWriteWithRights()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = new BypassBusinessBase();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId(1);
      context.Assert.AreEqual(1, testObj.ReadId());
      context.Assert.AreEqual(true, propertyChangedFired);
      context.Assert.AreEqual(true, testObj.IsDirty);
      context.Assert.Success();
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassWriteNoRightsDoNotBypass()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      BypassBusinessBase testObj = new BypassBusinessBase();
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
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadNoRightsDoNotBypass()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = new BypassBusinessBase();
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
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }


    
    [TestMethod]
    public void TestBypassReadWriteNoRightsBackingField()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      bool propertyChangedFired = false;

      BypassBusinessBase testObj = new BypassBusinessBase();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId4ByPass(1);
      context.Assert.AreEqual(false, testObj.IsDirty);
      context.Assert.AreEqual(1, testObj.ReadId4ByPass());
      context.Assert.AreEqual(false, propertyChangedFired);
      context.Assert.Success();
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadWriteWithRightsBackingField()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = new BypassBusinessBase();
      testObj.PropertyChanged += (o, e) =>
      {
        propertyChangedFired = true;
      };
      testObj.LoadId3(1);
      context.Assert.AreEqual(true, testObj.IsDirty);
      context.Assert.AreEqual(1, testObj.ReadId3());
      context.Assert.AreEqual(true, propertyChangedFired);
      context.Assert.Success();
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassWriteNoRightsDoNotBypassBackingField()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      BypassBusinessBase testObj = new BypassBusinessBase();
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
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadNoRightsDoNotBypassBackingField()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      bool propertyChangedFired = false;
      BypassBusinessBase testObj = new BypassBusinessBase();
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
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }

    [TestMethod]
    public void TestBypassFactory()
    {
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.User = GetPrincipal("Admin");

      BypassBusinessBaseUsingFactory obj = BypassBusinessBaseUsingFactory.GetObject();
      context.Assert.AreEqual(false, obj.IsDirty);
      context.Assert.AreEqual(7, obj.ReadId2ByPass());
      context.Assert.Success();
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      context.Complete();
    }
  }
}