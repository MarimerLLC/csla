using System;
using System.ComponentModel;
using Csla;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using XamarinExample.ViewModels;

namespace XamarinExample.Views
{
  // Learn more about making custom code visible in the Xamarin.Forms previewer
  // by visiting https://aka.ms/xamarinforms-previewer
  [DesignTimeVisible(false)]
  public partial class ItemDetailPage : ContentPage
  {
    ItemDetailViewModel viewModel;

    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
      InitializeComponent();

      BindingContext = this.viewModel = viewModel;
    }

    public ItemDetailPage()
    {
      InitializeComponent();

      var item = DataPortal.Create<BusinessLibrary.PersonInfo>();

      viewModel = new ItemDetailViewModel(item);
      BindingContext = viewModel;
    }
  }
}