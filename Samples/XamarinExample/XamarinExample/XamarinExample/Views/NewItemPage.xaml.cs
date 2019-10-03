using System;
using System.Collections.Generic;
using System.ComponentModel;
using BusinessLibrary;
using Csla;
using Csla.Rules;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinExample.ViewModels;

namespace XamarinExample.Views
{
  // Learn more about making custom code visible in the Xamarin.Forms previewer
  // by visiting https://aka.ms/xamarinforms-previewer
  [DesignTimeVisible(false)]
  public partial class NewItemPage : ContentPage
  {
    private ItemEditViewModel viewmodel;

    public NewItemPage()
    {
      InitializeComponent();

      BindingContext = viewmodel = new ItemEditViewModel();
    }

    async void Save_Clicked(object sender, EventArgs e)
    {
      if (!viewmodel.CanSave)
        return;
      await viewmodel.Model.SaveAndMergeAsync();
      MessagingCenter.Send(this, "AddItem", viewmodel.Model);
      await Navigation.PopModalAsync();
    }

    async void Cancel_Clicked(object sender, EventArgs e)
    {
      await Navigation.PopModalAsync();
    }
  }
}