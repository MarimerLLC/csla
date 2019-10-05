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
    public EditItemPage(ItemEditViewModel vm)
    {
      InitializeComponent();

      BindingContext = vm;

      //ErrorText.Text = NameInfo.ErrorText;
      //WarnText.Text = NameInfo.WarningText;
      //InfoText.Text = NameInfo.InformationText;
    }
  }
}