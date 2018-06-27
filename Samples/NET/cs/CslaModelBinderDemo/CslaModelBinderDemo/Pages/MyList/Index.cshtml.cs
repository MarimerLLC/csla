using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication3.Pages.MyList
{
  public class IndexModel : PageModel
  {
    [BindProperty]
    public MyList DataList { get; set; }

    public async Task OnGetAsync()
    {
      DataList = await Csla.DataPortal.FetchAsync<MyList>();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      foreach (var item in DataList.Where(r => r.Remove).ToList())
        DataList.Remove(item);
      DataList = await DataList.SaveAsync();
      return RedirectToPage("Index");
    }
  }
}
