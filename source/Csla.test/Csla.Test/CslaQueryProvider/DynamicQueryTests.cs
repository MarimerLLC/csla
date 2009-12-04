using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.DynamicQuery
{
  [Serializable]
  public class RandomThing : BusinessBase<RandomThing>, IConvertible
  {
    [Indexable(IndexModeEnum.IndexModeAlways)]
    public int SomeVal { get; set; }
    public int SomeOtherVal { get; set; }
    public int YetAnotherVal { get; set; }
    public Guid Id { get; set; }

    public Nullable<int> ANullableVal
    {
      get { return new Nullable<int>(SomeVal); }
    }

    public DateTime SomeDateTime
    {
      get { return new DateTime((long)SomeVal); }
    }

    public RandomThing(int x)
    {
      MarkAsChild();
      SomeVal = x;
      Id = Guid.NewGuid();
    }

    #region IConvertible Members

    TypeCode IConvertible.GetTypeCode()
    {
      throw new NotImplementedException();
    }

    bool IConvertible.ToBoolean(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    byte IConvertible.ToByte(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    char IConvertible.ToChar(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    DateTime IConvertible.ToDateTime(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    decimal IConvertible.ToDecimal(IFormatProvider provider)
    {
      return Convert.ToDecimal(SomeVal);
    }

    double IConvertible.ToDouble(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    short IConvertible.ToInt16(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    int IConvertible.ToInt32(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    long IConvertible.ToInt64(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    sbyte IConvertible.ToSByte(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    float IConvertible.ToSingle(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    string IConvertible.ToString(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    object IConvertible.ToType(Type conversionType, IFormatProvider provider)
    {
      if (conversionType == typeof(RandomThing))
        return this;
      else
        throw new NotImplementedException();
    }

    ushort IConvertible.ToUInt16(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    uint IConvertible.ToUInt32(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    ulong IConvertible.ToUInt64(IFormatProvider provider)
    {
      throw new NotImplementedException();
    }

    #endregion
  }

  [Serializable]
  class CollectionExtendingIQueryable<T> : BusinessListBase<CollectionExtendingIQueryable<T>, T> where T : Csla.Core.IEditableBusinessObject { }

  class CustomRandomThingComparer : IEqualityComparer<RandomThing>
  {

    #region IEqualityComparer<RandomThing> Members

    bool IEqualityComparer<RandomThing>.Equals(RandomThing x, RandomThing y)
    {
      return x.SomeVal == y.SomeVal;
    }

    int IEqualityComparer<RandomThing>.GetHashCode(RandomThing obj)
    {
      return obj.SomeVal.GetHashCode();
    }

    #endregion
  }
  
  [TestClass]
  public class DynamicQueryTests
  {
    [TestMethod]
    public void dynamic_where_works()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      for (int i = 0; i < 42; i++)
        random.Add(new RandomThing(i));
      var oneItem = random.Where("SomeVal == 0");
      Assert.IsTrue(oneItem.Count() == 1);
      Assert.IsTrue(oneItem.First().SomeVal == 0);
    }

    [TestMethod]
    public void dynamic_orderby_works()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      for (int i = 0; i < 42; i++)
        random.Add(new RandomThing(i));
      var oneItem = random.Where("1 == 1").OrderBy("SomeVal Descending");
      Assert.IsTrue(oneItem.Count() == 42);
      Assert.IsTrue(oneItem.First().SomeVal == 41);
    }

    [TestMethod]
    public void dynamic_thenby_works()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(5) { SomeOtherVal = 101, YetAnotherVal = 1000 });
      random.Add(new RandomThing(5) { SomeOtherVal = 100, YetAnotherVal = 1001 });
      random.Add(new RandomThing(5) { SomeOtherVal = 100, YetAnotherVal = 1000 });
      random.Add(new RandomThing(3) { SomeOtherVal = 101, YetAnotherVal = 1000 });
      random.Add(new RandomThing(3) { SomeOtherVal = 100, YetAnotherVal = 1001 });
      random.Add(new RandomThing(3) { SomeOtherVal = 100, YetAnotherVal = 1000 });
      random.Add(new RandomThing(1) { SomeOtherVal = 101, YetAnotherVal = 1000 });
      random.Add(new RandomThing(1) { SomeOtherVal = 100, YetAnotherVal = 1001 });
      random.Add(new RandomThing(1) { SomeOtherVal = 100, YetAnotherVal = 1000 });

      var oneItem = random.Where("1 == 1").OrderBy("SomeOtherVal Descending, YetAnotherVal Ascending");
      Assert.IsTrue(oneItem.First().SomeOtherVal == 101);
      Assert.IsTrue(oneItem.First().YetAnotherVal == 1000);
    }
  }
}
