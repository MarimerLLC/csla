using System;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.Events;
using Rolodex.Silverlight.ViewModels;
using Rolodex.Silverlight.Services;

namespace Rolodex.Silverlight.Main.ViewModels
{
    public partial class MenuViewModel : RolodexSimpleViewModel<RolodexPrincipal>, IMenuViewModel
    {
        #region Constructor

        public MenuViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer unityContainer)
            : base(regionManager, eventAggregator, unityContainer)
        {

        }

        protected override void CreateCommands()
        {
            base.CreateCommands();
            MenuCommand = new DelegateCommand<string>(MenuSelected);
        }

        #endregion

        #region Commands

        private DelegateCommand<string> menuCommand;
        public DelegateCommand<string> MenuCommand
        {
            get
            {
                return menuCommand;
            }
            set
            {
                menuCommand = value;
                OnPropertyChanged("MenuCommand");
            }
        }


        public void MenuSelected(string menu)
        {
            var service = (RolodexService)Enum.Parse(typeof (RolodexService), menu, true);
            Aggregator.GetEvent<ServiceSelected>().Publish(new ServiceEventArgs(service));
        }

        #endregion

    
    }
}