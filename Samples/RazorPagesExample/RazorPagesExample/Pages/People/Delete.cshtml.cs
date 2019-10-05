using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLibrary;
using Csla;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesExample.Pages.People
{
  public class DeleteModel : PageModel
  {
    [BindProperty]
    public PersonEdit Item { get; set; }

    public async Task OnGet(int id)
    {
      Item = await DataPortal.FetchAsync<PersonEdit>(id);
    }

    public async Task<ActionResult> OnDelete()
    {
      await DataPortal.DeleteAsync<PersonEdit>(Item.Id);
      return RedirectToPage("/People/Index");
    }
  }
}