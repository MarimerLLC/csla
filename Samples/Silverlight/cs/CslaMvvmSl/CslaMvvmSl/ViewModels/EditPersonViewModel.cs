using System;
using Csla.Xaml;

namespace CslaMvvmSl.ViewModels
{
  public class EditPersonViewModel : ViewModel<Library.Person>
  {
    public EditPersonViewModel()
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { Text = "Creating Person", IsBusy = true });
      BeginRefresh("NewPerson");
    }

    public EditPersonViewModel(int id)
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { Text = "Retrieving Person", IsBusy = true });
      BeginRefresh("GetPerson", id);
    }

    protected override void OnRefreshed()
    {
      base.OnRefreshed();
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
    }

    protected override void OnError(Exception error)
    {
      base.OnError(error);
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsOk = false });
      Bxf.Shell.Instance.ShowError(error.Message, "Person error");
    }
  }
}
