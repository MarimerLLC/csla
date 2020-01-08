using ProjectTracker.Ui.Xamarin;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsUi.Views
{
  public partial class Dashboard : ContentPage
  {
    public Dashboard()
    {
      InitializeComponent();
      BindingContext = new ViewModels.DashboardViewModel();
    }

    private async void ShowProjectList(object sender, EventArgs e)
    {
      await App.NavigateTo(typeof(Views.ProjectList));
    }

    private async void ShowResourceList(object sender, EventArgs e)
    {
      await App.NavigateTo(typeof(Views.ResourceList));
    }

    private async void ViewRoles(object sender, EventArgs e)
    {
      await App.NavigateTo(typeof(Views.RoleList));
    }
  }
}
