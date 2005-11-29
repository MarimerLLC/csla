using System;
using System.Collections.Generic;
using System.Text;
using Csla.Test.Security;
using System.Data;
using System.Data.SqlClient;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.DPException
{
    [TestClass()]
    public class DataPortalExceptionTests
    {
        [TestMethod()]
        public void CheckInnerExceptionsOnSave()
        {
            Csla.Test.DataPortal.TransactionalRoot root = Csla.Test.DataPortal.TransactionalRoot.NewTransactionalRoot();
            root.FirstName = "Billy";
            root.LastName = "lastname";
            root.SmallColumn = "too long for the database"; //normally would be prevented through validation

            try
            {
                root = root.Save();
            }
            catch (Csla.DataPortalException ex)
            {
                //check base exception
                Assert.AreEqual("DataPortal.Update failed", ex.Message);
                //check inner exception
                Assert.AreEqual("DataPortal_Insert method call failed", ex.InnerException.Message);
                //check inner exception of inner exception
                Assert.AreEqual("String or binary data would be truncated.\r\nThe statement has been terminated.", ex.InnerException.InnerException.Message);

                //check what caused base exception and base's innerexception
                Assert.IsTrue(ex.Source == "Csla" && ex.InnerException.Source == "Csla");
                //check what caused inner exception's inner exception (i.e. the root exception)
                Assert.AreEqual(".Net SqlClient Data Provider", ex.InnerException.InnerException.Source);

                Assert.AreEqual("Csla.Test.DataPortal.TransactionalRoot", ex.BusinessObject.GetType().ToString());
            }
        }

        [TestMethod()]
        public void CheckInnerExceptionsOnDelete()
        {
            string baseException = string.Empty;
            string baseInnerException = string.Empty;
            string baseInnerInnerException = string.Empty;

            try
            {
                //this will throw an exception
                Csla.Test.DataPortal.TransactionalRoot.DeleteTransactionalRoot(13);
            }
            catch (Csla.DataPortalException ex)
            {
                baseException = ex.Message;
                baseInnerException = ex.InnerException.Message;
                baseInnerInnerException = ex.InnerException.InnerException.Message;
            }

            Assert.AreEqual("DataPortal.Delete failed", baseException);
            Assert.AreEqual("DataPortal_Delete method call failed", baseInnerException);
            Assert.AreEqual("DataPortal_Delete: you chose an unlucky number", baseInnerInnerException);
        }

        [TestMethod()]
        public void CheckInnerExceptionsOnFetch()
        {
            string baseException = string.Empty;
            string baseInnerException = string.Empty;
            string baseInnerInnerException = string.Empty;

            try
            {
                //this will throw an exception
                Csla.Test.DataPortal.TransactionalRoot root = 
                    Csla.Test.DataPortal.TransactionalRoot.GetTransactionalRoot(13);
            }
            catch (Csla.DataPortalException ex)
            {
                baseException = ex.Message;
                baseInnerException = ex.InnerException.Message;
                baseInnerInnerException = ex.InnerException.InnerException.Message;
            }

            Assert.AreEqual("DataPortal.Fetch failed", baseException);
            Assert.AreEqual("DataPortal_Fetch method call failed", baseInnerException);
            Assert.AreEqual("DataPortal_Fetch: you chose an unlucky number", baseInnerInnerException);
        }
    }
}