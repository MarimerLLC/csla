using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using Csla.Test.ValidationRules;
using UnitDriven;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 

namespace Csla.Test.Serialization
{
  [TestClass()]
  public class SerializationTests : TestBase
  {
    [Serializable]
    private class TestBusinessBindingListBaseCollection : BusinessBindingListBase<TestBusinessBindingListBaseCollection, TestIndexableItem>
    {
      public TestBusinessBindingListBaseCollection(int sampleSize)
        : this(sampleSize, 100, true) { }
      public TestBusinessBindingListBaseCollection(int sampleSize, int sparsenessFactor, bool randomize)
      {
        Random rnd = new Random();
        for (int i = 0; i < sampleSize; i++)
        {
          int nextRnd;
          if (randomize)
            nextRnd = rnd.Next(sampleSize / sparsenessFactor);
          else
            nextRnd = i / sparsenessFactor;
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
    private class TestCollection : BusinessBindingListBase<TestCollection, TestItem>
    {
    }

    [Serializable]
    private class TestItem : BusinessBase<TestItem>
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
    private class TestIndexableItem : BusinessBase<TestIndexableItem>, Csla.Core.IReadOnlyObject
    {
      [Indexable]
      public string IndexedString { get; set; }
      [Indexable]
      public int IndexedInt { get; set; }
      public string NonIndexedString { get; set; }


      #region IReadOnlyObject Members

      bool Csla.Core.IReadOnlyObject.CanReadProperty(string propertyName)
      {
        throw new NotImplementedException();
      }

      #endregion
    }

    [TestMethod()]
    public void TestIndexAfterSerialization()
    {
      UnitTestContext context = GetContext();
      var sampleSize = 100000;
      Console.WriteLine("Creating " + sampleSize + " element collection...");
      var blbCollection = new TestBusinessBindingListBaseCollection(sampleSize);
      Console.WriteLine("Collection established.");

      //first query establishes the index
      var controlSet = blbCollection.ToList();

      var primeQuery = from i in blbCollection where i.IndexedString == "42" select i;
      var forcedPrimeITeration = primeQuery.ToArray();

      Stopwatch watch = new Stopwatch();
      //clone a collection - this will force a serialization/unserialization cycle
      var clonedCollection = blbCollection.Clone();
      //prime the clone, which will build the index since the cloned collection will lack one
      //  initially
      var primeClone = from i in clonedCollection where i.IndexedString == "42" select i;

      watch.Start();
      var indexedQuery = from i in clonedCollection where i.IndexedString == "42" select i;
      var forcedIterationIndexed = indexedQuery.ToArray();
      watch.Stop();

      var indexedRead = watch.ElapsedMilliseconds;

      watch.Reset();

      watch.Start();
      var nonIndexedQuery = from i in clonedCollection where i.NonIndexedString == "42" select i;
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
      context.Assert.IsTrue(indexedRead < nonIndexedRead);
      context.Assert.IsTrue(forcedIterationIndexed.Count() == forcedIterationNonIndexed.Count());
      context.Assert.Success();
    }

    [TestMethod()]
    public void TestWithoutSerializableHandler()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      SerializationRoot root = new SerializationRoot();
      nonSerializableEventHandler handler = new nonSerializableEventHandler();
      handler.Reg(root);
      root.Data = "something";
      context.Assert.AreEqual(1, Csla.ApplicationContext.GlobalContext["PropertyChangedFiredCount"]);
      root.Data = "something else";
      context.Assert.AreEqual(2, Csla.ApplicationContext.GlobalContext["PropertyChangedFiredCount"]);

      //serialize an object with eventhandling objects that are nonserializable
      root = root.Clone();
      root.Data = "something new";

      //still at 2 even though we changed the property again 
      //when the clone method performs serialization, the nonserializable 
      //object containing an event handler for the propertyChanged event
      //is lost
      context.Assert.AreEqual(2, Csla.ApplicationContext.GlobalContext["PropertyChangedFiredCount"]);
      context.Assert.Success();
    }
    
    [TestMethod()]
    public void Clone()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      SerializationRoot root = new SerializationRoot();

      root = (SerializationRoot)root.Clone();

      context.Assert.AreEqual(
        true, 
        Csla.ApplicationContext.GlobalContext["Deserialized"],
        "Deserialized not called");
      context.Assert.Success();
    }
    
    [TestMethod()]
    public void SerializableEvents()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();

      SerializationRoot root = new SerializationRoot();
      TestEventSink handler = new TestEventSink();

      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler
          (OnIsDirtyChanged);

      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler
          (StaticOnIsDirtyChanged);

      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler
          (PublicStaticOnIsDirtyChanged);

      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler
          (OnIsDirtyChanged);  //will call this method twice since it is assigned twice

