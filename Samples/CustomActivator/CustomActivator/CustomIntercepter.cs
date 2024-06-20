using System;
using System.Threading.Tasks;

namespace CustomActivator
{
  public class CustomIntercepter : Csla.Server.IInterceptDataPortal
  {
    public void Complete(Csla.Server.InterceptArgs e)
    {
      Console.WriteLine("Interceptor Complete");
    }

    public Task InitializeAsync(Csla.Server.InterceptArgs e)
    {
      Console.WriteLine("Interceptor Initialize");
      return Task.CompletedTask;
    }
  }
}
