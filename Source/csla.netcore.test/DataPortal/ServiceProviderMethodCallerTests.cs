//-----------------------------------------------------------------------
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
      Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(null, typeof(Create), null);
    }

    [TestMethod]
    public void FindMethodNoCriteriaNoDI()
    {
      var obj = new SimpleNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodCriteriaDI()
    {
      var obj = new CriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodBadCriteriaDI()
    {
      var obj = new CriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodMultipleCriteriaDI()
    {
      var obj = new MultipleCriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { 123, "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodMultipleCriteriaDIInterleaved()
    {
      var obj = new MultipleCriteriaCreateWithDIInterleaved();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { 123, "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodCriteriaMultipleDI()
    {
      var obj = new CriteriaCreateWithMultipleDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { 123 });
      Assert.IsNotNull(method);
      Assert.AreEqual(3, method.GetParameters().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.AmbiguousMatchException))]
    public void FindMethodCriteriaMultipleAmbiguousDI()
    {
      var obj = new CriteriaCreateWithMultipleAmbiguousDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { 123 });
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodSingleCriteriaInvalid()
    {
      var obj = new SimpleNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodSingleCriteriaValid()
    {
      var obj = new SimpleCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodSingleCriteriaBadType()
    {
      var obj = new SimpleCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_Create()
    {
      var obj = new OldStyleCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_Create_Criteria()
    {
      var obj = new OldStyleCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodAmbiguousCriteria()
    {
      var obj = new AmbiguousNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), null);
      Assert.IsNotNull(method);
      Assert.AreEqual(1, method.GetParameters().Count());
    }
  }

  [Serializable]
  public class OldStyleCreate : BusinessBase<SimpleNoCriteriaCreate>
  {
    protected override void DataPortal_Create()
    {
    }

    private void DataPortal_Create(int id)
    {
    }
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
  public class SimpleCriteriaCreate : BusinessBase<SimpleNoCriteriaCreate>
  {
    [Create]
    private void Create(int id) { }
  }

  [Serializable]
  public class AmbiguousNoCriteriaCreate : BusinessBase<AmbiguousNoCriteriaCreate>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create([FromServices] ICloneable x) { }
  }

  [Serializable]
  public class CriteriaCreateWithDI : BusinessBase<CriteriaCreateWithDI>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, [FromServices] ICloneable x) { }
  }

  [Serializable]
  public class MultipleCriteriaCreateWithDI : BusinessBase<MultipleCriteriaCreateWithDI>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, string foo, [FromServices] ICloneable x) { }
  }

  [Serializable]
  public class CriteriaCreateWithMultipleDI : BusinessBase<CriteriaCreateWithMultipleDI>
  {
    [Create]
    private void Create(int id, [FromServices] ICloneable x) { }

    [Create]
    private void Create(int id, [FromServices] ICloneable x, [FromServices] IAsyncResult y) { }
  }

  [Serializable]
  public class CriteriaCreateWithMultipleAmbiguousDI : BusinessBase<CriteriaCreateWithMultipleAmbiguousDI>
  {
    [Create]
    private void Create(int id, [FromServices] ICloneable x) { }

    [Create]
    private void Create(int id, [FromServices] ICloneable x, [FromServices] IAsyncResult y) { }

    [Create]
    private void Create(int id, [FromServices] ICloneable x, [FromServices] IFormattable y) { }
  }

  [Serializable]
  public class MultipleCriteriaCreateWithDIInterleaved : BusinessBase<MultipleCriteriaCreateWithDIInterleaved>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, [FromServices] ICloneable x, string foo) { }
  }
}
