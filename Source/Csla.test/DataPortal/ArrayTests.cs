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
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void Test_DataPortal_Array()
    {
      IDataPortal<ArrayDataPortalClass> dataPortal = _testDIContext.CreateDataPortal<ArrayDataPortalClass>();

      TestResults.Reinitialise();
      _ = ArrayDataPortalClass.Get(dataPortal, new int[] { 1, 2, 3 });
      Assert.AreEqual("Fetch(int[] values)", TestResults.GetResult("Method"));

      TestResults.Reinitialise();
      _ = ArrayDataPortalClass.Get(dataPortal, new string[] { "a", "b", "c" });
      Assert.AreEqual("Fetch(string[] values)", TestResults.GetResult("Method"));
    }

    [TestMethod]
    public void Test_DataPortal_Params()
    {
      IDataPortal<ArrayDataPortalClass> dataPortal = _testDIContext.CreateDataPortal<ArrayDataPortalClass>();

      TestResults.Reinitialise();
      _ = ArrayDataPortalClass.GetParams(dataPortal, 1, 2, 3);
      Assert.AreEqual("Fetch(int[] values)", TestResults.GetResult("Method"));

      TestResults.Reinitialise();
      _ = ArrayDataPortalClass.GetParams(dataPortal, "a", "b", "c");
      Assert.AreEqual("Fetch(string[] values)", TestResults.GetResult("Method"));
    }

    [TestMethod]
    [ExpectedException(typeof(AmbiguousMatchException))]
    public void Test_DataPortal_Int_Null()
    {
      IDataPortal<ArrayDataPortalClass> dataPortal = _testDIContext.CreateDataPortal<ArrayDataPortalClass>();

      try
      {
        _ = ArrayDataPortalClass.Get(dataPortal, default(int[]));
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
      IDataPortal<ArrayDataPortalClass> dataPortal = _testDIContext.CreateDataPortal<ArrayDataPortalClass>();

      try
      {
        _ = ArrayDataPortalClass.Get(dataPortal, default(string[]));
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
      IChildDataPortal<ArrayDataPortalClass> childDataPortal = _testDIContext.CreateChildDataPortal<ArrayDataPortalClass>();

      _ = ArrayDataPortalClass.GetChild(childDataPortal, new int[] { 1, 2, 3 });
      Assert.AreEqual("FetchChild(int[] values)", TestResults.GetResult("Method"));

      TestResults.Reinitialise();
      _ = ArrayDataPortalClass.GetChild(childDataPortal, new string[] { "a", "b", "c" });
      Assert.AreEqual("FetchChild(string[] values)", TestResults.GetResult("Method"));
    }

    [TestMethod]
    public void Test_ChildDataPortal_Params()
    {
      IChildDataPortal<ArrayDataPortalClass> childDataPortal = _testDIContext.CreateChildDataPortal<ArrayDataPortalClass>();

      _ = ArrayDataPortalClass.GetChildParams(childDataPortal, 1, 2, 3);
      Assert.AreEqual("FetchChild(int[] values)", TestResults.GetResult("Method"));

      TestResults.Reinitialise();
      _ = ArrayDataPortalClass.GetChildParams(childDataPortal, "a", "b", "c");
      Assert.AreEqual("FetchChild(string[] values)", TestResults.GetResult("Method"));
    }

  }

  [Serializable]
  public class ArrayDataPortalClass
    : BusinessBase<ArrayDataPortalClass>
  {

    public static ArrayDataPortalClass Get(IDataPortal<ArrayDataPortalClass> dataPortal, int[] values)
    {
      return dataPortal.Fetch(values);
    }

    public static ArrayDataPortalClass Get(IDataPortal<ArrayDataPortalClass> dataPortal, string[] values)
    {
      return dataPortal.Fetch(values);
    }

    public static ArrayDataPortalClass GetParams(IDataPortal<ArrayDataPortalClass> dataPortal, params int[] values)
    {
      return dataPortal.Fetch(values);
    }

    public static ArrayDataPortalClass GetParams(IDataPortal<ArrayDataPortalClass> dataPortal, params string[] values)
    {
      return dataPortal.Fetch(values);
    }

    public static ArrayDataPortalClass GetChild(IChildDataPortal<ArrayDataPortalClass> dataPortal, int[] values)
    {
      return dataPortal.FetchChild(values);
    }

    public static ArrayDataPortalClass GetChild(IChildDataPortal<ArrayDataPortalClass> dataPortal, string[] values)
    {
      return dataPortal.FetchChild(values);
    }

    public static ArrayDataPortalClass GetChildParams(IChildDataPortal<ArrayDataPortalClass> dataPortal, params int[] values)
    {
      return dataPortal.FetchChild(values);
    }

    public static ArrayDataPortalClass GetChildParams(IChildDataPortal<ArrayDataPortalClass> dataPortal, params string[] values)
    {
      return dataPortal.FetchChild(values);
    }

    [Fetch]
    private void Fetch(int[] values)
    {
      TestResults.Add("Method", "Fetch(int[] values)");
    }

    [Fetch]
    private void Fetch(string[] values)
    {
      TestResults.Add("Method", "Fetch(string[] values)");
    }

    [FetchChild]
    private void FetchChild(int[] values)
    {
      TestResults.Add("Method", "FetchChild(int[] values)");
    }

    [FetchChild]
    private void FetchChild(string[] values)
    {
      TestResults.Add("Method", "FetchChild(string[] values)");
    }
  }

}
