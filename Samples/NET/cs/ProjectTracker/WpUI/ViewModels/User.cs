using System.ComponentModel;

namespace WpUI.ViewModels
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
        MainViewModel.ReloadMainView();
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
        "/Login.xaml",
        "loginViewSource",
        new ViewModels.Login(),
        "Dialog");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
