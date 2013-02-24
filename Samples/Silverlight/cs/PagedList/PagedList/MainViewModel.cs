using System;
using System.Windows;
using Csla.Xaml;

namespace PagedList
{
  public class MainViewModel : ViewModel<Library.DataList>
  {
    public MainViewModel()
    {
      // set to true for automatic paged loading, false to use the Next button
      AutoLoad = true;

      if (AutoLoad)
        BeginRefresh("GetListPaged");
      else
        BeginRefresh("GetFirstPage");
    }

    protected override void OnModelChanged(Library.DataList oldValue, Library.DataList newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      TotalRowCount = Model.TotalRowCount;
      LoadedRowCount = Model.Count;
      Model.CollectionChanged += (o, e) => LoadedRowCount = Model.Count;
    }

    protected override void OnError(Exception error)
    {
      base.OnError(error);
      MessageBox.Show(error.ToString());
    }

    public static readonly DependencyProperty AutoLoadProperty =
      DependencyProperty.Register("AutoLoad", typeof(bool), typeof(MainViewModel), null);
    public bool AutoLoad
    {
      get { return (bool)GetValue(AutoLoadProperty); }
      set { SetValue(AutoLoadProperty, value); }
    }

    public static readonly DependencyProperty LoadedRowCountProperty =
      DependencyProperty.Register("LoadedRowCount", typeof(int), typeof(MainViewModel), null);
    public int LoadedRowCount
    {
      get { return (int)GetValue(LoadedRowCountProperty); }
      set { SetValue(LoadedRowCountProperty, value); }
    }

    public static readonly DependencyProperty TotalRowCountProperty =
      DependencyProperty.Register("TotalRowCount", typeof(int), typeof(MainViewModel), null);
    public int TotalRowCount
    {
      get { return (int)GetValue(TotalRowCountProperty); }
      set { SetValue(TotalRowCountProperty, value); }
    }

    private int _lastPage;

    public void NextPage()
    {
      if (AutoLoad)
        throw new InvalidOperationException("NextPage");

      _lastPage++;
      Library.DataList.GetPage(_lastPage, (o, e) =>
        {
          if (e.Error == null)
            Model.AddRange(e.Object);
        });
    }
  }
}
