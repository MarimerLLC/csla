//-----------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base type for creating your own viewmodel</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Csla.Rules;

namespace Csla.Blazor
{
  /// <summary>
  /// Base type for creating your own viewmodel.
  /// </summary>
  public class ViewModel<T>
  {
    private readonly IDataPortal<T> _dataPortal = new DataPortal<T>();

    public async Task<T> RefreshAsync(params object[] parameters)
    {
      try
      {
        Model = await DoRefreshAsync(parameters);
      }
      catch (DataPortalException ex)
      {
        Model = default;
        ViewModelErrorText = ex.BusinessException.Message;
        Console.WriteLine(ex.ToString());
      }
      catch (Exception ex)
      {
        Model = default;
        ViewModelErrorText = ex.Message;
        Console.WriteLine(ex.ToString());
      }
      return Model;
    }

    protected virtual async Task<T> DoRefreshAsync(params object[] parameters)
    {
      if (parameters == null || parameters.Length == 0 || (parameters.Length == 1 && parameters[0] == null))
        return await _dataPortal.CreateAsync();
      else
        return await _dataPortal.FetchAsync(parameters);
    }

    public async Task SaveAsync()
    {
      if (Model is Core.ITrackStatus obj && !obj.IsSavable)
      {
        ViewModelErrorText = ModelErrorText;
        return;
      }
      try
      {
        Model = await DoSaveAsync();
      }
      catch (Exception ex)
      {
        ViewModelErrorText = ex.Message;
        Console.WriteLine(ex.ToString());
      }
    }

    protected virtual async Task<T> DoSaveAsync()
    {
      if (Model is Core.ISavable savable)
      {
        var result = (T)await savable.SaveAsync();
        if (Model is Core.IEditableBusinessObject editable)
          new Core.GraphMerger().MergeGraph(editable, (Core.IEditableBusinessObject)result);
        else
          Model = result;
      }
      return Model;
    }

    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T Model { get; set; }

    public string ViewModelErrorText { get; set; }

    protected virtual string ModelErrorText
    {
      get
      {
        if (Model is IDataErrorInfo obj)
        {
          return obj.Error;
        }
        return string.Empty;
      }
    }

    public string GetErrorText(string propertyName)
    {
      var result = string.Empty;
      if (Model is Core.BusinessBase obj)
      {
        BrokenRule worst = (from r in obj.BrokenRulesCollection
                            where r.Property == propertyName &&
                                  r.Severity == RuleSeverity.Error
                            select r).FirstOrDefault();
        if (worst != null)
        {
          result = worst.Description;
        }
      }
      return result;
    }

    public string GetWarnText(string propertyName)
    {
      var result = string.Empty;
      if (Model is Core.BusinessBase obj)
      {
        BrokenRule worst = (from r in obj.BrokenRulesCollection
                            where r.Property == propertyName && 
                                  r.Severity == RuleSeverity.Warning
                            select r).FirstOrDefault();
        if (worst != null)
        {
          result = worst.Description;
        }
      }
      return result;
    }

    public string GetInfoText(string propertyName)
    {
      var result = string.Empty;
      if (Model is Core.BusinessBase obj)
      {
        BrokenRule worst = (from r in obj.BrokenRulesCollection
                            where r.Property == propertyName &&
                                  r.Severity == RuleSeverity.Information
                            select r).FirstOrDefault();
        if (worst != null)
        {
          result = worst.Description;
        }
      }
      return result;
    }

    public bool CanRead(string propertyName)
    {
      if (Model is Security.IAuthorizeReadWrite obj)
        return obj.CanReadProperty(propertyName);
      else
        return true;
    }

    public bool CanWrite(string propertyName)
    {
      if (Model is Security.IAuthorizeReadWrite obj)
        return obj.CanWriteProperty(propertyName);
      else
        return true;
    }

    public bool IsBusy(string propertyName)
    {
      if (Model is Core.BusinessBase obj)
        return obj.IsPropertyBusy(propertyName);
      else
        return false;
    }

    public static bool CanCreateObject()
    {
      return BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof(T));
    }

    public static bool CanGetObject()
    {
      return BusinessRules.HasPermission(AuthorizationActions.GetObject, typeof(T));
    }

    public static bool CanEditObject()
    {
      return BusinessRules.HasPermission(AuthorizationActions.EditObject, typeof(T));
    }

    public static bool CanDeleteObject()
    {
      return BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(T));
    }
  }
}
