//-----------------------------------------------------------------------
// <copyright file="DataPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Test.DataBinding;
using System.Data;
using System.Data.SqlClient;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.DataPortal
{
    [TestClass()]
    public class DataPortalTests
    {   
        private static string CONNECTION_STRING = WellKnownValues.DataPortalTestDatabase;
        public void ClearDataBase()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            SqlCommand cm = new SqlCommand("DELETE FROM Table2", cn);

            try
            {
                cn.Open();
                cm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //do nothing
            }
            finally
            {
                cn.Close();
            }
        }

        [TestMethod()]
        [Ignore]
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
            catch
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
              Assert.IsTrue(ex.Message.StartsWith("DataPortal.Update failed"), "Invalid exception message");
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
            catch
            {
                //do nothing
            }
            finally
            {
                cn.Close();
            }

            ClearDataBase();
        }

#if DEBUG
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
              Assert.IsTrue(ex.Message.StartsWith("DataPortal.Update failed"), "Invalid exception message");
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
#endif

        [TestMethod()]
        
        public void StronglyTypedDataPortalMethods()
        {
            //test strongly-typed DataPortal_Fetch method
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.Test.DataPortal.StronglyTypedDP root = Csla.Test.DataPortal.StronglyTypedDP.GetStronglyTypedDP(456);

            Assert.AreEqual("Fetched", Csla.ApplicationContext.GlobalContext["StronglyTypedDP"]);
            Assert.AreEqual("fetched existing data", root.Data);
            Assert.AreEqual(456, root.Id); 
       
            //test strongly-typed DataPortal_Create method
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.Test.DataPortal.StronglyTypedDP root2 = Csla.Test.DataPortal.StronglyTypedDP.NewStronglyTypedDP();

            Assert.AreEqual("Created", Csla.ApplicationContext.GlobalContext["StronglyTypedDP"]);
            Assert.AreEqual("new default data", root2.Data);
            Assert.AreEqual(56, root2.Id);

            //test strongly-typed DataPortal_Delete method
            Csla.Test.DataPortal.StronglyTypedDP.DeleteStronglyTypedDP(567);
            Assert.AreEqual(567, Csla.ApplicationContext.GlobalContext["StronglyTypedDP_Criteria"]);
        }

        [TestMethod]
        
        public void EncapsulatedIsBusyFails()
        {
          try
          {
            var obj = Csla.DataPortal.Fetch<EncapsulatedBusy>();
          }
          catch (DataPortalException ex)
          {
            Assert.IsInstanceOfType(ex.InnerException, typeof(InvalidOperationException));
            return;
          }
          Assert.Fail("Expected exception");
        }

        [TestMethod]
        
        public void FactoryIsBusyFails()
        {
          try
          {
            var obj = Csla.DataPortal.Fetch<FactoryBusy>();
          }
          catch (DataPortalException ex)
          {
            Assert.IsInstanceOfType(ex.InnerException, typeof(InvalidOperationException));
            return;
          }
          Assert.Fail("Expected exception");
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

        [TestMethod]
        
        public void DataPortalBrokerTests()
        {
          ApplicationContext.GlobalContext.Clear();
          Csla.Server.DataPortalBroker.DataPortalServer = new CustomDataPortalServer();

          try
          {
            var single = Csla.Test.DataPortalTest.Single.NewObject();

            Assert.AreEqual(ApplicationContext.GlobalContext["Single"], "Created");
            Assert.AreEqual(ApplicationContext.GlobalContext["CustomDataPortalServer"], "Create Called");

            ApplicationContext.GlobalContext.Clear();

            single.Save();

            Assert.AreEqual(ApplicationContext.GlobalContext["Single"], "Inserted");
            Assert.AreEqual(ApplicationContext.GlobalContext["CustomDataPortalServer"], "Update Called");

            ApplicationContext.GlobalContext.Clear();

            single = Csla.Test.DataPortalTest.Single.GetObject(1);

            Assert.AreEqual(ApplicationContext.GlobalContext["Single"], "Fetched");
            Assert.AreEqual(ApplicationContext.GlobalContext["CustomDataPortalServer"], "Fetch Called");

            ApplicationContext.GlobalContext.Clear();

            single.Save();

            Assert.AreEqual(ApplicationContext.GlobalContext["Single"], "Updated");
            Assert.AreEqual(ApplicationContext.GlobalContext["CustomDataPortalServer"], "Update Called");

            ApplicationContext.GlobalContext.Clear();

            Csla.Test.DataPortalTest.Single.DeleteObject(1);

            Assert.AreEqual(ApplicationContext.GlobalContext["Single"], "Deleted");
            Assert.AreEqual(ApplicationContext.GlobalContext["CustomDataPortalServer"], "Delete Called");
          }
          finally
          {
            ApplicationContext.GlobalContext.Clear();
            Csla.Server.DataPortalBroker.DataPortalServer = null;
          }
        }

        [TestMethod]
        
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

  [Serializable]
  public class EncapsulatedBusy : BusinessBase<EncapsulatedBusy>
  {
    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
      MarkBusy();
    }

    private void DataPortal_Fetch()
    {
      MarkBusy();
    }
  }

  [Serializable]
  [Csla.Server.ObjectFactory(typeof(FactoryBusyFactory))]
  public class FactoryBusy : BusinessBase<FactoryBusy>
  {
    public void MarkObjectBusy()
    {
      MarkBusy();
    }
  }

  public class FactoryBusyFactory : Csla.Server.ObjectFactory
  {
    public FactoryBusy Fetch()
    {
      var obj = new FactoryBusy();
      MarkOld(obj);
      obj.MarkObjectBusy();
      return obj;
    }
  }
}