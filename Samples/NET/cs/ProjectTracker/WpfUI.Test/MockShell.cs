using Bxf;

namespace WpfUI.Test
{
  public class MockShell : Shell
  {
    public MockShell()
    {
      base.ViewFactory = new MockViewFactory();
    }

    protected override void InitializeBindingResource(IView view)
    {
      // do not initialize binding resource
    }

    public class MockViewFactory : IViewFactory
    {
      public IView GetView(string viewName, string bindingResourceKey, object model)
      {
        return null;
      }
    }
  }
}
