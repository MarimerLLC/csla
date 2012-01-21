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
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
        };

      SetUsername();
    }

    private void SetUsername()
    {
      var old = UserName;

      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        UserName = Csla.ApplicationContext.User.Identity.Name;
      else
        UserName = "guest";

      if (UserName != old)
        App.ViewModel.MainPageViewModel.Refresh();
    }

    private string _userName = "guest";
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
