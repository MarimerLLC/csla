using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLibrary;
using Csla;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesExample.Pages
{
  public class ListPeopleModel : PageModel
  {

    public PersonList PersonList { get; set; }

    public async Task OnGet()
    {
      PersonList = await DataPortal.FetchAsync<PersonList>();
    }
  }
}