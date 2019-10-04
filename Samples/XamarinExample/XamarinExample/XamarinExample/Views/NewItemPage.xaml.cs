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
    public NewItemPage()
    {
      InitializeComponent();

      BindingContext = new ItemEditViewModel();
    }
  }
}