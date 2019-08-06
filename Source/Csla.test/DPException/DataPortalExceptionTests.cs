//-----------------------------------------------------------------------
// <copyright file="DataPortalExceptionTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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
#if DEBUG
        [TestMethod()]
        
        public void CheckInnerExceptionsOnSave()
        {
            Csla.ApplicationContext.Clear();

            Csla.Test.DataPortal.TransactionalRoot root = Csla.Test.DataPortal.TransactionalRoot.NewTransactionalRoot();
            root.FirstName = "Billy";
            root.LastName = "lastname";
            root.SmallColumn = "too long for the database"; //normally would be prevented through validation
            
            string baseException = string.Empty;
            string baseInnerException = string.Empty;
            string baseInnerInnerException = string.Empty;
            string exceptionSource = string.Empty;

            try
            {
                root = root.Save();
            }
            catch (Csla.DataPortalException ex)
            {
                baseException = ex.Message;
                baseInnerException = ex.InnerException.Message;
                baseInnerInnerException = ex.InnerException.InnerException.Message;
                exceptionSource = ex.InnerException.InnerException.Source;
                Assert.IsNull(ex.BusinessObject, "Business object shouldn't be returned");
            }

            //check base exception
            Assert.IsTrue(baseException.StartsWith("DataPortal.Update failed"), "Exception should start with 'DataPortal.Update failed'");
            Assert.IsTrue(baseException.Contains("String or binary data would be truncated."), 
              "Exception should contain 'String or binary data would be truncated.'");
            //check inner exception
            Assert.AreEqual("TransactionalRoot.DataPortal_Insert method call failed", baseInnerException);
            //check inner exception of inner exception
            Assert.AreEqual("String or binary data would be truncated.\r\nThe statement has been terminated.", baseInnerInnerException);

            //check what caused inner exception's inner exception (i.e. the root exception)
            Assert.AreEqual(".Net SqlClient Data Provider", exceptionSource);

            //verify that the implemented method, DataPortal_OnDataPortal 
            //was called for the business object that threw the exception
            Assert.AreEqual("Called", Csla.ApplicationContext.GlobalContext["OnDataPortalException"]);
        }
#endif

        [TestMethod()]
        public void CheckInnerExceptionsOnDelete()
        {
            Csla.ApplicationContext.Clear();

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

            Assert.IsTrue(baseException.StartsWith("DataPortal.Delete failed"), "Should start with 'DataPortal.Delete failed'");
            Assert.IsTrue(baseException.Contains("DataPortal_Delete: you chose an unlucky number"));
            Assert.AreEqual("TransactionalRoot.DataPortal_Delete method call failed", baseInnerException);
            Assert.AreEqual("DataPortal_Delete: you chose an unlucky number", baseInnerInnerException);

            //verify that the implemented method, DataPortal_OnDataPortal 
            //was called for the business object that threw the exception
            Assert.AreEqual("Called", Csla.ApplicationContext.GlobalContext["OnDataPortalException"]);
        }

        [TestMethod()]
        public void CheckInnerExceptionsOnFetch()
        {
            Csla.ApplicationContext.GlobalContext.Clear();

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

            Assert.IsTrue(baseException.StartsWith("DataPortal.Fetch failed"), "Should start with 'DataPortal.Fetch failed'");
            Assert.IsTrue(baseException.Contains("DataPortal_Fetch: you chose an unlucky number"), 
              "Should contain with 'DataPortal_Fetch: you chose an unlucky number'");
            Assert.AreEqual("TransactionalRoot.DataPortal_Fetch method call failed", baseInnerException);
            Assert.AreEqual("DataPortal_Fetch: you chose an unlucky number", baseInnerInnerException);

            //verify that the implemented method, DataPortal_OnDataPortal 
            //was called for the business object that threw the exception
            Assert.AreEqual("Called", Csla.ApplicationContext.GlobalContext["OnDataPortalException"]);
        }
    }
}