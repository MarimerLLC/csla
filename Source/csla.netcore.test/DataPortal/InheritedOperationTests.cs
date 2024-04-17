﻿//-----------------------------------------------------------------------
// <copyright file="InheritedOperationTests.cs" company="Marimer LLC">
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
using Csla.Configuration;
using Csla.TestHelpers;
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
  public class InheritedOperationTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void BusinessBindingListFetch()
    {
      var obj = FetchPersonList();
      Assert.IsNotNull(obj);
    }

    [TestMethod]
    public void BusinessObjectFetch()
    {
      var obj = CreatePersonEdit();
      Assert.IsNotNull(obj);
    }

    private PersonList FetchPersonList()
    {
      IDataPortal<PersonList> dataPortal = _testDIContext.CreateDataPortal<PersonList>();
      return dataPortal.Fetch(new PersonList.Criteria());
    }

    private PersonEdit CreatePersonEdit()
    {
      IDataPortal<PersonEdit> dataPortal = _testDIContext.CreateDataPortal<PersonEdit>();
      return dataPortal.Create();
    }
  }

  [Serializable]
  public class PersonList : PersonListBase;

  [Serializable]
  public class PersonListBase : Csla.BusinessBindingListBase<PersonList, PersonEdit>
  {
    private void DataPortal_Fetch(Criteria criteria)
    {
    }

    [Serializable()]
    public class Criteria : Csla.CriteriaBase<Criteria>;
  }

  [Serializable]
  public class PersonEdit : PersonEditBase<PersonEdit>
  {
    private new void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }
  }

  [Serializable]
  public class PersonEditBase<T> : BusinessBase<PersonEditBase<T>>
  {
    protected void DataPortal_Create()
    {
    }
  }
}