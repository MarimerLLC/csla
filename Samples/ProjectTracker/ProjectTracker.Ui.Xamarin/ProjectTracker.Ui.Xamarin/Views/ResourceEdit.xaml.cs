using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsUi.Views
{
  public partial class ResourceEdit : ContentPage
  {
    public ResourceEdit(int id)
    {
      InitializeComponent();
      BindingContext = new ViewModels.ResourceEdit(id);
    }
  }
}
