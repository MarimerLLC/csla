using System.Threading.Tasks;
using BlazorExample.Shared;
using Csla;
using Csla.Blazor;

namespace BlazorExample.Client.ViewModels
{
  public class PersonListViewModel : ViewModel<PersonList>
  {
    protected override async Task<PersonList> DoRefreshAsync(params object[] parameters)
    {
      return await DataPortal.FetchAsync<PersonList>();
    }
  }
}
