using System;
using System.Security.Principal;
using System.Threading.Tasks;
using ProjectTracker.Library.Security;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class Login : Csla.Axml.ViewModel<IPrincipal>
    {
        public Login()
        {
            this.Model = Csla.ApplicationContext.User;
        }

        private string _userName = "manager";
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged("UserName"); }
        }

        private string _password = "manager";
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password"); }
        }

        public async Task LoginUserAsync()
        {
            this.IsBusy = true;
            try
            {
                await PTPrincipal.LoginAsync(UserName, Password);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
