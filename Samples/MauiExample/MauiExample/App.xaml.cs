using Csla;

namespace MauiExample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

  public static ApplicationContext ApplicationContext { get; set; }
}
