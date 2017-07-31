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
    public void IdentityInitializedBusinessListBase()
    {
      var obj = Csla.DataPortal.Create<FooList>();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
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
    public void IdentityInitializedDynamiceBindingListBase()
    {
      var obj = Csla.DataPortal.Create<FooDynamicBindingList>();
      Assert.IsTrue(((IBusinessObject)obj).Identity >= 0);
    }
  }

  [Serializable]
  public class Foo : BusinessBase<Foo>
  { }

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
