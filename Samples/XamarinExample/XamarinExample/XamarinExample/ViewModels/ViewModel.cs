using System;
using System.Collections.Generic;
using System.Text;
using Csla.Xaml;
using Xamarin.Forms;

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

    private static INavigation _navigation = App.Current.MainPage.Navigation;
    protected INavigation Navigation
    {
      get => _navigation;
      set => _navigation = value;
    }

    protected virtual void Initialize()
    { }
  }
}
