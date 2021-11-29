using System.Security.Claims;

namespace WinFormsExample
{
  public partial class MainForm : Form
  {
    public MainForm(IServiceProvider serviceProvider, Csla.ApplicationContext applicationContext)
    {
      Instance = this;
      ServiceProvider = serviceProvider;
      ApplicationContext = applicationContext;
      InitializeComponent();
      ShowPage(typeof(Pages.HomePage));
    }

    public static MainForm Instance { get; private set; }
    private Csla.ApplicationContext ApplicationContext { get; }
    private IServiceProvider ServiceProvider { get; }

    public void ShowPage(Type pageType)
    {
      SetPageTitle();
      var page = ServiceProvider.GetService(pageType) as Control;
      contentPanel.Controls.Clear();
      contentPanel.Controls.Add(page);
    }

    public void ShowPage(Type pageType, object context)
    {
      SetPageTitle("");
      var page = ServiceProvider.GetService(pageType) as Control;
      if (page is Pages.IUseContext iuc)
        iuc.Context = context;
      contentPanel.Controls.Clear();
      contentPanel.Controls.Add(page);
    }

    public void SetPageTitle()
    {
      SetPageTitle(string.Empty);
    }

    public void SetPageTitle(string title)
    {
      if (string.IsNullOrEmpty(title))
        Text = "WinFormsExample";
      else
        Text = $"WinFormsExample - {title}";
    }

    private void homeButton_Click(object sender, EventArgs e)
    {
      if (contentPanel.Controls.Count == 0 || !contentPanel.Controls[0].GetType().Equals(typeof(Pages.HomePage)))
        ShowPage(typeof(Pages.HomePage));
    }

    private void loginButton_Click(object sender, EventArgs e)
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
        loginButton.Text = "Login";
      else
        loginButton.Text = ApplicationContext.User.Identity.Name;
    }
  }
}