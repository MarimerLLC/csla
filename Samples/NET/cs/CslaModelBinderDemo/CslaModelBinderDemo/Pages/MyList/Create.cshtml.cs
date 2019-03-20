using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication3.Pages.MyList
{
  public class CreateModel : PageModel
  {
    public async Task<IActionResult> OnGet()
    {
      MyItem = await Csla.DataPortal.CreateAsync<MyItem>();
      return Page();
    }

    [BindProperty]
    public MyItem MyItem { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      MyItem = await MyItem.SaveAsync();

      return RedirectToPage("./Index");
    }
  }
}