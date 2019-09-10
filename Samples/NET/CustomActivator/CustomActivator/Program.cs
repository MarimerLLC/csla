using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Configuration;

namespace CustomActivator
{
  class Program
  {
    static void Main(string[] args)
    {
      CslaConfiguration.Configure()
        .DataPortal()
        .Activator(new CustomActivator())
        .InterceptorType(typeof(CustomIntercepter));

      var obj = DataPortal.Fetch<ITestItem>("Rocky");
      Console.WriteLine(obj.Name);
      Console.ReadLine();
    }
  }

  public interface ITestItem : IBusinessBase
  {
    string Name { get; set; }
  }

  [Serializable]
  public class TestItem : BusinessBase<TestItem>, ITestItem
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [Fetch]
    private void Fetch(string name)
    {
      using (BypassPropertyChecks)
      {
        Name = name;
      }
    }
  }

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

  public class CustomActivator : Csla.Server.IDataPortalActivator
  {
    public object CreateInstance(Type requestedType)
    {
      Console.WriteLine($"CreateInstance of {requestedType.Name}");
      return Activator.CreateInstance(ResolveType(requestedType));
    }

    public void InitializeInstance(object obj)
    {
      Console.WriteLine("InitializeInstance");
    }

    public void FinalizeInstance(object obj)
    {
      Console.WriteLine("FinalizeInstance");
    }

    public Type ResolveType(Type requestedType)
    {
      Console.WriteLine($"ResolveType {requestedType.FullName} on {ApplicationContext.LogicalExecutionLocation}");
      if (requestedType.Equals(typeof(ITestItem)))
        return typeof(TestItem);
      else
        return requestedType;
    }
  }
}
