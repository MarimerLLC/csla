//-----------------------------------------------------------------------
// <copyright file="TestCriteria.cs" company="Marimer LLC">
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
using Csla.Serialization;

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

namespace cslalighttest.Stereotypes
{
  [TestClass]
  public class TestCriteria : TestBase
  {
    [TestMethod]
    public void CustomCriteria()
    {
      var context = GetContext();

      var obj = new CustomCriteria();
      context.Assert.IsNotNull(obj);
      obj.Id = 123;
      context.Assert.AreEqual(123, obj.Id);

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void SerializeCriteria()
    {
      var context = GetContext();

      var obj = new CustomCriteria();
      context.Assert.IsNotNull(obj);
      obj.Id = 123;
      context.Assert.AreEqual(123, obj.Id);

      obj = (CustomCriteria)Csla.Core.ObjectCloner.Clone(obj);
      context.Assert.AreEqual(123, obj.Id);

      context.Assert.Success();
      context.Complete();
    }
  }

  [Serializable]
  public class CustomCriteria : CriteriaBase<CustomCriteria>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return ReadProperty(IdProperty); }
      set { LoadProperty(IdProperty, value); }
    }
  }
}