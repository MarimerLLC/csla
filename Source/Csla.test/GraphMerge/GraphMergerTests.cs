﻿//-----------------------------------------------------------------------
// <copyright file="GraphMergeTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 

namespace Csla.Test.GraphMerge
{
  [TestClass]
  public class GraphMergerTests
  {
    [TestMethod]
    public void MergeInsert()
    {
      var obj = Csla.DataPortal.Create<Foo>();
      obj.Name = "1";
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.MockUpdated();

      var changed = false;
      obj.PropertyChanged += (o, e) => { changed = true; };

      var merger = new GraphMerger();
      merger.MergeGraph(obj, cloned);
      Assert.AreEqual(cloned.Name, obj.Name);
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.AreEqual(cloned.IsNew, obj.IsNew);
      Assert.IsFalse(obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsValid);
      Assert.IsTrue(changed);
    }

    [TestMethod]
    public void MergeUpdate()
    {
      var obj = Csla.DataPortal.Create<Foo>();
      obj.Name = "1";
      obj.MockUpdated();
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.MockUpdated();

      var changed = false;
      obj.PropertyChanged += (o, e) => { changed = true; };

      var merger = new GraphMerger();
      merger.MergeGraph(obj, cloned);
      Assert.AreEqual(cloned.Name, obj.Name);
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.AreEqual(cloned.IsNew, obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsValid);
      Assert.IsTrue(changed);
    }

    [TestMethod]
    public void MergeRuleUnbroken()
    {
      var obj = Csla.DataPortal.Create<Foo>();
      obj.Name = "2";
      obj.MockUpdated();
      var cloned = obj.Clone();
      cloned.Name = "1";
      cloned.MockUpdated();

      var changed = false;
      obj.PropertyChanged += (o, e) => { changed = true; };

      var merger = new GraphMerger();
      merger.MergeGraph(obj, cloned);
      Assert.AreEqual(cloned.Name, obj.Name);
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.AreEqual(cloned.IsNew, obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsTrue(obj.IsValid);
      Assert.IsTrue(changed);
    }

    [TestMethod]
    public void MergeDelete()
    {
      var obj = Csla.DataPortal.Create<Foo>();
      obj.Name = "1";
      obj.MockUpdated(); // make it an old object
      obj.MarkForDelete(); // mark for deletion
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.MockDeleted(); // mock DP delete result

      var changed = false;
      obj.PropertyChanged += (o, e) => { changed = true; };

      var merger = new GraphMerger();
      merger.MergeGraph(obj, cloned);
      Assert.AreEqual(cloned.Name, obj.Name);
      Assert.IsTrue(obj.IsNew, "obj.IsTrue");
      Assert.IsFalse(obj.IsDeleted, "obj.IsDeleted");
      Assert.AreEqual(cloned.IsDeleted, obj.IsDeleted, "IsDeleted equal");
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.AreEqual(cloned.IsNew, obj.IsNew);
      Assert.IsFalse(obj.IsValid);
      Assert.IsTrue(changed);
    }

    [TestMethod]
    public void MergeList()
    {
      var obj = Csla.DataPortal.Create<FooList>();
      obj.AddNew().Name = "existing";
      obj[0].MockUpdated();
      obj.AddNew().Name = "to be deleted";
      obj.Remove(obj[1]);
      obj.AddNew().Name = "new";
      obj.AddNew().Name = "remove in clone";
      Assert.AreEqual(3, obj.Count, "preclone count");
      var cloned = obj.Clone();
      Assert.AreEqual(3, cloned.Count, "postclone count");
      cloned.Remove(cloned.Where(_=>_.Name== "remove in clone").First());
      Assert.AreEqual(2, cloned.Count, "postclone count after removed obj");
      Assert.AreEqual(cloned[0].Name, obj[0].Name, "postclone [0]");
      Assert.AreEqual(cloned[1].Name, obj[1].Name, "postclone [1]");
      cloned[0].Name = "existing (cloned)";
      cloned[1].Name = "new (cloned)";
      cloned.AddNew().Name = "new in clone";
      cloned.MockUpdated();

      var changed = false;
      obj.CollectionChanged += (o, e) => { changed = true; };

      var merger = new GraphMerger();
      merger.MergeGraph<FooList, Foo>(obj, cloned);
      Assert.AreEqual(cloned.Count, obj.Count, "count");
      Assert.AreEqual(3, obj.Count, "count 3");
      Assert.AreEqual(cloned[0].Name, obj[0].Name, "[0]");
      Assert.AreEqual(cloned[1].Name, obj[1].Name, "[1]");
      Assert.AreEqual(cloned[2].Name, obj[2].Name, "[2]");
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.IsFalse(obj.IsDirty);
    }
  }
}
