using System;
using System.Collections.Generic;
using System.Text;
using Csla.Xaml;

namespace XamarinExample.ViewModels
{
  public class ViewModel<T> : ViewModelBase<T>
  {
    private string title;
    public string Title
    {
      get { return title; }
      set { title = value; OnPropertyChanged(nameof(Title)); }
    }
  }
}
