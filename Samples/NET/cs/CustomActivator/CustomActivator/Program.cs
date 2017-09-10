using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace CustomActivator
{
  class Program
  {
    static void Main(string[] args)
    {
      Csla.Server.DataPortal.InterceptorType = typeof(CustomIntercepter);
      Csla.ApplicationContext.DataPortalActivator = new CustomActivator();

      var obj = DataPortal.Fetch<ITestItem>("Rocky");
      Console.WriteLine(obj.Name);
      Console.ReadLine();
    }
  }

  public interface ITestItem : Csla.IBusinessBase
  {
    string Name { get; set; }
  }

  [Serializable]
  public class TestItem : BusinessBase<TestItem>, ITestItem
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public TestItem()
    {
      
    }

    private void DataPortal_Fetch(string id)
    {
      using (BypassPropertyChecks)
      {
        this.Name = id;
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
      Console.WriteLine("CreateInstance of " + requestedType.Name);
      requestedType = typeof(TestItem);
      return Activator.CreateInstance(requestedType);
    }

    public void InitializeInstance(object obj)
    {
      Console.WriteLine("InitializeInstance");
    }

    public void FinalizeInstance(object obj)
    {
      Console.WriteLine("FinalizeInstance");
    }
  }
}
