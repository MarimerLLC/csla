using System;
using Csla.Xaml;

namespace CslaMvvmSl.ViewModels
{
  public class PersonItemViewModel : ViewModel<Library.Person>
  {
    public PersonItemViewModel(Library.Person person)
    {
      ManageObjectLifetime = false;
      Model = person;
    }

    public override void Remove(object sender, ExecuteEventArgs e)
    {
      Model.Parent.RemoveChild(Model);
    }

    public void EditPerson()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(EditPerson).AssemblyQualifiedName,
        "editPersonViewModelViewSource",
        new EditPersonViewModel(Model.Id),
        "Content");
    }
  }
}
