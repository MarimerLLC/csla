using System.Security.Claims;
using System.Security.Principal;
using Csla.Testing;
using GraphMergerTest.Business;
using GraphMergerTest.Dal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphMergerTest.BusinessTests
{
  [TestClass]
  public class TestBase
  {
    public static CslaTestHost TestHost { get; private set; }

    public static Widget.Factory WidgetFactory { get; private set; }

    public static void TestBaseClassInitialize()
    {
      TestHost = CslaTestHost.Create(t => t
        .AsPrincipal(CreateDefaultClaimsPrincipal())
        .ConfigureServices(services =>
        {
          services.AddTransient<IWidgetDal, DalMock.WidgetDal>();
          services.AddTransient<IChildItemDal, DalMock.ChildItemDal>();
          services.AddTransient<Widget.Factory>();
        }));

      WidgetFactory = TestHost.Services.GetRequiredService<Widget.Factory>();
    }

    public static void TestBaseClassCleanup()
    {
      TestHost?.Dispose();
      TestHost = null;
      WidgetFactory = null;
    }

    private static ClaimsPrincipal CreateDefaultClaimsPrincipal()
    {
      var identity = new ClaimsIdentity(new GenericIdentity("Admin"));

      identity.AddClaim(new Claim("Id", Guid.NewGuid().ToString()));
      identity.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));

      return new ClaimsPrincipal(identity);
    }
  }
}
