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

    private static MainForm _mainForm;

    private UserControl _currentControl;

    public MainForm()
    {
      InitializeComponent();

      _mainForm = this;

      this.Loaded += new RoutedEventHandler(MainForm_Loaded);
    }

    private void MainForm_Loaded(object sender, RoutedEventArgs e)
    {
      ApplyAuthorization();
      this.Title = "Project Tracker";
    }

    public static void ShowControl(UserControl control)
    {
      _mainForm.ShowUserControl(control);
    }

    private void ShowUserControl(UserControl control)
    {
      UnhookTitleEvent(_currentControl);

      contentArea.Children.Clear();
      if (control != null)
        contentArea.Children.Add(control);
      _currentControl = control;
      
      HookTitleEvent(_currentControl);
    }

    private void UnhookTitleEvent(UserControl control)
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

    private void SetTitle(object sender, EventArgs e)
    {
      EditForm form = sender as EditForm;
      if (form != null && !string.IsNullOrEmpty(form.Title))
        _mainForm.Title = string.Format("Project Tracker - {0}", ((EditForm)sender).Title);
      else
        _mainForm.Title = string.Format("Project Tracker");
    }

    #endregion

    #region Authorization

    private void ApplyAuthorization()
    {
      this.NewProjectButton.IsEnabled =
        Csla.Security.AuthorizationRules.CanCreateObject(typeof(Project));
      this.CloseProjectButton.IsEnabled =
        Csla.Security.AuthorizationRules.CanEditObject(typeof(Project));
      this.NewResourceButton.IsEnabled =
        Csla.Security.AuthorizationRules.CanCreateObject(typeof(Resource));
    }

    #endregion

    #region Menu items

    private void NewProject(object sender, EventArgs e)
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

    private void ShowProjectList(object sender, EventArgs e)
    {
      ProjectList frm = new ProjectList();
      ShowControl(frm);
    }

    private void ShowResourceList(object sender, EventArgs e)
    {
      ResourceList frm = new ResourceList();
      ShowControl(frm);
    }

    private void NewResource(object sender, EventArgs e)
    {
      ResourceEdit frm = new ResourceEdit(0);
      ShowControl(frm);
    }

    private void ShowRolesEdit(object sender, EventArgs e)
    {
      RolesEdit frm = new RolesEdit();
      ShowControl(frm);
    }

    private void CloseProject(object sender, RoutedEventArgs e)
    {
      ProjectSelect frm = new ProjectSelect();
      bool result = (bool)frm.ShowDialog();
      if (result)
      {
        Guid id = frm.ProjectId;
        ProjectCloser.CloseProject(id);
        MessageBox.Show("Project closed",
          "Close project", MessageBoxButton.OK, MessageBoxImage.Information);
      }
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
          ProjectTracker.Library.Security.PTPrincipal.Login(
            username, password);
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
            string.Format("Logged in as {0}", 
            Csla.ApplicationContext.User.Identity.Name);
          LoginButtonText.Text = "Log out";
        }
      }

      ApplyAuthorization();
      IRefresh p = _currentControl as IRefresh;
      if (p != null)
        p.Refresh();
    }

    #endregion
  }
}