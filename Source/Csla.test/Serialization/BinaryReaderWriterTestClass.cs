using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using Csla.Core;
using Csla;

namespace Csla.Test.Serialization
{
  public enum RandomEnum
  {
    None = 0,
    Value1 = 1,
    Value2 = 2
  }

  [Serializable]
  public class BinaryReaderWriterTestClassList : BusinessListBase<BinaryReaderWriterTestClassList, BinaryReaderWriterTestClass>
  {
    public BinaryReaderWriterTestClassList()
    {

    }

    public void Setup()
    {
    }

    public static BinaryReaderWriterTestClassList NewBinaryReaderWriterTestClassList(IDataPortal<BinaryReaderWriterTestClassList> dataPortal)
    {
      return dataPortal.Create();
    }

    [Create]
    private void Create([Inject] IDataPortal<BinaryReaderWriterTestClass> childDataPortal)
    {
      Add(BinaryReaderWriterTestClass.NewBinaryReaderWriterTestClass(childDataPortal, true));
      Add(BinaryReaderWriterTestClass.NewBinaryReaderWriterTestClass(childDataPortal, true));
      this[0].Setup();
      this[1].Setup();
      this[1].StringTest = "random";
    }
  }

  [Serializable]
  public class BinaryReaderWriterTestClass : BusinessBase<BinaryReaderWriterTestClass>
  {

    public BinaryReaderWriterTestClass()
    {

    }

    public BinaryReaderWriterTestClass(bool isChild)
    {
      if (isChild)
        MarkAsChild();
    }

    public void Setup()
    {
      this.BoolTest = true;
      this.ByteArrayTest = new byte[] { 1, 2, 3, 4 };
      this.ByteTest = 3;
      this.CharArrayTest = "abc".ToArray();
      this.CharTest = "a".ToArray()[0];
      this.DateTimeOffsetTest = new DateTimeOffset(new DateTime(2001, 1, 2, 3, 4, 5, 6), new TimeSpan(0, 1, 0, 0));
      this.DateTimeTest = new DateTime(2000, 1, 2, 3, 4, 5, 6);
      this.DecimalTest = 1.2m;
      this.DoubleTest = 123.45567;
      this.GuidTest = Guid.NewGuid();
      this.Int16Test = 1;
      this.Int32Test = 2;
      this.Int64Test = 3;
      this.SByteTest = 4;
      this.SingleTest = (Single)12.3;
      this.StringTest = "abcd";
      this.TimeSpanTest = new TimeSpan(2, 3, 4, 5, 7);
      this.UInt16Test = 11;
      this.UInt32Test = 22;
      this.UInt64Test = 33;
      this.EnumTest = RandomEnum.Value1;
      this.NullableInt = null;
      this.NullableButSetInt = 22;
      this.EmptySmartDateTest = new Csla.SmartDate(false);
      this.FilledSmartDateTest = new Csla.SmartDate(new DateTime(2001, 1, 1), true) { FormatString = "yyyy/MM/dd" };
    }

    public static readonly PropertyInfo<RandomEnum> EnumTestProperty = RegisterProperty<RandomEnum>(c => c.EnumTest);
    /// <Summary>
    /// Gets or sets the EnumTest value.
    /// </Summary>
    public RandomEnum EnumTest
    {
      get { return GetProperty(EnumTestProperty); }
      set { SetProperty(EnumTestProperty, value); }
    }

    public static readonly PropertyInfo<bool> BoolTestProperty = RegisterProperty<bool>(c => c.BoolTest);
    /// <Summary>
    /// Gets or sets the BoolTest value.
    /// </Summary>
    public bool BoolTest
    {
      get { return GetProperty(BoolTestProperty); }
      set { SetProperty(BoolTestProperty, value); }
    }

    public static readonly PropertyInfo<char> CharTestProperty = RegisterProperty<char>(c => c.CharTest);
    /// <Summary>
    /// Gets or sets the CharTest value.
    /// </Summary>
    public char CharTest
    {
      get { return GetProperty(CharTestProperty); }
      set { SetProperty(CharTestProperty, value); }
    }

    public static readonly PropertyInfo<SByte> SByteTestProperty = RegisterProperty<SByte>(c => c.SByteTest);
    /// <Summary>
    /// Gets or sets the SByteTest value.
    /// </Summary>
    public SByte SByteTest
    {
      get { return GetProperty(SByteTestProperty); }
      set { SetProperty(SByteTestProperty, value); }
    }

