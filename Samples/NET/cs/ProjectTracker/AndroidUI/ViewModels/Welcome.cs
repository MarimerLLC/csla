using System.Security.Principal;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class Welcome : Csla.Axml.ViewModel<IPrincipal>
    {
        public Welcome()
        {
            Library.Security.PTPrincipal.NewUser += () =>
            {
                SetUsername();
            };

            SetUsername();
        }

        public void Refresh()
        {
            this.SetUsername();
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
    }
}
