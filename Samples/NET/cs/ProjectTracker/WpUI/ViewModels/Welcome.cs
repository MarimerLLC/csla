using System.ComponentModel;

namespace WpUI.ViewModels
{
  public class Welcome : INotifyPropertyChanged
  {
    public Welcome()
    {
      ProjectTracker.Library.Security.PTPrincipal.NewUser += () =>
      {
        SetUsername();
        if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
          App.ViewModel.ShowStatus(
            new Bxf.Status { Text = "Welcome " + Csla.ApplicationContext.User.Identity.Name });
        else
          App.ViewModel.ShowStatus(new Bxf.Status { Text = "Welcome guest user" });
        App.ViewModel.ReloadMainView();
      };

      SetUsername();
    }

    private void SetUsername()
    {
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        UserName = Csla.ApplicationContext.User.Identity.Name;
      else
        UserName = "guest";
    }

    private string _userName;
    public string UserName
    {
      get { return _userName; }
      set { _userName = value; OnPropertyChanged("UserName"); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
