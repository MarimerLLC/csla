using System;

namespace CustomActivator
{
  public class CustomIntercepter : Csla.Server.IInterceptDataPortal
  {
    public void Complete(Csla.Server.InterceptArgs e)
    {
      Console.WriteLine("Complete");
    }

    public void Initialize(Csla.Server.InterceptArgs e)
    {
      Console.WriteLine("Initialize");
    }
  }
}
