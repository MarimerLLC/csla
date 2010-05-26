using System;
using System.Collections.ObjectModel;
using System.Linq;
using Csla.Xaml;

namespace CslaMvvmSl.ViewModels
{
  public class ListPersonsViewModel : ViewModel<Library.PersonList>
  {
    public ListPersonsViewModel()
    {
      this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ListPersonsViewModel_PropertyChanged);
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { Text = "Getting list", IsBusy = true });
      BeginRefresh("GetPersonList");
    }

    void ListPersonsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      var tmp = e;  
    }

    public ObservableCollection<PersonItemViewModel> PersonList
    {
      get
      {
        var result = new ObservableCollection<PersonItemViewModel>();
        if (Model != null)
          foreach (var item in Model.OrderBy(c => c.Id))
            result.Add(new PersonItemViewModel(item));
        return result;
      }
    }

    protected override void OnModelChanged(Library.PersonList oldValue, Library.PersonList newValue)
    {
      if (oldValue != null)
        oldValue.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(model_CollectionChanged);
      base.OnModelChanged(oldValue, newValue);
      if (newValue != null)
        newValue.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(model_CollectionChanged);
    }

    void model_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      CreateView();
    }

    protected override void OnRefreshed()
    {
      base.OnRefreshed();
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
      CreateView();
    }

    private void CreateView()
    {
      OnPropertyChanged("PersonList");
    }

    protected override void OnError(Exception error)
    {
      base.OnError(error);
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsOk = false });
      Bxf.Shell.Instance.ShowError(error.Message, "PersonList error");
    }
  }
}
