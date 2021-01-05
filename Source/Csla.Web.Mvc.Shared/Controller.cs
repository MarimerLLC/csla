//-----------------------------------------------------------------------
// <copyright file="Controller.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides methods that respond to HTTP requests</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NETSTANDARD2_0  || NET5_0 || NETCORE3_1
using System.Threading.Tasks;
using Csla.Core;
using Csla.Rules;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Provides methods that respond to HTTP requests
  /// in an ASP.NET MVC web site.
  /// </summary>
#if NETSTANDARD2_0 || NET5_0 || NETCORE3_1
  public class Controller : Microsoft.AspNetCore.Mvc.Controller
#else
  public class Controller : System.Web.Mvc.Controller
#endif
  {
#if NETSTANDARD2_0  || NET5_0 || NETCORE3_1
    /// <summary>
    /// Performs a Save() operation on an
    /// editable business object, with appropriate
    /// validation and exception handling.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="item">The business object to insert.</param>
    /// <param name="forceUpdate">true to force Save() to be an update.</param>
    /// <returns>true the Save() succeeds, false if not.</returns>
    protected async Task<bool> SaveObjectAsync<T>(T item, bool forceUpdate) 
      where T : class, Core.ISavable
    {
      return await SaveObjectAsync(item, null, forceUpdate);
    }

    /// <summary>
    /// Performs a Save() operation on an
    /// editable business object, with appropriate
    /// validation and exception handling.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="item">The business object to insert.</param>
    /// <param name="updateModel">Delegate that invokes the UpdateModel() method.</param>
    /// <param name="forceUpdate">true to force Save() to be an update.</param>
    /// <returns>true the Save() succeeds, false if not.</returns>
    protected virtual async Task<bool> SaveObjectAsync<T>(T item, Action<T> updateModel, bool forceUpdate) 
      where T : class, Core.ISavable
    {
      try
      {
        ViewData.Model = item;
        updateModel?.Invoke(item);
        if (item is BusinessBase bb && !bb.IsValid)
        {
          AddBrokenRuleInfo(item, null);
          return false;
        }
        ViewData.Model = await item.SaveAsync(forceUpdate);
        return true;
      }
      catch (ValidationException ex)
      {
        AddBrokenRuleInfo(item, ex.Message);
        return false;
      }
      catch (DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError(string.Empty, ex.BusinessException.Message);
        else
          ModelState.AddModelError(string.Empty, ex.Message);
        return false;
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);
        return false;
      }
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
            ModelState.AddModelError(string.Empty, rule.Description);
          else
            ModelState.AddModelError(rule.Property, rule.Description);
        }
      }
      else
      {
        ModelState.AddModelError(string.Empty, defaultText);
      }
    }
#else
    /// <summary>
    /// Performs a Save() operation on an
    /// editable business object, with appropriate
    /// validation and exception handling.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="item">The business object to insert.</param>
    /// <param name="forceUpdate">true to force Save() to be an update.</param>
    /// <returns>true the Save() succeeds, false if not.</returns>
    protected bool SaveObject<T>(T item, bool forceUpdate) 
      where T : class, Core.ISavable
    {
      return SaveObject(item,
        null,
        forceUpdate);
    }

    /// <summary>
    /// Performs a Save() operation on an
    /// editable business object, with appropriate
    /// validation and exception handling.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="item">The business object to insert.</param>
    /// <param name="updateModel">Delegate that invokes the UpdateModel() method.</param>
    /// <param name="forceUpdate">true to force Save() to be an update.</param>
    /// <returns>true the Save() succeeds, false if not.</returns>
    protected virtual bool SaveObject<T>(T item, Action<T> updateModel, bool forceUpdate) 
      where T : class, Core.ISavable
    {
      try
      {
        ViewData.Model = item;
        updateModel?.Invoke(item);
#if NETSTANDARD1_6
        ViewData.Model = item.SaveAsync(forceUpdate).Result;
#else
        ViewData.Model = item.Save(forceUpdate);
#endif
        return true;
      }
      catch (DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError(string.Empty, ex.BusinessException.Message);
        else
          ModelState.AddModelError(string.Empty, ex.Message);
        return false;
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);
        return false;
      }
    }
#endif

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="obj">
    /// Object on which to call the method. 
    /// </param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called,
    /// and no PropertyChanging or PropertyChanged events are raised.
    /// Loading values does not cause validation rules to be
    /// invoked.
    /// </remarks>
    protected void LoadProperty<P>(object obj, PropertyInfo<P> propertyInfo, P newValue)
    {
      new ObjectManager().LoadProperty(obj, propertyInfo, newValue);
    }

    private class ObjectManager : Server.ObjectFactory
    {
      public new void LoadProperty<P>(object obj, PropertyInfo<P> propertyInfo, P newValue)
      {
        base.LoadProperty(obj, propertyInfo, newValue);
      }
    }
  }
}
