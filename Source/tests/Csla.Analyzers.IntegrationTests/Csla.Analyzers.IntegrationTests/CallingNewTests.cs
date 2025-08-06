using System;
using Csla.Server;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public class A : BusinessBase<A> { }

  public class B : ObjectFactory
  {
    public void Foo()
    {
      var a = new A();
    }
  }

  public class C
  {
    public void Foo()
    {
      // This should be an error
      // because you can't create a new business object
      // outside of an ObjectFactory.
      var a = new A();
    }
  }
}
