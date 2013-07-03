using System;
using System.Linq;
using AsyncLookupRule;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationRules.Test
{
  [TestClass()]
  public class RootTest : IntegrationRuleTest<Root>
  {
    [TestMethod()]
    public void TestSyncRule()
    {
      // using default constructor
      // call CheckRules
      CheckRules();
      // wait for sync and async rules to complete
      Assert.IsTrue(BusinessObject.BrokenRulesCollection.Any(p => p.Property == Root.NameProperty.Name));
    }

    [TestMethod()]
    public void TestAsyncRule()
    {
      // for async rules that are only allowed to run when a property is changed
      RulesCompletedWaitHandle.Reset();
      BusinessObject.CustomerId = 10;
      // wait for sync and async rules to complete
      RulesCompletedWaitHandle.WaitOne(1000);
      Assert.IsFalse(BusinessObject.BrokenRulesCollection.Any(p => p.Property == Root.NameProperty.Name));
    }
  }
}
