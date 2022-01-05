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
using Csla.TestHelpers;

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
            TestResults.Reinitialise();
            Csla.Test.DataPortal.StronglyTypedDP root = Csla.Test.DataPortal.StronglyTypedDP.GetStronglyTypedDP(456);

            Assert.AreEqual("Fetched", TestResults.GetResult("StronglyTypedDP"));
            Assert.AreEqual("fetched existing data", root.Data);
            Assert.AreEqual(456, root.Id); 
       
            //test strongly-typed DataPortal_Create method
            TestResults.Reinitialise();
            Csla.Test.DataPortal.StronglyTypedDP root2 = Csla.Test.DataPortal.StronglyTypedDP.NewStronglyTypedDP();

            Assert.AreEqual("Created", TestResults.GetResult("StronglyTypedDP"));
            Assert.AreEqual("new default data", root2.Data);
            Assert.AreEqual(56, root2.Id);

            //test strongly-typed DataPortal_Delete method
            Csla.Test.DataPortal.StronglyTypedDP.DeleteStronglyTypedDP(567);
            Assert.AreEqual(567, TestResults.GetResult("StronglyTypedDP_Criteria"));
        }

        [TestMethod]
        
        public void EncapsulatedIsBusyFails()
        {
          IDataPortal<EncapsulatedBusy> dataPortal = DataPortalFactory.CreateDataPortal<EncapsulatedBusy>();

          try
          {
            var obj = dataPortal.Fetch();
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
          IDataPortal<FactoryBusy> dataPortal = DataPortalFactory.CreateDataPortal<FactoryBusy>();

          try
          {
            var obj = dataPortal.Fetch();
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
            IDataPortal<DpRoot> dataPortal = DataPortalFactory.CreateDataPortal<DpRoot>();

            // TODO: Fix test
            //Csla.DataPortal.DataPortalInvoke += new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvoke);
            //Csla.DataPortal.DataPortalInvokeComplete += new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvokeComplete);

            //try
            //{
            //    TestResults.Reinitialise();
            //    DpRoot root = dataPortal.Create(new DpRoot.Criteria());

            //    root.Data = "saved";
            //    TestResults.Reinitialise();
            //    root = root.Save();

            //    Assert.IsTrue((bool)TestResults.GetResult("dpinvoke"], "DataPortalInvoke not called");
            //    Assert.IsTrue((bool)TestResults.GetResult("dpinvokecomplete"], "DataPortalInvokeComplete not called");
            //    Assert.IsTrue((bool)TestResults.GetResult("serverinvoke"], "Server DataPortalInvoke not called");
            //    Assert.IsTrue((bool)TestResults.GetResult("serverinvokecomplete"], "Server DataPortalInvokeComplete not called");
            //}
            //finally
            //{
            //    Csla.DataPortal.DataPortalInvoke -= new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvoke);
            //    Csla.DataPortal.DataPortalInvokeComplete -= new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvokeComplete);
            //}
        }

    [TestMethod]

    public void DataPortalBrokerTests()
    {
      TestResults.Reinitialise();
      Csla.Server.DataPortalBroker.DataPortalServer = new CustomDataPortalServer();

      try
      {
        var single = Csla.Test.DataPortalTest.Single.NewObject();

        Assert.AreEqual(TestResults.GetResult("Single"], "Created");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Create Called");

        TestResults.Reinitialise();

        single.Save();

        Assert.AreEqual(TestResults.GetResult("Single"], "Inserted");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Update Called");

        TestResults.Reinitialise();

        single = Csla.Test.DataPortalTest.Single.GetObject(1);

        Assert.AreEqual(TestResults.GetResult("Single"], "Fetched");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Fetch Called");

        TestResults.Reinitialise();

        single.Save();

        Assert.AreEqual(TestResults.GetResult("Single"], "Updated");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Update Called");

        TestResults.Reinitialise();

        Csla.Test.DataPortalTest.Single.DeleteObject(1);

        Assert.AreEqual(TestResults.GetResult("Single"), "Deleted");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Delete Called");
      }
      finally
      {
        TestResults.Reinitialise();
        Csla.Server.DataPortalBroker.DataPortalServer = null;
      }
    }

    [TestMethod]

    public void CallDataPortalOverrides()
    {
      TestResults.Reinitialise();
      ParentEntity parent = ParentEntity.NewParentEntity();
      parent.Data = "something";

      Assert.AreEqual(false, parent.IsDeleted);
      Assert.AreEqual(true, parent.IsValid);
      Assert.AreEqual(true, parent.IsNew);
      Assert.AreEqual(true, parent.IsDirty);
      Assert.AreEqual(true, parent.IsSavable);

      parent = parent.Save();

      Assert.AreEqual("Inserted", TestResults.GetResult("ParentEntity"));

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

      Assert.AreEqual("Updated", TestResults.GetResult("ParentEntity"));

      parent.Delete();
      Assert.AreEqual(true, parent.IsDeleted);
      parent = parent.Save();
      Assert.AreEqual("Deleted Self", TestResults.GetResult("ParentEntity"));

      ParentEntity.DeleteParentEntity(33);
      Assert.AreEqual("Deleted", TestResults.GetResult("ParentEntity"));
      Assert.AreEqual(false, parent.IsDeleted);
      Assert.AreEqual(true, parent.IsValid);
      Assert.AreEqual(true, parent.IsNew);
      Assert.AreEqual(true, parent.IsDirty);
      Assert.AreEqual(true, parent.IsSavable);

      ParentEntity.GetParentEntity(33);
      Assert.AreEqual("Fetched", TestResults.GetResult("ParentEntity"));
    }

    private void ClientPortal_DataPortalInvoke(DataPortalEventArgs obj)
    {
      TestResults.Add("dpinvoke", "true");
    }

    private void ClientPortal_DataPortalInvokeComplete(DataPortalEventArgs obj)
    {
      TestResults.Add("dpinvokecomplete", "true");
    }

  }

  [Serializable]
  public class EncapsulatedBusy : BusinessBase<EncapsulatedBusy>
  {
    [Create]
		protected void DataPortal_Create()
    {
      BusinessRules.CheckRules();
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