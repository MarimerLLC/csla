//-----------------------------------------------------------------------
// <copyright file="InheritedOperationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
      return dataPortal.Fetch(new PersonListBase.Criteria());
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
  public class PersonListBase : BusinessBindingListBase<PersonList, PersonEdit>
  {
    private void DataPortal_Fetch(Criteria criteria)
    {
    }

    [Serializable]
    public class Criteria : ReadOnlyBase<Criteria>;
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