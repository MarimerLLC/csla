//-----------------------------------------------------------------------
// <copyright file="AsyncRuleTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This only works on Silverlight because when run through NUnit it is not running</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnitDriven;
using System.Threading.Tasks;
using Csla.TestHelpers;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.ValidationRules
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [TestClass]
  public class AsyncRuleTests : TestBase
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void TestAsyncRulesValid()
    {
      IDataPortal<HasAsyncRule> dataPortal = _testDIContext.CreateDataPortal<HasAsyncRule>();

      UnitTestContext context = GetContext();

      HasAsyncRule har = dataPortal.Create();
      context.Assert.IsTrue(har.IsValid, "IsValid 1");

      har.ValidationComplete += (o, e) =>
      {
        context.Assert.IsTrue(har.IsValid, "IsValid 2");
        context.Assert.Success();
      };
      har.Name = "success";
      context.Complete();
    }

    [TestMethod]
    public void TestAsyncRuleError()
    {
      IDataPortal<HasAsyncRule> dataPortal = _testDIContext.CreateDataPortal<HasAsyncRule>();

      UnitTestContext context = GetContext();

      HasAsyncRule har = dataPortal.Create();
      context.Assert.IsTrue(har.IsValid, "IsValid 1");

      har.ValidationComplete += (o, e) =>
      {
        context.Assert.IsFalse(har.IsValid, "IsValid 2");
        context.Assert.AreEqual(1, har.BrokenRulesCollection.Count);
        context.Assert.Success();
      };
      har.Name = "error";
      context.Complete();
    }

    [TestMethod]
    public void InvalidAsyncRule()
    {
      IDataPortal<HasInvalidAsyncRule> dataPortal = _testDIContext.CreateDataPortal<HasInvalidAsyncRule>();

      TestResults.Reinitialise();

      UnitTestContext context = GetContext();
      var root = dataPortal.Create();
      root.ValidationComplete += (o, e) =>
        {
          context.Assert.IsFalse(root.IsValid);
          context.Assert.AreEqual(1, root.GetBrokenRules().Count);
          context.Assert.AreEqual("Operation is not valid due to the current state of the object.", root.GetBrokenRules()[0].Description);
          context.Assert.Success();
        };
      root.Validate();
      context.Complete();
    }

    [Ignore] // frequently times out on appveyor server
    [TestMethod]
    public void ValidateMultipleObjectsSimultaneously()
    {
      UnitTestContext context = GetContext();

      context.Assert.Try(() =>
        {
          int iterations = 20;
          int completed = 0;
          for (int x = 0; x < iterations; x++)
          {
            HasAsyncRule har = new HasAsyncRule();
            har.ValidationComplete += (o, e) =>
            {
              context.Assert.AreEqual("error", har.Name);
              context.Assert.AreEqual(1, har.BrokenRulesCollection.Count);
              System.Diagnostics.Debug.WriteLine(har.BrokenRulesCollection.Count);
              completed++;
              if (completed == iterations)
                context.Assert.Success();
            };

            // set this to error so we can verify that all 6 rules get run for
            // each object. This is essentially the only way to communicate back
            // with the object except byref properties.
            har.Name = "error";
          }
        });

      context.Complete();
    }


    [TestMethod]
    public void TestAsyncRulesAndSyncRulesValid()
    {
      IDataPortal<AsyncRuleRoot> dataPortal = _testDIContext.CreateDataPortal<AsyncRuleRoot>();

      UnitTestContext context = GetContext();

      var har = dataPortal.Create();
      context.Assert.IsTrue(string.IsNullOrEmpty(har.CustomerNumber));
      context.Assert.IsTrue(string.IsNullOrEmpty(har.CustomerName));
      context.Assert.IsFalse(har.IsValid, "IsValid 1");


      har.ValidationComplete += (o, e) =>
      {
        context.Assert.IsFalse(string.IsNullOrEmpty(har.CustomerNumber));
        context.Assert.IsFalse(string.IsNullOrEmpty(har.CustomerName));

        context.Assert.IsTrue(har.IsValid, "IsValid 2");
        context.Assert.Success();
      };
      har.CustomerNumber = "123456";

      context.Complete();
    }

    [TestMethod]
    public async Task TestAsyncAwaitRule()
    {
      IDataPortal<AsyncRuleRoot> dataPortal = _testDIContext.CreateDataPortal<AsyncRuleRoot>();

      var har = dataPortal.Create();
      var tcs = new TaskCompletionSource<bool>();
      har.ValidationComplete += (o, e) =>
      {
        Assert.AreEqual("abc", har.AsyncAwait, "ends with value");
        tcs.SetResult(true);
      };
      har.AsyncAwait = "123456";
      await tcs.Task;
    }

  }
}