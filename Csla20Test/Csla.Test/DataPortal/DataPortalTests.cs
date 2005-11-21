using System;
using System.Collections.Generic;
using System.Text;
using Csla.Test.DataBinding;
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

namespace Csla.Test.DataPortal
{
    [TestClass()]
    public class DataPortalTests
    {
        //pull from ConfigurationManager
        private const string CONNECTION_STRING = 
            "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|DataPortalTestDatabase.mdf;Integrated Security=True;User Instance=True";
        
        public void ClearDataBase()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            SqlCommand cm = new SqlCommand("DELETE FROM Table2", cn);

            try
            {
                cn.Open();
                cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                cn.Close();
            }
        }

        [TestMethod()]
        public void TestEnterpriseServicesTransactionalUpdate()
        {
            Csla.Test.DataPortal.ESTransactionalRoot tr = Csla.Test.DataPortal.ESTransactionalRoot.NewESTransactionalRoot();
            tr.FirstName = "Bill";
            tr.LastName = "Johnson";
            //setting smallColumn to a string less than or equal to 5 characters will
            //not cause the transaction to rollback
            tr.SmallColumn = "abc";

            tr = tr.Save();

            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            SqlCommand cm = new SqlCommand("SELECT * FROM Table2", cn);

            try
            {
                cn.Open();
                SqlDataReader dr = cm.ExecuteReader();

                //will have rows since no sqlexception was thrown on the insert
                Assert.AreEqual(true, dr.HasRows);
                dr.Close();
            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                cn.Close();
            }

            ClearDataBase();

            Csla.Test.DataPortal.ESTransactionalRoot tr2 = Csla.Test.DataPortal.ESTransactionalRoot.NewESTransactionalRoot();
            tr2.FirstName = "Jimmy";
            tr2.LastName = "Smith";
            //intentionally input a string longer than varchar(5) to 
            //cause a sql exception and rollback the transaction
            tr2.SmallColumn = "this will cause a sql exception";

            try
            {
                //will throw a sql exception since the SmallColumn property is too long
                tr2 = tr2.Save();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("DataPortal.Update failed", ex.Message);
            }

            //within the DataPortal_Insert method, two commands are run to insert data into
            //the database. Here we verify that both commands have been rolled back
            try
            {
                cn.Open();
                SqlDataReader dr = cm.ExecuteReader();

                //should not have rows since both commands were rolled back
                Assert.AreEqual(false, dr.HasRows);
                dr.Close();
            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                cn.Close();
            }

            ClearDataBase();
        }

        [TestMethod()]
        public void TestTransactionScopeUpdate()
        {
            Csla.Test.DataPortal.TransactionalRoot tr = Csla.Test.DataPortal.TransactionalRoot.NewTransactionalRoot();
            tr.FirstName = "Bill";
            tr.LastName = "Johnson";
            //setting smallColumn to a string less than or equal to 5 characters will
            //not cause the transaction to rollback
            tr.SmallColumn = "abc";

            tr = tr.Save();

            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            SqlCommand cm = new SqlCommand("SELECT * FROM Table2", cn);

            try
            {
                cn.Open();
                SqlDataReader dr = cm.ExecuteReader();

                //will have rows since no sqlexception was thrown on the insert
                Assert.AreEqual(true, dr.HasRows);
                dr.Close();
            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                cn.Close();
            }

            ClearDataBase();

            Csla.Test.DataPortal.TransactionalRoot tr2 = Csla.Test.DataPortal.TransactionalRoot.NewTransactionalRoot();
            tr2.FirstName = "Jimmy";
            tr2.LastName = "Smith";
            //intentionally input a string longer than varchar(5) to 
            //cause a sql exception and rollback the transaction
            tr2.SmallColumn = "this will cause a sql exception";

            try
            {
                //will throw a sql exception since the SmallColumn property is too long
                tr2 = tr2.Save();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("DataPortal.Update failed", ex.Message);
            }

            //within the DataPortal_Insert method, two commands are run to insert data into
            //the database. Here we verify that both commands have been rolled back
            try
            {
                cn.Open();
                SqlDataReader dr = cm.ExecuteReader();

                //should not have rows since both commands were rolled back
                Assert.AreEqual(false, dr.HasRows);
                dr.Close();
            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                cn.Close();
            }

            ClearDataBase();
        }

        [TestMethod()]
        public void DataPortalEvents()
        {
            Csla.DataPortal.DataPortalInvoke += new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvoke);
            Csla.DataPortal.DataPortalInvokeComplete += new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvokeComplete);

            try
            {
                ApplicationContext.GlobalContext.Clear();
                DpRoot root = DpRoot.NewRoot();

                root.Data = "saved";
                Csla.ApplicationContext.GlobalContext.Clear();
                root = root.Save();

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

        [Test]
        public void CallDataPortalOverrides()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ParentEntity parent = ParentEntity.NewParentEntity();
            parent.Data = "something";

            Assert.AreEqual(false, parent.IsDeleted);
            Assert.AreEqual(true, parent.IsValid);
            Assert.AreEqual(true, parent.IsNew);
            Assert.AreEqual(true, parent.IsDirty);
            Assert.AreEqual(true, parent.IsSavable);

            parent = parent.Save();

            Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["ParentEntity"]);

            Assert.AreEqual(false, parent.IsDeleted);
            Assert.AreEqual(true, parent.IsValid);
            Assert.AreEqual(false, parent.IsNew);
            Assert.AreEqual(false, parent.IsDirty);
            Assert.AreEqual(false, parent.IsSavable);

            parent.Data = "something new";

            Assert.AreEqual(false, parent.IsDeleted);
            Assert.AreEqual(true, parent.IsValid);
            Assert.AreEqual(false, parent.IsNew);
            Assert.AreEqual(true, parent.IsDirty);
            Assert.AreEqual(true, parent.IsSavable);

            parent = parent.Save();

            Assert.AreEqual("Updated", Csla.ApplicationContext.GlobalContext["ParentEntity"]);

            parent.Delete();
            Assert.AreEqual(true, parent.IsDeleted);
            parent = parent.Save();
            Assert.AreEqual("Deleted Self", Csla.ApplicationContext.GlobalContext["ParentEntity"]);

            ParentEntity.DeleteParentEntity(33);
            Assert.AreEqual("Deleted", Csla.ApplicationContext.GlobalContext["ParentEntity"]);
            Assert.AreEqual(false, parent.IsDeleted);
            Assert.AreEqual(true, parent.IsValid);
            Assert.AreEqual(true, parent.IsNew);
            Assert.AreEqual(true, parent.IsDirty);
            Assert.AreEqual(true, parent.IsSavable);

            ParentEntity.GetParentEntity(33);
            Assert.AreEqual("Fetched", Csla.ApplicationContext.GlobalContext["ParentEntity"]);
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
