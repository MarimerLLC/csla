using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Csla.Configuration;
using Csla.Core;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

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
        .AddServerSideBlazor());
      IServiceProvider provider = services.BuildServiceProvider();
      var manager = provider.GetRequiredService<IContextManager>();
      Assert.IsInstanceOfType(manager, typeof(Csla.AspNetCore.ApplicationContextManager));
    }
#endif
  }

  public class TestAuthenticationStateProvider : AuthenticationStateProvider
  {
    public override Task<AuthenticationState> GetAuthenticationStateAsync() 
      => Task.FromResult<AuthenticationState>(null);
  }
}
