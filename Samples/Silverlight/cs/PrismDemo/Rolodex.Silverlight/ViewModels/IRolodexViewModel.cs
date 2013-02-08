using System.ComponentModel;

namespace Rolodex.Silverlight.ViewModels
{
    public interface IRolodexViewModel : INotifyPropertyChanged
    {
        bool IsBusy { get; }
        void Activated();
        bool IsDirty { get; }
        void Cleanup();
        void Initialize();
        void Initialize(object parameter);
    }
}
