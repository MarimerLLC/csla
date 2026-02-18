//-----------------------------------------------------------------------
// <copyright file="DataPortalExceptionTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DPException
{
  [TestClass]
  public class DataPortalExceptionTests
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

    [TestMethod]
    public void CheckInnerExceptionsOnSave()
    {
      IDataPortal<DataPortal.TransactionalRoot> dataPortal = _testDIContext.CreateDataPortal<DataPortal.TransactionalRoot>();

      DataPortal.TransactionalRoot root = DataPortal.TransactionalRoot.NewTransactionalRoot(dataPortal);
      root.FirstName = "Billy";
      root.LastName = "lastname";
      root.SmallColumn = "too long for the database"; //normally would be prevented through validation

      string baseException = string.Empty;
      string baseInnerException = string.Empty;
      string baseInnerInnerException = string.Empty;

      try
      {
        root = root.Save();
      }
      catch (DataPortalException ex)
      {
        baseException = ex.Message;
        baseInnerException = ex.InnerException.Message;
        baseInnerInnerException = ex.InnerException.InnerException?.Message;
        Assert.IsNull(ex.BusinessObject, "Business object shouldn't be returned");
      }

      //check base exception
      Assert.IsTrue(baseException.StartsWith("DataPortal.Update failed"), "Exception should start with 'DataPortal.Update failed'");
      Assert.IsTrue(baseException.Contains("CHECK constraint failed"),
        "Exception should contain 'CHECK constraint failed'");
      //check inner exception
      Assert.AreEqual("TransactionalRoot.DataPortal_Insert method call failed", baseInnerException);
      //check inner exception of inner exception (SQLite CHECK constraint violation)
      Assert.IsTrue(baseInnerInnerException.Contains("CHECK constraint failed"),
        "Inner inner exception should contain 'CHECK constraint failed'");

      //verify that the implemented method, DataPortal_OnDataPortal
      //was called for the business object that threw the exception
      Assert.AreEqual("Called", TestResults.GetResult("OnDataPortalException"));
    }

    [TestMethod]
    public void CheckInnerExceptionsOnDelete()
    {
      IDataPortal<DataPortal.TransactionalRoot> dataPortal = _testDIContext.CreateDataPortal<DataPortal.TransactionalRoot>();

      string baseException = string.Empty;
      string baseInnerException = string.Empty;
      string baseInnerInnerException = string.Empty;

      try
      {
        //this will throw an exception
        DataPortal.TransactionalRoot.DeleteTransactionalRoot(13, dataPortal);
      }
      catch (DataPortalException ex)
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
      Assert.AreEqual("Called", TestResults.GetResult("OnDataPortalException"));
    }

    [TestMethod]
    public void CheckInnerExceptionsOnFetch()
    {
      IDataPortal<DataPortal.TransactionalRoot> dataPortal = _testDIContext.CreateDataPortal<DataPortal.TransactionalRoot>();

      string baseException = string.Empty;
      string baseInnerException = string.Empty;
      string baseInnerInnerException = string.Empty;

      try
      {
        //this will throw an exception
        DataPortal.TransactionalRoot root =
            DataPortal.TransactionalRoot.GetTransactionalRoot(13, dataPortal);
      }
      catch (DataPortalException ex)
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
      Assert.AreEqual("Called", TestResults.GetResult("OnDataPortalException"));
    }

    [TestMethod]
    public void CheckBusinessErrorInfoIsNullWhennErrorInfoIsNull() {
      IDataPortal<DataPortal.TransactionalRoot> dataPortal = _testDIContext.CreateDataPortal<DataPortal.TransactionalRoot>();

      try 
      {
        DataPortal.TransactionalRoot root = DataPortal.TransactionalRoot.GetTransactionalRoot(13, dataPortal);

        Assert.Fail("The previous operation should have thrown an Exception and not executed successfully.");
      } 
      catch (DataPortalException ex) 
      {
        Assert.IsNull(ex.ErrorInfo, $"{nameof(DataPortalException)}.{nameof(DataPortalException.ErrorInfo)} should have been null but is not.");
        Assert.IsNull(ex.BusinessErrorInfo, $"{nameof(DataPortalException)}.{nameof(DataPortalException.BusinessErrorInfo)} should have been null but is not.");
      }
    }
  }
}