using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace RazorPagesExample.Pages
{
  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  [IgnoreAntiforgeryToken]
  public class ErrorModel : PageModel
  {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public string? RequestId { get; set; }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
      _logger = logger;
    }

    public void OnGet()
    {
      RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
  }
}