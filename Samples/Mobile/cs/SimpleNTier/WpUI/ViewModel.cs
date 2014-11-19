using System;

namespace WpUI
{
  public class ViewModel<T> : Csla.Xaml.ViewModel<T>, IViewModel
  {
    public ViewModel()
    {
      Header = "<null>";
    }

    public string Header { get; protected set; }
  }
}
