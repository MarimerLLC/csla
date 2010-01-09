using Csla;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test
{
  /// <summary>
  ///This is a test class for LinqObservableCollectionTest and is intended
  ///to contain all LinqObservableCollectionTest Unit Tests
  ///</summary>
  [TestClass()]
  public class LinqObservableCollectionTest
  {
    [TestMethod]
    public void Create()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);
      Assert.IsNotNull(obj);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("b", obj[0]);
    }

    [TestMethod]
    public void AddItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (o, e) =>
        {
          sourceChanged = true;
        };
      obj.CollectionChanged += (o, e) =>
        {
          objChanged = true;
        };
      source.Add("z");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(4, source.Count);
      Assert.AreEqual(2, obj.Count);
      Assert.AreEqual("z", source[3]);
      Assert.AreEqual("z", obj[1]);

      sourceChanged = false;
      objChanged = false;
      obj.Add("x");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(5, source.Count);
      Assert.AreEqual(3, obj.Count);
      Assert.AreEqual("x", source[4]);
      Assert.AreEqual("x", obj[2]);
    }

    [TestMethod]
    public void InsertItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (o, e) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (o, e) =>
      {
        objChanged = true;
      };
      source.Insert(1, "z");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(4, source.Count);
      Assert.AreEqual(2, obj.Count);
      Assert.AreEqual("z", source[1]);
      Assert.AreEqual("z", obj[1]);

      sourceChanged = false;
      objChanged = false;
      obj.Insert(1, "x");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(5, source.Count);
      Assert.AreEqual(3, obj.Count);
      Assert.AreEqual("x", source[1]);
      Assert.AreEqual("x", obj[1]);
    }

    [TestMethod]
    public void RemoveAt()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c", "bb" };
      var query = from r in source
                  where r[0] == 'b'
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (o, e) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (o, e) =>
      {
        objChanged = true;
      };
      source.RemoveAt(1);
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("c", source[1]);
      Assert.AreEqual("bb", obj[0]);

      sourceChanged = false;
      objChanged = false;
      obj.RemoveAt(0);
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(2, source.Count);
      Assert.AreEqual(0, obj.Count);
      Assert.AreEqual("a", source[0]);
      Assert.AreEqual("c", source[1]);
    }

    [TestMethod]
    public void RemoveItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c", "bb" };
      var query = from r in source
                  where r[0] == 'b'
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (o, e) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (o, e) =>
      {
        objChanged = true;
      };
      source.Remove("b");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("c", source[1]);
      Assert.AreEqual("bb", obj[0]);

      sourceChanged = false;
      objChanged = false;
      obj.Remove("bb");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(2, source.Count);
      Assert.AreEqual(0, obj.Count);
      Assert.AreEqual("a", source[0]);
      Assert.AreEqual("c", source[1]);
    }

    [TestMethod]
    public void ReplaceItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (o, e) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (o, e) =>
      {
        objChanged = true;
      };
      source[0] = "z";
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsFalse(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("z", source[0]);

      source[1] = "x";
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("x", source[1]);
      Assert.AreEqual("x", obj[0]);

      obj[0] = "r";
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("r", source[1]);
      Assert.AreEqual("r", obj[0]);
    }

    [TestMethod]
    public void MoveItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (o, e) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (o, e) =>
      {
        objChanged = true;
      };
      source.Move(1, 2);
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsFalse(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("b", obj[0]);
      Assert.AreEqual("b", source[2]);
      Assert.AreEqual("c", source[1]);
    }

    [TestMethod]
    public void ClearSource()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (o, e) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (o, e) =>
      {
        objChanged = true;
      };
      source.Clear();
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(0, source.Count);
      Assert.AreEqual(0, obj.Count);
    }

    [TestMethod]
    public void ClearView()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (o, e) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (o, e) =>
      {
        objChanged = true;
      };
      obj.Clear();
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(2, source.Count);
      Assert.AreEqual(0, obj.Count);
      Assert.AreEqual("a", source[0]);
      Assert.AreEqual("c", source[1]);
    }
  }
}
