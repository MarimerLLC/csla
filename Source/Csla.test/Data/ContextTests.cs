//-----------------------------------------------------------------------
// <copyright file="ContextTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Csla.Data;
using System.Configuration;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.Data
{
  [TestClass]
  public class ContextTests
  {
    private const string TestDBConnection = nameof(WellKnownValues.DataPortalTestDatabase);
    private const string InvalidTestDBConnection = "DataPortalTestDatabaseConnectionStringXXXXXXX";

    private const string ConnectionWithMissingDB = nameof(WellKnownValues.DataPortalTestDatabaseWithInvalidDBValue);
    

    #region Invalid connection strings
    [TestMethod]
    [ExpectedException(typeof(System.Collections.Generic.KeyNotFoundException))]
    public void InvalidConnectionSetting_Throws_ConfigurationErrorsException_for_SqlConnection()
    {
      using (var objectContextManager = ConnectionManager<SqlConnection>.GetManager(InvalidTestDBConnection, true))
      {
      }
    }

    [TestMethod]
    [ExpectedException(typeof(System.Collections.Generic.KeyNotFoundException))]
    public void InvalidConnectionSetting_Throws_ConfigurationErrorsException_for_LinqToSqlContextDataContext()
    {
      using (var objectContextManager = ContextManager<TestLinqToSqlContextDataContext>.GetManager(InvalidTestDBConnection, true))
      {
      }
    }

    [TestMethod]
    [ExpectedException(typeof(System.Collections.Generic.KeyNotFoundException))]
    public void InvalidConnectionSetting_Throws_ConfigurationErrorsException_for_EntitiesContextDataContext()
    {
      using (var objectContextManager = ObjectContextManager<DataPortalTestDatabaseEntities>.GetManager("DataPortalTestDatabaseEntitiesxxxxxx", true))
      {
      }
    }

#if DEBUG
    [TestMethod]
    [ExpectedException(typeof(SqlException))]
    
    public void ConnectionSetting_with_Invalid_DB_Throws_ConfigurationErrorsException_for_SqlConnection()
    {
      //throws SqlException
      using (var objectContextManager = ConnectionManager<SqlConnection>.GetManager(ConnectionWithMissingDB, true))
      {
      }
    }
#endif

#if DEBUG

    [TestMethod]
    [ExpectedException(typeof(SqlException))]
    
    public void ConnectionSetting_with_Invalid_DB_Throws_ConfigurationErrorsException_for_LinqToSqlContextDataContext()
    {
      using (var objectContextManager = ContextManager<TestLinqToSqlContextDataContext>.GetManager(ConnectionWithMissingDB, true))
      {
        Assert.IsNotNull(objectContextManager);
        //throws SqlException
        var count = objectContextManager.DataContext.Table1s.GetNewBindingList().Count;
      }
    }

    [TestMethod]
    [ExpectedException(typeof(EntityException))]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void ConnectionSetting_with_Invalid_DB_Throws_ConfigurationErrorsException_for_EntitiesContextDataContext()
    {
      using (var objectContextManager = ObjectContextManager<DataPortalTestDatabaseEntities>.GetManager(WellKnownValues.EntityConnectionWithMissingDBConnectionStringName, true))
      {
        Assert.IsNotNull(objectContextManager);
        //Throws EntityException
        var table = (from p in objectContextManager.ObjectContext.Table2
                     select p).ToList();
      }
    }


#endif
    #endregion

    #region Data

#if DEBUG
    [TestMethod]
    
    public void ExecuteReader_on_Table2_returns_reader_with_3_fields()
    {
      using (var objectContextManager = ConnectionManager<SqlConnection>.GetManager(TestDBConnection, true))
      {
        Assert.IsNotNull(objectContextManager);
        using (var command = new SqlCommand("Select * From Table2", objectContextManager.Connection))
        {
          command.CommandType = CommandType.Text;
          using (var reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
            Assert.IsTrue(reader.FieldCount == 3, "Did not get reader");
        }
      }
    }
#endif

#if DEBUG
    [TestMethod]
    
    public void Table1_retreived_through_LingToSqlDataContext_has_records()
    {
      using (var objectContextManager = ContextManager<TestLinqToSqlContextDataContext>.GetManager(TestDBConnection, true))
      {
        Assert.IsNotNull(objectContextManager);
        Assert.IsTrue(objectContextManager.DataContext.Table1s.GetNewBindingList().Count > 0, "Data in table is missing");
      }
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void Table2_retreived_through_LingToEntitiesDataContext_has_records()
    {
      using (var objectContextManager = ObjectContextManager<DataPortalTestDatabaseEntities>.GetManager(nameof(WellKnownValues.DataPortalTestDatabaseEntities), true))
      {
        Assert.IsNotNull(objectContextManager);

        var query = from p in objectContextManager.ObjectContext.Table2
                    select p;

        Assert.IsTrue(query.ToList().Count > 0, "Data in table is missing");
      }
    }
#endif
    #endregion

    #region Transaction Manager

#if DEBUG
    [TestMethod]
    
    public void Using_TransactionManager_Insert_of_2records_rolls_back_if_second_record_fails_insert()
    {
      ApplicationContext.LocalContext.Clear();
      var list = TransactionContextUserList.GetList();
      int counter = list.Count;

      list.Add(new TransactionContextUser
                 {
                   FirstName = "First",
                   LastName = "Last",
                   SmallColumn = "aaaa"
                 });

      list.Add(new TransactionContextUser
                 {
                   FirstName = "First1",
                   LastName = "Last1",
                   SmallColumn = "bbbbbbbbbbbbbb"
                 });

      bool gotError = false;
      try
      {
        list.Save();
      }
      catch (DataPortalException ex)
      {
        // will be thrown from SQL server
        gotError = true;
      }

      Assert.IsTrue(gotError, "SQL should have thrown an error");
      int tCount = 0;
      foreach (var r in ApplicationContext.LocalContext.Keys)
        if (r.ToString().StartsWith("__transaction:"))
          tCount++;
      Assert.AreEqual(0, tCount, "Transaction context should have been null");

      list = TransactionContextUserList.GetList();
      Assert.AreEqual(counter, list.Count, "Data should not have been saved.");
    }


    [TestMethod]
    
    public void Using_TransactionManager_Insert_2records_increases_count_by2_then_removing_them_decreases_count_by2()
    {
      ApplicationContext.LocalContext.Clear();
      var list = TransactionContextUserList.GetList();
      int beforeInsertCount = list.Count;

      list.AddRange(new[]
                      {
                        new TransactionContextUser
                          {
                            FirstName = "First",
                            LastName = "Last",
                            SmallColumn = "aaaa"
                          },
                        new TransactionContextUser
                          {
                            FirstName = "First1",
                            LastName = "Last",
                            SmallColumn = "bbb"
                          }
                      });

      list.Save();

      int tCount = 0;
      foreach (var r in ApplicationContext.LocalContext.Keys)
        if (r.ToString().StartsWith("__transaction:"))
          tCount++;
      Assert.AreEqual(0, tCount, "Transaction context should have been null");

      list = TransactionContextUserList.GetList();
      Assert.AreEqual(beforeInsertCount + 2, list.Count, "Data should have been saved.");

      list.Remove(list.Last(o => o.LastName == "Last"));
      list.Remove(list.Last(o => o.LastName == "Last"));

      list.Save();

      tCount = 0;
      foreach (var r in ApplicationContext.LocalContext.Keys)
        if (r.ToString().StartsWith("__transaction:"))
          tCount++;
      Assert.AreEqual(0, tCount, "Transaction context should have been null");

      list = TransactionContextUserList.GetList();
      Assert.AreEqual(beforeInsertCount, list.Count, "Data should not have been saved.");
    }

    [TestMethod]
    
    public void TestTransactionsManaagerConnectionProperty()
    {
      using (var manager = TransactionManager<SqlConnection, SqlTransaction>.GetManager(TestDBConnection, true))
      {
        Assert.AreSame(manager.Connection, manager.Transaction.Connection, "COnnection is not correct");
        Assert.IsNotNull(manager.Connection, "Connection should not be null.");
        Assert.IsNotNull(manager.Transaction, "Transaction should not be null.");
      }
    }
#endif

#endregion

  }
}
