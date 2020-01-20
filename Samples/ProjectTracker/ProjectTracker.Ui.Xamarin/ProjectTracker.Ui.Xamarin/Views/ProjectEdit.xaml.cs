using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsUi.Views
{
  public partial class ProjectEdit : ContentPage
  {
    public ProjectEdit(int id)
    {
      InitializeComponent();
      BindingContext = new ViewModels.ProjectEdit(id);
    }
  }
}