    public static readonly PropertyInfo<byte> ByteTestProperty = RegisterProperty<byte>(c => c.ByteTest);
    /// <Summary>
    /// Gets or sets the ByteTest value.
    /// </Summary>
    public byte ByteTest
    {
      get { return GetProperty(ByteTestProperty); }
      set { SetProperty(ByteTestProperty, value); }
    }

    public static readonly PropertyInfo<Int16> Int16TestProperty = RegisterProperty<Int16>(c => c.Int16Test);
    /// <Summary>
    /// Gets or sets the Int16Test value.
    /// </Summary>
    public Int16 Int16Test
    {
      get { return GetProperty(Int16TestProperty); }
      set { SetProperty(Int16TestProperty, value); }
    }

    public static readonly PropertyInfo<UInt16> UInt16TestProperty = RegisterProperty<UInt16>(c => c.UInt16Test);
    /// <Summary>
    /// Gets or sets the UInt16Test value.
    /// </Summary>
    public UInt16 UInt16Test
    {
      get { return GetProperty(UInt16TestProperty); }
      set { SetProperty(UInt16TestProperty, value); }
    }

    public static readonly PropertyInfo<Int32> Int32TestProperty = RegisterProperty<Int32>(c => c.Int32Test);
    /// <Summary>
    /// Gets or sets the IntTest value.
    /// </Summary>
    public Int32 Int32Test
    {
      get { return GetProperty(Int32TestProperty); }
      set { SetProperty(Int32TestProperty, value); }
    }

    public static readonly PropertyInfo<UInt32> UInt32TestProperty = RegisterProperty<UInt32>(c => c.UInt32Test);
    /// <Summary>
    /// Gets or sets the UInt32Test value.
    /// </Summary>
    public UInt32 UInt32Test
    {
      get { return GetProperty(UInt32TestProperty); }
      set { SetProperty(UInt32TestProperty, value); }
    }

    public static readonly PropertyInfo<Int64> Int64TestProperty = RegisterProperty<Int64>(c => c.Int64Test);
    /// <Summary>
    /// Gets or sets the Int64Test value.
    /// </Summary>
    public Int64 Int64Test
    {
      get { return GetProperty(Int64TestProperty); }
      set { SetProperty(Int64TestProperty, value); }
    }

    public static readonly PropertyInfo<UInt64> UInt64TestProperty = RegisterProperty<UInt64>(c => c.UInt64Test);
    /// <Summary>
    /// Gets or sets the UInt64Test value.
    /// </Summary>
    public UInt64 UInt64Test
    {
      get { return GetProperty(UInt64TestProperty); }
      set { SetProperty(UInt64TestProperty, value); }
    }

    public static readonly PropertyInfo<Single> SingleTestProperty = RegisterProperty<Single>(c => c.SingleTest);
    /// <Summary>
    /// Gets or sets the SingleTest value.
    /// </Summary>
    public Single SingleTest
    {
      get { return GetProperty(SingleTestProperty); }
      set { SetProperty(SingleTestProperty, value); }
    }

    public static readonly PropertyInfo<double> DoubleTestProperty = RegisterProperty<double>(c => c.DoubleTest);
    /// <Summary>
    /// Gets or sets the DoubleTest value.
    /// </Summary>
    public double DoubleTest
    {
      get { return GetProperty(DoubleTestProperty); }
      set { SetProperty(DoubleTestProperty, value); }
    }

    public static readonly PropertyInfo<decimal> DecimalTestProperty = RegisterProperty<decimal>(c => c.DecimalTest);
    /// <Summary>
    /// Gets or sets the DecimalTest value.
    /// </Summary>
    public decimal DecimalTest
    {
      get { return GetProperty(DecimalTestProperty); }
      set { SetProperty(DecimalTestProperty, value); }
    }

    public static readonly PropertyInfo<DateTime> DateTimeTestProperty = RegisterProperty<DateTime>(c => c.DateTimeTest);
    /// <Summary>
    /// Gets or sets the DateTimeTest value.
    /// </Summary>
    public DateTime DateTimeTest
    {
      get { return GetProperty(DateTimeTestProperty); }
      set { SetProperty(DateTimeTestProperty, value); }
    }

    public static readonly PropertyInfo<string> StringTestProperty = RegisterProperty<string>(c => c.StringTest);
    /// <Summary>
    /// Gets or sets the StringTest value.
    /// </Summary>
    public string StringTest
    {
      get { return GetProperty(StringTestProperty); }
      set { SetProperty(StringTestProperty, value); }
    }

