using System.ComponentModel;

namespace Rolodex.Silverlight.Core
{
  public class SecondaryModel : INotifyPropertyChanged
  {
    private object _model;

    public object Model
    {
      get { return _model; }
      set
      {
        _model = value;
        OnPropertyChanged("Model");
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }
}