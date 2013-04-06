using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#if !SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Csla.DataPortalClient;
using UnitDriven;

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class RuleTests : TestBase
  {

#if !SILVERLIGHT

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

#endif

#if SILVERLIGHT
    [TestMethod]
    public void CleanupWhenAddBusinessRulesThrowsException()
    {
      RootThrowsException.Counter = 0;
      var context = GetContext();

      Csla.DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy<>).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;


      // AddBusinessRules throw an ArgumentException
      // Exception is thorown in constructor on client side 
      RootThrowsException.NewRoot((o, e) =>
                                    {
                                      context.Assert.IsNotNull(e.Error);
                                      context.Assert.IsTrue(typeof(DataPortalException) == e.Error.GetType());
                                      context.Assert.Success();
                                    });


      context.Complete();

      // should fail again as type rules should be cleaned up 
      // AddBusinessRules throw an ArgumentException
      RootThrowsException.NewRoot((o, e) =>
                                    {
                                      context.Assert.IsNotNull(e.Error);
                                      context.Assert.IsTrue(typeof(DataPortalException) == e.Error.GetType());
                                      context.Assert.Success();
                                    });

      context.Complete();
    }

    [TestMethod]
    public void CleanupWhenAddBusinessRulesThrowsExceptionLocalProxy()
    {
      RootThrowsException.Counter = 0;
      var context = GetContext();

      Csla.DataPortal.ProxyTypeName = "Local";


      // AddBusinessRules throw an ArgumentException
      // Exception is thorown in constructor on client side 
      try
      {
        RootThrowsException.NewRoot((o, e) =>
        {
          context.Assert.Fail("Should never get here when exception is thrown in constructor");
        });
      }
      catch (Exception e)
      {
        context.Assert.IsNotNull(e);
        context.Assert.Success();
      }

      context.Complete();

      try
      {
        // should fail again as type rules should be cleaned up 
        // AddBusinessRules throw an ArgumentException
        RootThrowsException.NewRoot((o, e) =>
        {
          context.Assert.Fail("Should never get here when exception is thrown in constructor");
        });
      }
      catch (Exception e)
      {
        context.Assert.IsNotNull(e);
        context.Assert.Success();
      }

      context.Complete();
    }
#endif
  }
}