    public static readonly PropertyInfo<TimeSpan> TimeSpanTestProperty = RegisterProperty<TimeSpan>(c => c.TimeSpanTest);
    /// <Summary>
    /// Gets or sets the TimeSpanTest value.
    /// </Summary>
    public TimeSpan TimeSpanTest
    {
      get { return GetProperty(TimeSpanTestProperty); }
      set { SetProperty(TimeSpanTestProperty, value); }
    }

    public static readonly PropertyInfo<DateTimeOffset> DateTimeOffsetTestProperty = RegisterProperty<DateTimeOffset>(c => c.DateTimeOffsetTest);
    /// <Summary>
    /// Gets or sets the DateTimeOffsetTest value.
    /// </Summary>
    public DateTimeOffset DateTimeOffsetTest
    {
      get { return GetProperty(DateTimeOffsetTestProperty); }
      set { SetProperty(DateTimeOffsetTestProperty, value); }
    }

    public static readonly PropertyInfo<byte[]> ByteArrayTestProperty = RegisterProperty<byte[]>(c => c.ByteArrayTest);
    /// <Summary>
    /// Gets or sets the ByteArrayTest value.
    /// </Summary>
    public byte[] ByteArrayTest
    {
      get { return GetProperty(ByteArrayTestProperty); }
      set { SetProperty(ByteArrayTestProperty, value); }
    }

    public static readonly PropertyInfo<char[]> CharArrayTestProperty = RegisterProperty<char[]>(c => c.CharArrayTest);
    /// <Summary>
    /// Gets or sets the CharArrayTest value.
    /// </Summary>
    public char[] CharArrayTest
    {
      get { return GetProperty(CharArrayTestProperty); }
      set { SetProperty(CharArrayTestProperty, value); }
    }

    public static readonly PropertyInfo<Guid> GuidTestProperty = RegisterProperty<Guid>(c => c.GuidTest);
    /// <Summary>
    /// Gets or sets the GuidTest value.
    /// </Summary>
    public Guid GuidTest
    {
      get { return GetProperty(GuidTestProperty); }
      set { SetProperty(GuidTestProperty, value); }
    }


    public static readonly PropertyInfo<int?> NullableIntProperty = RegisterProperty<int?>(c => c.NullableInt);
    /// <Summary>
    /// Gets or sets the NullableInt value.
    /// </Summary>
    public int? NullableInt
    {
      get { return GetProperty(NullableIntProperty); }
      set { SetProperty(NullableIntProperty, value); }
    }

    public static readonly PropertyInfo<int?> NullableButSetIntProperty = RegisterProperty<int?>(c => c.NullableButSetInt);
    /// <Summary>
    /// Gets or sets the NullableButSetInt value.
    /// </Summary>
    public int? NullableButSetInt
    {
      get { return GetProperty(NullableButSetIntProperty); }
      set { SetProperty(NullableButSetIntProperty, value); }
    }

    public static readonly PropertyInfo<Csla.SmartDate> EmptySmartDateTestProperty = RegisterProperty<Csla.SmartDate>(c => c.EmptySmartDateTest);
    /// <Summary>
    /// Gets or sets the EmptySmartDateTest value.
    /// </Summary>
    public Csla.SmartDate EmptySmartDateTest
    {
      get { return GetProperty(EmptySmartDateTestProperty); }
      set { SetProperty(EmptySmartDateTestProperty, value); }
    }

    public static readonly PropertyInfo<Csla.SmartDate> FilledSmartDateTestProperty = RegisterProperty<Csla.SmartDate>(c => c.FilledSmartDateTest);
    /// <Summary>
    /// Gets or sets the EmptySmartDateTest value.
    /// </Summary>
    public Csla.SmartDate FilledSmartDateTest
    {
      get { return GetProperty(FilledSmartDateTestProperty); }
      set { SetProperty(FilledSmartDateTestProperty, value); }
    }

    internal static BinaryReaderWriterTestClass NewBinaryReaderWriterTestClass(IDataPortal<BinaryReaderWriterTestClass> dataPortal)
    {
      return NewBinaryReaderWriterTestClass(dataPortal, false);
    }

    internal static BinaryReaderWriterTestClass NewBinaryReaderWriterTestClass(IDataPortal<BinaryReaderWriterTestClass> dataPortal, bool isChild)
    {
      return dataPortal.Create(isChild);
    }

    [Create]
    private void Create(bool isChild)
    {
      if (isChild)
        MarkAsChild();
    }
  }
}
