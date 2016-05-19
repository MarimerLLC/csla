using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamarinFormsUi.Views
{
  public partial class ProjectList : ContentPage
  {
    public ProjectList()
    {
      InitializeComponent();
    }

    public async Task InitAsync()
    {
      BindingContext = await new ViewModels.ProjectList().InitAsync();
    }
  }
}
