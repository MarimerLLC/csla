//-----------------------------------------------------------------------
// <copyright file="AsyncRuleTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This only works on Silverlight because when run through NUnit it is not running</summary>
//-----------------------------------------------------------------------

using Csla.Rules;
using Csla.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [TestClass]
  public class AsyncRuleTests
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
    public void TestAsyncRulesValid()
    {
      IDataPortal<HasAsyncRule> dataPortal = _testDIContext.CreateDataPortal<HasAsyncRule>();

      HasAsyncRule har = dataPortal.Create();
      Assert.IsTrue(har.IsValid, "IsValid 1");

      har.ValidationComplete += (_, _) =>
      {
        Assert.IsTrue(har.IsValid, "IsValid 2");
      };
      har.Name = "success";
    }

    [TestMethod]
    public void TestAsyncRuleError()
    {
      IDataPortal<HasAsyncRule> dataPortal = _testDIContext.CreateDataPortal<HasAsyncRule>();

      HasAsyncRule har = dataPortal.Create();
      Assert.IsTrue(har.IsValid, "IsValid 1");

      har.ValidationComplete += (_, _) =>
      {
        Assert.IsFalse(har.IsValid, "IsValid 2");
        Assert.AreEqual(1, har.BrokenRulesCollection.Count);
      };
      har.Name = "error";
    }

    [TestMethod]
    public void InvalidAsyncRule()
    {
      IDataPortal<HasInvalidAsyncRule> dataPortal = _testDIContext.CreateDataPortal<HasInvalidAsyncRule>();

      var root = dataPortal.Create();
      root.ValidationComplete += (_, _) =>
        {
          Assert.IsFalse(root.IsValid);
          int actual = root.GetBrokenRules().Count;
          Assert.AreEqual(1, actual);
          string actual1 = root.GetBrokenRules()[0].Description;
          Assert.AreEqual("Operation is not valid due to the current state of the object.", actual1);
        };
      root.Validate();
    }

    [Ignore] // frequently times out on appveyor server
    [TestMethod]
    public void ValidateMultipleObjectsSimultaneously()
    {
      int iterations = 20;
      int completed = 0;
      for (int x = 0; x < iterations; x++)
      {
        HasAsyncRule har = new HasAsyncRule();
        har.ValidationComplete += (_, _) =>
        {
          Assert.AreEqual("error", har.Name);
          Assert.AreEqual(1, har.BrokenRulesCollection.Count);
          System.Diagnostics.Debug.WriteLine(har.BrokenRulesCollection.Count);
          completed++;
          Assert.AreEqual(completed, iterations);
        };

        // set this to error so we can verify that all 6 rules get run for
        // each object. This is essentially the only way to communicate back
        // with the object except byref properties.
        har.Name = "error";
      }
    }

    [TestMethod]
    public void TestAsyncRulesAndSyncRulesValid()
    {
      IDataPortal<AsyncRuleRoot> dataPortal = _testDIContext.CreateDataPortal<AsyncRuleRoot>();

      var har = dataPortal.Create();
      Assert.IsTrue(string.IsNullOrEmpty(har.CustomerNumber));
      Assert.IsTrue(string.IsNullOrEmpty(har.CustomerName));
      Assert.IsFalse(har.IsValid, "IsValid 1");

      har.ValidationComplete += (_, _) =>
      {
        Assert.IsFalse(string.IsNullOrEmpty(har.CustomerNumber));
        Assert.IsFalse(string.IsNullOrEmpty(har.CustomerName));

        Assert.IsTrue(har.IsValid, "IsValid 2");
      };
      har.CustomerNumber = "123456";
    }

    [TestMethod]
    public async Task TestAsyncAwaitRule()
    {
      IDataPortal<AsyncRuleRoot> dataPortal = _testDIContext.CreateDataPortal<AsyncRuleRoot>();

      var har = dataPortal.Create();
      var tcs = new TaskCompletionSource<bool>();
      har.ValidationComplete += (_, _) =>
      {
        Assert.AreEqual("abc", har.AsyncAwait, "ends with value");
        tcs.SetResult(true);
      };
      har.AsyncAwait = "123456";
      await tcs.Task;
    }


    [TestMethod]
    public async Task MyTestMethod()
    {
      IDataPortal<AsyncRuleRoot> dataPortal = _testDIContext.CreateDataPortal<AsyncRuleRoot>();

      var har = dataPortal.Create("SomeRandomText");
      await har.WaitForIdle();

      var affectedProperties = await har.CheckRulesForPropertyAsyncAwait();
      using (new AssertionScope())
      {
        har.AsyncAwait.Should().Be("abc");
        affectedProperties.Should().ContainSingle().Which.Should().Be(nameof(AsyncRuleRoot.AsyncAwait));
      }
    }

    [TestMethod($"When an async rule throws an exception the framework must invoke {nameof(IUnhandledAsyncRuleExceptionHandler)}.{nameof(IUnhandledAsyncRuleExceptionHandler.CanHandle)}.")]
    public async Task AsyncRuleException_Testcase01()
    {
      var diContext = CreateDIContextForAsyncRuleExceptions();

      var unhandledExceptionHandler = (TestUnhandledAsyncRuleExceptionHandler)diContext.ServiceProvider.GetRequiredService<IUnhandledAsyncRuleExceptionHandler>();

      var cp = diContext.CreateDataPortal<DelayedAsynRuleExceptionRoot>();
      var bo = await cp.CreateAsync();

      var tcs = new TaskCompletionSource();
      unhandledExceptionHandler.CanHandleInspector = (_, _) => tcs.SetResult();

      await ForceThreadSwitch(bo, TimeSpan.FromMilliseconds(25));

      await tcs.Task.WaitAsync(TimeSpan.FromMilliseconds(150));
    }

    [TestMethod($"When an async rules exception can be handled by {nameof(IUnhandledAsyncRuleExceptionHandler)} it must invoke {nameof(IUnhandledAsyncRuleExceptionHandler)}.{nameof(IUnhandledAsyncRuleExceptionHandler.Handle)}.")]
    public async Task AsyncRuleException_Testcase02()
    {
      var diContext = CreateDIContextForAsyncRuleExceptions();

      var unhandledExceptionHandler = (TestUnhandledAsyncRuleExceptionHandler)diContext.ServiceProvider.GetRequiredService<IUnhandledAsyncRuleExceptionHandler>();

      var cp = diContext.CreateDataPortal<DelayedAsynRuleExceptionRoot>();
      var bo = await cp.CreateAsync();

      bool canHandleInvoked = false;
      unhandledExceptionHandler.HandleInspector = (_, _, _) => canHandleInvoked = true;

      await ForceThreadSwitch(bo, TimeSpan.FromMilliseconds(25));

      await Task.Delay(TimeSpan.FromMilliseconds(150));

      canHandleInvoked.Should().BeTrue();
    }

    [TestMethod($"When the default {nameof(IUnhandledAsyncRuleExceptionHandler)} is used the exception must be handled by a global unhandled exception handler (for this test it's the {nameof(AppDomain)}.{nameof(AppDomain.CurrentDomain)}.{nameof(AppDomain.CurrentDomain.UnhandledException)} event).")]
    public async Task AsyncRuleException_Testcase03()
    {
      var diContext = CreateDIContextForAsyncRuleExceptions();

      bool unobservedTaskExceptionFound = false;
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      try
      {
        var unhandledExceptionHandler = (TestUnhandledAsyncRuleExceptionHandler)diContext.ServiceProvider.GetRequiredService<IUnhandledAsyncRuleExceptionHandler>();
        unhandledExceptionHandler.CanHandleResult = false;
        var cp = diContext.CreateDataPortal<DelayedAsynRuleExceptionRoot>();
        var bo = await cp.CreateAsync();

        await ForceThreadSwitch(bo, TimeSpan.FromMilliseconds(25));

        await Task.Delay(TimeSpan.FromMilliseconds(150));

        unobservedTaskExceptionFound.Should().BeTrue();
      }
      finally
      {
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
      }

      void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) => unobservedTaskExceptionFound = true;
    }


    private static TestDIContext CreateDIContextForAsyncRuleExceptions() => TestDIContextFactory.CreateDefaultContext(servics => servics.AddSingleton<IUnhandledAsyncRuleExceptionHandler, TestUnhandledAsyncRuleExceptionHandler>());
    private static async Task ForceThreadSwitch(DelayedAsynRuleExceptionRoot root, TimeSpan exceptionDelay)
    {
      await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
      root.ExceptionDelay = exceptionDelay;
    }
  }
}