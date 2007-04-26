using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectTracker.Library;

namespace PTWpf
{
  /// <summary>
  /// Interaction logic for MainPage.xaml
  /// </summary>

  public partial class MainPage : System.Windows.Controls.Page
  {
    public MainPage()
    {
      InitializeComponent();

      ProjectTracker.Library.Security.PTPrincipal.Logout();
      _principal = (ProjectTracker.Library.Security.PTPrincipal)Csla.ApplicationContext.User;

      _mainForm = this;
      _mainFrame = frame1;

      this.WindowTitle = "Project Tracker";
      this.Loaded += new RoutedEventHandler(MainPage_Loaded);
    }

    void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
      _mainFrame.Navigated += new System.Windows.Navigation.NavigatedEventHandler(NavigationService_Navigated);
      Csla.DataPortal.DataPortalInitInvoke += new Action<object>(DataPortal_DataPortalInitInvoke);
    }

    private static ProjectTracker.Library.Security.PTPrincipal _principal;

    /// <summary>
    /// This method ensures that the thread about to do
    /// data access has a valid PTPrincipal object, and is
    /// needed because of the way WPF doesn't move the 
    /// main thread's principal object to other threads
    /// automatically.
    /// </summary>
    /// <param name="obj"></param>
    void DataPortal_DataPortalInitInvoke(object obj)
    {
      if (!(Csla.ApplicationContext.User is ProjectTracker.Library.Security.PTPrincipal))
      {
        Csla.ApplicationContext.User = _principal;
      }
    }

    static void NavigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
      Page page = e.Content as Page;
      if (page != null)
        _mainForm.WindowTitle = string.Format("Project Tracker - {0}", page.Title);
    }

    static Page _mainForm;
    static Frame _mainFrame;

    public static void ShowPage(Page page)
    {
      _mainFrame.NavigationService.Navigate(page);
    }
    
    void NewProject(object sender, EventArgs e)
    {
      try
      {
        ProjectEdit frm = new ProjectEdit(ProjectTracker.Library.Project.NewProject());
        ShowPage(frm);
      }
      catch (System.Security.SecurityException ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    void NewResource(object sender, EventArgs e)
    {
      try
      {
        ProjectEdit frm = new ProjectEdit(ProjectTracker.Library.Project.NewProject());
        ShowPage(frm);
        //frame1.NavigationService.Navigate(frm);
      }
      catch (System.Security.SecurityException ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    void LogInOut(object sender, EventArgs e)
    {
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
      {
        ProjectTracker.Library.Security.PTPrincipal.Logout();
        CurrentUser.Text = "Not logged in";
        LoginButtonText.Text = "Log in";
      }
      else
      {
        Login frm = new Login();
        frm.ShowDialog();
        if (frm.Result)
        {
          string username = frm.UsernameTextBox.Text;
          string password = frm.PasswordTextBox.Password;
          ProjectTracker.Library.Security.PTPrincipal.Login(username, password);
        }
        if (!Csla.ApplicationContext.User.Identity.IsAuthenticated)
        {
          ProjectTracker.Library.Security.PTPrincipal.Logout();
          CurrentUser.Text = "Not logged in";
          LoginButtonText.Text = "Log in";
        }
        else
        {
          CurrentUser.Text =
            string.Format("Logged in as {0}", Csla.ApplicationContext.User.Identity.Name);
          LoginButtonText.Text = "Log out";
        }
      }
      IRefresh p = _mainFrame.Content as IRefresh;
      if (p != null)
        p.Refresh();
    }
  }
}