using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DataPortalInstrumentation.Pages
{
  public class CallServerModel : PageModel
  {
    public async Task OnGetAsync()
    {
      await Csla.DataPortal.FetchAsync<Worker>(123);
    }
  }
}