using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Csla.Test.DataPortal
{
    [NUnit.Framework.TestFixture()]
    public class DataPortalTests
    {
        [NUnit.Framework.Test()]
        public void DataPortalEvents()
        {

            Csla.DataPortal.DataPortalInvoke += new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvoke);
            Csla.DataPortal.DataPortalInvokeComplete += new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvokeComplete);

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
                Csla.DataPortal.DataPortalInvoke -= new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvoke);
                Csla.DataPortal.DataPortalInvokeComplete -= new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvokeComplete);
            }
        }

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
