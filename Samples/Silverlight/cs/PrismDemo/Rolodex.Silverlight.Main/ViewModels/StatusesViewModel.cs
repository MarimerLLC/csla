using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.ViewModels;

namespace Rolodex.Silverlight.Main.ViewModels
{
    public partial class StatusesViewModel : RolodexViewModel<EditableEmployeeStatusList>, IStatusesViewModel
    {
        #region Constructor

        public StatusesViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer unityContainer)
            : base(regionManager, eventAggregator, unityContainer)
        {

        }

        #endregion

        #region methods

        public override void Initialize()
        {
            base.Initialize();
            IsBusy = true;
            EditableEmployeeStatusList.GetEditableEmployeeStatusList(
                (o, e) =>
                {
                    IsBusy = false;
                    if (HasNoException(e))
                    {
                        Model = e.Object;
                    }
                });
        }

        #endregion
    }
}