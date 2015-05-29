using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileUI
{
  public partial class MainPage
  {
    public MainPage()
    {
      InitializeComponent();

      this.SaveItem.Activated += (o, e) => 
      {
        ViewModel.SaveData();
      };
    }

    void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      BindingContext = ViewModel.Model;
    }

    private OrderVm _viewModel;
    public OrderVm ViewModel {
      get { return _viewModel; }
      set
      {
        _viewModel = value;
        _viewModel.PropertyChanged += MainViewModel_PropertyChanged;
      }
    }
  }
}
