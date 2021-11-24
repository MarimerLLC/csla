using System;
using System.Security.Claims;
using System.Windows;
using System.Windows.Controls;
using Csla;

namespace WpfExample
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow(IServiceProvider serviceProvider, ApplicationContext applicationContext)
    {
      Instance = this;
      ServiceProvider = serviceProvider;
      ApplicationContext = applicationContext;
      InitializeComponent();
      ShowPage(typeof(Pages.HomePage));
    }

    private ApplicationContext ApplicationContext { get; }
    private IServiceProvider ServiceProvider { get; }

    private void CloseAction(object sender, RoutedEventArgs e)
    {
      if (!contentArea.Content.GetType().Equals(typeof(Pages.HomePage)))
        ShowPage(typeof(Pages.HomePage));
    }

    private void LoginAction(object sender, RoutedEventArgs e)
    {
      if (ApplicationContext.User.Identity == null || !ApplicationContext.User.Identity.IsAuthenticated)
      {
        var claims = new Claim[]
        {
          new Claim(ClaimTypes.Name, "Test User"),
          new Claim(ClaimTypes.Role, "Admin"),
        };
        var identity = new ClaimsIdentity(claims, "Test", ClaimTypes.Name, ClaimTypes.Role);
        ApplicationContext.User = new ClaimsPrincipal(identity);
      }
      else
      {
        ApplicationContext.User = new ClaimsPrincipal();
      }

      if (ApplicationContext.User.Identity == null || !ApplicationContext.User.Identity.IsAuthenticated)
        loginButton.Content = "Login";
      else
        loginButton.Content = ApplicationContext.User.Identity.Name;
    }

    public static MainWindow Instance { get; private set; }

    public void ShowPage(Type pageType)
    {
      SetPageTitle("");
      SetToolbarContent(null);
      SetStatusbarContent(null);
      var page = ServiceProvider.GetService(pageType);
      contentArea.Content = page;
    }

    public void ShowPage(Type pageType, object context)
    {
      SetPageTitle("");
      SetToolbarContent(null);
      SetStatusbarContent(null);
      var page = ServiceProvider.GetService(pageType) as UserControl;
      if (page is Pages.IUseContext iuc)
        iuc.Context = context;
      contentArea.Content = page;
    }

    public void SetPageTitle(string title)
    { 
      if (string.IsNullOrEmpty(title))
        Title = "WpfExample";
      else
        Title = $"WpfExample - {title}";
    }

    public void SetToolbarContent(UserControl userControl)
    {
      toolbarContent.Content = userControl;
    }

    public void SetStatusbarContent(UserControl userControl)
    {
      statusbarContent.Content = userControl;
    }
  }
}
