using System;
using System.Collections.Generic;
using System.Text;
using Csla.Xaml;

namespace XamarinExample.ViewModels
{
  public class ViewModel<T> : ViewModelBase<T>
  {
    public ViewModel()
    {
      Initialize();
    }

    private string title;
    public string Title
    {
      get { return title; }
      set { title = value; OnPropertyChanged(nameof(Title)); }
    }

    protected virtual void Initialize()
    { }
  }
}
