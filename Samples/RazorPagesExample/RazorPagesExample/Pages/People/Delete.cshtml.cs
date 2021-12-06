using BusinessLibrary;
using Csla;
using Csla.AspNetCore.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace RazorPagesExample.Pages.People
{
  public class DeleteModel : PageModel<PersonEdit>
  {
    public DeleteModel(ApplicationContext applicationContext, IDataPortal<PersonEdit> portal)
      : base(applicationContext)
    {
      _portal = portal;
    }

    private IDataPortal<PersonEdit> _portal;

    public async Task OnGet(int id)
    {
      Item = await _portal.FetchAsync(id);
    }

    public async Task<ActionResult> OnPost(int id)
    {
      await _portal.DeleteAsync(id);
      return RedirectToPage("/People/Index");
    }
  }
}