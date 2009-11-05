using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Csla.Silverlight;

namespace MVVMexperiment
{
  public class DataListViewModel : ViewModel<DataList>
  {
    public DataListViewModel()
    {
      this.PropertyChanged += (o, e) =>
        {
          if (e.PropertyName == "Error" && Error != null)
          {
            MessageBox.Show(Error.ToString(), "Data error", MessageBoxButton.OK);
          }
        };

      DoRefresh("GetList", 0);
    }

    public void Load(object sender, ExecuteEventArgs e)
    {
      var tag = e.TriggerSource.Tag.ToString();
      int id = 0;
      if (!string.IsNullOrEmpty(tag))
        id = int.Parse(tag);
      DoRefresh("GetList", id);
    }

    public void ShowItem(Data methodParameter)
    {
      SelectedData = methodParameter;
    }

    public void ShowItem2(object sender, Csla.Silverlight.ExecuteEventArgs e)
    {
      var x = SelectedData;
    }

    public void ProcessItems(System.Collections.ObjectModel.ObservableCollection<object> methodParameter)
    {
      var x = methodParameter;
    }

    public void ProcessItems2(object sender, Csla.Silverlight.ExecuteEventArgs e)
    {
      var listBox = ((System.Windows.Controls.Control)e.TriggerSource).Tag as System.Windows.Controls.ListBox;
      //var selection = listBox.SelectedItems;
      var selection = new List<Data>();
      foreach (var item in listBox.SelectedItems)
        selection.Add((Data)item);
      // process selection
      var form = new DetailPage();
      var vm = form.Resources["ViewModel"] as DetailModel;
      if (vm != null)
        vm.SelectedItems = selection;
      MainPageModel.ShowForm(form);
    }

    /// <summary>
    /// Gets or sets the SelectedData object.
    /// </summary>
    public static readonly DependencyProperty SelectedDataProperty =
        DependencyProperty.Register("SelectedData", typeof(Data), typeof(DataListViewModel), new PropertyMetadata(null));
    /// <summary>
    /// Gets or sets the SelectedData object.
    /// </summary>
    public Data SelectedData
    {
      get { return (Data)GetValue(SelectedDataProperty); }
      set { SetValue(SelectedDataProperty, value); OnPropertyChanged("SelectedData"); }
    }

    /// <summary>
    /// Gets or sets the SelectedItems object.
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register("SelectedItems", typeof(System.Collections.ObjectModel.ObservableCollection<object>), typeof(DataListViewModel), new PropertyMetadata(null));
    /// <summary>
    /// Gets or sets the SelectedItems object.
    /// </summary>
    public System.Collections.ObjectModel.ObservableCollection<object> SelectedItems
    {
      get { return (System.Collections.ObjectModel.ObservableCollection<object>)GetValue(SelectedItemsProperty); }
      set { SetValue(SelectedItemsProperty, value); OnPropertyChanged("SelectedItems"); }
    }
  }
}
