//-----------------------------------------------------------------------
// <copyright file="AppContextTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests to see if correct app context is used</summary>
//-----------------------------------------------------------------------

using System.Linq;
using Csla.Configuration;
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Tests.AspNet
{
  [TestClass]
  public class AppContextTests
  {
    [TestMethod]
    public void UseAspnetContextManager()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .AddAspNet());
      var mgrs = services.Where(s => s.ServiceType == typeof(IContextManager));
      Assert.AreEqual(1, mgrs.Count(), "One context manager should be registered");

      var serviceProvider = services.BuildServiceProvider();

      var webManager = serviceProvider.GetServices<IContextManager>().FirstOrDefault();
      Assert.IsInstanceOfType(webManager, typeof(Csla.Web.ApplicationContextManager));
      Assert.IsFalse(webManager.IsValid, "Context manager should not be valid");

      var applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
      Assert.IsInstanceOfType(applicationContext.ContextManager, typeof(ApplicationContextManagerAsyncLocal));
    }
  }
}
