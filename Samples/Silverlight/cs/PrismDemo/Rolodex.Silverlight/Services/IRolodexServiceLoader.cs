using Rolodex.Silverlight.ViewModels;
using Rolodex.Silverlight.Views;

namespace Rolodex.Silverlight.Services
{
    public interface IRolodexServiceLoader
    {
        void RegisterService<TViewInterface, TView, TViewModelInterface, TViewModel>(RolodexService rolodexService)
            where TViewInterface : IRolodexView
            where TView : TViewInterface
            where TViewModelInterface : IRolodexViewModel
            where TViewModel : TViewModelInterface;

        void LoadService(RolodexService rolodexService);

        void LoadService(RolodexService rolodexService, object parameter);

        void LoadService(RolodexService rolodexService, string regionName);

        void LoadService(RolodexService rolodexService, string regionName, object parameter);
    }
}
