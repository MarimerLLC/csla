using Csla;
using Csla.DataPortalClient;
using UnitDriven;
using Csla.Testing.Business.CommandBase;
using System;
using Csla.Test.MethodCaller;

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

namespace cslalighttest.MethodCaller
{
  [TestClass]
  public class MethodCallerTests : TestBase
  {
    [TestMethod]
    public void CallSuccess()
    {
      var context = GetContext();
      Csla.Reflection.MethodCaller.CallMethod(this, "DoSuccess");
      context.Assert.IsTrue(true);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void CallException()
    {

      var context = GetContext();
      try
      {
        Csla.Reflection.MethodCaller.CallMethod(this, "DoException");
        context.Assert.IsFalse(true);
      }
      catch (Exception ex)
      {
        context.Assert.IsTrue(ex is Csla.Reflection.CallMethodException, "Should be a CallMethodException");
        context.Assert.IsTrue(ex.InnerException is NotSupportedException, "Inner should be a NotSupportedException");
        context.Assert.Success();
      }
      context.Complete();
    }

    [TestMethod]
    public void CallInnerException()
    {

      var context = GetContext();
      try
      {
        Csla.Reflection.MethodCaller.CallMethod(this, "DoInnerException");
        context.Assert.IsFalse(true);
      }
      catch (Exception ex)
      {
        context.Assert.IsTrue(ex is Csla.Reflection.CallMethodException, "Should be a CallMethodException");
        context.Assert.IsTrue(ex.InnerException is MemberAccessException, "Inner should be a NotSupportedException");
        context.Assert.IsTrue(ex.InnerException.InnerException is NotSupportedException, "Inner should be a NotSupportedException");
        context.Assert.Success();
      }
      context.Complete();
    }

    [TestMethod]
    public void CallSuccessArrayParameters()
    {

      var context = GetContext();
      System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
      list.Add(1);
      list.Add(2);

      object[] rows = new object[2] { 1, 2 };

      var returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", list);
      context.Assert.AreEqual(returnValue, 1);
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", 10, rows);
      context.Assert.AreEqual(returnValue, 2);
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", rows);
      context.Assert.AreEqual(returnValue, 3);
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", 10);
      context.Assert.AreEqual(returnValue, 4);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void CallSuccessParams()
    {
      var context = GetContext();
      var returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "MethodWithParams", new object[] { 1, 2 });
      context.Assert.AreEqual(returnValue, 1);
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "MethodWithParams", new object[] { 123 });
      context.Assert.AreEqual(returnValue, 1);
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "MethodWithParams");
      context.Assert.AreEqual(returnValue, 1);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void CallSuccessInheritedMethods()
    {

      var context = GetContext();
      var returnValue1 = Csla.Reflection.MethodCaller.CallMethod(new Caller1(), "Method1");
      context.Assert.AreEqual(returnValue1, 1);
      var returnValue2 = Csla.Reflection.MethodCaller.CallMethod(new Caller2(), "Method1");
      context.Assert.AreEqual(returnValue2, 2);
      var returnValue3 = Csla.Reflection.MethodCaller.CallMethod(new Caller3(), "Method1");
      context.Assert.AreEqual(returnValue3, 2);

      returnValue1 = Csla.Reflection.MethodCaller.CallMethod(new Caller1(), "Method2", 0);
      context.Assert.AreEqual(returnValue1, 1);
      returnValue2 = Csla.Reflection.MethodCaller.CallMethod(new Caller2(), "Method2", 0);
      context.Assert.AreEqual(returnValue2, 2);
      returnValue3 = Csla.Reflection.MethodCaller.CallMethod(new Caller3(), "Method2", 0);
      context.Assert.AreEqual(returnValue3, 2);
      context.Assert.Success();
      context.Complete();
    }

    public void DoSuccess()
    {
      var x = 0;
    }

    public void DoException()
    {
      throw new NotSupportedException("DoException");
    }

    public void DoInnerException()
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

    public int GetData(System.Collections.Generic.List<int> list)
    {
      return 1;
    }

    public int GetData(int anumber, object[] rows)
    {
      return 2;
    }

    public int GetData(object[] rows)
    {
      return 3;
    }

    public int GetData(int anumber)
    {
      return 4;
    }

    public int MethodWithParams(params object[] e)
    {
      return 1;
    }
  }
}
