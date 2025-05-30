//-----------------------------------------------------------------------
// <copyright file="AppContextTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Test to see if contexts get cleared out properly</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Core;
using Csla.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.AppContext
{
  [TestClass]
  public class AppContextTests
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
    public void UseDefaultApplicationContextManager()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var mgrs = services.Where(s => s.ServiceType == typeof(IContextManager));
      Assert.AreEqual(0, mgrs.Count(), "No context manager should be registered");

      var serviceProvider = services.BuildServiceProvider();

      var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
      Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(ApplicationContextManagerAsyncLocal));
    }

    [TestMethod]
    public void UseCustomApplicationContextManager()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      services.AddScoped<Csla.Core.IContextManager, ApplicationContextManagerTls>();
      var serviceProvider = services.BuildServiceProvider();

      var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
      Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(ApplicationContextManagerTls));
    }

    [TestMethod]
    public void SimpleTest()
    {
      IDataPortal<SimpleRoot> dataPortal = _testDIContext.CreateDataPortal<SimpleRoot>();

      // TODO: How do we do this test in Csla 6?
      //ApplicationContext.ClientContext["v1"] = "client";

      dataPortal.Fetch(new SimpleRoot.Criteria("data"));

      //Assert.AreEqual("client", ApplicationContext.ClientContext["v1"], "client context didn't roundtrip");
      Assert.AreEqual("Fetched", TestResults.GetResult("Root"), "global context missing server value");
    }

    /// <summary>
    /// Test the Client Context
    /// </summary>
    /// <remarks>
    /// Clearing the GlobalContext clears the ClientContext also? 
    /// Should the ClientContext be cleared explicitly also?
    /// </remarks>
    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void ClientContext()
    {
      IDataPortal<Basic.Root> dataPortal = _testDIContext.CreateDataPortal<Basic.Root>();
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();

      var testContext = "client context data";
      applicationContext.ClientContext.Add("clientcontext", testContext);
      Assert.AreEqual(testContext, applicationContext.ClientContext["clientcontext"], "Matching data not retrieved");

      Basic.Root root = dataPortal.Create(new Basic.Root.Criteria());
      root.Data = "saved";
      Assert.AreEqual("saved", root.Data, "Root data should be 'saved'");
      Assert.AreEqual(true, root.IsDirty, "Object should be dirty");
      Assert.AreEqual(true, root.IsValid, "Object should be valid");

      TestResults.Reinitialise();
      root = root.Save();

      Assert.IsNotNull(root, "Root object should not be null");
      Assert.AreEqual("Inserted", TestResults.GetResult("Root"), "Object not inserted");
      Assert.AreEqual("saved", root.Data, "Root data should be 'saved'");
      Assert.AreEqual(false, root.IsNew, "Object should not be new");
      Assert.AreEqual(false, root.IsDeleted, "Object should not be deleted");
      Assert.AreEqual(false, root.IsDirty, "Object should not be dirty");

      //TODO: Is there a modern equivalent of this?
      //Assert.AreEqual("client context data", Csla.ApplicationContext.ClientContext["clientcontext"], "Client context data lost");
      Assert.AreEqual("client context data", TestResults.GetResult("clientcontext"), "Global context data lost");
      Assert.AreEqual("new global value", TestResults.GetResult("globalcontext"), "New global value lost");
    }

    /// <summary>
    /// Tests the addition of a key-value pair to the ContextDictionary.
    /// This method verifies that a new key-value pair can be successfully added
    /// and subsequently retrieved from the ContextDictionary, ensuring the dictionary's
    /// add functionality works as expected.
    /// </summary>
    [TestMethod]
    public void ContextDictionaryAdd()
    {
      var contextDectionary = new ContextDictionary();
      string key = "key1";
      string value = "value1";

      contextDectionary.Add(key, value);

      Assert.AreEqual(value, contextDectionary[key]);
    }

    /// <summary>
    /// Tests that adding a duplicate key to the ContextDictionary throws the correct exception.
    /// This method verifies that the ContextDictionary enforces unique keys by attempting to add
    /// a key-value pair where the key already exists in the dictionary. The expected behavior is
    /// that an ArgumentException is thrown, indicating the key's presence and preventing the addition.
    /// </summary>
    [TestMethod]
    public void ContextDictionaryAddThrowsCorrectException()
    {
      var contextDectionary = new ContextDictionary();
      string key = "key1";
      string value = "value1";

      contextDectionary[key] = value;

      Assert.ThrowsException<System.ArgumentException>(() => contextDectionary.Add(key, value));
    }

    /// <summary>
    /// Tests the removal of a key-value pair from the ContextDictionary.
    /// This method verifies that a key-value pair can be successfully removed from the ContextDictionary,
    /// ensuring the dictionary's remove functionality works as expected. It adds a key-value pair, removes it,
    /// and then checks that the key no longer exists in the dictionary.
    /// </summary>
    [TestMethod]
    public void ContextDictionaryRemove()
    {
      var contextDectionary = new ContextDictionary();
      string key = "key1";
      string value = "value1";


      contextDectionary[key] = value;
      contextDectionary.Remove(key);

      Assert.IsFalse(contextDectionary.ContainsKey(key));
    }

    /// <summary>
    /// Tests that attempting to remove a non-existent key from the ContextDictionary throws the correct exception.
    /// This method verifies the dictionary's behavior when a removal operation is attempted for a key that does not exist.
    /// The expected behavior is that a NotSupportedException is thrown, indicating that the operation is not supported
    /// for keys that are not present in the dictionary.
    /// </summary>
    [TestMethod]
    public void ContextDictionaryRemoveThrowsCorrectException()
    {
      var contextDictionary = new ContextDictionary();
      string key = "key1";
      Assert.ThrowsException<System.NotSupportedException>(() => contextDictionary.Remove(key));
    }

    #region FailCreateContext

    /// <summary>
    /// Test the FaileCreate Context
    /// </summary>
    [TestMethod]
    public void FailCreateContext()
    {
      IDataPortal<ExceptionRoot> dataPortal = _testDIContext.CreateDataPortal<ExceptionRoot>();
      
      ExceptionRoot root;
      var ex = Assert.ThrowsException<DataPortalException>(() => 
      {
        root = dataPortal.Create(new ExceptionRoot.Criteria());
      });
      
      Assert.IsNull(ex.BusinessObject, "Business object shouldn't be returned");
      Assert.AreEqual("Fail create", ex.GetBaseException().Message, "Base exception message incorrect");
      Assert.IsTrue(ex.Message.StartsWith("DataPortal.Create failed"), "Exception message incorrect");

      Assert.AreEqual("create", TestResults.GetResult("create"), "GlobalContext not preserved");
    }

    #endregion

    #region FailFetchContext

    [TestMethod]
    public void FailFetchContext()
    {
      IDataPortal<ExceptionRoot> dataPortal = _testDIContext.CreateDataPortal<ExceptionRoot>();
      
      ExceptionRoot root = null;
      var ex = Assert.ThrowsException<DataPortalException>(() => 
      {
        root = dataPortal.Fetch(new ExceptionRoot.Criteria("fail"));
      });
      
      Assert.IsNull(ex.BusinessObject, "Business object shouldn't be returned");
      Assert.AreEqual("Fail fetch", ex.GetBaseException().Message, "Base exception message incorrect");
      Assert.IsTrue(ex.Message.StartsWith("DataPortal.Fetch failed"), "Exception message incorrect");

      Assert.AreEqual("create", TestResults.GetResult("create"), "GlobalContext not preserved");
    }

    #endregion

    [TestMethod]
    public void FailUpdateContext()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(opts => opts.
        DataPortal(cfg => cfg.
          AddServerSideDataPortal(dpc => dpc.
            DataPortalReturnObjectOnException =true)));
      IDataPortal<ExceptionRoot> dataPortal = testDIContext.CreateDataPortal<ExceptionRoot>();
      
      ExceptionRoot root = null;
      
      try
      {
        var ex = Assert.ThrowsException<DataPortalException>(() => 
        {
          root = dataPortal.Create(new ExceptionRoot.Criteria());
        });
          
        root = (ExceptionRoot)ex.BusinessObject;
        Assert.AreEqual("Fail create", ex.GetBaseException().Message, "Base exception message incorrect");
        Assert.IsTrue(ex.Message.StartsWith("DataPortal.Create failed"), "Exception message incorrect");
      }
      finally
      {
      }
    }

    #region FailDeleteContext

    [TestMethod]
    public void FailDeleteContext()
    {
      IDataPortal<ExceptionRoot> dataPortal = _testDIContext.CreateDataPortal<ExceptionRoot>();

      ExceptionRoot root = null;

      var ex = Assert.ThrowsException<DataPortalException>(() => 
      {
        dataPortal.Delete("fail");
      });
      
      root = (ExceptionRoot)ex.BusinessObject;
      Assert.AreEqual("Fail delete", ex.GetBaseException().Message, "Base exception message incorrect");
      Assert.IsTrue(ex.Message.StartsWith("DataPortal.Delete failed"), "Exception message incorrect");
      
      Assert.IsNull(root, "Business object returned");
      Assert.AreEqual("create", TestResults.GetResult("create"), "GlobalContext not preserved");
    }

    #endregion

    private Basic.Root GetRoot(string data)
    {
      IDataPortal<Basic.Root> dataPortal = _testDIContext.CreateDataPortal<Basic.Root>();

      return dataPortal.Fetch(new Basic.Root.Criteria(data));
    }

  }
}
