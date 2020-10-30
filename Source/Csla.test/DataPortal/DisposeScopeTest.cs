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
    public void Test_Scope_DoesNotDispose()
    {
      // CSLA should not dispose of the default service provider.
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddScoped<DisposableClass>();

      ApplicationContext.DefaultServiceProvider = serviceCollection.BuildServiceProvider();

      var classA = ClassA.GetClassA();
      var classB = classA.ChildB;

      Assert.AreEqual(classA.DisposableClass.Id, classB.DisposableClass.Id, "Ids must be the same");
      Assert.IsFalse(classA.DisposableClass.IsDisposed, "Object must not be disposed");
    }

  }

  public class DisposableClass
  : IDisposable
  {
    public Guid Id { get; } = Guid.NewGuid();
    public bool IsDisposed { get; private set; } = false;
    public void Dispose()
    {
      IsDisposed = true;
    }
  }

  public class ClassA : BusinessBase<ClassA>
  {
    public ClassB ChildB { get; set; }
    public DisposableClass DisposableClass { get; set; }

    public static ClassA GetClassA()
    {
      return Csla.DataPortal.Fetch<ClassA>();
    }

    [Fetch]
    private void Fetch([Inject]DisposableClass disposable)
    {
      DisposableClass = disposable;

      if (disposable.IsDisposed)
      {
        throw new ObjectDisposedException(nameof(disposable));
      }

      ChildB = ClassB.GetClassB();

      if (disposable.IsDisposed)
      {
        throw new ObjectDisposedException(nameof(disposable));
      }
    }
  }

  public class ClassB : BusinessBase<ClassB>
  {
    public DisposableClass DisposableClass { get; set; }
    public Guid Id { get; set; }

    public static ClassB GetClassB()
    {
      return Csla.DataPortal.Fetch<ClassB>();
    }

    [Fetch]
    private void Fetch([Inject]DisposableClass disposable)
    {
      DisposableClass = disposable;

      if (disposable.IsDisposed)
      {
        throw new ObjectDisposedException(nameof(disposable));
      }
    }
  }
}
