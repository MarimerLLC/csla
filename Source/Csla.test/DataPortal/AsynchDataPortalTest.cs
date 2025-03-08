//-----------------------------------------------------------------------
// <copyright file="AsynchDataPortalTest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Create is an exception , if BO does not have DP_Create() overload</summary>
//-----------------------------------------------------------------------

using Csla.Test.Basic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using UnitDriven;
using Csla.Testing.Business.DataPortal;
using Single = Csla.Test.DataPortalTest.Single;
using Csla.Test.DataPortalTest;
using Csla.TestHelpers;
using cslalighttest.CslaDataProvider;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class AsynchDataPortalTest : TestBase
  {
    private CultureInfo CurrentCulture;
    private CultureInfo CurrentUICulture;
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Setup()
    {
      TestResults.Reinitialise();
      CurrentCulture = Thread.CurrentThread.CurrentCulture;
      CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
    }

    [TestCleanup]
    public void Cleanup()
    {
      Thread.CurrentThread.CurrentCulture = CurrentCulture;
      Thread.CurrentThread.CurrentUICulture = CurrentUICulture;
    }

    #region Create

    [TestMethod]
    public async Task BeginCreate_overload_without_parameters()
    {
      IDataPortal<Single> dataPortal = _testDIContext.CreateDataPortal<Single>();

      var created = await dataPortal.CreateAsync();
      Assert.AreEqual(created.Id, 0);
    }

    [TestMethod]
    public async Task BeginCreate_overload_with_Criteria_passed_and_Id_set()
    {
      IDataPortal<Single> dataPortal = _testDIContext.CreateDataPortal<Single>();

      var created = await dataPortal.CreateAsync(100);
      Assert.AreEqual(created.Id, 100);
    }

    [TestMethod]
    [ExpectedException(typeof(DataPortalException))]
    public async Task BeginCreate_with_exception()
    {
      IDataPortal<Single> dataPortal = _testDIContext.CreateDataPortal<Single>();

      await dataPortal.CreateAsync(9999);
    }

    [TestMethod]
    public async Task CreateAsync_NoCriteria()
    {
      IDataPortal<Single> dataPortal = _testDIContext.CreateDataPortal<Single>();

      var result = await dataPortal.CreateAsync();
      Assert.IsNotNull(result);
      Assert.AreEqual(0, result.Id);
    }

    [TestMethod]
    public async Task CreateAsync_WithCriteria()
    {
      IDataPortal<Single2> dataPortal = _testDIContext.CreateDataPortal<Single2>();

      var result = await dataPortal.CreateAsync(123);
      Assert.IsNotNull(result);
      Assert.AreEqual(123, result.Id);
    }


    [TestMethod]
    public async Task CreateAsync_WithException()
    {
      IDataPortal<Single2> dataPortal = _testDIContext.CreateDataPortal<Single2>();
        
      try
      {
        var result = await dataPortal.CreateAsync(9999);
        Assert.Fail("Expected exception not thrown");
      }
      catch (Exception ex)
      {
        Assert.IsInstanceOfType(ex, typeof(DataPortalException));
      }
    }

    [TestMethod]
    [Timeout(1000)]
    public async Task CreateAsync_Parallel()
    {
      var dataPortal = _testDIContext.CreateDataPortal<SingleWithFactory>();

      var testcount = 500;
      var list = new List<int>(testcount);
      for (var i = 0; i < testcount; i++)
      {
        list.Add(i);
      }

      var tasks = list.AsParallel().Select(x => dataPortal.CreateAsync(x));
      await Task.WhenAll(tasks);
    }

    #endregion

    #region Fetch

    [TestMethod]
    public async Task FetchAsync_NoCriteria()
    {
      IDataPortal<Single2> dataPortal = _testDIContext.CreateDataPortal<Single2>();

      var result = await dataPortal.FetchAsync();
      Assert.IsNotNull(result);
      Assert.AreEqual(0, result.Id);
    }

    [TestMethod]
    public async Task FetchAsync_WithCriteria()
    {
      IDataPortal<Single2> dataPortal = _testDIContext.CreateDataPortal<Single2>();
      
      var result = await dataPortal.FetchAsync(123);
      Assert.IsNotNull(result);
      Assert.AreEqual(123, result.Id);
    }


    [TestMethod]
    public async Task FetchAsync_WithException()
    {
      IDataPortal<Single2> dataPortal = _testDIContext.CreateDataPortal<Single2>();

      try
      {
        var result = await dataPortal.FetchAsync(9999);
        Assert.Fail("Expected exception not thrown");
      }
      catch (Exception ex)
      {
        Assert.IsInstanceOfType(ex, typeof(DataPortalException));
      } 
    }

    [TestMethod]
    [Timeout(1000)]
    public async Task FetchAsync_Parallel()
    {
      IDataPortal<SingleWithFactory> dataPortal = _testDIContext.CreateDataPortal<SingleWithFactory>();

      var list = new List<int>(500);
      for (var i = 0; i < 500; i++)
      {
        list.Add(i);
      }

      var tasks = list.AsParallel().Select(_ => dataPortal.FetchAsync());
      await Task.WhenAll(tasks);
    }

    #endregion

#if DEBUG

    [TestMethod]
    public async Task SaveAsync()
    {
      IDataPortal<Single2> dataPortal = _testDIContext.CreateDataPortal<Single2>();

      var result = await dataPortal.CreateAsync(0);
      Assert.IsNotNull(result);
      Assert.AreEqual(0, result.Id);
      Assert.IsTrue(result.IsNew);
      Assert.IsTrue(result.IsDirty);
      result = await result.SaveAsync();
      Assert.IsFalse(result.IsNew);
      Assert.IsFalse(result.IsDirty);
    }

    [TestMethod]
    public async Task SaveAsyncWithException()
    {
      var context = GetContext();
      await context.Assert.Try(async () =>
      {
        IDataPortal<Single2> dataPortal = _testDIContext.CreateDataPortal<Single2>();

        var result = await dataPortal.CreateAsync(555);
        Assert.IsNotNull(result);
        Assert.AreEqual(555, result.Id);
        Assert.IsTrue(result.IsNew);
        Assert.IsTrue(result.IsDirty);
        
        try
        {
          result = await result.SaveAsync();
          Assert.Fail("Expected exception not thrown");
        }
        catch (Exception ex)
        {
          context.Assert.IsTrue(ex.GetType() == typeof(DataPortalException));
        }

        context.Assert.Success();
      });
      context.Complete();
    }

#endif

    #region ExecuteCommand

    [TestMethod]
    public async Task ExecuteCommand_called_without_UserState_results_in_UserState_defaulted_to_Null_server()
    {
      IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();

      var command = dataPortal.Create();
      var result = await dataPortal.ExecuteAsync(command);
      Assert.AreEqual("Executed", result.AProperty);
    }

    [TestMethod]
    public async Task ExecuteAsync()
    {
      IDataPortal<SingleCommand> dataPortal = _testDIContext.CreateDataPortal<SingleCommand>();

      SingleCommand cmd = dataPortal.Create(123);
      var result = await dataPortal.ExecuteAsync(cmd);
      Assert.IsNotNull(result);
      Assert.AreEqual(124, result.Value);
    }

    [TestMethod]
    public async Task ExecuteAsyncWithException()
    {
      IDataPortal<SingleCommand> dataPortal = _testDIContext.CreateDataPortal<SingleCommand>();

      try
      {
        var cmd = dataPortal.Create(555);
        var result = await dataPortal.ExecuteAsync(cmd);
        Assert.Fail("Expected exception not thrown");
      }
      catch (Exception ex)
      {
        Assert.IsInstanceOfType(ex, typeof(DataPortalException));
      }
    }
    #endregion

    /// <summary>
    /// Create is an exception , if BO does not have DP_Create() overload
    /// with that signature, ends up calling parameterless DP_Create() - this is by design
    /// </summary>
    [TestMethod]
    public async Task BeginCreate_Calling_BO_Without_DP_CREATE_Returns_no_Error_info()
    {
      IDataPortal<CustomerWO_DP_XYZ> dataPortal = _testDIContext.CreateDataPortal<CustomerWO_DP_XYZ>();

      await dataPortal.CreateAsync();
    }

    [TestMethod]
    public async Task BeginFetch_sends_cultureinfo_to_dataportal()
    {
      IDataPortal<AsyncPortalWithCulture> dataPortal = _testDIContext.CreateDataPortal<AsyncPortalWithCulture>();
      
      string expectedCulture = Thread.CurrentThread.CurrentCulture.Name;
      string expectedUICulture = Thread.CurrentThread.CurrentUICulture.Name;

      var command = dataPortal.Create();
      var result = await dataPortal.ExecuteAsync(command);
      Assert.AreEqual(expectedCulture, result.CurrentCulture);
      Assert.AreEqual(expectedUICulture, result.CurrentUICulture);
    }
  }
}