using System;
using System.ComponentModel;

namespace WpfUI.ViewModels
{
  public class Error : INotifyPropertyChanged
  {
    private string _errorContent;
    public string ErrorContent
    {
      get { return _errorContent; }
      set { _errorContent = value; OnPropertyChanged("ErrorContent"); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
