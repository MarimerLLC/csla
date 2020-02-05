using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class DisposeScopeTest
  {
    [TestMethod]
    public void Test_Scope_Dispose()
    {
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddScoped<DisposableClass>();

      ApplicationContext.DefaultServiceProvider = serviceCollection.BuildServiceProvider();

      _ = ClassA.GetClassA();
    }

  }

  public class DisposableClass
  : IDisposable
  {
    public bool IsDisposed { get; private set; } = false;
    public void Dispose()
    {
      IsDisposed = true;
    }
  }

  public class ClassA : BusinessBase<ClassA>
  {
    public static ClassA GetClassA()
    {
      return Csla.DataPortal.Fetch<ClassA>();
    }

    [Fetch]
    private void Fetch([Inject]DisposableClass disposable)
    {
      if (disposable.IsDisposed)
      {
        throw new ObjectDisposedException(nameof(disposable));
      }

      _ = ClassB.GetClassB();

      if (disposable.IsDisposed)
      {
        throw new ObjectDisposedException(nameof(disposable));
      }
    }
  }

  public class ClassB : BusinessBase<ClassB>
  {
    public static ClassB GetClassB()
    {
      return Csla.DataPortal.Fetch<ClassB>();
    }

    [Fetch]
    private void Fetch([Inject]DisposableClass disposable)
    {
      if (disposable.IsDisposed)
      {
        throw new ObjectDisposedException(nameof(disposable));
      }
    }
  }
}
