using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLibrary;
using Csla;
using Csla.AspNetCore.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesExample.Pages.People
{
  public class DeleteModel : PageModel<PersonEdit>
  {
    public async Task OnGet(int id)
    {
      Item = await DataPortal.FetchAsync<PersonEdit>(id);
    }

    public async Task<ActionResult> OnPost()
    {
      await DataPortal.DeleteAsync<PersonEdit>(Item.Id);
      return RedirectToPage("/People/Index");
    }
  }
}