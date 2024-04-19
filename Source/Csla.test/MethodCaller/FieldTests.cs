//-----------------------------------------------------------------------
// <copyright file="FieldTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.MethodCaller
{
  [TestClass]
  public class FieldTests
  {

      private class Test1 {
#pragma warning disable CS0414
      private string _f1 = "private"; // accessed by tests via reflection
#pragma warning restore CS0414
      public string _f2 = "public";
    }

    [TestMethod]
    public void PrivateFieldGetSuccess()
    {
      var instance = new Test1();
      var expected = "private";
      var actual = Csla.Data.DataMapper.GetFieldValue(instance, "_f1");
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void PrivateFieldSetFail()
    {
      var instance = new Test1();
      var expected = "success";
      Csla.Data.DataMapper.SetFieldValue(instance, "_f1", expected);
      var actual = Csla.Data.DataMapper.GetFieldValue(instance, "_f1");
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void PublicFieldGetSuccess()
    {
      var instance = new Test1();
      var expected = "public";
      var actual = Csla.Data.DataMapper.GetFieldValue(instance, "_f2");
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void PublicFieldSetSuccess()
    {
      var instance = new Test1();
      var expected = "one";
      Csla.Data.DataMapper.SetFieldValue(instance, "_f2", expected);

      var actual = instance._f2;
      Assert.AreEqual(expected, actual);
    }
  }
}