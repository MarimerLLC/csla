using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLibrary;
using Csla;
using Csla.Core;
using Csla.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesExample.Pages.People
{
  public class EditModel : PageModel
  {
    [BindProperty]
    public PersonEdit Item { get; set; }

    public async Task OnGet(int id)
    {
      Item = await DataPortal.FetchAsync<PersonEdit>(id);
    }

    public async Task<ActionResult> OnPost()
    {
      try
      {
        if (Item is BusinessBase bb && !bb.IsValid)
          AddBrokenRuleInfo(Item, null);
        if (ModelState.IsValid)
        {
          Item = await Item.SaveAsync(true);
          return RedirectToPage("/People/Index");
        }
      }
      catch (ValidationException ex)
      {
        AddBrokenRuleInfo(Item, ex.Message);
      }
      catch (DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError(string.Empty, ex.BusinessException.Message);
        else
          ModelState.AddModelError(string.Empty, ex.Message);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);
      }
      return Page();
    }

    private void AddBrokenRuleInfo<T>(T item, string defaultText) where T : class, ISavable
    {
      if (item is BusinessBase bb)
      {
        var errors = bb.BrokenRulesCollection.
          Where(r => r.Severity == RuleSeverity.Error);
        foreach (var rule in errors)
        {
          if (string.IsNullOrEmpty(rule.Property))
          {
            ModelState.AddModelError(string.Empty, rule.Description);
          }
          else
          {
            var modelItem = ModelState.Where(r => r.Key == rule.Property || r.Key.EndsWith($".{rule.Property}")).FirstOrDefault();
            ModelState.AddModelError(modelItem.Key, rule.Description);
          }
        }
      }
      else
      {
        ModelState.AddModelError(string.Empty, defaultText);
      }
    }

    public string ErrorFor<T>(T source, string propertyName)
    {
      var result = string.Empty;
      if (source is BusinessBase bb)
        result = bb.BrokenRulesCollection.ToString(",", RuleSeverity.Error, propertyName);
      return result;
    }

    public string WarningFor<T>(T source, string propertyName)
    {
      var result = string.Empty;
      if (source is BusinessBase bb)
        result = bb.BrokenRulesCollection.ToString(",", RuleSeverity.Warning, propertyName);
      return result;
    }

    public string InformationFor<T>(T source, string propertyName)
    {
      var result = string.Empty;
      if (source is BusinessBase bb)
        result = bb.BrokenRulesCollection.ToString(",", RuleSeverity.Information, propertyName);
      return result;
    }
  }
}