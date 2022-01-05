//-----------------------------------------------------------------------
// <copyright file="AsynchDataPortalTest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Create is an exception , if BO does not have DP_Create() overload</summary>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Linq;
using Csla.Test.Basic;
using System.Threading.Tasks;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

using System.Threading;
using System.Globalization;
using UnitDriven;
using Csla.Testing.Business.DataPortal;
using Single = Csla.Test.DataPortalTest.Single;
using Csla.Test.DataPortalTest;
using System;
using Csla.TestHelpers;
using cslalighttest.CslaDataProvider;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class AsynchDataPortalTest : TestBase
  {
    private CultureInfo CurrentCulture;
    private CultureInfo CurrentUICulture;

    [TestInitialize]
    public void Setup()
    {
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
      IDataPortal<Single> dataPortal = DataPortalFactory.CreateDataPortal<Single>();

      var created = await dataPortal.CreateAsync();
      Assert.AreEqual(created.Id, 0);
    }

    [TestMethod]

    public async Task BeginCreate_overload_with_Criteria_passed_and_Id_set()
    {
      IDataPortal<Single> dataPortal = DataPortalFactory.CreateDataPortal<Single>();

      var created = await dataPortal.CreateAsync(100);
      Assert.AreEqual(created.Id, 100);
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.DataPortalException))]
    
    public async Task BeginCreate_with_exception()
    {
      IDataPortal<Single> dataPortal = DataPortalFactory.CreateDataPortal<Single>();

      await dataPortal.CreateAsync(9999);
    }

    [TestMethod]
    
    public async Task CreateAsync_NoCriteria()
    {
      IDataPortal<Single> dataPortal = DataPortalFactory.CreateDataPortal<Single>();

      var result = await dataPortal.CreateAsync();
      Assert.IsNotNull(result);
      Assert.AreEqual(0, result.Id);
    }

    [TestMethod]
    
    public async Task CreateAsync_WithCriteria()
    {
      IDataPortal<Single2> dataPortal = DataPortalFactory.CreateDataPortal<Single2>();

      var result = await dataPortal.CreateAsync(123);
      Assert.IsNotNull(result);
      Assert.AreEqual(123, result.Id);
    }


    [TestMethod]
    
    public void CreateAsync_WithException()
    {
      var lck = new AutoResetEvent(false);
      new Action(async () =>
      {
        IDataPortal<Single2> dataPortal = DataPortalFactory.CreateDataPortal<Single2>();
        
        try
        {
          var result = await dataPortal.CreateAsync(9999);
          Assert.Fail("Expected exception not thrown");
        }
        catch (Exception ex)
        {
          Assert.IsInstanceOfType(ex, typeof(Csla.DataPortalException));
        }
        finally
        {
          lck.Set();
        }
      }).Invoke();
      lck.WaitOne();
    }

    [TestMethod]
    [Timeout(1000)]
    
    public async Task CreateAsync_Parrallel()
    {
      IDataPortal<SingleWithFactory> dataPortal = DataPortalFactory.CreateDataPortal<SingleWithFactory>();
      
      var list = new List<int>(500);
      for (var i = 0; i < 500; i++)
      {
        list.Add(i);
      }

      var tasks = list.AsParallel().Select(x => dataPortal.CreateAsync());
      await Task.WhenAll(tasks);
    }

    #endregion

    #region Fetch

    [TestMethod]
    
    public async Task FetchAsync_NoCriteria()
    {
      IDataPortal<Single2> dataPortal = DataPortalFactory.CreateDataPortal<Single2>();

      var result = await dataPortal.FetchAsync();
      Assert.IsNotNull(result);
      Assert.AreEqual(0, result.Id);
    }

    [TestMethod]
    
    public async Task FetchAsync_WithCriteria()
    {
      IDataPortal<Single2> dataPortal = DataPortalFactory.CreateDataPortal<Single2>();

      var result = await dataPortal.FetchAsync(123);
      Assert.IsNotNull(result);
      Assert.AreEqual(123, result.Id);
    }


    [TestMethod]
    
    public void FetchAsync_WithException()
    {
      var lck = new AutoResetEvent(false);
      new Action(async () =>
      {
        IDataPortal<Single2> dataPortal = DataPortalFactory.CreateDataPortal<Single2>();

        try
        {
          var result = await dataPortal.FetchAsync(9999);
          Assert.Fail("Expected exception not thrown");
        }
        catch (Exception ex)
        {
          Assert.IsInstanceOfType(ex, typeof(Csla.DataPortalException));
        }
        finally
        {
          lck.Set();
        }
      }).Invoke();
      lck.WaitOne();
    }

    [TestMethod]
    [Timeout(1000)]
    
    public async Task FetchAsync_Parrallel()
    {
      IDataPortal<SingleWithFactory> dataPortal = DataPortalFactory.CreateDataPortal<SingleWithFactory>();

      var list = new List<int>(500);
      for (var i = 0; i < 500; i++)
      {
        list.Add(i);
      }

      var tasks = list.AsParallel().Select(x => dataPortal.FetchAsync());
      await Task.WhenAll(tasks);
    }

    #endregion

