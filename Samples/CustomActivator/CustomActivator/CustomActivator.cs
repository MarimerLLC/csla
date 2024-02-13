using System;
using Csla;
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;

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
      Console.WriteLine($"InitializeInstance({obj.GetType().Name})");
    }

    public void FinalizeInstance(object obj)
    {
      Console.WriteLine($"FinalizeInstance({obj.GetType().Name})");
    }

    public object CreateInstance(Type requestedType, params object[] parameters)
    {
      Console.WriteLine($"{nameof(CreateInstance)}({requestedType.Name})");
      object result;
      var realType = ResolveType(requestedType);
      var serviceProvider = (IServiceProvider)ApplicationContext.GetRequiredService<IServiceProvider>();
      result = ActivatorUtilities.CreateInstance(serviceProvider, realType, parameters);
      if (result is IUseApplicationContext tmp)
      {
        tmp.ApplicationContext = serviceProvider.GetRequiredService<ApplicationContext>();
      }
      InitializeInstance(result);
      return result;
    }

    public Type ResolveType(Type requestedType)
    {
      var resolvedType = requestedType;
      if (requestedType.Equals(typeof(ITestItem)))
        resolvedType = typeof(TestItem);
      Console.WriteLine($"{nameof(ResolveType)}({requestedType.Name})->{resolvedType.Name}");
      return resolvedType;
    }
  }
}
