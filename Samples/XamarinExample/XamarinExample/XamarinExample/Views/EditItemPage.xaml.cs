using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinExample.ViewModels;

namespace XamarinExample.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class EditItemPage : ContentPage
  {
    private ItemEditViewModel viewmodel;

    public EditItemPage(ItemEditViewModel vm)
    {
      InitializeComponent();

      BindingContext = viewmodel = vm;
    }

    async void Save_Clicked(object sender, EventArgs e)
    {
      if (!viewmodel.CanSave)
        return;
      await viewmodel.Model.SaveAndMergeAsync();
      MessagingCenter.Send(this, "EditItem", viewmodel.Model);
      await Navigation.PopModalAsync();
    }

    async void Cancel_Clicked(object sender, EventArgs e)
    {
      await Navigation.PopModalAsync();
    }
  }
}