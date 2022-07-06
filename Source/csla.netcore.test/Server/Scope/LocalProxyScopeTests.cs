//-----------------------------------------------------------------------
// <copyright file="LocalProxyScopeTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using System.IO;
using Csla.Runtime;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 
using Csla;

namespace Csla.Test.Server.Scope
{
  [TestClass]
  public class LocalProxyScopeTests
  {
    [TestMethod]
    public void UsingLocalProxy()
    {
      // set up DI
      var services = new ServiceCollection();
      services.AddCsla();
      services.AddScoped<GuidProvider>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      // test code
      var gp = provider.GetRequiredService<GuidProvider>();
      var proxy = applicationContext.CurrentServiceProvider.GetRequiredService<Csla.DataPortalClient.IDataPortalProxy>();
      var options = applicationContext.CurrentServiceProvider.GetRequiredService<Csla.Channels.Local.LocalProxyOptions>();
      Assert.IsNotNull(applicationContext);
      Assert.IsInstanceOfType(proxy, typeof(Channels.Local.LocalProxy));
      Assert.IsTrue(options.UseLocalScope, "UseLocalScope");

      var dp = provider.GetRequiredService<IDataPortal<Root>>();
      var obj = dp.Create();
      Assert.AreNotEqual(gp.Guid, obj.Guid, "Guids should not match");
      Assert.IsTrue(obj.GuidProvider.Disposed, "provider should be disposed");
      Assert.IsFalse(gp.Disposed, "provider should not be disposed");
    }
  }
}
