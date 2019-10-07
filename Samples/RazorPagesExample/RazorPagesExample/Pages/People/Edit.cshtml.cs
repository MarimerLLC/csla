using System.Threading.Tasks;
using BusinessLibrary;
using Csla;
using Csla.Web.Shared.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace RazorPagesExample.Pages.People
{
  public class EditModel : PageModel<PersonEdit>
  {
    public async Task OnGet(int id)
    {
      Item = await DataPortal.FetchAsync<PersonEdit>(id);
    }

    public async Task<ActionResult> OnPost()
    {
      return await SaveAsync();
    }
  }
}