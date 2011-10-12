using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.Events;
using Rolodex.Silverlight.ViewModels;

namespace Rolodex.Silverlight.Main.ViewModels
{
    public partial class LoginViewModel : RolodexSimpleViewModel<RolodexPrincipal>, ILoginViewModel
    {
        #region Constructor

        public LoginViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer unityContainer)
            : base(regionManager, eventAggregator, unityContainer)
        {

        }

        protected override void CreateCommands()
        {
            base.CreateCommands();
            LoginCommand = new DelegateCommand<string>(Login, CanLogin);
        }

        #endregion

        #region Properties

        private string userName = string.Empty;
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
                loginCommand.RaiseCanExecuteChanged();
            }
        }

        private string statusMessage = string.Empty;
        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

        #endregion

        #region Commands

        private DelegateCommand<string> loginCommand;
        public DelegateCommand<string> LoginCommand
        {
            get
            {
                return loginCommand;
            }
            set
            {
                loginCommand = value;
                OnPropertyChanged("LoginCommand");
            }
        }

        public bool CanLogin(string parameter)
        {
            return !string.IsNullOrEmpty(UserName);
        }

        public void Login(string password)
        {
            IsBusy = true;
            RolodexPrincipal.Login(userName, password, (o, e) =>
            {
                IsBusy = false;
                if (base.HasNoException(e))
                {
                    if (RolodexIdentity.CurrentIdentity.IsAuthenticated)
                    {
                        StatusMessage = "Login successful";
                    }
                    else
                    {
                        StatusMessage = "Login failed";
                    }
                    Aggregator.GetEvent<LoginCompleted>().Publish(RolodexIdentity.CurrentIdentity.IsAuthenticated);
                }
            });
        }

        #endregion

    
    }
}