#if DEBUG

    [TestMethod]
    
    public async Task SaveAsync()
    {
      IDataPortal<Single2> dataPortal = DataPortalFactory.CreateDataPortal<Single2>();

      var result = await dataPortal.CreateAsync();
      Assert.IsNotNull(result);
      Assert.AreEqual(0, result.Id);
      Assert.IsTrue(result.IsNew);
      Assert.IsTrue(result.IsDirty);
      result = await result.SaveAsync();
      Assert.IsFalse(result.IsNew);
      Assert.IsFalse(result.IsDirty);
    }

    [TestMethod]
    
    public void SaveAsyncWithException()
    {
      var context = GetContext();
      context.Assert.Try(async () =>
      {
        IDataPortal<Single2> dataPortal = DataPortalFactory.CreateDataPortal<Single2>();

        var result = await dataPortal.CreateAsync(555);
        Assert.IsNotNull(result);
        Assert.AreEqual(555, result.Id);
        Assert.IsTrue(result.IsNew);
        Assert.IsTrue(result.IsDirty);
        var lck = new AutoResetEvent(false);
        new Action(async () =>
        {
          try
          {
            result = await result.SaveAsync();
            Assert.Fail("Expected exception not thrown");
          }
          catch (Exception ex)
          {
            context.Assert.IsTrue(ex.GetType() == typeof(Csla.DataPortalException));
          }
          finally
          {
            lck.Set();
          }
        }).Invoke();
        lck.WaitOne();
        context.Assert.Success();
      });
      context.Complete();
    }

#endif

    #region ExecuteCommand

    [TestMethod]
    
    public async Task ExecuteCommand_called_without_UserState_results_in_UserState_defaulted_to_Null_server()
    {
      IDataPortal<CommandObject> dataPortal = DataPortalFactory.CreateDataPortal<CommandObject>();

      var command = new CommandObject();
      var result = await dataPortal.ExecuteAsync(command);
      Assert.AreEqual("Executed", result.AProperty);
    }

    [TestMethod]
    
    public async Task ExecuteAsync()
    {
      IDataPortal<SingleCommand> dataPortal = DataPortalFactory.CreateDataPortal<SingleCommand>();

      var result = await dataPortal.ExecuteAsync(
        new SingleCommand { Value = 123 });
      Assert.IsNotNull(result);
      Assert.AreEqual(124, result.Value);
    }

    [TestMethod]
    
    public void ExecuteAsyncWithException()
    {
      var lck = new AutoResetEvent(false);
      new Action(async () =>
      {
        IDataPortal<SingleCommand> dataPortal = DataPortalFactory.CreateDataPortal<SingleCommand>();

        try
        {
          var result = await dataPortal.ExecuteAsync(
            new SingleCommand { Value = 555 });
          Assert.Fail("Expected exception not thrown");
        }
        catch (Exception ex)
        {
          Assert.IsInstanceOfType(ex, typeof(Csla.DataPortalException));
        }
        finally
        {
          lck.Set();
        }
      }).Invoke();
      lck.WaitOne();
    }
    #endregion

    /// <summary>
    /// Create is an exception , if BO does not have DP_Create() overload
    /// with that signature, ends up calling parameterless DP_Create() - this is by design
    /// </summary>
    [TestMethod]
    
    public async Task BeginCreate_Calling_BO_Without_DP_CREATE_Returns_no_Error_info()
    {
      IDataPortal<CustomerWO_DP_XYZ> dataPortal = DataPortalFactory.CreateDataPortal<CustomerWO_DP_XYZ>();

      await dataPortal.CreateAsync();
    }

    [TestMethod]

    public async Task BeginFetch_sends_cultureinfo_to_dataportal()
    {
      IDataPortal<AsyncPortalWithCulture> dataPortal = DataPortalFactory.CreateDataPortal<AsyncPortalWithCulture>();
      
      string expectedCulture = Thread.CurrentThread.CurrentCulture.Name;
      string expectedUICulture = Thread.CurrentThread.CurrentUICulture.Name;

      var command = new AsyncPortalWithCulture();
      var result = await dataPortal.ExecuteAsync(command);
      Assert.AreEqual(expectedCulture, result.CurrentCulture);
      Assert.AreEqual(expectedUICulture, result.CurrentUICulture);
    }
  }
}