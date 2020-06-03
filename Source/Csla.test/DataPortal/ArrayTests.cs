using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class ArrayTests
  {
    [TestMethod]
    public void Test_DataPortal_Array()
    {
      _ = ArrayDataPortalClass.Get(new int[] { 1, 2, 3 });
      Assert.AreEqual("Fetch(int[] values)", ApplicationContext.GlobalContext["Method"]);

      _ = ArrayDataPortalClass.Get(new string[] { "a", "b", "c" });
      Assert.AreEqual("Fetch(string[] values)", ApplicationContext.GlobalContext["Method"]);
    }

    [TestMethod]
    public void Test_DataPortal_Params()
    {
      _ = ArrayDataPortalClass.GetParams(1, 2, 3);
      Assert.AreEqual("Fetch(int[] values)", ApplicationContext.GlobalContext["Method"]);

      _ = ArrayDataPortalClass.GetParams("a", "b", "c");
      Assert.AreEqual("Fetch(string[] values)", ApplicationContext.GlobalContext["Method"]);
    }

    [TestMethod]
    [ExpectedException(typeof(AmbiguousMatchException))]
    public void Test_DataPortal_Int_Null()
    {
      try
      {
        _ = ArrayDataPortalClass.Get(default(int[]));
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
      try
      {
        _ = ArrayDataPortalClass.Get(default(string[]));
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
      _ = ArrayDataPortalClass.GetChild(new int[] { 1, 2, 3 });
      Assert.AreEqual("FetchChild(int[] values)", ApplicationContext.GlobalContext["Method"]);

      _ = ArrayDataPortalClass.GetChild(new string[] { "a", "b", "c" });
      Assert.AreEqual("FetchChild(string[] values)", ApplicationContext.GlobalContext["Method"]);
    }

    [TestMethod]
    public void Test_ChildDataPortal_Params()
    {
      _ = ArrayDataPortalClass.GetChildParams(1, 2, 3);
      Assert.AreEqual("FetchChild(int[] values)", ApplicationContext.GlobalContext["Method"]);

      _ = ArrayDataPortalClass.GetChildParams("a", "b", "c");
      Assert.AreEqual("FetchChild(string[] values)", ApplicationContext.GlobalContext["Method"]);
    }


  }

  [Serializable]
  public class ArrayDataPortalClass
    : BusinessBase<ArrayDataPortalClass>
  {

    public static ArrayDataPortalClass Get(int[] values)
    {
      return Csla.DataPortal.Fetch<ArrayDataPortalClass>(values);
    }

    public static ArrayDataPortalClass Get(string[] values)
    {
      return Csla.DataPortal.Fetch<ArrayDataPortalClass>(values);
    }

    public static ArrayDataPortalClass GetParams(params int[] values)
    {
      return Csla.DataPortal.Fetch<ArrayDataPortalClass>(values);
    }

    public static ArrayDataPortalClass GetParams(params string[] values)
    {
      return Csla.DataPortal.Fetch<ArrayDataPortalClass>(values);
    }

    public static ArrayDataPortalClass GetChild(int[] values)
    {
      return Csla.DataPortal.FetchChild<ArrayDataPortalClass>(values);
    }

    public static ArrayDataPortalClass GetChild(string[] values)
    {
      return Csla.DataPortal.FetchChild<ArrayDataPortalClass>(values);
    }

    public static ArrayDataPortalClass GetChildParams(params int[] values)
    {
      return Csla.DataPortal.FetchChild<ArrayDataPortalClass>(values);
    }

    public static ArrayDataPortalClass GetChildParams(params string[] values)
    {
      return Csla.DataPortal.FetchChild<ArrayDataPortalClass>(values);
    }

    [Fetch]
    private void Fetch(int[] values)
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Method", "Fetch(int[] values)");
    }

    [Fetch]
    private void Fetch(string[] values)
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Method", "Fetch(string[] values)");
    }

    [FetchChild]
    private void FetchChild(int[] values)
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Method", "FetchChild(int[] values)");
    }

    [FetchChild]
    private void FetchChild(string[] values)
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Method", "FetchChild(string[] values)");
    }
  }

}
