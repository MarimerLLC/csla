using System.Threading.Tasks;
using Csla;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DataPortalInstrumentation.Pages
{
  public class CallServerModel : PageModel
  {
    public CallServerModel(IDataPortal<Worker> portal)
    {
      Portal = portal;
    }

    private IDataPortal<Worker> Portal;

    public async Task OnGetAsync()
    {
      await Portal.FetchAsync(123);
    }
  }
}