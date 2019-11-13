using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace Csla.Blazor
{
  public class CslaAuthenticationStateProvider : AuthenticationStateProvider
  {
    HttpContext context;

    public CslaAuthenticationStateProvider(IHttpContextAccessor ctx)
    {
      context = ctx.HttpContext;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      //ProjectTracker.Library.Security.PTPrincipal.Load("manager");
      return Task.FromResult(new AuthenticationState((ClaimsPrincipal)Csla.ApplicationContext.User));
    }
  }
}
