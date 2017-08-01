//-----------------------------------------------------------------------
// <copyright file="IdentityTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;

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
    [TestMethod]
    public void IdentityInitializedBusinessBase()
    {
      var obj = Csla.DataPortal.Create<Foo>();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }

    [TestMethod]
    public void IdentityNewBaseChild()
    {
      var obj = Csla.DataPortal.Create<Foo>();
      obj.AddChild();
      Assert.IsTrue(((IBusinessObject)obj.Child).Identity >= 0);
      Assert.IsTrue(((IBusinessObject)obj).Identity != ((IBusinessObject)obj.Child).Identity);
    }

    [TestMethod]
    public void IdentityBaseClone()
    {
      var obj = Csla.DataPortal.Create<Foo>();
      obj.AddChild();
      obj.Child.Name = "child";
      var cloned = obj.Clone();
      Assert.AreEqual(((IBusinessObject)obj).Identity, ((IBusinessObject)cloned).Identity, "root");
      Assert.AreEqual(((IBusinessObject)obj.Child).Identity, ((IBusinessObject)cloned.Child).Identity, "child");
    }

    [TestMethod]
    public void IdentityInitializedBusinessListBase()
    {
      var obj = Csla.DataPortal.Create<FooList>();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }

    [TestMethod]
    public void IdentityNewListChild()
    {
      var obj = Csla.DataPortal.Create<FooList>();
      obj.AddNew();
      Assert.IsTrue(((IBusinessObject)obj[0]).Identity >= 0);
      Assert.IsTrue(((IBusinessObject)obj).Identity != ((IBusinessObject)obj[0]).Identity);
    }

    [TestMethod]
    public void IdentityListClone()
    {
      var obj = Csla.DataPortal.Create<FooList>();
      obj.AddNew();
      var cloned = obj.Clone();
      Assert.AreEqual(((IBusinessObject)obj).Identity, ((IBusinessObject)cloned).Identity, "root");
      Assert.AreEqual(((IBusinessObject)obj[0]).Identity, ((IBusinessObject)cloned[0]).Identity, "child");
    }

    [TestMethod]
    public void IdentityPostCloneIdentityManager()
    {
      var obj = Csla.DataPortal.Create<FooList>();
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
      var obj = Csla.DataPortal.Create<FooBindingList>();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }

    [TestMethod]
    public void IdentityInitializedDynamicListBase()
    {
      var obj = Csla.DataPortal.Create<FooDynamicList>();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }

    [TestMethod]
    public void IdentityInitializedDynamicBindingListBase()
    {
      var obj = Csla.DataPortal.Create<FooDynamicBindingList>();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }
  }

  [Serializable]
  public class Foo : BusinessBase<Foo>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<Foo> ChildProperty = RegisterProperty<Foo>(c => c.Child);
    public Foo Child
    {
      get { return GetProperty(ChildProperty); }
      set { SetProperty(ChildProperty, value); }
    }

    public void AddChild()
    {
      var child = Csla.DataPortal.Create<Foo>();
      child.MarkAsChild();
      LoadProperty(ChildProperty, child);
    }
  }

  [Serializable]
  public class FooList : BusinessListBase<FooList, Foo>
  { }

  [Serializable]
  public class FooBindingList : BusinessBindingListBase<FooBindingList, Foo>
  { }

  [Serializable]
  public class FooDynamicList : DynamicListBase<Foo>
  {
    private void DataPortal_Create()
    { }
  }

  [Serializable]
  public class FooDynamicBindingList : DynamicBindingListBase<Foo>
  {
    private void DataPortal_Create()
    { }
  }
}
