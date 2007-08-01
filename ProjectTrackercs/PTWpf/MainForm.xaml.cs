using System;
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
  /// Interaction logic for MainForm.xaml
  /// </summary>
  public partial class MainForm : Window
  {
    #region Navigation and Plumbing

    private static ProjectTracker.Library.Security.PTPrincipal _principal;
    private static MainForm _mainForm;

    private UserControl _currentControl;

    public MainForm()
    {
      InitializeComponent();

      _mainForm = this;

      this.Loaded += new RoutedEventHandler(MainForm_Loaded);
      Csla.DataPortal.DataPortalInitInvoke += new Action<object>(DataPortal_DataPortalInitInvoke);
    }

    void MainForm_Loaded(object sender, RoutedEventArgs e)
    {
      ProjectTracker.Library.Security.PTPrincipal.Logout();
      _principal = (ProjectTracker.Library.Security.PTPrincipal)
        Csla.ApplicationContext.User;

      this.Title = "Project Tracker";
    }

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
      if (!ReferenceEquals(Csla.ApplicationContext.User, _principal))
        Csla.ApplicationContext.User = _principal;
    }


    public static void ShowControl(UserControl control)
    {
      _mainForm.ShowUserControl(control);
    }

    private void ShowUserControl(UserControl control)
    {
      UnHookTitleEvent(_currentControl);

      contentArea.Children.Clear();
      if (control != null)
        contentArea.Children.Add(control);
      _currentControl = control;
      
      HookTitleEvent(_currentControl);
    }

    private void UnHookTitleEvent(UserControl control)
    {
      EditForm form = control as EditForm;
      if (form != null)
        form.TitleChanged -= new EventHandler(SetTitle);
    }

    private void HookTitleEvent(UserControl control)
    {
      SetTitle(control, EventArgs.Empty);
      EditForm form = control as EditForm;
      if (form != null)
        form.TitleChanged += new EventHandler(SetTitle);
    }

    void SetTitle(object sender, EventArgs e)
    {
      EditForm form = sender as EditForm;
      if (form != null && !string.IsNullOrEmpty(form.Title))
        _mainForm.Title = string.Format("Project Tracker - {0}", ((EditForm)sender).Title);
      else
        _mainForm.Title = string.Format("Project Tracker");
    }

    #endregion

    #region Menu items

    void NewProject(object sender, EventArgs e)
    {
      try
      {
        ProjectEdit frm = new ProjectEdit(Guid.Empty);
        ShowControl(frm);
      }
      catch (System.Security.SecurityException ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    void ShowProjectList(object sender, EventArgs e)
    {
      ProjectList frm = new ProjectList();
      ShowControl(frm);
    }

    void ShowResourceList(object sender, EventArgs e)
    {
      ResourceList frm = new ResourceList();
      ShowControl(frm);
    }

    void NewResource(object sender, EventArgs e)
    {
      ResourceEdit frm = new ResourceEdit(0);
      ShowControl(frm);
    }

    void ShowRolesEdit(object sender, EventArgs e)
    {
      RolesEdit frm = new RolesEdit();
      ShowControl(frm);
    }

    #endregion

    #region Login/Logout

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
      _principal = (ProjectTracker.Library.Security.PTPrincipal)
        Csla.ApplicationContext.User;

      IRefresh p = _currentControl as IRefresh;
      if (p != null)
        p.Refresh();
    }

    #endregion
  }
}