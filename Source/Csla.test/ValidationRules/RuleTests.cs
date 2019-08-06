using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.DataPortalClient;
using UnitDriven;
using System.Threading.Tasks;

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class RuleTests : TestBase
  {
    [TestMethod]
    public async Task CleanupWhenAddBusinessRulesThrowsException()
    {
      RootThrowsException.Counter = 0;

      // AddBusinessRules throw an ArgumentException
      // In .NET the exception will occur serverside and returned i DatPortalEventArgs
      try
      {
        await Csla.DataPortal.CreateAsync<RootThrowsException>();
      }
      catch (DataPortalException ex)
      {
        Assert.IsTrue(ex.InnerException is ArgumentException);
      }

      // should fail again as type rules should be cleaned up 
      // AddBusinessRules throw an ArgumentException
      try
      {
        await Csla.DataPortal.CreateAsync<RootThrowsException>();
      }
      catch (DataPortalException ex)
      {
        Assert.IsTrue(ex.InnerException is ArgumentException);
      }
    }
  }
}
