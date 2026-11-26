using BusinessLibrary;
using Csla;
using Csla.AspNetCore.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace RazorPagesExample.Pages.People
{
  public class EditModel : PageModel<PersonEdit>
  {
    public EditModel(ApplicationContext applicationContext, IDataPortal<PersonEdit> portal)
      : base(applicationContext)
    {
      _portal = portal;
    }

    private IDataPortal<PersonEdit> _portal;

    public async Task OnGet(int id)
    {
      if (id == -1)
        Item = await _portal.CreateAsync();
      else
        Item = await _portal.FetchAsync(id);
    }

    public async Task<IActionResult> OnPost(int id)
    {
      // Id has a private setter, so the binder doesn't set it from the
      // form (a posted Id could point the save at another person). Load
      // the person the route names, then bind the posted fields into it.
      Item = id == -1 ? await _portal.CreateAsync() : await _portal.FetchAsync(id);
      ModelState.Clear();
      if (await TryUpdateModelAsync(Item, nameof(Item)) && await SaveAsync())
        return RedirectToPage(new { id = Item.Id });
      return Page();
    }
  }
}