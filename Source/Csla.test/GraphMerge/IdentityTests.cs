//-----------------------------------------------------------------------
// <copyright file="IdentityTests.cs" company="Marimer LLC">
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
using Csla.Core;
using Csla.TestHelpers;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 

namespace Csla.Test.GraphMerge
{
  [TestClass()]
  public class IdentityTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void IdentityInitializedBusinessBase()
    {
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }

    [TestMethod]
    public void IdentityNewBaseChild()
    {
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.AddChild(dataPortal);
      Assert.IsTrue(((IBusinessObject)obj.Child).Identity >= 0);
      Assert.IsTrue(((IBusinessObject)obj).Identity != ((IBusinessObject)obj.Child).Identity);
    }

    [TestMethod]
    public void IdentityBaseClone()
    {
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.AddChild(dataPortal);
      obj.Child.Name = "child";
      var cloned = obj.Clone();
      Assert.AreEqual(((IBusinessObject)obj).Identity, ((IBusinessObject)cloned).Identity, "root");
      Assert.AreEqual(((IBusinessObject)obj.Child).Identity, ((IBusinessObject)cloned.Child).Identity, "child");
    }

    [TestMethod]
    
    public void IdentityInitializedBusinessListBase()
    {
      IDataPortal<FooList> dataPortal = _testDIContext.CreateDataPortal<FooList>();

      var obj = dataPortal.Create();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }

    [TestMethod]
    public void IdentityNewListChild()
    {
      IDataPortal<FooList> dataPortal = _testDIContext.CreateDataPortal<FooList>();

      var obj = dataPortal.Create();
      obj.AddNew();
      Assert.IsTrue(((IBusinessObject)obj[0]).Identity >= 0);
      Assert.IsTrue(((IBusinessObject)obj).Identity != ((IBusinessObject)obj[0]).Identity);
    }

    [TestMethod]
    
    public void IdentityListClone()
    {
      IDataPortal<FooList> dataPortal = _testDIContext.CreateDataPortal<FooList>();

      var obj = dataPortal.Create();
      obj.AddNew();
      var cloned = obj.Clone();
      Assert.AreEqual(((IBusinessObject)obj).Identity, ((IBusinessObject)cloned).Identity, "root");
      Assert.AreEqual(((IBusinessObject)obj[0]).Identity, ((IBusinessObject)cloned[0]).Identity, "child");
    }

    [TestMethod]
    
    public void IdentityPostCloneIdentityManager()
    {
      IDataPortal<FooList> dataPortal = _testDIContext.CreateDataPortal<FooList>();

      var obj = dataPortal.Create();
      obj.AddNew();
      var cloned = obj.Clone();
      cloned.AddNew();
      Assert.IsTrue(((IBusinessObject)cloned[1]).Identity > -1);
      var identities = new List<int>();
      identities.Add(((IBusinessObject)cloned).Identity);
      identities.Add(((IBusinessObject)cloned[0]).Identity);
      Assert.IsFalse(identities.Contains(((IBusinessObject)cloned[1]).Identity), "new identity a repeat");
    }

    [TestMethod]
    
    public void IdentityInitializedBusinessBindingListBase()
    {
      IDataPortal<FooBindingList> dataPortal = _testDIContext.CreateDataPortal<FooBindingList>();

      var obj = dataPortal.Create();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }

    [TestMethod]
    
    public void IdentityInitializedDynamicListBase()
    {
      IDataPortal<FooDynamicList> dataPortal = _testDIContext.CreateDataPortal<FooDynamicList>();

      var obj = dataPortal.Create();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }

    [TestMethod]
    
    public void IdentityInitializedDynamicBindingListBase()
    {
      IDataPortal<FooDynamicBindingList> dataPortal = _testDIContext.CreateDataPortal<FooDynamicBindingList>();

      var obj = dataPortal.Create();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }
  }
}
