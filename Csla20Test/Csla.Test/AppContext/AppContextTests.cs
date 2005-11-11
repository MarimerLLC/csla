using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using NUnit.Framework;
using System.Threading;

namespace Csla.Test.AppContext
{
    [NUnit.Framework.TestFixture()]
    public class AppContextTests
    {
        #region NoContext
        /// <summary>
        /// Test to see if contexts get cleared out properly
        /// </summary>
        /// <remarks>
        /// This test only passes if "CSLA" is all capitol letters. Using "Csla",
        /// as the namespace implies, is incorrect.
        /// </remarks>
        [NUnit.Framework.Test()]
        public void NoContext()
        {
            //clear the contexts
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.Clear();

            SimpleRoot root = SimpleRoot.GetSimpleRoot("simple");

            //Shouldn't this be "Csla.ClientContext"? Why the inconsistencies?
            System.LocalDataStoreSlot slot = Thread.GetNamedDataSlot("CSLA.ClientContext");
            Assert.IsNull(Thread.GetData(slot), "ClientContext should be null");

            slot = Thread.GetNamedDataSlot("CSLA.GlobalContext");
            Assert.IsNull(Thread.GetData(slot), "GlobalContext should be null");
        }
        #endregion

        #region ClientContext
        /// <summary>
        /// Test the Client Context
        /// </summary>
        /// <remarks>
        /// Clearing the GlobalContext clears the ClientContext also? 
        /// Should the ClientContext be cleared explicitly also?
        /// </remarks>
        [NUnit.Framework.Test()]
        public void ClientContext()
        {
            #if csla20cs
            #warning Fails CS Tests, passes VB tests. Might be related to the lack of virtual on IsDirty error.
            #endif

            Csla.ApplicationContext.GlobalContext.Clear();

            Csla.ApplicationContext.ClientContext.Add("clientcontext", "client context data");
            Assert.AreEqual("client context data", Csla.ApplicationContext.ClientContext["clientcontext"]);

            Csla.Test.Basic.Root root = Csla.Test.Basic.Root.NewRoot();
            root.Data = "saved";
            Assert.AreEqual("saved", root.Data);
            Assert.AreEqual(true, root.IsDirty);
            Assert.AreEqual(true, root.IsValid);

            Csla.ApplicationContext.GlobalContext.Clear();
            root = root.Save();

            Assert.IsNotNull(root);
            Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["Root"]);
            Assert.AreEqual("saved", root.Data);
            Assert.AreEqual(false, root.IsNew);
            Assert.AreEqual(false, root.IsDeleted);
            Assert.AreEqual(false, root.IsDirty);

            Assert.AreEqual("client context data", Csla.ApplicationContext.ClientContext["clientcontext"]);
            Assert.AreEqual("client context data", Csla.ApplicationContext.GlobalContext["clientcontext"]);
        }
        #endregion

        #region GlobalContext
        /// <summary>
        /// Test the Global Context
        /// </summary>
        [NUnit.Framework.Test()]
        public void GlobalContext()
        {
            Csla.ApplicationContext.GlobalContext.Clear();

            ApplicationContext.GlobalContext["globalcontext"] = "global context data";
            Assert.AreEqual("global context data", ApplicationContext.GlobalContext["globalcontext"], "first");

            Csla.Test.Basic.Root root = Csla.Test.Basic.Root.NewRoot();
            root.Data = "saved";
            Assert.AreEqual("saved", root.Data);
            Assert.AreEqual(true, root.IsDirty);
            Assert.AreEqual(true, root.IsValid);

            Csla.ApplicationContext.GlobalContext.Clear();
            root = root.Save();

            Assert.IsNotNull(root);
            Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["Root"]);
            Assert.AreEqual("saved", root.Data);
            Assert.AreEqual(false, root.IsNew);
            Assert.AreEqual(false, root.IsDeleted);
            Assert.AreEqual(false, root.IsDirty);

