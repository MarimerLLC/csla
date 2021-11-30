using BusinessLibrary;
using Csla;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesExample.Pages
{
  public class ListPeopleModel : PageModel
  {
    public ListPeopleModel(IDataPortal<PersonList> portal)
    {
      _portal = portal;
    }

    private readonly IDataPortal<PersonList> _portal;

    public PersonList PersonList { get; set; }

    public async Task OnGet()
    {
      PersonList = await _portal.FetchAsync();
    }
  }
}