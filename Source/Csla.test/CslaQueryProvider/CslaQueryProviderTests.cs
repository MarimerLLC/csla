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

namespace Csla.Test.CslaQueryProvider
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
      get { return SomeVal; }
    }

    [Indexable(IndexModeEnum.IndexModeAlways)]
    public Nullable<Guid> ANullableGuid
    {
      get { return Id; }
    }

    public DateTime SomeDateTime
    {
      get { return new DateTime(SomeVal); }
    }

    public DateTime? SomeDateTimeNullable
    {
      get { return SomeDateTime; }
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
  class CollectionExtendingIQueryable<T> : BusinessBindingListBase<CollectionExtendingIQueryable<T>, T> where T : Csla.Core.IEditableBusinessObject { }

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

  class InnerCollectionItem : BusinessBase<InnerCollectionItem>
  {
    public string TestProp { get; set; }
  }

  class InnerCollection : BusinessBindingListBase<InnerCollection, InnerCollectionItem>
  {
  }

  class OuterCollectionItem : BusinessBase<OuterCollectionItem>
  {
    public InnerCollection TestInnerCollection { get; set; }
  }

  class OuterCollection : BusinessBindingListBase<OuterCollection, OuterCollectionItem>
  {
  }
  
  [TestClass]
  public class CslaQueryProviderTests
  {
    private class TestEqualityComparer : IEqualityComparer<RandomThing>
    {
      public bool Equals(RandomThing x, RandomThing y)
      {
        return x.SomeVal == y.SomeVal;
      }

      public int GetHashCode(RandomThing obj)
      {
        return 0;
      }
    }
    
    [TestMethod]
    public void group_by_query_case_returns_correctly_with_custom_equality_comparison()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      var rnd = new Random();
      for (var i = 0; i < 42; i++)
        random.Add(new RandomThing(rnd.Next(300)) { SomeOtherVal = 0 });
      var groupByQuery = random.GroupBy(x => x, new TestEqualityComparer());
      var iteratedResult = groupByQuery.ToArray();
      Assert.IsNotNull(iteratedResult);
    }
    
    [TestMethod]
    public void query_returning_nothing_gives_back_empty_set_not_null()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      var rnd = new Random();
      for (var i = 0; i < 42; i++)
        random.Add(new RandomThing(rnd.Next(300)) { SomeOtherVal = 0 });
      var emptySetOne = from r in random where r.SomeOtherVal == 10000 select r;
      var emptySetTwo = from r in random where r.SomeVal == 10000 select r;
      Assert.AreEqual(0, emptySetOne.Count());
      Assert.AreEqual(0, emptySetTwo.Count());
      Assert.AreEqual(null, emptySetOne.FirstOrDefault());
      Assert.AreEqual(null, emptySetTwo.FirstOrDefault());
    }

    [TestMethod]
    public void nested_collections_return_valid_results()
    {
      var item = new InnerCollectionItem() { TestProp = "42" };
      var innerCollection = new InnerCollection();
      innerCollection.Add(item);
      var outerItem = new OuterCollectionItem() { TestInnerCollection = innerCollection };
      var outerCollection = new OuterCollection();
      outerCollection.Add(outerItem);

      var get42back =  from outer in outerCollection
                       from inner in outerItem.TestInnerCollection
                       select inner;
      Assert.IsTrue(get42back.First().TestProp == "42");
      
    }
    
    [TestMethod]
    public void TestQueryProviderCount()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      for (int i = 0; i < 42; i++)
        random.Add(new RandomThing(rnd.Next(300)));
      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;
      int manualCountUnder100 = 0;
      int filteredCount;


      //do filtered count first
      filteredCount = filteredResult.Count<RandomThing>();
      foreach (RandomThing x in random)
        if (x.SomeVal < 100)
          manualCountUnder100++;
      Assert.AreEqual(manualCountUnder100, filteredCount);
    }

    [TestMethod]
    public void TestQueryProviderCountWithCriteria()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      for (int i = 0; i < 42; i++)
        random.Add(new RandomThing(rnd.Next(300)));
      var filteredResult = from r in random
                           where r.SomeVal < 200
                           select r;
      int manualCountUnder100 = 0;
      int filteredCount;


      //do filtered count first
      filteredCount = filteredResult.Count<RandomThing>((RandomThing i) => i.SomeVal < 100);
      foreach (RandomThing x in random)
        if (x.SomeVal < 100)
          manualCountUnder100++;
      Assert.AreEqual(manualCountUnder100, filteredCount);
    }

    [TestMethod]
    public void TestQueryProviderAverageWithCriteria()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(5));
      random.Add(new RandomThing(7));
      random.Add(new RandomThing(69));
      //get a filtered result that includes everything
      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;
      double average = filteredResult.Average( i => i.SomeVal * 2);

      Assert.AreEqual(42.0, average);
    }

    [TestMethod]
    public void TestQueryProviderAny()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(5));
      random.Add(new RandomThing(7));
      //get a filtered result that includes everything
      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;

      bool has = filteredResult.Any<RandomThing>();
      bool hasNot = filteredResult.Any<RandomThing>(i => i.SomeVal >= 100);

      Assert.IsTrue(has);
      Assert.IsFalse(hasNot);
    }

    [TestMethod]
    public void TestMaxWithDateParameter()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(0));
      random.Add(new RandomThing(1));
      //somedatetime is the int represented as date time ticks
      var z = random.Max(x => x.SomeDateTime);


      Assert.IsTrue(z.Ticks == 1);
    }

    [TestMethod]
    public void TestQueryProviderAnyOnEmpty()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;

      bool hasNot = filteredResult.Any<RandomThing>();

      Assert.IsFalse(hasNot);
    }

    [TestMethod]
    public void TestQueryProviderCast()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(5));
      random.Add(new RandomThing(7));
      //get a filtered result that includes everything
      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;

      var casted = filteredResult.Cast<RandomThing>();
      foreach (var item in casted)
        Assert.IsTrue(item is RandomThing);
    }


    [TestMethod]
    public void TestQueryProviderConcat()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      random.Add(new RandomThing(42)); //first one has to be under 100 to run our removal tests correctly
      for (int i = 0; i < 99; i++)
        random.Add(new RandomThing(rnd.Next(300)));


      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;

      var differentFilteredResult = from r in random
                                    where r.SomeVal < 200
                                    select r;

      int filteredCount = ((LinqBindingList<RandomThing>)filteredResult).Count<RandomThing>();
      int differentFilteredCount = ((LinqBindingList<RandomThing>)differentFilteredResult).Count<RandomThing>();

      var concatResult = filteredResult.Concat<RandomThing>(differentFilteredResult);
      int concatResultCount = concatResult.Count<RandomThing>();

      Assert.IsTrue(concatResultCount == filteredCount + differentFilteredCount);

    }

    [TestMethod]
    public void TestQueryProviderSelectProjection()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      for (int i = 0; i < 99; i++)
        random.Add(new RandomThing(rnd.Next(300)));

      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;

      foreach (var result in filteredResult)
        Assert.IsTrue(result is RandomThing);

      var subFilteredResult = from r in filteredResult
                              where r.SomeVal < 50
                              select r.SomeVal;
      foreach (var result in subFilteredResult)
        Assert.IsTrue(result < 50);
    }

    [TestMethod]
    public void TestQueryProviderSelectProjectionWithSerialization()
    {
      CollectionExtendingIQueryable<RandomThing> randomBase = new CollectionExtendingIQueryable<RandomThing>();
      var random = randomBase.Clone();

      Random rnd = new Random();
      for (int i = 0; i < 99; i++)
        random.Add(new RandomThing(rnd.Next(300)));

      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;

      foreach (var result in filteredResult)
        Assert.IsTrue(result is RandomThing);

      var subFilteredResult = from r in filteredResult
                              where r.SomeVal < 50
                              select r.SomeVal;
      foreach (var result in subFilteredResult)
        Assert.IsTrue(result < 50);
    }

    [TestMethod]
    public void TestQueryProviderContains()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      for (int i = 0; i < 99; i++)
        random.Add(new RandomThing(rnd.Next(300)));
      RandomThing threeNines = new RandomThing(999);
      random.Add(threeNines);

      var doesntHaveIt = from r in random
                           where r.SomeVal < 100
                           select r;
      var hasIt = from r in random
                           where r.SomeVal > 100
                           select r;
      Assert.IsFalse(doesntHaveIt.Contains<RandomThing>(threeNines));
      Assert.IsTrue(hasIt.Contains<RandomThing>(threeNines));
    }

    [TestMethod]
    public void TestQueryProviderDefaultIfEmpty()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      for (int i = 0; i < 99; i++)
        random.Add(new RandomThing(rnd.Next(300)));
      var defaultTest = default(RandomThing);

      var notThere = from r in random where r.SomeVal > 1000 select r;
      var resultDefault = notThere.DefaultIfEmpty<RandomThing>();

      Assert.IsTrue(defaultTest == resultDefault.ToArray<RandomThing>()[0]);
    }

    [TestMethod]
    public void TestQueryProviderDistinct()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));

      var source = from r in random where r.SomeVal < 1000 select r;
      var distinctTest = source.Distinct<RandomThing>(new CustomRandomThingComparer());

      Assert.IsTrue(distinctTest.Count<RandomThing>() == 3);
    }

    [TestMethod]
    public void TestQueryProviderElementAt()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(7));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(69));
      random.Add(new RandomThing(3));

      var source = from r in random where r.SomeVal < 1000 && r.SomeVal > 0 select r;
      var elementAtTest = source.ElementAt<RandomThing>(3);

      Assert.IsTrue(elementAtTest.SomeVal == 69);
    }

    [TestMethod]
    public void TestQueryProviderElementAtOrDefault()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(7));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(69));
      random.Add(new RandomThing(3));

      var source = from r in random where r.SomeVal < 1000 && r.SomeVal > 0 select r;
      var elementAtTest = source.ElementAtOrDefault<RandomThing>(3);
      var defaultValue = default(RandomThing);
      var defaultTest = source.ElementAtOrDefault<RandomThing>(403);

      Assert.IsTrue(elementAtTest.SomeVal == 69);
      Assert.IsTrue(defaultValue == defaultTest);
    }

    [TestMethod]
    public void TestQueryProviderExcept()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal < 10 select r;
      var b = from r in random where r.SomeVal > 1 select r;
      var diff = a.Except(b, new CustomRandomThingComparer());
      Assert.IsTrue(diff.ToArray()[0].SomeVal == 1);
      Assert.IsTrue(diff.Count() == 1);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestQueryProviderFirst()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal >= 0 select r;

      var fCrit = a.First(r => r.SomeVal >= 5);
      var fSing = a.First();

      Assert.IsTrue(fCrit.SomeVal == 5);
      Assert.IsTrue(fSing.SomeVal == 1);

      var fNull = a.First(r => r.SomeVal >= 1000);
    }

    [TestMethod]
    public void TestQueryProviderFirstOrDefault()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal >= 0 select r;

      var fCrit = a.FirstOrDefault(r => r.SomeVal > 42);
      var fRes = a.FirstOrDefault(r => r.SomeVal == 1);
      var def = default(RandomThing);
      Assert.IsTrue(fCrit == def);
      Assert.IsTrue(fRes.SomeVal == 1);
    }

    [TestMethod]
    public void TestQueryProviderGroupBy()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(17));

      var sub = from r in random where r.SomeVal < 10 select r;

      var a =
        from r in sub
        where r.SomeVal >= 0
        group r by r.SomeVal into g
        select new { SomeVal = g.Key, Elements = g };

      Assert.IsTrue(a.Count() == 3);
      Assert.IsTrue(a.ToArray()[0].Elements.Count() == 1);
      Assert.IsTrue(a.ToArray()[1].Elements.Count() == 2);
      Assert.IsTrue(a.ToArray()[2].Elements.Count() == 3);
    }

    [TestMethod]
    public void TestQueryProviderGroupJoin()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(17));

      var otherStuff = new Dictionary<int, string>();
      otherStuff.Add(1, "Harry");
      otherStuff.Add(2, "Sally");
      otherStuff.Add(69, "Bob Marley");

      var a =
        from o in otherStuff
        join r in random on o.Key equals r.SomeVal into ro
        from subsub in ro.DefaultIfEmpty()
        where subsub == default(RandomThing)
        select o;

      Assert.IsTrue(a.ToArray()[0].Value == "Bob Marley");

      var b =
        from r in random
        join o in otherStuff on r.SomeVal equals o.Key into ro
        from subsub in ro.DefaultIfEmpty()
        where subsub.Key == 1
        select r;

      Assert.IsTrue(b.ToArray()[0].SomeVal == 1);
    }

    [TestMethod]
    public void TestQueryProviderIntersect()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(17));

      var sub = from r in random where r.SomeVal < 10 select r;

      var a =
        from r in sub
        where r.SomeVal == 4
        select r;

      var intSect = random.Intersect(a);

      Assert.IsTrue(intSect.Count() == 3);
      Assert.IsTrue(a.ToArray()[0].SomeVal == 4);
      Assert.IsTrue(a.ToArray()[1].SomeVal == 4);
      Assert.IsTrue(a.ToArray()[2].SomeVal == 4);
    }

    [TestMethod]
    public void TestQueryProviderJoin()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(17));

      var otherStuff = new Dictionary<int, string>();
      otherStuff.Add(1, "Harry");
      otherStuff.Add(2, "Sally");
      otherStuff.Add(69, "Bob Marley");

      var a =
        from r in random
        join o in otherStuff on r.SomeVal equals o.Key
        select new { o.Value, r.Id };

      Assert.IsTrue(a.Count() == 3);
      Assert.IsTrue(a.ToArray()[0].Value == "Harry");
      Assert.IsTrue(a.ToArray()[1].Value == "Sally");
      Assert.IsTrue(a.ToArray()[2].Value == "Sally");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestQueryProviderLast()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal >= 0 select r;

      var lCrit = a.Last(r => r.SomeVal <= 1);
      var lSing = a.Last();

      Assert.IsTrue(lCrit.SomeVal == 1);
      Assert.IsTrue(lSing.SomeVal == 5);
      var lNull = a.Last(r => r.SomeVal >= 1000);

    }

    [TestMethod]
    public void TestQueryProviderLastOrDefault()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal >= 0 select r;

      var lCrit = a.LastOrDefault(r => r.SomeVal > 42);
      var lRes = a.LastOrDefault(r => r.SomeVal == 1);
      var def = default(RandomThing);
      Assert.IsTrue(lCrit == def);
      Assert.IsTrue(lRes.SomeVal == 1);
    }

    [TestMethod]
    public void TestQueryProviderLongCount()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
    
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal >= 0 select r;
      Assert.IsTrue(a.LongCount(r => r.SomeVal < 5) == 4);
    }

    [TestMethod]
    public void TestQueryProviderMax()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal >= 0 select r;

      Assert.IsTrue(a.Max(r => r.SomeVal) == 5);
    }

    [TestMethod]
    public void TestQueryProviderMaxDateTimeNullable()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal >= 0 select r;
      DateTime? maxDateTime = random.Max(r => r.SomeDateTimeNullable);

      Assert.IsTrue(a.Max(r => r.SomeDateTimeNullable).Value  == new DateTime(5));
    }

    [TestMethod]
    public void TestQueryProviderMin()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var a = from r in random where r.SomeVal >= 0 select r;
      Assert.IsTrue(a.Min(r => r.SomeVal) == 1);
    }

    [TestMethod]
    public void TestQueryProviderOfType()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var emptyset = random.OfType<DateTime>();
      var fullset = random.OfType<RandomThing>();

      Assert.IsTrue(emptyset.Count() == 0);
      Assert.IsTrue(fullset.Count() == random.Count());
    }

    [TestMethod]
    public void TestQueryProviderCastFrom()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var result = from RandomThing r in random
                   where r.SomeVal == 2
                   select r;
      Assert.IsTrue(result.Count() == 1);
    }

    [TestMethod]
    public void LinqBindingList_is_generated_by_orderby()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(5));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(1));

      var result = random.OrderBy(x => x.SomeVal);
      Assert.AreEqual(result.GetType(),typeof(LinqBindingList<RandomThing>));
    }

    [TestMethod]
    public void thenby_generates_correct_result()
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

      var result =
        (LinqBindingList<RandomThing>)random
          .OrderBy(x => x.SomeVal)
          .ThenBy(x => x.SomeOtherVal)
          .ThenByDescending(x => x.YetAnotherVal);
      var firstResult = result.First();
      Assert.IsTrue(
        firstResult.YetAnotherVal == 1001
        && firstResult.SomeOtherVal == 100
        && firstResult.SomeVal == 1
        );
      Assert.AreEqual(result.GetType(), typeof(LinqBindingList<RandomThing>));
    }

    [TestMethod]
    public void LinqBindingList_is_generated_by_thenby()
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

      var result =
        (LinqBindingList<RandomThing>)random
          .OrderBy(x => x.SomeVal)
          .ThenBy(x => x.SomeOtherVal)
          .ThenBy(x => x.YetAnotherVal);

      Assert.AreEqual(result.GetType(), typeof(LinqBindingList<RandomThing>));
    }

    [TestMethod]
    public void LinqBindingList_is_generated_by_thenby_with_where()
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

      var r1 = (LinqBindingList<RandomThing>)random
          .Where(x => true);

      var r2 = r1.OrderBy(x => x.SomeVal);
      var r3 = r2.ThenBy(y => y.SomeOtherVal);
      var r4 = r3.ThenBy(y => y.YetAnotherVal);
      
      
      var result =
        (LinqBindingList<RandomThing>)random
          .Where(x => true)
          .OrderBy(x => x.SomeVal)
          .ThenBy(y => y.SomeOtherVal)
          .ThenBy(y => y.YetAnotherVal);

      Assert.AreEqual(result.GetType(), typeof(LinqBindingList<RandomThing>));
    }

    [TestMethod]
    public void LinqBindingList_generated_by_orderby_removal_by_item_reflects_to_source()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      var random2 = new RandomThing(2);
      random.Add(new RandomThing(5));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(3));
      random.Add(random2);
      random.Add(new RandomThing(1));

      var result = (LinqBindingList<RandomThing>) random.OrderBy(x => x.SomeVal);
      result.Remove(random2);
      Assert.IsTrue((from r in random where r.SomeVal == 2 select r).Count() == 0);
    }

    [TestMethod]
    public void LinqBindingList_generated_by_orderby_added_items_still_in_proper_order()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(5));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(1));

      var result = (LinqBindingList<RandomThing>)random.OrderBy(x => x.SomeVal);
      //because result is sorted by number, adding this item should result in it being the
      //first item in result, and the last in random
      random.Add(new RandomThing(0));

      Assert.IsTrue(result.First().SomeVal == 0);
      Assert.IsTrue(random.Last().SomeVal == 0);
    }

    [TestMethod]
    public void LinqBindingList_generated_by_orderby_and_subsequebt_thenbys_with_added_items_still_in_proper_order()
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

      var result = 
        (LinqBindingList<RandomThing>)random
          .OrderBy(x => x.SomeVal)
          .ThenBy(x => x.SomeOtherVal)
          .ThenBy(x => x.YetAnotherVal);
      //because result is sorted by number, adding this item should result in it being the
      //first item in result, and the last in random
      random.Add(new RandomThing(0));

      Assert.IsTrue(result.First().SomeVal == 0);
      Assert.IsTrue(random.Last().SomeVal == 0);
    }

    [TestMethod]
    public void TestQueryProviderOrderBy()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(5));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(1));

      var inOrder = from r in random
                         orderby r.SomeVal
                         select r;

      Assert.IsTrue(inOrder.First().SomeVal == 1);
    }

    [TestMethod]
    public void TestQueryProviderOrderByDescending()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var reverseOrder = from r in random
                         orderby r.SomeVal descending
                         select r;

      Assert.IsTrue(reverseOrder.First().SomeVal == 5);
    }


    [TestMethod]
    public void TestQueryProviderReverse()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();

      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(3));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(5));

      var reverse = random.Reverse();
      Assert.IsTrue(reverse.First().SomeVal == 5);
    }

    [TestMethod]
    public void TestQueryProviderSelectMany()
    {
      CollectionExtendingIQueryable<RandomThing> random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(2));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(4));
      random.Add(new RandomThing(17));


      var a = from r in random from o in r.Id.ToByteArray() select o;

    }

    [TestMethod]
    public void TestQueryProviderSequenceEqual()
    {
      var randomA = new CollectionExtendingIQueryable<RandomThing>();
      var randomB = new CollectionExtendingIQueryable<RandomThing>();
      randomA.Add(new RandomThing(1));
      randomA.Add(new RandomThing(2));
      randomB.Add(new RandomThing(1));
      randomB.Add(new RandomThing(2));
      var shouldBeTrue = randomA.SequenceEqual(randomB, new CustomRandomThingComparer());
      randomB.Add(new RandomThing(3));
      var shouldBeFalse = randomA.SequenceEqual(randomB, new CustomRandomThingComparer());
      Assert.IsTrue(shouldBeTrue);
      Assert.IsFalse(shouldBeFalse);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestQueryProviderSingle()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      random.Add(new RandomThing(1));
      var shouldBeTrue1 = random.Single() is RandomThing;
      random.Add(new RandomThing(2));
      var shouldBeTrue2 = random.Single(x => x.SomeVal == 1) is RandomThing;
      bool madeException = false;
      Assert.IsTrue(shouldBeTrue1);
      Assert.IsTrue(shouldBeTrue2);
      var shouldThrowException = random.Single();
    }

    [TestMethod]
    public void TestQueryProviderSingleOrDefault()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      var defaultRandom = default(RandomThing);
      var shouldBeTrue1 = defaultRandom == random.SingleOrDefault();
      random.Add(new RandomThing(2));
      var shouldBeTrue2 = defaultRandom == random.SingleOrDefault(x => x.SomeVal == 42);
      var shouldBeTrue3 = (new RandomThing(2)).SomeVal == random.SingleOrDefault(x => x.SomeVal == 2).SomeVal;
      Assert.IsTrue(shouldBeTrue1);
      Assert.IsTrue(shouldBeTrue2);
      Assert.IsTrue(shouldBeTrue3);
    }

    [TestMethod]
    public void TestQueryProviderSkip()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      for (int i = 0; i < 10; i++)
        random.Add(new RandomThing(i));
      var subset = random.Skip(5);
      var shouldBeSixth = subset.First();
      Assert.IsTrue(shouldBeSixth.SomeVal == 5);
    }

    [TestMethod]
    public void TestQueryProviderSkipWhile()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      for (int i = 30; i >= 0; i--)
        random.Add(new RandomThing(i));
      var subset = random.SkipWhile(x => x.SomeVal > 20);
      var shouldBeTwenty = subset.First();
      Assert.IsTrue(shouldBeTwenty.SomeVal == 20);
    }

    [TestMethod]
    public void TestQueryProviderSum()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      for (int i = 1; i <= 4; i++)
        random.Add(new RandomThing(i));
      var sumAll = random.Sum(x => x.SomeVal);
      Assert.IsTrue(sumAll == 10);
    }

    [TestMethod]
    public void TestQueryProviderTake()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      for (int i = 1; i <= 10; i++)
        random.Add(new RandomThing(i));
      var firstThree = random.Take(3);
      var arrayOfThree = firstThree.ToArray();
      Assert.IsTrue(arrayOfThree[0].SomeVal == 1);
      Assert.IsTrue(arrayOfThree[1].SomeVal == 2);
      Assert.IsTrue(arrayOfThree[2].SomeVal == 3);
    }

    [TestMethod]
    public void TestQueryProviderTakeWhile()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      for (int i = 1; i <= 10; i++)
        random.Add(new RandomThing(i));
      var firstThree = random.TakeWhile(x => x.SomeVal <= 3);
      var arrayOfThree = firstThree.ToArray();
      Assert.IsTrue(arrayOfThree[0].SomeVal == 1);
      Assert.IsTrue(arrayOfThree[1].SomeVal == 2);
      Assert.IsTrue(arrayOfThree[2].SomeVal == 3);
    }

    [TestMethod]
    public void TestQueryProviderUnion()
    {
      var randomA = new CollectionExtendingIQueryable<RandomThing>();
      var randomB = new CollectionExtendingIQueryable<RandomThing>();
      for (int i = 1; i <= 10; i++)
        randomA.Add(new RandomThing(i));
      for (int i = 9; i <= 20; i++)
        randomB.Add(new RandomThing(i));
      Assert.IsTrue(randomB.Union(randomA, new CustomRandomThingComparer()).Count() == 20);
    }

    [TestMethod]
    public void TestViewSyncronization()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      random.Add(new RandomThing(42)); //first one has to be under 100 to run our removal tests correctly
      for (int i = 0; i < 99; i++)
        random.Add(new RandomThing(rnd.Next(300)));


      var filteredResult = from r in random
                           where r.SomeVal < 100
                           select r;

      var differentFilteredResult = from r in random
                                    where r.SomeVal < 200
                                    select r;

      int filteredCountPreRemoval = filteredResult.Count();
      int differentFilteredCountPreRemoval = differentFilteredResult.Count();
      int originalCountPreRemoval = random.Count;

      //need to cast it to a LinqBindingList from IQueryable in order to call special CSLA methods for removal
      ((LinqBindingList<RandomThing>)filteredResult).RemoveAt(0);
      
      int filteredCountPostRemoval = filteredResult.Count();
      int differentFilteredCountPostRemoval = differentFilteredResult.Count();
      int originalCountPostRemoval = random.Count;

      //removal from the filtered result should decrement all of the counts by one, since they share the same base list
      Assert.AreEqual(filteredCountPreRemoval - 1, filteredCountPostRemoval);
      Assert.AreEqual(differentFilteredCountPreRemoval - 1, differentFilteredCountPostRemoval);
      Assert.AreEqual(originalCountPreRemoval - 1, originalCountPostRemoval);
    }

    [TestMethod]
    public void TestLastWithNullableGuid()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      for (int i = 0; i < 99; i++)
        random.Add(new RandomThing(rnd.Next(300)));
      var lastItem = random.Last().ANullableGuid;
    }

    [TestMethod]
    public void TestLinqBindingListDisposal()
    {
      var random = new CollectionExtendingIQueryable<RandomThing>();
      Random rnd = new Random();
      random.Add(new RandomThing(42)); //first one has to be under 100 to run our removal tests correctly
      for (int i = 0; i < 99; i++)
        random.Add(new RandomThing(rnd.Next(300)));

      for (int i = 0; i < 10000; i++)
      {
        using (var subset = (LinqBindingList<RandomThing>) from item in random where item.SomeVal < 1000 select item)
        {
          //at this point, we are merely testing that we are still "alive"
          Assert.IsTrue(true);
        }
      }
    }

  }


}
