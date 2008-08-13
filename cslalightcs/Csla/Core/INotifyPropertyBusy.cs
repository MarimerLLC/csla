using System.ComponentModel;

namespace Csla.Core
{
  public interface INotifyPropertyBusy : INotifyPropertyChanged
  {
    event PropertyChangedEventHandler PropertyBusy;
    event PropertyChangedEventHandler PropertyIdle;
  }
}
