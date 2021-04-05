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
using System.Threading.Tasks;
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
      Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(null, null);
    }

    [TestMethod]
    public void FindMethodInterfaceCriteria()
    {
      var obj = new InterfaceCriteria();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(
        obj, new object[] { new MyCriteria() });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodNullableCriteria()
    {
      var obj = new NullableCriteria();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method, "with int");
      method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { null });
      Assert.IsNotNull(method, "with null");
    }

    [TestMethod]
    public async Task FindMethodNullableCriteriaWithValueViaDataPortal()
    {
      var obj = await Csla.DataPortal.CreateAsync<NullableCriteria>(123);
      Assert.IsNotNull(obj);
    }

    [TestMethod]
    public async Task FindMethodNullableCriteriaWithNullViaDataPortal()
    {
      var obj = await Csla.DataPortal.CreateAsync<NullableCriteria>(null);
      Assert.IsNotNull(obj);
    }

    [TestMethod]
    public void FindMethodNoCriteriaNoDI()
    {
      var obj = new SimpleNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { null });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodCriteriaDI()
    {
      var obj = new CriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodBadCriteriaDI()
    {
      var obj = new CriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodMultipleCriteriaDI()
    {
      var obj = new MultipleCriteriaCreateWithDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123, "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodMultipleCriteriaDIInterleaved()
    {
      var obj = new MultipleCriteriaCreateWithDIInterleaved();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123, "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodCriteriaMultipleDI()
    {
      var obj = new CriteriaCreateWithMultipleDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      method.PrepForInvocation();
      Assert.IsNotNull(method);
      Assert.AreEqual(3, method.Parameters.Count());
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.AmbiguousMatchException))]
    public void FindMethodCriteriaMultipleAmbiguousDI()
    {
      var obj = new CriteriaCreateWithMultipleAmbiguousDI();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodSingleCriteriaInvalid()
    {
      var obj = new SimpleNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodSingleCriteriaValid()
    {
      var obj = new SimpleCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodObjectCriteriaValid()
    {
      var obj = new ObjectCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodObjectCriteriaSubtype()
    {
      var obj = new ObjectCriteriaCreateSubtype();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodSingleCriteriaBadType()
    {
      var obj = new SimpleCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { "hi" });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateBase()
    {
      var obj = new OldStyleNoOverride();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateCriteria()
    {
      var obj = new OldStyleCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateCriteriaBase()
    {
      var obj = new OldStyleCriteriaBase();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_Create()
    {
      var obj = new OldStyleCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodDataPortal_Create_Criteria()
    {
      var obj = new OldStyleCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, new object[] { 123 });
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethodAmbiguousCriteria()
    {
      var obj = new AmbiguousNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(obj, null);
      method.PrepForInvocation();
      Assert.IsNotNull(method);
      Assert.AreEqual(1, method.Parameters.Count());
    }

    [TestMethod]
    public void FindChildLegacyUpdate()
    {
      var obj = new BasicChild();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<UpdateChildAttribute>(obj, null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindChildParamsUpdate()
    {
      var obj = new ParamsChild();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<UpdateChildAttribute>(obj, null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindChildIntUpdate()
    {
      var obj = new ModernChild();
      object[] paramsArray = Server.DataPortal.GetCriteriaArray(123);
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<UpdateChildAttribute>(obj, paramsArray);
      Assert.IsNotNull(method);

      var dp = new Csla.Server.ChildDataPortal();
      dp.Update(obj, 42);
      Assert.AreEqual(42, obj.Id);
      obj = new ModernChild();
      dp.Update(obj, 123);
      Assert.AreEqual(123, obj.Id);
    }

    [TestMethod]
    public void Issue2109()
    {
      var obj = new Issue2109List();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<FetchAttribute>(obj, new object[] { new string[] { "a" } });
      Assert.IsNotNull(method, "string[]");
      var criteria = new My2109Criteria();
      method = Csla.Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<FetchAttribute>(obj, new object[] { criteria });
      Assert.IsNotNull(method, "ICriteriaBase");

      obj = Csla.DataPortal.Fetch<Issue2109List>(new My2109Criteria());
      Assert.IsNotNull(obj, "Fetch via criteria");
      Assert.AreEqual("Csla.Test.DataPortal.My2109Criteria", obj[0].Name);
      obj = Csla.DataPortal.Fetch<Issue2109List>(new string[] { "a" });
      Assert.IsNotNull(obj, "Fetch via string array");
      Assert.AreEqual("a", obj[0].Name);
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

  public interface ICriteria : IReadOnlyBase
  {
    int Id { get; }
  }

  public class MyCriteria : ReadOnlyBase<MyCriteria>, ICriteria
  {
    public int Id => 123;
  }

  [Serializable]
  public class InterfaceCriteria : BusinessBase<InterfaceCriteria>
  {
    [Create]
    private void Create(ICriteria criteria) { }
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

  [Serializable]
  public class ModernChild : BusinessBase<ModernChild>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    [UpdateChild]
    [InsertChild]
    private void UpdateOrInsert(int id)
    {
      using (BypassPropertyChecks)
        Id = id;
    }
  }

  [Serializable]
  public class NullableCriteria : BusinessBase<NullableCriteria>
  {
    [Create]
    private void Create(int? c)
    { }
  }

  [Serializable]
  public class Issue2109List : BusinessListBase<Issue2109List, Issue2109>
  {
    private void DataPortal_Fetch(ICriteriaBase criteria)
    {
      using (LoadListMode)
      {
        Add(Csla.DataPortal.Fetch<Issue2109>(criteria.ToString()));
      }
    }

    private void DataPortal_Fetch(IEnumerable<string> criteria)
    {
      using (LoadListMode)
      {
        Add(Csla.DataPortal.Fetch<Issue2109>(criteria.First().ToString()));
      }
    }
  }

  [Serializable]
  public class Issue2109 : BusinessBase<Issue2109>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    private void DataPortal_Fetch(string name)
    {
      using (BypassPropertyChecks)
      {
        Name = name;
      }
      MarkAsChild();
    }
  }

  public interface ICriteriaBase
  { }

  [Serializable]
  public class My2109Criteria : ICriteriaBase
  {
  }
}
