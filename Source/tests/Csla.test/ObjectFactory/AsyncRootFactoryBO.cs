using Csla.Server;

namespace Csla.Test.ObjectFactory;

[ObjectFactory("Csla.Test.ObjectFactory.RootFactory, Csla.Tests")]
[CslaImplementProperties]
public partial class AsyncRootFactoryBO : BusinessBase<AsyncRootFactoryBO>
{
  public partial string Text { get; set; }
}