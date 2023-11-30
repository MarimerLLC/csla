using System;
using System.Security.Claims;
using System.Security.Principal;
using Csla;
using Csla.Configuration;
using Csla.Core;
using Csla.Server.Dashboard;
using GraphMergerTest.Business;
using GraphMergerTest.Dal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GraphMergerTest.BusinessTests
{
    public static class TestDIContextFactory
    {
        public static TestDIContext CreateDefaultContext()
        {
            var principal = CreateDefaultClaimsPrincipal();

            return CreateContext(principal);
        }

        public static TestDIContext CreateContext(ClaimsPrincipal principal)
        {
            return CreateContext(null, principal);
        }

        public static TestDIContext CreateContext(Action<CslaOptions> customCslaOptions)
        {
            var principal = CreateDefaultClaimsPrincipal();

            return CreateContext(customCslaOptions, principal);
        }

        public static TestDIContext CreateContext(Action<CslaOptions> customCslaOptions, ClaimsPrincipal principal, bool useDalEf = false)
        {
            var services = new ServiceCollection();

            services.TryAddScoped   <IContextManager, ApplicationContextManager>();
            services.TryAddSingleton<IDashboard,      Dashboard>();

            if (useDalEf)
            {
            }
            else
            {
                services.AddTransient<IWidgetDal,    DalMock.WidgetDal>();
                services.AddTransient<IChildItemDal, DalMock.ChildItemDal>();
            }

            services.AddTransient<Widget.Factory>();

            services.AddCsla(customCslaOptions);

            var serviceProvider = services.BuildServiceProvider();

            var context         = serviceProvider.GetRequiredService<ApplicationContext>();
            context.Principal   = principal;

            return new TestDIContext(serviceProvider);
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
