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
      List<TypeMap> customSerializers = [
        new TypeMap { OriginalType = typeof(ClaimsPrincipal),
                      SerializerType = typeof(ClaimsPrincipalFormatter) }
      ];
      services.AddCsla(o => o
        .Serialization(o => o
          .AddMobileFormatter(o => o.CustomSerializers.AddRange(customSerializers))));

      serviceProvider = services.BuildServiceProvider();
      applicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
    }

    private ApplicationContext applicationContext { get; set; }

    [TestMethod]
    public void CloneClaimsPrincipal()
    {
      var i = new ClaimsIdentity();
      i.AddClaim(new Claim("name", "Franklin"));
      var p = new ClaimsPrincipal(i);
      var p1 = (ClaimsPrincipal)Core.ObjectCloner.GetInstance(applicationContext).Clone(p);
      Assert.AreNotSame(p, p1, "Should be different instances");
      Assert.AreEqual(p.Claims.Count(), p1.Claims.Count(), "Should have same number of claims");
      var c = p1.Claims.Where(r => r.Type == "name").First();
      Assert.AreEqual("Franklin", c.Value, "Claim value should match");
    }
  }

  public class ClaimsPrincipalFormatter : IMobileSerializer
  {
    ApplicationContext Csla.Core.IUseApplicationContext.ApplicationContext { get; set; }

    public object Deserialize(SerializationInfo info)
    {
      var state = info.GetValue<byte[]>("s");
      using var buffer = new MemoryStream(state);
      using var reader = new BinaryReader(buffer);
      var mobile = new ClaimsPrincipal(reader);
      return mobile;
    }

    public void Serialize(object obj, SerializationInfo info)
    {
      if (obj is not ClaimsPrincipal principal)
        throw new ArgumentException("obj.GetType() != ClaimsPrincipal", nameof(obj));

      using var buffer = new MemoryStream();
      using var writer = new BinaryWriter(buffer);
      principal.WriteTo(writer);
      info.AddValue("s", buffer.ToArray());
    }
  }
}
