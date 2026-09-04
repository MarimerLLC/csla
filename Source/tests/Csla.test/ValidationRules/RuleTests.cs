using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Testing;

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class RuleTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestMethod]
    public async Task CleanupWhenAddBusinessRulesThrowsException()
    {
      IDataPortal<RootThrowsException> dataPortal = _testHost.GetDataPortal<RootThrowsException>();

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