            Assert.AreEqual("new global value", ApplicationContext.GlobalContext["globalcontext"], "Second");
        }
        #endregion

        #region Dataportal Events
        
        /// <summary>
        /// Test the dataportal events
        /// </summary>
        /// <remarks>
        /// How does the GlobalContext get the keys "dpinvoke" and "dpinvokecomplete"?
        /// 
        /// In the vb version of this test it calls RemoveHandler in VB. Unfortunately removing handlers aren't quite
        /// that easy in C# I had to declare EventHandlers that could be added and removed. Also I found out that the
        /// VB library does not seem to contain the DataPortalInvokeEventHandler object so I put a conditional compile
        /// flag around this method and set a warning message.
        /// </remarks>
        [NUnit.Framework.Test()]
        public void DataPortalEvents()
        {
            ApplicationContext.GlobalContext.Clear();
            ApplicationContext.Clear();
            ApplicationContext.GlobalContext["global"] = "global";

            this.InvokeHandler = new Action<DataPortalEventArgs>(OnDataPortaInvoke);
            this.InvokeCompleteHandler = new Action<DataPortalEventArgs>(OnDataPortalInvokeComplete);

            Csla.DataPortal.DataPortalInvoke += this.InvokeHandler;
            Csla.DataPortal.DataPortalInvoke += this.InvokeCompleteHandler;
            Csla.DataPortal.DataPortalInvokeComplete += this.InvokeCompleteHandler;

            Csla.Test.Basic.Root root = Csla.Test.Basic.Root.GetRoot("testing");

            Csla.DataPortal.DataPortalInvoke -= this.InvokeHandler;
            Csla.DataPortal.DataPortalInvokeComplete -= this.InvokeCompleteHandler;

            Assert.AreEqual("global", Csla.ApplicationContext.GlobalContext["ClientInvoke"], "Client invoke incorrect");
            Assert.AreEqual("global", Csla.ApplicationContext.GlobalContext["ClientInvokeComplete"], "Client invoke complete");
            Assert.AreEqual("global", Csla.ApplicationContext.GlobalContext["dpinvoke"], "Server invoke incorrect");
            Assert.AreEqual("global", Csla.ApplicationContext.GlobalContext["dpinvokecomplete"], "Server invoke compelte incorrect");

            //Maybe GetRoot should be called again, here, and tested to 
            //make sure that the EventHandlers were removed properly.
        }
        private Action<DataPortalEventArgs> InvokeHandler;
        private Action<DataPortalEventArgs> InvokeCompleteHandler;

        private void OnDataPortaInvoke(DataPortalEventArgs e)
        {
            Console.WriteLine("OnDataPortalInvoke");
            Csla.ApplicationContext.GlobalContext["ClientInvoke"] = ApplicationContext.GlobalContext["global"];
        }
        private void OnDataPortalInvokeComplete(DataPortalEventArgs e)
        {
            Console.WriteLine("OnDataPortalInvokeComplete");
            Csla.ApplicationContext.GlobalContext["ClientInvokeComplete"] = ApplicationContext.GlobalContext["global"];
        }
        #endregion

        #region FailCreateContext
        /// <summary>
        /// Test the FaileCreate Context
        /// </summary>
        [NUnit.Framework.Test()]
        public void FailCreateContext()
        {
            ApplicationContext.GlobalContext.Clear();
            ApplicationContext.Clear();

            ExceptionRoot root;
            try
            {
                root = ExceptionRoot.NewExceptionRoot();
                Assert.Fail("Exception didn't occur");
            }
            catch (DataPortalException ex)
            {
                root = (ExceptionRoot)ex.BusinessObject;
                Assert.AreEqual("Fail create", ex.GetBaseException().Message, "Base exception message incorrect");
                Assert.AreEqual("DataPortal.Create failed", ex.Message, "Exception message incorrect");
            }

            Assert.AreEqual("<new>", root.Data, "Business object not returned");
            Assert.AreEqual("create", ApplicationContext.GlobalContext["create"], "GlobalContext not preserved");
        }
        #endregion

        #region FailFetchContext
        [NUnit.Framework.Test()]
        public void FailFetchContext()
        {
            ApplicationContext.GlobalContext.Clear();
            ExceptionRoot root;
            try
            {
                root = ExceptionRoot.GetExceptionRoot("fail");
                Assert.Fail("Exception didn't occur");
            }
            catch (DataPortalException ex)
            {
                root = (ExceptionRoot)ex.BusinessObject;
                Assert.AreEqual("Fail fetch", ex.GetBaseException().Message, "Base exception message incorrect");
                Assert.AreEqual("DataPortal.Fetch failed", ex.Message, "Exception message incorrect");
            }

            Assert.AreEqual("fail", root.Data, "Business object not returned");
            Assert.AreEqual("create", ApplicationContext.GlobalContext["create"], "GlobalContext not preserved");
        }
        #endregion

        #region FailUpdateContext
        [NUnit.Framework.Test()]
        public void FailUpdateContext()
        {
            ApplicationContext.GlobalContext.Clear();

            ExceptionRoot root;
            try
            {
                root = ExceptionRoot.NewExceptionRoot();
                Assert.Fail("Create exception didn't occur");
            }
            catch (DataPortalException ex)
            {
                root = (ExceptionRoot)ex.BusinessObject;
                Assert.AreEqual("Fail create", ex.GetBaseException().Message, "Base exception message incorrect");
                Assert.AreEqual("DataPortal.Create failed", ex.Message, "Exception message incorrect");
            }

            root.Data = "boom";
            try
            {
                root = root.Save();
                Assert.Fail("Insert exception didn't occur");
            }
            catch (DataPortalException ex)
            {
                root = (ExceptionRoot)ex.BusinessObject;
                Assert.AreEqual("Fail insert", ex.GetBaseException().Message, "Base exception message incorrect");
                Assert.AreEqual("DataPortal.Update failed", ex.Message, "Exception message incorrect");
            }

            Assert.AreEqual("boom", root.Data, "Business object not returned");
            Assert.AreEqual("create", ApplicationContext.GlobalContext["create"], "GlobalContext not preserved");
        }
        #endregion

        #region FailDeleteContext
        [NUnit.Framework.Test()]
        public void FailDeleteContext()
        {
            ApplicationContext.GlobalContext.Clear();
            ApplicationContext.Clear();

            ExceptionRoot root = null;
            try
            {
                ExceptionRoot.DeleteExceptionRoot("fail");
                Assert.Fail("Exception didn't occur");
            }
            catch (DataPortalException ex)
            {
                root = (ExceptionRoot)ex.BusinessObject;
                Assert.AreEqual("Fail delete", ex.GetBaseException().Message, "Base exception message incorrect");
                Assert.AreEqual("DataPortal.Delete failed", ex.Message, "Exception message incorrect");
            }
            Assert.IsNull(root, "Business object returned");
            Assert.AreEqual("create", ApplicationContext.GlobalContext["create"], "GlobalContext not preserved");
        }
        #endregion
    }
}
