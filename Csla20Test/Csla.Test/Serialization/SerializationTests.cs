using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Csla.Test.Serialization
{
    [TestFixture( )]
    class SerializationTests
    {
        [Test( )]
        public void Clone( )
        {
            Csla.ApplicationContext.GlobalContext.Clear( );
            SerializationRoot root = new SerializationRoot( );

            root = (SerializationRoot)root.Clone( );

            Assert.AreEqual(true, Csla.ApplicationContext.GlobalContext["Deserialized"], 
                "Deserialized not called");
        }

        [Test( )]
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
                (OnIsDirtyChanged);

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
    }
}