      handler.Reg(root);

      root.Data = "abc";

      context.Assert.AreEqual("abc", root.Data, "Data value not set");

      context.Assert.AreEqual(
        "OnIsDirtyChanged",
        Csla.ApplicationContext.GlobalContext["OnIsDirtyChanged"],
        "Didn't call local handler");

      context.Assert.AreEqual(
        "StaticOnIsDirtyChanged",
        Csla.ApplicationContext.GlobalContext["StaticOnIsDirtyChanged"],
        "Didn't call static handler");

      Assert.AreEqual(
        "PublicStaticOnIsDirtyChanged",
        Csla.ApplicationContext.GlobalContext["PublicStaticOnIsDirtyChanged"],
        "Didn't call public static handler");

      Assert.AreEqual(
        "Test.OnIsDirtyChanged",
        Csla.ApplicationContext.GlobalContext["Test.OnIsDirtyChanged"],
        "Didn't call serializable handler");

      Assert.AreEqual(
        "Test.PrivateOnIsDirtyChanged",
        Csla.ApplicationContext.GlobalContext["Test.PrivateOnIsDirtyChanged"],
        "Didn't call serializable private handler");

      root = (SerializationRoot)root.Clone();

      Csla.ApplicationContext.GlobalContext.Clear();

      root.Data = "xyz";

      context.Assert.AreEqual("xyz", root.Data, "Data value not set");

      context.Assert.AreEqual(null, Csla.ApplicationContext.GlobalContext["OnIsDirtyChanged"],
          "Called local handler after clone");

      context.Assert.AreEqual(null, Csla.ApplicationContext.GlobalContext["StaticOnIsDirtyChanged"],
          "Called static handler after clone");

      context.Assert.AreEqual(
        "PublicStaticOnIsDirtyChanged",
        Csla.ApplicationContext.GlobalContext["PublicStaticOnIsDirtyChanged"],
        "Didn't call public static handler after clone");

      context.Assert.AreEqual(
        "Test.OnIsDirtyChanged",
        Csla.ApplicationContext.GlobalContext["Test.OnIsDirtyChanged"],
        "Didn't call serializable handler after clone");

      context.Assert.AreEqual(null, Csla.ApplicationContext.GlobalContext["Test.PrivateOnIsDirtyChanged"],
          "Called serializable private handler after clone");

      context.Assert.Success();
    }
    
    [TestMethod()]
    public void TestValidationRulesAfterSerialization()
    {
      UnitTestContext context = GetContext();
      HasRulesManager.NewHasRulesManager((o, e) =>
      {
        HasRulesManager root = e.Object;
        root.Name = "";
        context.Assert.AreEqual(false, root.IsValid, "root should not start valid");

        root = root.Clone();
        context.Assert.AreEqual(false, root.IsValid, "root should not be valid after clone");
        root.Name = "something";
        context.Assert.AreEqual(true, root.IsValid, "root should be valid after property set");
        root = root.Clone();
        context.Assert.AreEqual(true, root.IsValid, "root should be valid after second clone");
        context.Assert.Success();
      });

      context.Complete();
    }
    
    [TestMethod()]
    public void TestAuthorizationRulesAfterSerialization()
    {
      Csla.Test.Security.PermissionsRoot root = Csla.Test.Security.PermissionsRoot.NewPermissionsRoot();

      try
      {
        root.FirstName = "something";
        Assert.Fail("Exception didn't occur");
      }
      catch (System.Security.SecurityException ex)
      {
        Assert.AreEqual("Property set not allowed", ex.Message);
      }

      Csla.Test.Security.TestPrincipal.SimulateLogin();

      try
      {
        root.FirstName = "something";
      }
      catch (System.Security.SecurityException ex)
      {
        Assert.Fail("exception occurred");
      }

      Csla.Test.Security.TestPrincipal.SimulateLogout();

      Csla.Test.Security.PermissionsRoot rootClone = root.Clone();

      try
      {
        rootClone.FirstName = "something else";
        Assert.Fail("Exception didn't occur");
      }
      catch (System.Security.SecurityException ex)
      {
        Assert.AreEqual("Property set not allowed", ex.Message);
      }

      Csla.Test.Security.TestPrincipal.SimulateLogin();

      try
      {
        rootClone.FirstName = "something new";
      }
      catch (System.Security.SecurityException ex)
      {
        Assert.Fail("exception occurred");
      }

      Csla.Test.Security.TestPrincipal.SimulateLogout();

    }

