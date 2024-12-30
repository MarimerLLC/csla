using MauiExample.Pages;

namespace MauiExample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
    RegisterRoutes();

  }

  public void RegisterRoutes()
  {
    Routing.RegisterRoute(Constants.PersonEditRoute, typeof(PersonEditPage));
    Routing.RegisterRoute(Constants.PersonListRoute, typeof(PersonListPage));
  }

  protected override Window CreateWindow(IActivationState? activationState)
  {
    return new Window(new AppShell());
  }
}
