using System;
using System.ComponentModel;

namespace WpfUI.ViewModels
{
  public class User : INotifyPropertyChanged
  {
    public User()
    {
      ProjectTracker.Library.Security.PTPrincipal.NewUser += () =>
      {
        SetUsername();
        if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
          Bxf.Shell.Instance.ShowStatus(
            new Bxf.Status { Text = "Welcome " + Csla.ApplicationContext.User.Identity.Name });
        else
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status { Text = "Welcome guest user" });
        MainPresenter.ShowMenu();
      };

      SetUsername();
    }

    private void SetUsername()
    {
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        UserName = Csla.ApplicationContext.User.Identity.Name;
      else
        UserName = "log in";
    }

    private string _userName;
    public string UserName
    {
      get { return _userName; }
      set { _userName = value; OnPropertyChanged("UserName"); }
    }

    public void LoginOut()
    {
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        ProjectTracker.Library.Security.PTPrincipal.Logout();
      Bxf.Shell.Instance.ShowView(
        typeof(Views.Login).AssemblyQualifiedName,
        "loginViewSource",
        new ViewModels.Login(),
        "Main");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
