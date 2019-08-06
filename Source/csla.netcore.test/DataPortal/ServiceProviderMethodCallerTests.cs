﻿//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodCallerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
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
  public class ServiceProviderMethodCallerTests
  {
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NoTarget()
    {
      Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(null, typeof(CreateAttribute), null);
    }

    [TestMethod]
    public void FindMethodNoCriteriaNoDI()
    {
      var obj = new SimpleNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodCriteriaDI()
    {
      var obj = new CriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodBadCriteriaDI()
    {
      var obj = new CriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodMultipleCriteriaDI()
    {
      var obj = new MultipleCriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123, "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodMultipleCriteriaDIInterleaved()
    {
      var obj = new MultipleCriteriaCreateWithDIInterleaved();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123, "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodCriteriaMultipleDI()
    {
      var obj = new CriteriaCreateWithMultipleDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
      Assert.AreEqual(3, method.GetParameters().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.AmbiguousMatchException))]
    public void FindMethodCriteriaMultipleAmbiguousDI()
    {
      var obj = new CriteriaCreateWithMultipleAmbiguousDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodSingleCriteriaInvalid()
    {
      var obj = new SimpleNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodSingleCriteriaValid()
    {
      var obj = new SimpleCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodObjectCriteriaValid()
    {
      var obj = new ObjectCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodObjectCriteriaSubtype()
    {
      var obj = new ObjectCriteriaCreateSubtype();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodSingleCriteriaBadType()
    {
      var obj = new SimpleCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateBase()
    {
      var obj = new OldStyleNoOverride();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateCriteria()
    {
      var obj = new OldStyleCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateCriteriaBase()
    {
      var obj = new OldStyleCriteriaBase();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_Create()
    {
      var obj = new OldStyleCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_Create_Criteria()
    {
      var obj = new OldStyleCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodAmbiguousCriteria()
    {
      var obj = new AmbiguousNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(CreateAttribute), null);
      Assert.IsNotNull(method);
      Assert.AreEqual(1, method.GetParameters().Count());
    }

    [TestMethod]
    public void FindChildLegacyUpdate()
    {
      var obj = new BasicChild();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(UpdateChildAttribute), null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindChildParamsUpdate()
    {
      var obj = new ParamsChild();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod(obj, typeof(UpdateChildAttribute), null);
      Assert.IsNotNull(method);
    }
  }

  [Serializable]
  public class OldStyleNoOverride : BusinessBase<OldStyleNoOverride>
  {
  }

  [Serializable]
  public class OldStyleCreate : BusinessBase<OldStyleCreate>
  {
    protected override void DataPortal_Create() { }

    private void DataPortal_Create(int id) { }
  }

  [Serializable]
  public class OldStyleCriteria : BusinessBase<OldStyleCriteria>
  {
    private void DataPortal_Create(int id) { }
  }

  [Serializable]
  public class OldStyleCriteriaBase : OldStyleCriteria
  {
  }

  [Serializable]
  public class SimpleNoCriteriaCreate : BusinessBase<SimpleNoCriteriaCreate>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(ICloneable x) { }
  }

  [Serializable]
  public class SimpleCriteriaCreate : BusinessBase<SimpleCriteriaCreate>
  {
    [Create]
    private void Create(int id) { }
  }

  [Serializable]
  public class ObjectCriteriaCreate : BusinessBase<ObjectCriteriaCreate>
  {
    [Create]
    private void Create(object id) { }
  }

  [Serializable]
  public class ObjectCriteriaCreateSubtype : ObjectCriteriaCreate
  { }

  [Serializable]
  public class AmbiguousNoCriteriaCreate : BusinessBase<AmbiguousNoCriteriaCreate>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create([Inject] ICloneable x) { }
  }

  [Serializable]
  public class CriteriaCreateWithDI : BusinessBase<CriteriaCreateWithDI>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, [Inject] ICloneable x) { }
  }

  [Serializable]
  public class MultipleCriteriaCreateWithDI : BusinessBase<MultipleCriteriaCreateWithDI>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, string foo, [Inject] ICloneable x) { }
  }

  [Serializable]
  public class CriteriaCreateWithMultipleDI : BusinessBase<CriteriaCreateWithMultipleDI>
  {
    [Create]
    private void Create(int id, [Inject] ICloneable x) { }

    [Create]
    private void Create(int id, [Inject] ICloneable x, [Inject] IAsyncResult y) { }
  }

  [Serializable]
  public class CriteriaCreateWithMultipleAmbiguousDI : BusinessBase<CriteriaCreateWithMultipleAmbiguousDI>
  {
    [Create]
    private void Create(int id, [Inject] ICloneable x) { }

    [Create]
    private void Create(int id, [Inject] ICloneable x, [Inject] IAsyncResult y) { }

    [Create]
    private void Create(int id, [Inject] ICloneable x, [Inject] IFormattable y) { }
  }

  [Serializable]
  public class MultipleCriteriaCreateWithDIInterleaved : BusinessBase<MultipleCriteriaCreateWithDIInterleaved>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, [Inject] ICloneable x, string foo) { }
  }

  [Serializable]
  public class BasicChild : BusinessBase<BasicChild>
  {
    private void Child_Update()
    {
      // nada
    }
  }

  [Serializable]
  public class ParamsChild : BusinessBase<ParamsChild>
  {
    private void Child_Update(params object[] parameters)
    {
      // nada
    }
  }
}
