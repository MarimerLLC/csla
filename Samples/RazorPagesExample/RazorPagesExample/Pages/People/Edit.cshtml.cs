using BusinessLibrary;
using Csla;
using Csla.AspNetCore.RazorPages;

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

    public async Task OnPost()
    {
      await SaveAsync((Item.Id > -1));
    }
  }
}