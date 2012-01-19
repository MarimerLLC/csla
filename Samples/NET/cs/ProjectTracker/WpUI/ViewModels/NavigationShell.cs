using System;
using System.Windows.Controls;

namespace WpUI.ViewModels
{
  public class NavigationShell : Bxf.Shell
  {
    public Bxf.IView PendingView { get; set; }

    public NavigationShell()
    {
      // use the custom view factory
      this.ViewFactory = new CustomViewFactory();
    }

    protected override void InitializeBindingResource(Bxf.IView view)
    {
      PendingView = view;
    }

    public class CustomViewFactory : Bxf.ViewFactory
    {
      protected override UserControl CreateUserControl(string viewName)
      {
        return null;
      }
    }
  }
}
