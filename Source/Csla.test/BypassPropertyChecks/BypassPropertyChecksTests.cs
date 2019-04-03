//-----------------------------------------------------------------------
// <copyright file="BypassPropertyChecksTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using Csla.Testing.Business.Security;
using UnitDriven;

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

    [TestMethod]
    public void TestBypassReadWriteWithRightsTurnNotificationBackOn()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassReadWriteNoRightsTurnNotificationBackOn()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }


    [TestMethod]
    public void TestBypassReadWriteNoRights()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadWriteWithRights()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassWriteNoRightsDoNotBypass()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadNoRightsDoNotBypass()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }


    
    [TestMethod]
    public void TestBypassReadWriteNoRightsBackingField()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadWriteWithRightsBackingField()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestBypassWriteNoRightsDoNotBypassBackingField()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }

    [TestMethod]
    public void TestBypassReadNoRightsDoNotBypassBackingField()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
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
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }

    [TestMethod]
    public void TestBypassFactory()
    {
      UnitTestContext context = GetContext();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type

      BypassBusinessBaseUsingFactory obj = BypassBusinessBaseUsingFactory.GetObject();
      context.Assert.AreEqual(false, obj.IsDirty);
      context.Assert.AreEqual(7, obj.ReadId2ByPass());
      context.Assert.Success();
#pragma warning disable CS0436 // Type conflicts with imported type
      Csla.Test.Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      context.Complete();
    }
  }
}