using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using UnitDriven;

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
    [TestMethod]
    public void TestAsyncRulesValid()
    {
      UnitTestContext context = GetContext();

      HasAsyncRule har = new HasAsyncRule();
      har.ValidationComplete += (o, e) =>
      {
        context.Assert.IsTrue(har.IsValid);
        context.Assert.IsTrue(har.IsSavable);
        context.Assert.Success();
      };

      har.Name = "success";
      context.Assert.IsTrue(har.IsValid);
      context.Assert.IsFalse(har.IsSavable);

      har.Reset.Set();
      context.Complete();
    }
    [TestMethod]
    public void TestAsyncRuleError()
    {
      UnitTestContext context = GetContext();

      HasAsyncRule har = new HasAsyncRule();
      har.ValidationComplete += (o, e) =>
      {
        context.Assert.IsFalse(har.IsValid);
        context.Assert.IsFalse(har.IsSavable);
        context.Assert.AreEqual(6, har.BrokenRulesCollection.Count);
        context.Assert.Success();
      };

      har.Name = "error";
      context.Assert.IsTrue(har.IsValid);
      context.Assert.IsFalse(har.IsSavable);

      har.Reset.Set();
      context.Complete();
    }

    [TestMethod]
    public void VerifyIsValidAndIsSavableChangedWhenAsyncRuleCompletes()
    {
      UnitTestContext context = GetContext();

      bool isValidChanged = false;
      bool isSavableChanged = false;

      HasAsyncRule har = new HasAsyncRule();
      har.ValidationComplete += (o, e) =>
      {
        context.Assert.IsTrue(isValidChanged);
        context.Assert.IsTrue(isSavableChanged);
        context.Assert.Success();
      };

      har.PropertyChanged += (o, e) =>
      {
        if (har.IsValid)
          isValidChanged = har.IsValid;
        
        if (har.IsSavable)
          isSavableChanged = har.IsSavable;
      };
      har.Name = "success";

      har.Reset.Set();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void InvalidAsyncRule()
    {
      UnitTestContext context = GetContext();

      context.Assert.Try(() => 
      {
        try
        {
          new HasInvalidAsyncRule();
        }
        catch (TypeInitializationException ex)
        {
          throw ex.InnerException;
        }
      });
      context.Assert.Fail();
      context.Complete();
    }

    [TestMethod]
    public void ValidateMultipleObjectsSimultaneously()
    {
      UnitTestContext context = GetContext();

      int iterations = 100;
      int completed = 0;
      for (int x = 0; x < iterations; x++)
      {
        HasAsyncRule har = new HasAsyncRule();
        har.ValidationComplete += (o, e) =>
        {
          context.Assert.AreEqual("error", har.Name);
          context.Assert.AreEqual(6, har.BrokenRulesCollection.Count);
          completed++;
          if (completed == iterations)
            context.Assert.Success();
        };

        // set this to error so we can verify that all 6 rules get run for
        // each object. This is essentially the only way to communicate back
        // with the object except byref properties.
        har.Name = "error";
        har.Reset.Set();
      }

      context.Complete();
    }

#if SILVERLIGHT
    /// <summary>
    /// This only works on Silverlight because when run through NUnit it is not running
    /// in the UI thread, thus the Dispatcher will not behave as expected. I'm not sure how to 
    /// simulate the UI thread in NUnit but maybe we can assume it will work since this is the
    /// expected behavior of BackgroundWorker and this test passes in Silverlight. Developers
    /// will be responsible for Dispatching back to the UI thread by either getting the current
    /// thread dispatcher or using a BackgroundWorker or, in silverlight, making a WCF service
    /// call will do this automatically.
    /// </summary>
    [TestMethod]
    public void AsyncCompleteVerifyUIThread()
    {
      UnitTestContext context = GetContext();
      Thread expected = Thread.CurrentThread;

      HasAsyncRule har = new HasAsyncRule();
      har.ValidationComplete += (o, e) =>
      {
        Thread actual = Thread.CurrentThread;
        context.Assert.AreSame(expected, actual);
        context.Assert.Success();
      };

      har.Name = "success";
      har.Reset.Set();
      context.Complete();
    }
#endif
  }
}
