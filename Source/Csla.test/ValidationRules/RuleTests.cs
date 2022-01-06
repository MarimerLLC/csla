using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.DataPortalClient;
using UnitDriven;
using System.Threading.Tasks;
using Csla.TestHelpers;

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class RuleTests : TestBase
  {
    private TestDIContext _testDIContext;

    [TestInitialize]
    public void TestInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }
    
    [TestMethod]
    public async Task CleanupWhenAddBusinessRulesThrowsException()
    {
      IDataPortal<RootThrowsException> dataPortal = _testDIContext.CreateDataPortal<RootThrowsException>();

      RootThrowsException.Counter = 0;

      // AddBusinessRules throw an ArgumentException
      // In .NET the exception will occur serverside and returned i DatPortalEventArgs
      try
      {
        await dataPortal.CreateAsync();
      }
      catch (DataPortalException ex)
      {
        Assert.IsTrue(ex.InnerException is ArgumentException);
      }

      // should fail again as type rules should be cleaned up 
      // AddBusinessRules throw an ArgumentException
      try
      {
        await dataPortal.CreateAsync();
      }
      catch (DataPortalException ex)
      {
        Assert.IsTrue(ex.InnerException is ArgumentException);
      }
    }
  }
}
