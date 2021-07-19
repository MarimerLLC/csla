//-----------------------------------------------------------------------
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
    [TestMethod]
    public void BusinessBindingListFetch()
    {
      var obj = PersonList.FetchPersonList();
      Assert.IsNotNull(obj);
    }

    [TestMethod]
    public void BusinessObjectFetch()
    {
      var obj = Csla.DataPortal.Create<PersonEdit>();
      Assert.IsNotNull(obj);
    }
  }

  [Serializable]
  public class PersonList : PersonListBase
  {
    public static PersonList FetchPersonList()
    {
      return Csla.DataPortal.Fetch<PersonList>(new Criteria());
    }
  }

  [Serializable]
  public class PersonListBase : Csla.BusinessBindingListBase<PersonList, PersonEdit>
  {
    private void DataPortal_Fetch(Criteria criteria)
    {
    }

    [Serializable()]
    protected class Criteria : Csla.CriteriaBase<Criteria>
    {
    }
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
    protected override void DataPortal_Create()
    {
    }
  }
}
