using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.DataPortalClient;
using UnitDriven;

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class RuleTests : TestBase
  {
    [TestMethod]
    public void CleanupWhenAddBusinessRulesThrowsException()
    {
      RootThrowsException.Counter = 0;
      var context = GetContext();

      // AddBusinessRules throw an ArgumentException
      // In .NET the exception will occur serverside and returned i DatPortalEventArgs
      RootThrowsException.NewRoot((o, e) =>
                                    {
                                      context.Assert.IsNotNull(e.Error);
                                      context.Assert.IsTrue(typeof(DataPortalException) == e.Error.GetType());
                                      context.Assert.IsTrue(typeof(ArgumentException) ==
                                                            e.Error.InnerException.GetType());

                                      context.Assert.Success();
                                    });

      context.Complete();

      // should fail again as type rules should be cleaned up 
      // AddBusinessRules throw an ArgumentException
      RootThrowsException.NewRoot((o, e) =>
                                    {
                                      context.Assert.IsNotNull(e.Error);
                                      context.Assert.IsTrue(typeof(DataPortalException) == e.Error.GetType());
                                      context.Assert.IsTrue(typeof(ArgumentException) ==
                                                            e.Error.InnerException.GetType());

                                      context.Assert.Success();
                                    });

      context.Complete();
    }
  }
}
