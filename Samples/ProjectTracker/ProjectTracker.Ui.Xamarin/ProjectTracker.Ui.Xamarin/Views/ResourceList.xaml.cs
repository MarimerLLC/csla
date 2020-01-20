using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamarinFormsUi.Views
{
  public partial class ResourceList : ContentPage
  {
    public ResourceList()
    {
      InitializeComponent();
      BindingContext = new ViewModels.ResourceList();
    }
  }
}
