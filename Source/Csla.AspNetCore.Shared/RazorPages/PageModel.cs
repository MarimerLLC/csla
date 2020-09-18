//-----------------------------------------------------------------------
// <copyright file="CslaPageModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Custom PageModel for CSLA .NET</summary>
//-----------------------------------------------------------------------
#if !BLAZOR
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Csla.Rules;
using System.Linq;
using Csla.Core;
using System.Threading.Tasks;

namespace Csla.AspNetCore.RazorPages
{
  /// <summary>
  /// Custom PageModel for CSLA .NET
  /// </summary>
  public class PageModel<T> : PageModel
    where T : ISavable
  {
    /// <summary>
    /// Gets or sets the business domain model object.
    /// </summary>
    [BindProperty]
    public T Item { get; set; }

    /// <summary>
    /// Save the Item
    /// </summary>
    /// <param name="forceUpdate">Indicates whether an update operation should be forced.</param>
    /// <returns>True if the save operation succeeds</returns>
    public async Task<bool> SaveAsync(bool forceUpdate = false)
    {
      try
      {
        if (Item is BusinessBase bb && !bb.IsValid)
          AddBrokenRuleInfo(Item, null);
        if (ModelState.IsValid)
        {
          Item = (T) await Item.SaveAsync(forceUpdate);
          return true;
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
      return false;
    }

    private void AddBrokenRuleInfo(T item, string defaultText)
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

    private readonly Dictionary<string, PropertyInfo> _info = new Dictionary<string, PropertyInfo>();

    /// <summary>
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the metastate of the property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <returns></returns>
    public PropertyInfo GetPropertyInfo(string propertyName)
    {
      if (!_info.TryGetValue(propertyName, out PropertyInfo info))
      {
        info = new PropertyInfo(Item, propertyName);
        _info.Add(propertyName, info);
      }
      return info;
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to create an instance of the
    /// business domain type
    /// </summary>
    /// <returns></returns>
    public static bool CanCreateItem()
    {
      return BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to retrieve an instance of the
    /// business domain type
    /// </summary>
    /// <returns></returns>
    public static bool CanGetItem()
    {
      return BusinessRules.HasPermission(AuthorizationActions.GetObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to edit/save an instance of the
    /// business domain type
    /// </summary>
    /// <returns></returns>
    public static bool CanEditItem()
    {
      return BusinessRules.HasPermission(AuthorizationActions.EditObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to delete an instance of the
    /// business domain type
    /// </summary>
    /// <returns></returns>
    public static bool CanDeleteItem()
    {
      return BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(T));
    }
  }
}
#endif