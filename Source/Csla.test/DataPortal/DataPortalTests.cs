//-----------------------------------------------------------------------
// <copyright file="DataPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Test.DataBinding;
using System.Data.SqlClient;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using FluentAssertions;
using Csla.Rules;
using Csla.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class DataPortalTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

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
    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void TestTransactionScopeUpdate()
    {
      IDataPortal<TransactionalRoot> dataPortal = _testDIContext.CreateDataPortal<TransactionalRoot>();

      TransactionalRoot tr = TransactionalRoot.NewTransactionalRoot(dataPortal);
      tr.FirstName = "Bill";
      tr.LastName = "Johnson";
      //setting smallColumn to a string less than or equal to 5 characters will
      //not cause the transaction to rollback
      tr.SmallColumn = "abc";

      tr = tr.Save();

      // TODO: These connection strings got lost, so I've tried to recreate them, but not sure how to do this
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
      catch (Exception)
      {
        //do nothing
      }
      finally
      {
        cn.Close();
      }

      ClearDataBase();

      TransactionalRoot tr2 = TransactionalRoot.NewTransactionalRoot(dataPortal);
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
      catch (Exception)
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

    [TestMethod]
    public void StronglyTypedDataPortalMethods()
    {
      IDataPortal<StronglyTypedDP> dataPortal = _testDIContext.CreateDataPortal<StronglyTypedDP>();

      //test strongly-typed DataPortal_Fetch method
      StronglyTypedDP root = StronglyTypedDP.GetStronglyTypedDP(456, dataPortal);

      Assert.AreEqual("Fetched", TestResults.GetResult("StronglyTypedDP"));
      Assert.AreEqual("fetched existing data", root.Data);
      Assert.AreEqual(456, root.Id);

      //test strongly-typed DataPortal_Create method
      TestResults.Reinitialise();
      StronglyTypedDP root2 = StronglyTypedDP.NewStronglyTypedDP(dataPortal);

      Assert.AreEqual("Created", TestResults.GetResult("StronglyTypedDP"));
      Assert.AreEqual("new default data", root2.Data);
      Assert.AreEqual(56, root2.Id);

      //test strongly-typed DataPortal_Delete method
      StronglyTypedDP.DeleteStronglyTypedDP(567, dataPortal);
      Assert.AreEqual("567", TestResults.GetResult("StronglyTypedDP_Criteria"));
    }

    [TestMethod]
    public void EncapsulatedIsBusyFails()
    {
      _testDIContext.ServiceProvider.GetRequiredService<CslaOptions>().DefaultWaitForIdleTimeoutInSeconds = 1;
      IDataPortal<EncapsulatedBusy> dataPortal = _testDIContext.CreateDataPortal<EncapsulatedBusy>();

      dataPortal.Invoking(dp => dp.Fetch()).Should().Throw<DataPortalException>().WithInnerException<TimeoutException>();
    }

    [TestMethod]
    public void FactoryIsBusyFails()
    {
      IDataPortal<FactoryBusy> dataPortal = _testDIContext.CreateDataPortal<FactoryBusy>();

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
    public void DataPortalBrokerTests()
    {
      var dps = _testDIContext.ServiceProvider.GetRequiredService<Server.DataPortalSelector>();
      var oldServer = Server.DataPortalBroker.DataPortalServer = new CustomDataPortalServer(dps);

      try
      {
        DataPortalTest.Single single = NewSingle();

        Assert.AreEqual(TestResults.GetResult("Single"), "Created");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Create Called");

        TestResults.Reinitialise();

        single.Save();

        Assert.AreEqual(TestResults.GetResult("Single"), "Inserted");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Update Called");

        TestResults.Reinitialise();

        single = GetSingle(1);

        Assert.AreEqual(TestResults.GetResult("Single"), "Fetched");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Fetch Called");

        TestResults.Reinitialise();

        single.Save();

        Assert.AreEqual(TestResults.GetResult("Single"), "Updated");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Update Called");

        TestResults.Reinitialise();

        DeleteSingle(1);

        Assert.AreEqual(TestResults.GetResult("Single"), "Deleted");
        Assert.AreEqual(TestResults.GetResult("CustomDataPortalServer"), "Delete Called");
      }
      finally
      {
        TestResults.Reinitialise();
        Server.DataPortalBroker.DataPortalServer = oldServer;
      }
    }

    [TestMethod]

    public void CallDataPortalOverrides()
    {
      IDataPortal<ParentEntity> dataPortal = _testDIContext.CreateDataPortal<ParentEntity>();

      ParentEntity parent = ParentEntity.NewParentEntity(dataPortal);
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

      ParentEntity.DeleteParentEntity(33, dataPortal);
      Assert.AreEqual("Deleted", TestResults.GetResult("ParentEntity"));
      Assert.AreEqual(false, parent.IsDeleted);
      Assert.AreEqual(true, parent.IsValid);
      Assert.AreEqual(true, parent.IsNew);
      Assert.AreEqual(true, parent.IsDirty);
      Assert.AreEqual(true, parent.IsSavable);

      ParentEntity.GetParentEntity(33, dataPortal);
      Assert.AreEqual("Fetched", TestResults.GetResult("ParentEntity"));
    }

    [TestMethod]
    public async Task WhenCreatingANewObjectThePortalMustWaitAfterCreateUntilTheObjectIsNotBusyAnymore()
    {
      var cslaOptions = _testDIContext.ServiceProvider.GetRequiredService<CslaOptions>();
      int oldTimeout = cslaOptions.DefaultWaitForIdleTimeoutInSeconds;
      try
      {
        cslaOptions.DefaultWaitForIdleTimeoutInSeconds = 5;

        var dataPortal = _testDIContext.CreateDataPortal<ObjectStillBusyAfterCreate>();

        var obj = await dataPortal.CreateAsync();

        obj.IsBusy.Should().BeFalse();
      }
      finally
      {
        cslaOptions.DefaultWaitForIdleTimeoutInSeconds = oldTimeout;
      }
    }

    private void ClientPortal_DataPortalInvoke(DataPortalEventArgs obj)
    {
      TestResults.Add("dpinvoke", "true");
    }

    private void ClientPortal_DataPortalInvokeComplete(DataPortalEventArgs obj)
    {
      TestResults.Add("dpinvokecomplete", "true");
    }

    private DataPortalTest.Single NewSingle()
    {
      IDataPortal<DataPortalTest.Single> dataPortal = _testDIContext.CreateDataPortal<DataPortalTest.Single>();

      return dataPortal.Create();
    }

    private DataPortalTest.Single GetSingle(int id)
    {
      IDataPortal<DataPortalTest.Single> dataPortal = _testDIContext.CreateDataPortal<DataPortalTest.Single>();

      return dataPortal.Fetch(id);
    }

    private void DeleteSingle(int id)
    {
      IDataPortal<DataPortalTest.Single> dataPortal = _testDIContext.CreateDataPortal<DataPortalTest.Single>();

      dataPortal.Delete(id);
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
  public class ObjectStillBusyAfterCreate : BusinessBase<ObjectStillBusyAfterCreate> 
  {
    public static PropertyInfo<string> RuleTriggerProperty = RegisterProperty<string>(nameof(RuleTrigger));
    public string RuleTrigger 
    { 
      get => ReadProperty(RuleTriggerProperty); 
      set => SetProperty(RuleTriggerProperty, value);
    }

    [Create]
    protected void Create() 
    {
      RuleTrigger = Guid.NewGuid().ToString();
    }

    protected override void AddBusinessRules() 
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new TwoSecondAsyncRule(RuleTriggerProperty));
    }

    private sealed class TwoSecondAsyncRule : BusinessRuleAsync 
    {
      public TwoSecondAsyncRule(IPropertyInfo primaryProperty) : base(primaryProperty) 
      {
      }

      /// <inheritdoc />
      protected override async Task ExecuteAsync(IRuleContext context) 
      {
        await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
      }
    }
  }

  [Serializable]
  [Server.ObjectFactory(typeof(FactoryBusyFactory))]
  public class FactoryBusy : BusinessBase<FactoryBusy>
  {
    public void MarkObjectBusy()
    {
      MarkBusy();
    }
  }

  public class FactoryBusyFactory : Csla.Server.ObjectFactory
  {
    public FactoryBusyFactory(ApplicationContext applicationContext) : base(applicationContext)
    {
    }

    public FactoryBusy Fetch()
    {
      var obj = (FactoryBusy)ApplicationContext.CreateInstance(typeof(FactoryBusy));
      MarkOld(obj);
      obj.MarkObjectBusy();
      return obj;
    }
  }
}