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
  }
}
