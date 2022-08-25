using System;
using Csla;

namespace CustomActivator
{
  public class CustomActivator : Csla.Server.IDataPortalActivator
  {
    public CustomActivator(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext;
    }

    private ApplicationContext ApplicationContext { get; set; }

    public void InitializeInstance(object obj)
    {
      Console.WriteLine("InitializeInstance");
    }

    public void FinalizeInstance(object obj)
    {
      Console.WriteLine("FinalizeInstance");
    }

    public object CreateInstance(Type requestedType)
    {
      // no longer invoked in CSLA 6
      throw new NotImplementedException(nameof(CreateInstance));
    }

    public Type ResolveType(Type requestedType)
    {
      // no longer invoked in CSLA 6
      throw new NotImplementedException(nameof(CreateInstance));
    }
  }
}
