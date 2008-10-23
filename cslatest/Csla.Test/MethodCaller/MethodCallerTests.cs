using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.MethodCaller
{
  [TestClass]
  public class MethodCallerTests
  {
    [TestMethod]
    public void CallSuccess()
    {
      Csla.Reflection.MethodCaller.CallMethod(this, "DoSuccess", null);
    }

    [TestMethod]
    public void CallException()
    {
      try
      {
        Csla.Reflection.MethodCaller.CallMethod(this, "DoException", null);
      }
      catch (Exception ex)
      {
        Assert.IsInstanceOfType(typeof(Csla.Reflection.CallMethodException), ex, "Should be a CallMethodException");
        Assert.IsInstanceOfType(typeof(NotSupportedException), ex.InnerException, "Inner should be a NotSupportedException");
      }
    }

    [TestMethod]
    public void CallInnerException()
    {
      try
      {
        Csla.Reflection.MethodCaller.CallMethod(this, "DoInnerException", null);
      }
      catch (Exception ex)
      {
        Assert.IsInstanceOfType(typeof(Csla.Reflection.CallMethodException), ex, "Should be a CallMethodException");
        Assert.IsInstanceOfType(typeof(MemberAccessException), ex.InnerException, "Inner should be a MemberAccessException");
        Assert.IsInstanceOfType(typeof(NotSupportedException), ex.InnerException.InnerException, "Inner inner should be a NotSupportedException");
      }
    }

    [TestMethod]
    public void CallSuccessArrayParameters()
    {
      System.Data.DataTable table = new System.Data.DataTable();
      table.Columns.Add("Column1");
      table.Rows.Add(new object[] {"1"});
      table.Rows.Add(new object[] { "2" });
      System.Data.DataRow[] rows = new System.Data.DataRow[2];
      rows[0] = table.Rows[0];
      rows[1] = table.Rows[0];
      var returnValue1 = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", table.Rows);
      Assert.AreEqual(returnValue1, 1);
      var returnValue2 = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", rows);
      Assert.AreEqual(returnValue2, 2);
    }

    [TestMethod]
    public void CallSuccessInheritedMethods()
    {
      
      var returnValue1 = Csla.Reflection.MethodCaller.CallMethod(new Caller1(), "Method1");
      Assert.AreEqual(returnValue1, 1);
      var returnValue2 = Csla.Reflection.MethodCaller.CallMethod(new Caller2(), "Method1");
      Assert.AreEqual(returnValue2, 2);
      var returnValue3 = Csla.Reflection.MethodCaller.CallMethod(new Caller3(), "Method1");
      Assert.AreEqual(returnValue3, 2);

      returnValue1 = Csla.Reflection.MethodCaller.CallMethod(new Caller1(), "Method2", 0);
      Assert.AreEqual(returnValue1, 1);
      returnValue2 = Csla.Reflection.MethodCaller.CallMethod(new Caller2(), "Method2", 0);
      Assert.AreEqual(returnValue2, 2);
      returnValue3 = Csla.Reflection.MethodCaller.CallMethod(new Caller3(), "Method2", 0);
      Assert.AreEqual(returnValue3, 2);
    }

    private void DoSuccess()
    {
      var x = 0;
    }

    private void DoException()
    {
      throw new NotSupportedException("DoException");
    }

    private void DoInnerException()
    {
      try
      {
        DoException();
      }
      catch (Exception ex)
      {
        throw new MemberAccessException("DoInnerException", ex);
      }
    }

    private int GetData(System.Data.DataRowCollection rows)
    {
      return 1;
    }

    private int GetData(params System.Data.DataRow[] rows)
    {
      return 2;
    }
  }
}
