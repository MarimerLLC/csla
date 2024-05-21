//-----------------------------------------------------------------------
// <copyright file="DataMapperTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataMapper
{
  [TestClass]
  public class DataMapperTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void DictionaryMap()
    {
      IDataPortal<ManagedTarget> dataPortal = _testDIContext.CreateDataPortal<ManagedTarget>();

      var target = dataPortal.Create();
      var source = new Dictionary<string, object>();
      source.Add("MyInt", 42);

      Data.DataMapper.Load(source, target, n => n);
      Assert.AreEqual(42, target.MyInt, "Int should match");

    }

    [TestMethod]
    public void NumericTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Data.DataMapper.SetPropertyValue(target, "MyInt", 42);
      Assert.AreEqual(42, target.MyInt, "Int should match");

      Data.DataMapper.SetPropertyValue(target, "MyInt", "24");
      Assert.AreEqual(24, target.MyInt, "Int from string should be 24");

      Data.DataMapper.SetPropertyValue(target, "MyInt", "");
      Assert.AreEqual(0, target.MyInt, "Int from empty string should be 0");

      Data.DataMapper.SetPropertyValue(target, "MyInt", null);
      Assert.AreEqual(0, target.MyInt, "Int from null should be 0");

      Data.DataMapper.SetPropertyValue(target, "MyDouble", 4.2);
      Assert.AreEqual(4.2, target.MyDouble, "Double should match");
    }

    [TestMethod]
    public void BooleanTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Data.DataMapper.SetPropertyValue(target, "MyBool", true);
      Assert.AreEqual(true, target.MyBool, "Bool should be true");

      Data.DataMapper.SetPropertyValue(target, "MyBool", false);
      Assert.AreEqual(false, target.MyBool, "Bool should be false");

      Data.DataMapper.SetPropertyValue(target, "MyBool", "");
      Assert.AreEqual(false, target.MyBool, "Bool from empty string should be false");

      Data.DataMapper.SetPropertyValue(target, "MyBool", null);
      Assert.AreEqual(false, target.MyBool, "Bool from null should be false");
    }

    [TestMethod]
    public void GuidTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Guid testValue = Guid.NewGuid();

      Data.DataMapper.SetPropertyValue(target, "MyGuid", testValue);
      Assert.AreEqual(testValue, target.MyGuid, "Guid values should match");

      Data.DataMapper.SetPropertyValue(target, "MyGuid", Guid.Empty);
      Assert.AreEqual(Guid.Empty, target.MyGuid, "Empty guid values should match");

      Data.DataMapper.SetPropertyValue(target, "MyGuid", testValue.ToString());
      Assert.AreEqual(testValue, target.MyGuid, "Guid values from string should match");
    }

    [TestMethod]
    public void NullableTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Data.DataMapper.SetPropertyValue(target, "MyNInt", 42);
      Assert.AreEqual(42, target.MyNInt, "Int should match");

      Data.DataMapper.SetPropertyValue(target, "MyNInt", 0);
      Assert.AreEqual(0, target.MyNInt, "Int should be 0");

      Data.DataMapper.SetPropertyValue(target, "MyNInt", string.Empty);
      Assert.AreEqual(null, target.MyNInt, "Int from string.Empty should be null");

      Data.DataMapper.SetPropertyValue(target, "MyNInt", null);
      Assert.AreEqual(null, target.MyNInt, "Int should be null");
    }

    [TestMethod]
    public void EnumTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Data.DataMapper.SetPropertyValue(target, "MyEnum", DataMapEnum.Second);
      Assert.AreEqual(DataMapEnum.Second, target.MyEnum, "Enum should be Second");

      Data.DataMapper.SetPropertyValue(target, "MyEnum", "First");
      Assert.AreEqual(DataMapEnum.First, target.MyEnum, "Enum should be First");

      Data.DataMapper.SetPropertyValue(target, "MyEnum", 2);
      Assert.AreEqual(DataMapEnum.Third, target.MyEnum, "Enum should be Third");
    }

    [TestMethod]
    public void DateTimeTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Data.DataMapper.SetPropertyValue(target, "MyDate", DateTime.Today);
      Assert.AreEqual(DateTime.Today, target.MyDate, "Date should be Today");

      Data.DataMapper.SetPropertyValue(target, "MyDate", "1/1/2007");
      Assert.AreEqual(new DateTime(2007, 1, 1), target.MyDate, "Date should be 1/1/2007");

      Data.DataMapper.SetPropertyValue(target, "MyDate", new Csla.SmartDate("1/1/2007"));
      Assert.AreEqual(new DateTime(2007, 1, 1), target.MyDate, "Date should be 1/1/2007");
    }

    [TestMethod]
    public void SmartDateTypes()
    {
      DataMapTarget target = new DataMapTarget();

      Data.DataMapper.SetPropertyValue(target, "MySmartDate", DateTime.Today);
      Assert.AreEqual(new Csla.SmartDate(DateTime.Today), target.MySmartDate, "SmartDate should be Today");

      Data.DataMapper.SetPropertyValue(target, "MySmartDate", "1/1/2007");
      Assert.AreEqual(new Csla.SmartDate(new DateTime(2007, 1, 1)), target.MySmartDate, "SmartDate should be 1/1/2007");

      Data.DataMapper.SetPropertyValue(target, "MySmartDate", new Csla.SmartDate("1/1/2007"));
      Assert.AreEqual(new Csla.SmartDate(new DateTime(2007, 1, 1)), target.MySmartDate, "SmartDate should be 1/1/2007");

      Data.DataMapper.SetPropertyValue(target, "MySmartDate", new DateTimeOffset(new DateTime(2004, 3, 2)));
      Assert.AreEqual(new Csla.SmartDate(new DateTime(2004, 3, 2)), target.MySmartDate, "SmartDate should be 3/2/2004");

      target.MySmartDate = new Csla.SmartDate(DateTime.Today, Csla.SmartDate.EmptyValue.MaxDate);
      Assert.IsFalse(target.MySmartDate.EmptyIsMin, "EmptyIsMin should be false before set");
      Data.DataMapper.SetPropertyValue(target, "MySmartDate", DateTime.Parse("1/1/2007"));
      Assert.IsFalse(target.MySmartDate.EmptyIsMin, "EmptyIsMin should be false after set");
    }

    [TestMethod]
    public void SetFields()
    {
      DataMapTarget target = new DataMapTarget();

      Data.DataMapper.SetFieldValue(target, "_int", 42);
      Assert.AreEqual(42, target.MyInt, "Int should match");

      Data.DataMapper.SetFieldValue(target, "_double", 4.2);
      Assert.AreEqual(4.2, target.MyDouble, "Double should match");

      Data.DataMapper.SetFieldValue(target, "_bool", true);
      Assert.AreEqual(true, target.MyBool, "Bool should be true");

      Data.DataMapper.SetFieldValue(target, "_bool", false);
      Assert.AreEqual(false, target.MyBool, "Bool should be false");

      Data.DataMapper.SetFieldValue(target, "_smartDate", "2/1/2007");
      Assert.AreEqual(new Csla.SmartDate("2/1/2007"), target.MySmartDate, "SmartDate should be 2/1/2007");

      Data.DataMapper.SetFieldValue(target, "_smartDate", new Csla.SmartDate("1/1/2007"));
      Assert.AreEqual(new Csla.SmartDate("1/1/2007"), target.MySmartDate, "SmartDate should be 1/1/2007");

      Data.DataMapper.SetFieldValue(target, "_smartDate", new DateTimeOffset(new DateTime(2004, 3, 2)));
      Assert.AreEqual(new Csla.SmartDate(new DateTime(2004, 3, 2)), target.MySmartDate, "SmartDate should be 3/2/2004");
    }

    [TestMethod]
    public void BasicDataMap()
    {
      Data.DataMap map = new Data.DataMap(typeof(DataMapTarget), typeof(DataMapTarget));

      DataMapTarget source = new DataMapTarget();
      DataMapTarget target = new DataMapTarget();
      source.MyInt = 123;
      source.MyDouble = 456;
      source.MyBool = true;
      source.MyEnum = DataMapEnum.Second;
      var g = Guid.NewGuid();
      source.MyGuid = g;
      source.MyNInt = 321;
      source.MySmartDate = new Csla.SmartDate(new DateTime(2002, 12, 4));
      source.MyDate = new DateTime(2002, 11, 2);
      source.MyString = "Third";

      map.AddFieldMapping("_int", "_int");
      map.AddFieldToPropertyMapping("_double", "MyDouble");
      map.AddPropertyMapping("MyBool", "MyBool");
      map.AddPropertyToFieldMapping("MyGuid", "_guid");
      map.AddPropertyMapping("MyEnum", "MyString");
      map.AddPropertyMapping("MyString", "MyEnum");

      Data.DataMapper.Map(source, target, map);

      Assert.AreEqual(123, target.MyInt, "Int should match");
      Assert.AreEqual(456, target.MyDouble, "Double should match");
      Assert.AreEqual(true, target.MyBool, "bool should match");
      Assert.AreEqual(g, target.MyGuid, "guid should match");
      Assert.AreEqual("Second", target.MyString, "string should match (converted enum)");
      Assert.AreEqual(DataMapEnum.Third, target.MyEnum, "enum should match (parsed enum)");
      Assert.AreNotEqual(source.MyDate, target.MyDate, "Dates should not match");
    }
  }

  public enum DataMapEnum
  {
    First,
    Second,
    Third
  }

  [Serializable]
  public class ManagedTarget : BusinessBase<ManagedTarget>
  {
    public static PropertyInfo<int> MyIntProperty = RegisterProperty<int>(c => c.MyInt);
    public int MyInt
    {
      get { return GetProperty(MyIntProperty); }
      set { SetProperty(MyIntProperty, value); }
    }

    [Create]
    private void Create()
    {
    }
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

    private DateTime _date;

    public DateTime MyDate
    {
      get { return _date; }
      set { _date = value; }
    }

    private Csla.SmartDate _smartDate;

    public Csla.SmartDate MySmartDate
    {
      get { return _smartDate; }
      set { _smartDate = value; }
    }

    private Guid _guid;

    public Guid MyGuid
    {
      get { return _guid; }
      set { _guid = value; }
    }

    private string _string;
    public string MyString
    {
      get { return _string; }
      set { _string = value; }
    }
  }
}