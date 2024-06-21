//-----------------------------------------------------------------------
// <copyright file="ClaimsPrincipalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Configuration;
using Csla.Serialization.Mobile;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class ClaimsPrincipalTests
  {
    [TestInitialize]
    public void TestInitialize()
    {
      IServiceProvider serviceProvider;
      var services = new ServiceCollection();
      services.TryAddSingleton<Core.IContextManager, ApplicationContextManagerUnitTests>();
      services.TryAddSingleton<Csla.Server.Dashboard.IDashboard, Csla.Server.Dashboard.Dashboard>();
      services.AddCsla();

      serviceProvider = services.BuildServiceProvider();
      ApplicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    }

    private ApplicationContext ApplicationContext { get; set; }

    [TestMethod]
    public void CloneClaimsPrincipal()
    {
      var i = new ClaimsIdentity();
      i.AddClaim(new Claim("name", "Franklin"));
      var p = new ClaimsPrincipal(i);
      var p1 = (ClaimsPrincipal)Core.ObjectCloner.GetInstance(ApplicationContext).Clone(p);
      Assert.AreNotSame(p, p1, "Should be different instances");
      Assert.AreEqual(p.Claims.Count(), p1.Claims.Count(), "Should have same number of claims");
      var c = p1.Claims.Where(r => r.Type == "name").First();
      Assert.AreEqual("Franklin", c.Value, "Claim value should match");
    }
  }
}
