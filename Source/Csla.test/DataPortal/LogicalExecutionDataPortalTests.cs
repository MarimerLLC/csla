﻿namespace Csla.Test.DataPortal
{
  //[TestClass]
  //public class LogicalExecutionDataPortalTests
  //{
  //  [TestMethod]
  //  public void Test_LogicalExecution_Values()
  //  {
  //    TestResults.Reinitialise();      

  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client, ApplicationContext.LogicalExecutionLocation, "Default value of 'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Client'");

  //    var dataPortal = new Server.DataPortal();

  //    dataPortal.Create(typeof(LogicalExecutionTestBusiness), null, new DataPortalContext(null, false), true).Wait();
  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Server, (ApplicationContext.LogicalExecutionLocations)ApplicationContext.GlobalContext["LogicalExecutionDataPortalTests_Create"], "'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Server' DURING 'Create'");
  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client, ApplicationContext.LogicalExecutionLocation, "'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Client' AFTER 'Create'");
  //    TestResults.Reinitialise();

  //    dataPortal.Fetch(typeof(LogicalExecutionTestBusiness), null, new DataPortalContext(null, false), true).Wait();
  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Server, (ApplicationContext.LogicalExecutionLocations)ApplicationContext.GlobalContext["LogicalExecutionDataPortalTests_Fetch"], "'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Server' DURING 'Fetch'");
  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client, ApplicationContext.LogicalExecutionLocation, "'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Client' AFTER 'Fetch'");
  //    TestResults.Reinitialise();

  //    dataPortal.Delete(typeof(LogicalExecutionTestBusiness), null, new DataPortalContext(null, false), true).Wait();
  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Server, (ApplicationContext.LogicalExecutionLocations)ApplicationContext.GlobalContext["LogicalExecutionDataPortalTests_Delete"], "'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Server' DURING 'Delete'");
  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client, ApplicationContext.LogicalExecutionLocation, "'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Client' AFTER 'Delete'");
  //    TestResults.Reinitialise();

  //    dataPortal.Update(new LogicalExecutionTestBusiness(), new DataPortalContext(null, false), true).Wait();
  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Server, (ApplicationContext.LogicalExecutionLocations)ApplicationContext.GlobalContext["LogicalExecutionDataPortalTests_Update"], "'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Server' DURING 'Update'");
  //    Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client, ApplicationContext.LogicalExecutionLocation, "'ApplicationContext.LogicalExecutionLocation' must be 'LogicalExecutionLocations.Client' AFTER 'Update'");
  //  }

  //  [Serializable]
  //  private class LogicalExecutionTestBusiness : BusinessBase<LogicalExecutionTestBusiness>
  //  {
  //    [Create]
  //    private void Create()
  //    {
  //      //ApplicationContext.GlobalContext["LogicalExecutionDataPortalTests_Create"] = ApplicationContext.LogicalExecutionLocation;
  //    }

  //    [Fetch]
  //    private void Fetch()
  //    {
  //      //ApplicationContext.GlobalContext["LogicalExecutionDataPortalTests_Fetch"] = ApplicationContext.LogicalExecutionLocation;
  //    }

  //    [Insert]
  //    private void Insert()
  //    {
  //      //ApplicationContext.GlobalContext["LogicalExecutionDataPortalTests_Update"] = ApplicationContext.LogicalExecutionLocation;
  //    }

  //    [Delete]
  //    private void Delete()
  //    {
  //      //ApplicationContext.GlobalContext["LogicalExecutionDataPortalTests_Delete"] = ApplicationContext.LogicalExecutionLocation;
  //    }
  //  }
  //}
}
