using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class ArrayTests
  {
    // TODO: Fix tests
    //[TestMethod]
    //public void Test_DataPortal_Array()
    //{
    //  _ = ArrayDataPortalClass.Get(new int[] { 1, 2, 3 });
    //  Assert.AreEqual("Fetch(int[] values)", ApplicationContext.GlobalContext["Method"]);
    //
    //  _ = ArrayDataPortalClass.Get(new string[] { "a", "b", "c" });
    //  Assert.AreEqual("Fetch(string[] values)", ApplicationContext.GlobalContext["Method"]);
    //}
    //
    //[TestMethod]
    //public void Test_DataPortal_Params()
    //{
    //  _ = ArrayDataPortalClass.GetParams(1, 2, 3);
    //  Assert.AreEqual("Fetch(int[] values)", ApplicationContext.GlobalContext["Method"]);
    //
    //  _ = ArrayDataPortalClass.GetParams("a", "b", "c");
    //  Assert.AreEqual("Fetch(string[] values)", ApplicationContext.GlobalContext["Method"]);
    //}

    [TestMethod]
    [ExpectedException(typeof(AmbiguousMatchException))]
    public void Test_DataPortal_Int_Null()
    {
      IDataPortal<ArrayDataPortalClass> dataPortal = DataPortalFactory.CreateDataPortal<ArrayDataPortalClass>();

      try
      {
        _ = dataPortal.Fetch(default(int[]));
      }
      catch (DataPortalException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        else
          throw ex;
      }
    }

    [TestMethod]
    [ExpectedException(typeof(AmbiguousMatchException))]
    public void Test_DataPortal_String_Null()
    {
      IDataPortal<ArrayDataPortalClass> dataPortal = DataPortalFactory.CreateDataPortal<ArrayDataPortalClass>();

      try
      {
        _ = dataPortal.Fetch(default(string[]));
      }
      catch (DataPortalException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        else
          throw ex;
      }
    }

    [TestMethod]
    public void Test_ChildDataPortal_Array()
    {
      IChildDataPortal<ArrayDataPortalClass> childDataPortal = DataPortalFactory.CreateChildDataPortal<ArrayDataPortalClass>();

      _ = childDataPortal.FetchChild(new int[] { 1, 2, 3 });
      // TODO: Fix tests
      //Assert.AreEqual("FetchChild(int[] values)", ApplicationContext.GlobalContext["Method"]);

      _ = childDataPortal.FetchChild(new string[] { "a", "b", "c" });
      // TODO: Fix tests
      //Assert.AreEqual("FetchChild(string[] values)", ApplicationContext.GlobalContext["Method"]);
    }

    [TestMethod]
    public void Test_ChildDataPortal_Params()
    {
      IChildDataPortal<ArrayDataPortalClass> childDataPortal = DataPortalFactory.CreateChildDataPortal<ArrayDataPortalClass>();

      _ = childDataPortal.FetchChild(1, 2, 3);
      // TODO: Fix tests
      //Assert.AreEqual("FetchChild(int[] values)", ApplicationContext.GlobalContext["Method"]);

      _ = childDataPortal.FetchChild("a", "b", "c");
      // TODO: Fix tests
      //Assert.AreEqual("FetchChild(string[] values)", ApplicationContext.GlobalContext["Method"]);
    }

  }

  [Serializable]
  public class ArrayDataPortalClass
    : BusinessBase<ArrayDataPortalClass>
  {

    [Fetch]
    private void Fetch(int[] values)
    {
      //ApplicationContext.GlobalContext.Clear();
      //ApplicationContext.GlobalContext.Add("Method", "Fetch(int[] values)");
    }

    [Fetch]
    private void Fetch(string[] values)
    {
      //ApplicationContext.GlobalContext.Clear();
      //ApplicationContext.GlobalContext.Add("Method", "Fetch(string[] values)");
    }

    [FetchChild]
    private void FetchChild(int[] values)
    {
      //ApplicationContext.GlobalContext.Clear();
      //ApplicationContext.GlobalContext.Add("Method", "FetchChild(int[] values)");
    }

    [FetchChild]
    private void FetchChild(string[] values)
    {
      //ApplicationContext.GlobalContext.Clear();
      //ApplicationContext.GlobalContext.Add("Method", "FetchChild(string[] values)");
    }
  }

}
