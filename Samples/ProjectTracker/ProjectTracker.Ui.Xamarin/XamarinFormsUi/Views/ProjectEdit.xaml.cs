using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamarinFormsUi.Views
{
  public partial class ProjectEdit : ContentPage
  {
    public int ProjectId { get; private set; }

    public ProjectEdit(int id)
    {
      ProjectId = id;
      InitializeComponent();
    }

    public async Task InitAsync()
    {
      ViewModels.ProjectEdit vm = null;
      try
      {
        vm = new ViewModels.ProjectEdit();
      }
      catch (Exception ex)
      {
        var x = ex;
      }
      vm.ProjectId = ProjectId;
      await vm.InitAsync();
      BindingContext = vm;
    }
  }
}