    private void OnIsDirtyChanged(object sender, PropertyChangedEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["OnIsDirtyChanged"] = "OnIsDirtyChanged";
    }

    private static void StaticOnIsDirtyChanged(object sender, PropertyChangedEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["StaticOnIsDirtyChanged"] =
          "StaticOnIsDirtyChanged";
    }

    public static void PublicStaticOnIsDirtyChanged(object sender, PropertyChangedEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["PublicStaticOnIsDirtyChanged"] =
          "PublicStaticOnIsDirtyChanged";
    }

    [TestMethod]
    public void DCClone()
    {
      System.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] =
        "NetDataContractSerializer";
      Assert.AreEqual(
        Csla.ApplicationContext.SerializationFormatters.NetDataContractSerializer,
        Csla.ApplicationContext.SerializationFormatter,
        "Formatter should be NetDataContractSerializer");

      DCRoot root = new DCRoot();
      root.Data = 123;
      DCRoot clone = root.Clone();

      Assert.IsFalse(ReferenceEquals(root, clone), "Object instance should be different");
      Assert.AreEqual(root.Data, clone.Data, "Data should match");
      Assert.IsTrue(root.IsDirty, "Root IsDirty should be true");
      Assert.IsTrue(clone.IsDirty, "Clone IsDirty should be true");
    }

    [TestMethod]
    public void DCEditLevels()
    {
      System.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] =
        "NetDataContractSerializer";
      Assert.AreEqual(
        Csla.ApplicationContext.SerializationFormatters.NetDataContractSerializer,
        Csla.ApplicationContext.SerializationFormatter,
        "Formatter should be NetDataContractSerializer");

      DCRoot root = new DCRoot();
      root.BeginEdit();
      root.Data = 123;
      root.CancelEdit();

      Assert.AreEqual(0, root.Data, "Data should be 0");

      root.BeginEdit();
      root.Data = 123;
      root.ApplyEdit();

      Assert.AreEqual(123, root.Data, "Data should be 123");
    }

    [TestMethod]
    public void AsyncLoadManagerSerializationTest()
    {
      Csla.Test.Basic.Children list = Csla.Test.Basic.Children.NewChildren();
      list.Add("1");
      list.Add("2");
      IEditableObject item = list[1] as IEditableObject;
      int editLevel = (int)item.GetType().GetProperty("EditLevel", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(item, null);
      object manager = item.GetType().GetProperty("LoadManager", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(item, null);

      item.BeginEdit();
      int newEditLevel = (int)item.GetType().GetProperty("EditLevel", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(item, null);
      Assert.AreEqual(editLevel + 1, newEditLevel, "Edit level incorrect after begin edit");
    }

    [TestMethod]
    public void SerializeCommand()
    {
      var cmd = new TestCommand();
      cmd.Name = "test data";

      var buffer = new System.IO.MemoryStream();
#if !SILVERLIGHT
      var bf = (TestCommand)Csla.Core.ObjectCloner.Clone(cmd);
      Assert.AreEqual(cmd.Name, bf.Name, "after BinaryFormatter");

      var ndcs = new System.Runtime.Serialization.NetDataContractSerializer();
      ndcs.Serialize(buffer, cmd);
      buffer.Position = 0;
      var n = (TestCommand)ndcs.Deserialize(buffer);
      Assert.AreEqual(cmd.Name, n.Name, "after NDCS");
#endif

      buffer = new System.IO.MemoryStream();
      var mf = new Csla.Serialization.Mobile.MobileFormatter();
      mf.Serialize(buffer, cmd);
      buffer.Position = 0;
      var m = (TestCommand)mf.Deserialize(buffer);
      Assert.AreEqual(cmd.Name, m.Name, "after MobileFormatter");
    }

#if !SILVERLIGHT
    [TestMethod]
    public void CommandOverDataPortal()
    {
      Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";
      try
      {
        var cmd = new TestCommand();
        cmd.Name = "test data";

        var result = Csla.DataPortal.Execute<TestCommand>(cmd);

        Assert.IsFalse(ReferenceEquals(cmd, result), "References should not match");
        Assert.AreEqual(cmd.Name + " server", result.Name);
      }
      finally
      {
        System.Configuration.ConfigurationManager.AppSettings["CslaDataPortalProxy"] = null;
      }
    }
#endif
  }

  [Serializable]
  public class TestCommand : CommandBase
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty(typeof(TestCommand), new PropertyInfo<string>("Name"));
    public string Name
    {
      get { return ReadProperty(NameProperty); }
      set { LoadProperty(NameProperty, value); }
    }

    protected override void DataPortal_Execute()
    {
      Name = Name + " server";     
    }
  }
}