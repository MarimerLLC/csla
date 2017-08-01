//-----------------------------------------------------------------------
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
  }
}
