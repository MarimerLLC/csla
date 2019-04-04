//-----------------------------------------------------------------------
// <copyright file="MethodCallerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Reflection;
using Csla.Reflection;
using UnitDriven;
using System.Collections.Generic;

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
        Assert.IsInstanceOfType(ex, typeof(Csla.Reflection.CallMethodException), "Should be a CallMethodException");
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
#if MSTEST
        Assert.IsInstanceOfType(ex, typeof(Csla.Reflection.CallMethodException), "Should be a CallMethodException");
        Assert.IsInstanceOfType(ex.InnerException, typeof(MemberAccessException), "Inner should be a MemberAccessException");
        Assert.IsInstanceOfType(ex.InnerException.InnerException, typeof(NotSupportedException), "Inner inner should be a NotSupportedException");
#else
        Assert.IsInstanceOfType(typeof(Csla.Reflection.CallMethodException), ex, "Should be a CallMethodException");
        Assert.IsInstanceOfType(typeof(MemberAccessException), ex.InnerException, "Inner should be a MemberAccessException");
        Assert.IsInstanceOfType(typeof(NotSupportedException), ex.InnerException.InnerException, "Inner inner should be a NotSupportedException");
#endif
      }
    }

    [TestMethod]
    public void CallSuccessArrayParameters()
    {
      Dictionary<string, List<int>> table = new Dictionary<string, List<int>>();

      table.Add("Column1", new List<int>());
      table["Column1"].Add(1);
      table["Column1"].Add(2);
      var returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", table["Column1"]);
      Assert.AreEqual(1, returnValue, "table.Rows");
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", table["Column1"].ToArray());
      Assert.AreEqual(2, returnValue, "rows");
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", new string[] { "a", "b" });
      Assert.AreEqual(3, returnValue, "string[]");
      returnValue = Csla.Reflection.MethodCaller.CallMethod(this, "GetData", 1, table);
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

#if DEBUG
#if !WINDOWS_PHONE
    [TestMethod]
    [Ignore]
    public void CallDynamicIsFasterThanReflectionSuccess()
    {
      int times = 100000;

      TimeSpan dynamicTime, reflectionTime;
      var start = DateTime.Now;
      for (int x = 0; x < times; x++)
      {
        Csla.Reflection.MethodCaller.CallMethod(this, "DoSuccess");
      }
      var end = DateTime.Now;
      dynamicTime = end - start;

      start = DateTime.Now;
      for (int x = 0; x < times; x++)
      {
        var doSuccess = this.GetType().GetMethod("DoSuccess", BindingFlags.Instance | BindingFlags.Public);
        doSuccess.Invoke(this, null);
      }
      end = DateTime.Now;
      reflectionTime = end - start;

      Assert.IsTrue(dynamicTime < reflectionTime, string.Format("Dynamic {0} should be faster than reflection {1}", dynamicTime, reflectionTime));
    }
#endif
#endif

    [TestMethod]
    [ExpectedException(typeof(CallMethodException))]
    public void CallWithInvalidParameterTypeFails()
    {
      MemoryStream ms = new MemoryStream();
      var t = this.GetType();
      var foo = t.GetMethod("foo");
      Csla.Reflection.MethodCaller.CallMethod(this, foo, "This should be a MemoryStream object not a string...");
    }

    [TestMethod]
    public void CallWithValidParameterTypeSucceeds()
    {
      MemoryStream ms = new MemoryStream();
      var t = this.GetType();
      var foo = t.GetMethod("foo");
      Csla.Reflection.MethodCaller.CallMethod(this, foo, ms);
    }

    public void foo(MemoryStream ms)
    {
      // As ridiculous as this Assert seems, it's actually possible for ms
      // to be the wrong type. See bug #550.
#if MSTEST
      Assert.IsInstanceOfType(ms, typeof(MemoryStream));
#else
      Assert.IsInstanceOfType(typeof(MemoryStream), ms);
#endif
    }

    public void DoSuccess()
    {
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

    public int GetData(object rows)
    {
      return 1;
    }

    public int GetData(params int[] rows)
    {
      return 2;
    }

    public int GetData(params string[] rows)
    {
      return 3;
    }

    public int GetData(int x, Dictionary<string, List<int>> rows)
    {
      return 4;
    }

    public int GetData(int x, params string[] rows)
    {
      return 5;
    }

    public int MethodWithParams(params object[] e)
    {
      return 1;
    }
  }
}