﻿//-----------------------------------------------------------------------
// <copyright file="CslaPageModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Custom PageModel for CSLA .NET</summary>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Csla.Rules;
using Csla.Core;
using System.Diagnostics.CodeAnalysis;

namespace Csla.AspNetCore.RazorPages
{
  /// <summary>
  /// Custom PageModel for CSLA .NET
  /// </summary>
  public class PageModel<T> : PageModel
    where T : ISavable
  {
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public PageModel(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    /// <summary>
    /// Gets or sets the business domain model object.
    /// </summary>
    [BindProperty]
    public T? Item { get; set; }

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
          AddBrokenRuleInfo(Item, string.Empty);
        if (ModelState.IsValid)
        {
          ThrowIfItemIsNull();
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

    private void AddBrokenRuleInfo(T? item, string defaultText)
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
            var modelItem = ModelState.FirstOrDefault(r => r.Key == rule.Property || r.Key.EndsWith($".{rule.Property}"));
            var key = modelItem.Key ?? string.Empty;
            ModelState.AddModelError(key, rule.Description);
          }
        }
      }
      else
      {
        ModelState.AddModelError(string.Empty, defaultText);
      }
    }

    private readonly Dictionary<string, PropertyInfo> _info = [];

    /// <summary>
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the metastate of the property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public PropertyInfo GetPropertyInfo(string propertyName)
    {
      if (!_info.TryGetValue(propertyName, out PropertyInfo? info))
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
    public bool CanCreateItem()
    {
      return BusinessRules.HasPermission(_applicationContext, AuthorizationActions.CreateObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to retrieve an instance of the
    /// business domain type
    /// </summary>
    public bool CanGetItem()
    {
      return BusinessRules.HasPermission(_applicationContext, AuthorizationActions.GetObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to edit/save an instance of the
    /// business domain type
    /// </summary>
    public bool CanEditItem()
    {
      return BusinessRules.HasPermission(_applicationContext, AuthorizationActions.EditObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to delete an instance of the
    /// business domain type
    /// </summary>
    public bool CanDeleteItem()
    {
      return BusinessRules.HasPermission(_applicationContext, AuthorizationActions.DeleteObject, typeof(T));
    }

    [MemberNotNull(nameof(Item))]
    private void ThrowIfItemIsNull()
    {
      if (Item is null)
        throw new InvalidOperationException($"{nameof(Item)} == null");
    }
  }
}