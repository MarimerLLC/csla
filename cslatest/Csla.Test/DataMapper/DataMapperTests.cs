using System;
using System.Collections.Generic;
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

namespace Csla.Test.DataMapper
{
  [TestClass]
  public class DataMapperTests
  {
    [TestMethod]
    public void NumericTypes()
    {
      DataMapTarget target = new DataMapTarget();
      
      Csla.Data.DataMapper.SetPropertyValue(target, "MyInt", 42);
      Assert.AreEqual(target.MyInt, 42, "Int should match");
      
      Csla.Data.DataMapper.SetPropertyValue(target, "MyDouble", 4.2);
      Assert.AreEqual(target.MyDouble, 4.2, "Double should match");
      
      Csla.Data.DataMapper.SetPropertyValue(target, "MyBool", true);
      Assert.AreEqual(target.MyBool, true, "Bool should be true");

      Csla.Data.DataMapper.SetPropertyValue(target, "MyBool", false);
      Assert.AreEqual(target.MyBool, false, "Bool should be false");
    }

    [TestMethod]
    public void NullableTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Csla.Data.DataMapper.SetPropertyValue(target, "MyNInt", 42);
      Assert.AreEqual(target.MyNInt, 42, "Int should match");

      Csla.Data.DataMapper.SetPropertyValue(target, "MyNInt", null);
      Assert.AreEqual(target.MyNInt, null, "Int should be null");
    }

    [TestMethod]
    public void EnumTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Csla.Data.DataMapper.SetPropertyValue(target, "MyEnum", DataMapEnum.Second);
      Assert.AreEqual(target.MyEnum, DataMapEnum.Second, "Enum should be Second");

      Csla.Data.DataMapper.SetPropertyValue(target, "MyEnum", "First");
      Assert.AreEqual(target.MyEnum, DataMapEnum.First, "Enum should be First");
    }
  }

  public enum DataMapEnum
  {
    First,
    Second,
    Third
  }

  public class DataMapTarget
  {
    private int _int;

    public int MyInt
    {
      get { return _int; }
      set { _int = value; }
    }

    private double _double;

    public double MyDouble
    {
      get { return _double; }
      set { _double = value; }
    }

    private bool _bool;

    public bool MyBool
    {
      get { return _bool; }
      set { _bool = value; }
    }

    private Nullable<int> _nint;

    public Nullable<int> MyNInt
    {
      get { return _nint; }
      set { _nint = value; }
    }

    private DataMapEnum _enum;

    public DataMapEnum MyEnum
    {
      get { return _enum; }
      set { _enum = value; }
    }
	
  }
}
