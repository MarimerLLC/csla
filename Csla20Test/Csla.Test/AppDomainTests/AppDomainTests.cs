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

namespace Csla.Test.AppDomainTests
{
    [TestClass]
    public class  AppDomainTestClass
    {
        [TestMethod]
        public void AppDomainTestIsCalled()
        {
            Assert.IsNotNull(System.Configuration.ConfigurationManager.AppSettings["CslaDataPortalProxy"], "Missing Proxy settings string");

            Console.WriteLine("Local: " + AppDomain.CurrentDomain.Id);

            Csla.DataPortal.DataPortalInvoke += new Action<DataPortalEventArgs>(DataPortal_DataPortalInvoke);
            Basic.Root r = Basic.Root.NewRoot();
            Csla.DataPortal.DataPortalInvoke -= new Action<DataPortalEventArgs>(DataPortal_DataPortalInvoke);
            
        }

        void DataPortal_DataPortalInvoke(DataPortalEventArgs obj)
        {
            Console.WriteLine("Remote: " + AppDomain.CurrentDomain.Id);
        }
    }
}
