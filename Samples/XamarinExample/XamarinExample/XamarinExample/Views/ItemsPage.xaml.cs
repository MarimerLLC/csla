using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using XamarinExample.Views;
using XamarinExample.ViewModels;
using BusinessLibrary;

namespace XamarinExample.Views
{
  // Learn more about making custom code visible in the Xamarin.Forms previewer
  // by visiting https://aka.ms/xamarinforms-previewer
  [DesignTimeVisible(false)]
  public partial class ItemsPage : ContentPage
  {
    ItemsViewModel viewModel;

    public ItemsPage()
    {
      InitializeComponent();

      BindingContext = viewModel = new ItemsViewModel();
    }

    async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
      await viewModel.EditItemAsync(args.SelectedItem as PersonInfo);
      ItemsListView.SelectedItem = null;
    }

    async void AddItem_Clicked(object sender, EventArgs e)
    {
      await viewModel.AddItemAsync();
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();
      if (viewModel.Model == null || viewModel.Model.Count == 0)
        viewModel.LoadItemsCommand.Execute(null);
    }
  }
}