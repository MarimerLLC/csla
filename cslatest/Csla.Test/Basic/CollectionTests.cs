using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.Basic
{
  [TestClass]
  public class CollectionTests
  {
    [TestMethod]
    public void SetLast()
    {
      TestCollection list = new TestCollection();
      list.Add(new TestItem());
      list.Add(new TestItem());
      TestItem oldItem = new TestItem();
      list.Add(oldItem);
      TestItem newItem = new TestItem();
      list[2] = newItem;
      Assert.AreEqual(3, list.Count, "List should have 3 items");
      Assert.AreEqual(newItem, list[2], "Last item should be newItem");
      Assert.AreEqual(true, list.ContainsDeleted(oldItem), "Deleted list should have old item");
    }

    [TestMethod]
    public void RootListGetRuleDescriptions()
    {
      RootList list = new RootList();
      RootListChild child = list.AddNew();
      string[] rules = child.GetRuleDescriptions();
    }

    [TestMethod]
    public void IndexOnReadOnlyWorks()
    {
      var sampleSize = 100000;
      Console.WriteLine("Creating " + sampleSize + " element collection...");
      var readOnlyCollection = new TestReadOnlyCollection(sampleSize);
      Console.WriteLine("Collection established.");
      
      //first query establishes the index
      var controlSet = readOnlyCollection.ToList();

      var primeQuery = from i in readOnlyCollection where i.IndexedString == "42" select i;
      var forcedPrimeITeration = primeQuery.ToArray();

      Stopwatch watch = new Stopwatch();

      watch.Start();
      var indexedQuery = from i in readOnlyCollection where i.IndexedString == "42" select i;
      var forcedIterationIndexed = indexedQuery.ToArray();
      watch.Stop();

      var indexedRead = watch.ElapsedMilliseconds;

      watch.Reset();
      
      watch.Start();
      var nonIndexedQuery = from i in readOnlyCollection where i.NonIndexedString == "42" select i;
      var forcedIterationNonIndexed = nonIndexedQuery.ToArray();
      watch.Stop();
      
      var nonIndexedRead = watch.ElapsedMilliseconds;

      watch.Reset();

      watch.Start();
      var controlQuery = from i in controlSet where i.IndexedString == "42" select i;
      var forcedControlIteration = controlQuery.ToArray();
      watch.Stop();

      var controlRead = watch.ElapsedMilliseconds;

      
      Console.WriteLine("Sample size = " + sampleSize);
      Console.WriteLine("Indexed Read = " + indexedRead + "ms");
      Console.WriteLine("Non-Indexed Read = " + nonIndexedRead + "ms");
      Console.WriteLine("Standard Linq-to-objects Read = " + controlRead + "ms");
      Assert.IsTrue(indexedRead < nonIndexedRead);
      Assert.IsTrue(forcedIterationIndexed.Count() == forcedIterationNonIndexed.Count());
    }

    [TestMethod]
    public void IndexOnBusinessListBaseWorks()
    {
      var sampleSize = 100000;
      Console.WriteLine("Creating " + sampleSize + " element collection...");
      var blbCollection = new TestBusinessListBaseCollection(sampleSize);
      Console.WriteLine("Collection established.");

      //first query establishes the index
      var controlSet = blbCollection.ToList();

      var primeQuery = from i in blbCollection where i.IndexedString == "42" select i;
      var forcedPrimeITeration = primeQuery.ToArray();

      Stopwatch watch = new Stopwatch();

      watch.Start();
      var indexedQuery = from i in blbCollection where i.IndexedString == "42" select i;
      var forcedIterationIndexed = indexedQuery.ToArray();
      watch.Stop();

      var indexedRead = watch.ElapsedMilliseconds;

      watch.Reset();

      watch.Start();
      var nonIndexedQuery = from i in blbCollection where i.NonIndexedString == "42" select i;
      var forcedIterationNonIndexed = nonIndexedQuery.ToArray();
      watch.Stop();

      var nonIndexedRead = watch.ElapsedMilliseconds;

      watch.Reset();

      watch.Start();
      var controlQuery = from i in controlSet where i.IndexedString == "42" select i;
      var forcedControlIteration = controlQuery.ToArray();
      watch.Stop();

      var controlRead = watch.ElapsedMilliseconds;


      Console.WriteLine("Sample size = " + sampleSize);
      Console.WriteLine("Indexed Read = " + indexedRead + "ms");
      Console.WriteLine("Non-Indexed Read = " + nonIndexedRead + "ms");
      Console.WriteLine("Standard Linq-to-objects Read = " + controlRead + "ms");
      Assert.IsTrue(indexedRead < nonIndexedRead);
      Assert.IsTrue(forcedIterationIndexed.Count() == forcedIterationNonIndexed.Count());
    }

    [TestMethod]
    public void QueryOnIndexedFieldThatCantUseIndexWorks()
    {
      var sampleSize = 1000;
      Console.WriteLine("Creating " + sampleSize + " element collection...");
      var blbCollection = new TestBusinessListBaseCollection(sampleSize);
      Console.WriteLine("Collection established.");
      var someQuery = from i in blbCollection where i.IndexedInt <= 1000 select i;
      //it should bring back everything 
      Assert.IsTrue(someQuery.Count() == 1000);
    }
 
  }

  [Serializable]
  public class TestReadOnlyCollection : ReadOnlyListBase<TestReadOnlyCollection, TestIndexableItem>
  {
    public TestReadOnlyCollection(int sampleSize)
    {
      //allow adds
      IsReadOnly = false;
      Random rnd = new Random();
      for(int i = 0; i < sampleSize; i++)
      {
        int nextRnd = rnd.Next(sampleSize / 100);
        Add(
          new TestIndexableItem 
          { 
            IndexedString = nextRnd.ToString(), 
            IndexedInt = nextRnd, 
            NonIndexedString = nextRnd.ToString() 
          }
          );
      }
      IsReadOnly = true;
    }
  }

  [Serializable]
  public class TestBusinessListBaseCollection : BusinessListBase<TestBusinessListBaseCollection, TestIndexableItem>
  {
    public TestBusinessListBaseCollection(int sampleSize)
    {
      Random rnd = new Random();
      for (int i = 0; i < sampleSize; i++)
      {
        int nextRnd = rnd.Next(sampleSize / 100);
        Add(
          new TestIndexableItem
          {
            IndexedString = nextRnd.ToString(),
            IndexedInt = nextRnd,
            NonIndexedString = nextRnd.ToString()
          }
          );
      }
    }
  }

  [Serializable]
  public class TestCollection : BusinessListBase<TestCollection, TestItem>
  {
  }

  [Serializable]
  public class TestItem : BusinessBase<TestItem>
  {
    protected override object GetIdValue()
    {
      return 0;
    }

    public TestItem()
    {
      MarkAsChild();
    }
  }

  [Serializable]
  public class TestIndexableItem : BusinessBase<TestIndexableItem>
  {
    [Indexable]
    public string IndexedString{get; set;}
    [Indexable]
    public int IndexedInt{get; set;}
    public string NonIndexedString{get; set;}

  }
}
