using System.Reflection;
using System.Security.Claims;
using Csla;
using Csla.Configuration;
using Csla.Core;
using Csla.Serialization.Mobile;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphMergerTest.BusinessTests
{
  /// <summary>
  /// This is the only test project that also runs on .NET Framework, where
  /// Csla.dll has its own ClaimsPrincipal serializer. Checks that the
  /// generated registration makes it the default there, too.
  /// </summary>
  [TestClass]
  public class ClaimsPrincipalSerializationTests
  {
    [TestMethod]
    public void CslaAssemblyRegistersClaimsPrincipalSerializer()
    {
      var registration = typeof(MobileFormatter).Assembly
        .GetCustomAttributes<MobileSerializerRegistrationAttribute>()
        .SingleOrDefault(r => r.Type == typeof(ClaimsPrincipal));

      Assert.IsNotNull(registration);
      Assert.AreEqual("ClaimsPrincipalSerializer", registration.SerializerType.Name);
    }

    [TestMethod]
    public void ClaimsPrincipalRoundTripsWithDefaultConfiguration()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o.Serialization(s => s.UseMobileFormatter()));
      services.AddScoped<IContextManager, ApplicationContextManagerAsyncLocal>();
      using var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "Admin"), new Claim(ClaimTypes.Role, "Administrator")], "test"));

      var clone = (ClaimsPrincipal)new ObjectCloner(applicationContext).Clone(principal);

      Assert.AreEqual("Admin", clone.Identity.Name);
      Assert.IsTrue(clone.IsInRole("Administrator"));
    }
  }
}
