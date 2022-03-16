using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace MvcExample
{
  public class CustomAuthenticationStateProvider : AuthenticationStateProvider
  {
    public override Task<AuthenticationState> GetAuthenticationStateAsync() => 
      Task.FromResult(new AuthenticationState(new System.Security.Claims.ClaimsPrincipal()));
  }
}
