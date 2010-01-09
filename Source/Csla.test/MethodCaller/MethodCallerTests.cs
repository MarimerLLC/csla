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
#if NUNIT
				Assert.IsInstanceOfType(typeof(Csla.Reflection.CallMethodException), ex, "Should be a CallMethodException");
				Assert.IsInstanceOfType(typeof(NotSupportedException), ex.InnerException, "Inner should be a NotSupportedException");
#else
        Assert.IsInstanceOfType(ex, typeof(Csla.Reflection.CallMethodException), "Should be a CallMethodException");
        Assert.IsInstanceOfType(ex.InnerException, typeof(NotSupportedException), "Inner should be a NotSupportedException");
#endif
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
#if NUNIT
        Assert.IsInstanceOfType(typeof(Csla.Reflection.CallMethodException), ex, "Should be a CallMethodException");
				Assert.IsInstanceOfType(typeof(MemberAccessException), ex.InnerException, "Inner should be a MemberAccessException");
        Assert.IsInstanceOfType(typeof(NotSupportedException), ex.InnerException.InnerException, "Inner inner should be a NotSupportedException");
#else
				Assert.IsInstanceOfType(ex, typeof(Csla.Reflection.CallMethodException), "Should be a CallMethodException");
				Assert.IsInstanceOfType(ex.InnerException, typeof(MemberAccessException), "Inner should be a MemberAccessException");
				Assert.IsInstanceOfType(ex.InnerException.InnerException, typeof(NotSupportedException), "Inner inner should be a NotSupportedException");
#endif
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
      var returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", table.Rows);
      Assert.AreEqual(1, returnValue, "table.Rows");
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", rows);
      Assert.AreEqual(2, returnValue, "rows");
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", new string[] { "a", "b" });
      Assert.AreEqual(3, returnValue, "string[]");
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", 1, rows);
      Assert.AreEqual(4, returnValue, "1, rows");
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", 1, new string[] { "a", "b" });
      Assert.AreEqual(5, returnValue, "1, string[]");
    }

    [TestMethod]
    public void CallSuccessParams()
    {
      var returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "MethodWithParams", new object[] { 1, 2 });
      Assert.AreEqual(returnValue, 1);
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "MethodWithParams", new object[] { 123 });
      Assert.AreEqual(returnValue, 1);
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "MethodWithParams");
      Assert.AreEqual(returnValue, 1);
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

    private int GetData(params string[] rows)
    {
      return 3;
    }

    private int GetData(int x, params System.Data.DataRow[] rows)
    {
      return 4;
    }

    private int GetData(int x, params string[] rows)
    {
      return 5;
    }

    public int MethodWithParams(params object[] e)
    {
      return 1;
    }
  }
}
