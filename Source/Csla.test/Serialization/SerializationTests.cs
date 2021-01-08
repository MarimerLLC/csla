//-----------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using Csla.Serialization;
using Csla.Test.ValidationRules;
using UnitDriven;
using System.Threading.Tasks;
using System.Security.Claims;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;
using System.IO;
#endif 

namespace Csla.Test.Serialization
{
  [TestClass()]
  public class SerializationTests : TestBase
  {
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

    private static ClaimsPrincipal GetPrincipal(params string[] roles)
    {
      var identity = new ClaimsIdentity();
      foreach (var item in roles)
        identity.AddClaim(new Claim(ClaimTypes.Role, item));
      return new ClaimsPrincipal(identity);
    }


    [TestMethod]
    public void SerializeDataPortalException()
    {
      var obj = new Csla.Server.DataPortalException("test message", new Exception("inner message"), null);
      var obj2 = (Csla.Server.DataPortalException)Csla.Core.ObjectCloner.Clone(obj);
      Assert.IsFalse(ReferenceEquals(obj, obj2));
      Assert.AreEqual(obj.Message, obj2.Message);
    }

    [TestMethod]
    public void CorrectDefaultSerializer()
    {
      var serializer = ApplicationContext.SerializationFormatter;
      Assert.AreEqual(ApplicationContext.SerializationFormatters.BinaryFormatter, serializer);
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
    public void TestSerializableEventsActionFails()
    {
      var root = new SerializationRoot();
      var nonSerClass = new NonSerializedClass();
      Action<object, PropertyChangedEventArgs> h = (sender, eventArgs) => { nonSerClass.Do(); };
      var method = typeof (Action<object, PropertyChangedEventArgs>).GetMethod("Invoke");
      var delgate = (PropertyChangedEventHandler)(object)method.CreateDelegate(typeof (PropertyChangedEventHandler), h);
      root.PropertyChanged += delgate;
      var b = new BinaryFormatterWrapper();
      try
      {
        b.Serialize(new MemoryStream(), root);
        Assert.Fail("Serialization should have thrown an exception");
      }
      catch (System.Runtime.Serialization.SerializationException)
      {
        // serialization failed as expected
      }
    }

    [TestMethod()]
    public void TestSerializableEventsActionSucceeds()
    {
      var root = new OverrideSerializationRoot();
      var nonSerClass = new NonSerializedClass();

      Action<object, PropertyChangedEventArgs> h = (sender, eventArgs) => { nonSerClass.Do(); };
      var method = typeof (Action<object, PropertyChangedEventArgs>).GetMethod("Invoke");
      var delgate = (PropertyChangedEventHandler)(object)method.CreateDelegate(typeof (PropertyChangedEventHandler), h);
      root.PropertyChanged += delgate;

      Action<object, PropertyChangingEventArgs> h1 = (sender, eventArgs) => { nonSerClass.Do(); };
      var method1 = typeof(Action<object, PropertyChangingEventArgs>).GetMethod("Invoke");
      var delgate1 = (PropertyChangingEventHandler)(object)method1.CreateDelegate(typeof(PropertyChangingEventHandler), h1);
      root.PropertyChanging += delgate1;

      var b = new BinaryFormatterWrapper();
      b.Serialize(new MemoryStream(), root);
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public async Task TestValidationRulesAfterSerialization()
    {
      UnitTestContext context = GetContext();
      var root = await Csla.DataPortal.CreateAsync<HasRulesManager>(new HasRulesManager.Criteria());
      root.Name = "";
      context.Assert.AreEqual(false, root.IsValid, "root should not start valid");

      root = root.Clone();
      context.Assert.AreEqual(false, root.IsValid, "root should not be valid after clone");
      root.Name = "something";
      context.Assert.AreEqual(true, root.IsValid, "root should be valid after property set");
      root = root.Clone();
      context.Assert.AreEqual(true, root.IsValid, "root should be valid after second clone");
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod()]
    public void TestSerializationCslaBinaryReaderWriterList()
    {
      var test = new BinaryReaderWriterTestClassList();
      BinaryReaderWriterTestClassList result;
      test.Setup();
      var serialized = MobileFormatter.SerializeToDTO(test);
      CslaBinaryWriter writer = new CslaBinaryWriter();
      byte[] data;
      using (var stream = new MemoryStream())
      {
        writer.Write(stream, serialized);
        data = stream.ToArray();
      }

      CslaBinaryReader reader = new CslaBinaryReader();
      using (var stream = new MemoryStream(data))
      {
        var deserialized = reader.Read(stream);
        result = (BinaryReaderWriterTestClassList)MobileFormatter.DeserializeFromDTO(deserialized);
      }

      Assert.AreEqual(test.Count, result.Count);
      for (int i = 0; i < test.Count; i++)
      {
        Assert.AreEqual(test[i].CharTest, result[i].CharTest);
        Assert.AreEqual(test[i].DateTimeOffsetTest, result[i].DateTimeOffsetTest);
        Assert.AreEqual(test[i].DateTimeTest, result[i].DateTimeTest);
        Assert.AreEqual(test[i].DecimalTest, result[i].DecimalTest);
        Assert.AreEqual(test[i].DoubleTest, result[i].DoubleTest);
        Assert.AreEqual(test[i].EnumTest, result[i].EnumTest);
        Assert.AreEqual(test[i].GuidTest, result[i].GuidTest);
        Assert.AreEqual(test[i].Int16Test, result[i].Int16Test);
        Assert.AreEqual(test[i].Int32Test, result[i].Int32Test);
        Assert.AreEqual(test[i].Int64Test, result[i].Int64Test);
        Assert.AreEqual(test[i].SByteTest, result[i].SByteTest);
        Assert.AreEqual(test[i].SingleTest, result[i].SingleTest);
        Assert.AreEqual(test[i].StringTest, result[i].StringTest);
        Assert.AreEqual(test[i].TimeSpanTest, result[i].TimeSpanTest);
        Assert.AreEqual(test[i].UInt16Test, result[i].UInt16Test);
        Assert.AreEqual(test[i].UInt32Test, result[i].UInt32Test);
        Assert.AreEqual(test[i].UInt64Test, result[i].UInt64Test);

        Assert.AreEqual(test[i].EmptySmartDateTest, result[i].EmptySmartDateTest);
        Assert.AreEqual(test[i].EmptySmartDateTest.FormatString, result[i].EmptySmartDateTest.FormatString);
        Assert.AreEqual(test[i].EmptySmartDateTest.EmptyIsMin, result[i].EmptySmartDateTest.EmptyIsMin);
        Assert.AreEqual(test[i].EmptySmartDateTest.IsEmpty, result[i].EmptySmartDateTest.IsEmpty);
        Assert.AreEqual(test[i].EmptySmartDateTest.Date, result[i].EmptySmartDateTest.Date);
                        
        Assert.AreEqual(test[i].FilledSmartDateTest, result[i].FilledSmartDateTest);
        Assert.AreEqual(test[i].FilledSmartDateTest.FormatString, result[i].FilledSmartDateTest.FormatString);
        Assert.AreEqual(test[i].FilledSmartDateTest.EmptyIsMin, result[i].FilledSmartDateTest.EmptyIsMin);
        Assert.AreEqual(test[i].FilledSmartDateTest.IsEmpty, result[i].FilledSmartDateTest.IsEmpty);
        Assert.AreEqual(test[i].FilledSmartDateTest.Date, result[i].FilledSmartDateTest.Date);
      }
    }


    [TestMethod()]
    public void TestSerializationCslaBinaryReaderWriter()
    {
      var test = new BinaryReaderWriterTestClass();
      BinaryReaderWriterTestClass result;
      test.Setup();
      var serialized = MobileFormatter.SerializeToDTO(test);
      CslaBinaryWriter writer = new CslaBinaryWriter();
      byte[] data;
      using (var stream = new MemoryStream())
      {
        writer.Write(stream, serialized);
        data = stream.ToArray();
      }
      
      CslaBinaryReader reader = new CslaBinaryReader();
      using (var stream = new MemoryStream(data))
      {
        var deserialized = reader.Read(stream);
        result = (BinaryReaderWriterTestClass)MobileFormatter.DeserializeFromDTO(deserialized);
      }
      Assert.AreEqual(test.BoolTest, result.BoolTest);
      Assert.AreEqual(test.ByteArrayTest.Length, result.ByteArrayTest.Length);
      for (int i = 0; i < test.ByteArrayTest.Length; i++)
      {
        Assert.AreEqual(test.ByteArrayTest[i], result.ByteArrayTest[i]);
      }
      
      Assert.AreEqual(test.ByteTest, result.ByteTest);
      Assert.AreEqual(test.CharArrayTest.Length, result.CharArrayTest.Length);
      for (int i = 0; i < test.CharArrayTest.Length; i++)
      {
        Assert.AreEqual(test.CharArrayTest[i], result.CharArrayTest[i]);
      }

      Assert.AreEqual(test.CharTest, result.CharTest);
      Assert.AreEqual(test.DateTimeOffsetTest, result.DateTimeOffsetTest);
      Assert.AreEqual(test.DateTimeTest, result.DateTimeTest);
      Assert.AreEqual(test.DecimalTest, result.DecimalTest);
      Assert.AreEqual(test.DoubleTest, result.DoubleTest);
      Assert.AreEqual(test.EnumTest, result.EnumTest);
      Assert.AreEqual(test.GuidTest, result.GuidTest);
      Assert.AreEqual(test.Int16Test, result.Int16Test);
      Assert.AreEqual(test.Int32Test, result.Int32Test);
      Assert.AreEqual(test.Int64Test, result.Int64Test);
      Assert.AreEqual(test.SByteTest, result.SByteTest);
      Assert.AreEqual(test.SingleTest, result.SingleTest);
      Assert.AreEqual(test.StringTest, result.StringTest);
      Assert.AreEqual(test.TimeSpanTest, result.TimeSpanTest);
      Assert.AreEqual(test.UInt16Test, result.UInt16Test);
      Assert.AreEqual(test.UInt32Test, result.UInt32Test);
      Assert.AreEqual(test.UInt64Test, result.UInt64Test);

      Assert.AreEqual(test.EmptySmartDateTest, result.EmptySmartDateTest);
      Assert.AreEqual(test.EmptySmartDateTest.FormatString, result.EmptySmartDateTest.FormatString);
      Assert.AreEqual(test.EmptySmartDateTest.EmptyIsMin, result.EmptySmartDateTest.EmptyIsMin);
      Assert.AreEqual(test.EmptySmartDateTest.IsEmpty, result.EmptySmartDateTest.IsEmpty);
      Assert.AreEqual(test.EmptySmartDateTest.Date, result.EmptySmartDateTest.Date);

      Assert.AreEqual(test.FilledSmartDateTest, result.FilledSmartDateTest);
      Assert.AreEqual(test.FilledSmartDateTest.FormatString, result.FilledSmartDateTest.FormatString);
      Assert.AreEqual(test.FilledSmartDateTest.EmptyIsMin, result.FilledSmartDateTest.EmptyIsMin);
      Assert.AreEqual(test.FilledSmartDateTest.IsEmpty, result.FilledSmartDateTest.IsEmpty);
      Assert.AreEqual(test.FilledSmartDateTest.Date, result.FilledSmartDateTest.Date);
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
      catch (Csla.Security.SecurityException ex)
      {
        Assert.AreEqual("Property set not allowed", ex.Message);
      }

      Csla.ApplicationContext.User = GetPrincipal("Admin");

      try
      {
        root.FirstName = "something";
      }
      catch (Csla.Security.SecurityException)
      {
        Assert.Fail("exception occurred");
      }

      Csla.ApplicationContext.User = new ClaimsPrincipal();

      Csla.Test.Security.PermissionsRoot rootClone = root.Clone();

      try
      {
        rootClone.FirstName = "something else";
        Assert.Fail("Exception didn't occur");
      }
      catch (Csla.Security.SecurityException ex)
      {
        Assert.AreEqual("Property set not allowed", ex.Message);
      }

      Csla.ApplicationContext.User = GetPrincipal("Admin");

      try
      {
        rootClone.FirstName = "something new";
      }
      catch (Csla.Security.SecurityException)
      {
        Assert.Fail("exception occurred");
      }

      Csla.ApplicationContext.User = new ClaimsPrincipal();

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
    [TestCategory("SkipWhenLiveUnitTesting")]
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
      var bf = (TestCommand)Csla.Core.ObjectCloner.Clone(cmd);
      Assert.AreEqual(cmd.Name, bf.Name, "after BinaryFormatter");

      var ndcs = new System.Runtime.Serialization.NetDataContractSerializer();
      ndcs.Serialize(buffer, cmd);
      buffer.Position = 0;
      var n = (TestCommand)ndcs.Deserialize(buffer);
      Assert.AreEqual(cmd.Name, n.Name, "after NDCS");

      buffer = new System.IO.MemoryStream();
      var mf = new Csla.Serialization.Mobile.MobileFormatter();
      mf.Serialize(buffer, cmd);
      buffer.Position = 0;
      var m = (TestCommand)mf.Deserialize(buffer);
      Assert.AreEqual(cmd.Name, m.Name, "after MobileFormatter");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
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

#if !NETFX_CORE
    [TestMethod]
    public void UseCustomSerializationFormatter()
    {
      System.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "Csla.Serialization.NetDataContractSerializerWrapper, Csla";
      try
      {
        var formatter = SerializationFormatterFactory.GetFormatter();

        Assert.AreEqual(ApplicationContext.SerializationFormatter, ApplicationContext.SerializationFormatters.CustomFormatter);
        Assert.IsInstanceOfType(formatter, typeof(NetDataContractSerializerWrapper));
      }
      finally
      {
        System.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = null;
      }
    }

    [TestMethod]
    public void UseNetDataContractSerializer()
    {
      System.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "NetDataContractSerializer";
      try
      {
        var formatter = SerializationFormatterFactory.GetFormatter();

        Assert.AreEqual(ApplicationContext.SerializationFormatter, ApplicationContext.SerializationFormatters.NetDataContractSerializer);
        Assert.IsInstanceOfType(formatter, typeof(NetDataContractSerializerWrapper));
      }
      finally
      {
        System.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = null;
      }
    }
  }
#endif

  [Serializable]
  public class TestCommand : CommandBase<TestCommand>
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