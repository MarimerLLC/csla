using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Csla.Test.DataPortal
{
    [NUnit.Framework.TestFixture()]
    class DataPortalTests
    {
        #if csla20vb
        #warning The VB version is unable to access DataPortal.DataPortalInvokeEventHandler or InvokeCompleteHandler
        [NUnit.Framework.Test()]
        public void DataPortalEvents()
        {
            Assert.Fail("The VB version is unable to access DataPortal.DataPortalInvokeEventHandler or InvokeCompleteHandler");
        }
        #elif csla20cs

        private Action<DataPortalEventArgs> InvokeHandler;
        private Action<DataPortalEventArgs> InvokeCompleteHandler;
        [NUnit.Framework.Test()]
        public void DataPortalEvents()
        {
            InvokeHandler = new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvoke);
            InvokeCompleteHandler = new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvokeComplete);

            Csla.DataPortal.DataPortalInvoke += InvokeHandler;
            Csla.DataPortal.DataPortalInvokeComplete += InvokeCompleteHandler;

            try
            {
                ApplicationContext.GlobalContext.Clear();
                DpRoot root = DpRoot.NewRoot();

                //root.Data = "saved";
                //Csla.ApplicationContext.GlobalContext.Clear();
                //root = root.Save();

                Assert.IsTrue((bool)ApplicationContext.GlobalContext["dpinvoke"], "DataPortalInvoke not called");
                Assert.IsTrue((bool)ApplicationContext.GlobalContext["dpinvokecomplete"], "DataPortalInvokeComplete not called");
                Assert.IsTrue((bool)ApplicationContext.GlobalContext["serverinvoke"], "Server DataPortalInvoke not called");
                Assert.IsTrue((bool)ApplicationContext.GlobalContext["serverinvokecomplete"], "Server DataPortalInvokeComplete not called");
            }
            finally
            {
                Csla.DataPortal.DataPortalInvoke -= InvokeHandler;
                Csla.DataPortal.DataPortalInvokeComplete -= InvokeCompleteHandler;
            }
        }
        #endif

        private void ClientPortal_DataPortalInvoke(DataPortalEventArgs obj)
        {
            ApplicationContext.GlobalContext["dpinvoke"] = true;
        }

        private void ClientPortal_DataPortalInvokeComplete(DataPortalEventArgs obj)
        {
            ApplicationContext.GlobalContext["dpinvokecomplete"] = true;
        }
        
    }
}
