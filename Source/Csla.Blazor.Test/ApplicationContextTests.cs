using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Csla.Configuration;
using Csla.Core;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Csla.Blazor.Test
{
  [TestClass]
  public class ApplicationContextTests
  {
#if NET5_0_OR_GREATER
    [TestMethod]
    public void CorrectManagerChosen()
    {
      IServiceCollection services = new ServiceCollection();
      services.AddHttpContextAccessor();
      services.AddScoped<Csla.AspNetCore.Blazor.ActiveCircuitState>();
      services.AddScoped(typeof(AuthenticationStateProvider), typeof(TestAuthenticationStateProvider));
      services.AddCsla(c => c
        .AddAspNetCore()
        .AddServerSideBlazor()
      );
      IServiceProvider provider = services.BuildServiceProvider();
      var managers = provider.GetRequiredService<IEnumerable<IContextManager>>();
      Assert.IsTrue(managers.Count() == 2);
      var blazorMgr = (Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor)managers.FirstOrDefault(mgr => mgr is Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor);
      var httpMgr = (Csla.AspNetCore.ApplicationContextManagerHttpContext)managers.FirstOrDefault(mgr => mgr is Csla.AspNetCore.ApplicationContextManagerHttpContext);
      Assert.IsNotNull(blazorMgr);
      Assert.IsTrue(blazorMgr.IsStatefulContext);
      Assert.IsFalse(blazorMgr.IsValid); //because no circuit exists


      Assert.IsNotNull(httpMgr);
      Assert.IsFalse(httpMgr.IsStatefulContext);
      Assert.IsFalse(httpMgr.IsValid); //because no httpcontext exists

      //in this scenario the appliclication context should choose the asynclocal as the 
      var appContext = provider.GetRequiredService<ApplicationContext>();
      Assert.IsNotNull(appContext);
      Assert.IsTrue(appContext.ContextManager is Csla.Core.ApplicationContextManagerAsyncLocal);

    }
#endif
  }

  public class TestAuthenticationStateProvider : AuthenticationStateProvider
  {
    public override Task<AuthenticationState> GetAuthenticationStateAsync() 
      => Task.FromResult<AuthenticationState>(null);
  }
}
