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
    [TestMethod]
    public void Test_DataPortal_Array()
    {
      IDataPortal<ArrayDataPortalClass> dataPortal = DataPortalFactory.CreateDataPortal<ArrayDataPortalClass>();

      _ = dataPortal.Fetch(new int[] { 1, 2, 3 });
      Assert.AreEqual("Fetch(int[] values)", TestResults.GetResult("Method"));

      _ = dataPortal.Fetch(new string[] { "a", "b", "c" });
      Assert.AreEqual("Fetch(string[] values)", TestResults.GetResult("Method"));
    }

    [TestMethod]
    public void Test_DataPortal_Params()
    {
      IDataPortal<ArrayDataPortalClass> dataPortal = DataPortalFactory.CreateDataPortal<ArrayDataPortalClass>();

      _ = dataPortal.Fetch(1, 2, 3);
      Assert.AreEqual("Fetch(int[] values)", TestResults.GetResult("Method"));

      _ = dataPortal.Fetch("a", "b", "c");
      Assert.AreEqual("Fetch(string[] values)", TestResults.GetResult("Method"));
    }

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
      Assert.AreEqual("FetchChild(int[] values)", TestResults.GetResult("Method"));

      _ = childDataPortal.FetchChild(new string[] { "a", "b", "c" });
      Assert.AreEqual("FetchChild(string[] values)", TestResults.GetResult("Method"));
    }

    [TestMethod]
    public void Test_ChildDataPortal_Params()
    {
      IChildDataPortal<ArrayDataPortalClass> childDataPortal = DataPortalFactory.CreateChildDataPortal<ArrayDataPortalClass>();

      _ = childDataPortal.FetchChild(1, 2, 3);
      Assert.AreEqual("FetchChild(int[] values)", TestResults.GetResult("Method"));

      _ = childDataPortal.FetchChild("a", "b", "c");
      Assert.AreEqual("FetchChild(string[] values)", TestResults.GetResult("Method"));
    }

  }

  [Serializable]
  public class ArrayDataPortalClass
    : BusinessBase<ArrayDataPortalClass>
  {

    [Fetch]
    private void Fetch(int[] values)
    {
      //TestResults.Reinitialise();
      //TestResults.Add("Method", "Fetch(int[] values)");
    }

    [Fetch]
    private void Fetch(string[] values)
    {
      //TestResults.Reinitialise();
      //TestResults.Add("Method", "Fetch(string[] values)");
    }

    [FetchChild]
    private void FetchChild(int[] values)
    {
      //TestResults.Reinitialise();
      //TestResults.Add("Method", "FetchChild(int[] values)");
    }

    [FetchChild]
    private void FetchChild(string[] values)
    {
      //TestResults.Reinitialise();
      //TestResults.Add("Method", "FetchChild(string[] values)");
    }
  }

}
