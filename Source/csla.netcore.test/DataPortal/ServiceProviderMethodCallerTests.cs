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
    public void FindMethodNoCriteriaNoDI()
    {
      var obj = new SimpleNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), null);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.AmbiguousMatchException))]
    public void FindMethodAmbiguousCriteria()
    {
      var obj = new AmbiguousNoCriteriaCreate();
      var method = Csla.Reflection.ServiceProviderMethodCaller.FindMethodForCriteria(obj, typeof(Create), null);
      Assert.IsNotNull(method);
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
  public class AmbiguousNoCriteriaCreate : BusinessBase<AmbiguousNoCriteriaCreate>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create([FromServices] ICloneable x) { }
  }
}
