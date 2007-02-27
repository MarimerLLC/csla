using System;
using System.Collections.Generic;
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

namespace Csla.Test.Serialization
{
    [TestClass()]
    public class SerializationTests
    {

        [TestMethod()]
        public void TestWithoutSerializableHandler()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            SerializationRoot root = new SerializationRoot();
            nonSerializableEventHandler handler = new nonSerializableEventHandler();
            handler.Reg(root);
            root.Data = "something";
            Assert.AreEqual(1, Csla.ApplicationContext.GlobalContext["PropertyChangedFiredCount"]);
            root.Data = "something else";
            Assert.AreEqual(2, Csla.ApplicationContext.GlobalContext["PropertyChangedFiredCount"]);

            //serialize an object with eventhandling objects that are nonserializable
            root = root.Clone();
            root.Data = "something new";

            //still at 2 even though we changed the property again 
            //when the clone method performs serialization, the nonserializable 
            //object containing an event handler for the propertyChanged event
            //is lost
            Assert.AreEqual(2, Csla.ApplicationContext.GlobalContext["PropertyChangedFiredCount"]);
        }

        [TestMethod()]
        public void Clone( )
        {
            Csla.ApplicationContext.GlobalContext.Clear( );
            SerializationRoot root = new SerializationRoot( );

            root = (SerializationRoot)root.Clone( );

            Assert.AreEqual(true, Csla.ApplicationContext.GlobalContext["Deserialized"], 
                "Deserialized not called");
        }

        [TestMethod()]
        public void SerializableEvents( )
        {
            Csla.ApplicationContext.GlobalContext.Clear( );

            SerializationRoot root = new SerializationRoot( );
            TestEventSink handler = new TestEventSink( );

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
            
            Assert.AreEqual("abc", root.Data, "Data value not set");
            
            Assert.AreEqual("OnIsDirtyChanged", 
                Csla.ApplicationContext.GlobalContext["OnIsDirtyChanged"],
                "Didn't call local handler");
            
            Assert.AreEqual("StaticOnIsDirtyChanged",
                Csla.ApplicationContext.GlobalContext["StaticOnIsDirtyChanged"], 
                "Didn't call static handler");

            Assert.AreEqual("PublicStaticOnIsDirtyChanged",
                Csla.ApplicationContext.GlobalContext["PublicStaticOnIsDirtyChanged"],
                "Didn't call public static handler");

            Assert.AreEqual("Test.OnIsDirtyChanged",
                Csla.ApplicationContext.GlobalContext["Test.OnIsDirtyChanged"],
                "Didn't call serializable handler");
            
            Assert.AreEqual("Test.PrivateOnIsDirtyChanged",
                Csla.ApplicationContext.GlobalContext["Test.PrivateOnIsDirtyChanged"],
                "Didn't call serializable private handler");

            root = (SerializationRoot)root.Clone( );

            Csla.ApplicationContext.GlobalContext.Clear( );

            root.Data = "xyz";

            Assert.AreEqual("xyz", root.Data, "Data value not set");

            Assert.AreEqual(null, Csla.ApplicationContext.GlobalContext["OnIsDirtyChanged"],
                "Called local handler after clone");

            Assert.AreEqual(null, Csla.ApplicationContext.GlobalContext["StaticOnIsDirtyChanged"],
                "Called static handler after clone");

            Assert.AreEqual("PublicStaticOnIsDirtyChanged",
                Csla.ApplicationContext.GlobalContext["PublicStaticOnIsDirtyChanged"],
                "Didn't call public static handler after clone");

            Assert.AreEqual("Test.OnIsDirtyChanged",
                Csla.ApplicationContext.GlobalContext["Test.OnIsDirtyChanged"],
                "Didn't call serializable handler after clone");

            Assert.AreEqual(null, Csla.ApplicationContext.GlobalContext["Test.PrivateOnIsDirtyChanged"],
                "Called serializable private handler after clone");
        }

        [TestMethod()]
        public void TestValidationRulesAfterSerialization()
        {
            Csla.Test.ValidationRules.HasRulesManager root = Csla.Test.ValidationRules.HasRulesManager.NewHasRulesManager();
            root.Name = "";
            Assert.AreEqual(false, root.IsValid);

            root = root.Clone();
            Assert.AreEqual(false, root.IsValid);
            root.Name = "something";
            Assert.AreEqual(true, root.IsValid);
            root = root.Clone();
            Assert.AreEqual(true, root.IsValid);
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

        private void OnIsDirtyChanged(object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["OnIsDirtyChanged"] = "OnIsDirtyChanged";
        }

        private static void StaticOnIsDirtyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["StaticOnIsDirtyChanged"] = 
                "StaticOnIsDirtyChanged";
        }

        public static void PublicStaticOnIsDirtyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
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

        Assert.IsFalse(ReferenceEquals(root,clone), "Object instance should be different");
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
    
    }
}