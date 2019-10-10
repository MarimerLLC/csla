//-----------------------------------------------------------------------
// <copyright file="PropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes metastate for a property</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Csla.Rules;

namespace Csla.AspNetCore
{
  /// <summary>
  /// Exposes metastate for a property.
  /// </summary>
  public class PropertyInfo : INotifyPropertyChanged
  {
    private object Model { get; }
    private string PropertyName { get; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="model">Model object</param>
    /// <param name="propertyName">Property name</param>
    public PropertyInfo(object model, string propertyName)
    {
      Model = model;
      PropertyName = propertyName;
      if (Model is INotifyPropertyChanged npc)
      {
        npc.PropertyChanged += Npc_PropertyChanged;
      }
    }

    private void Npc_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsBusy")
        OnPropertyChanged(nameof(IsBusy));
    }

    /// <summary>
    /// Indicate that all properties have changed
    /// to trigger a UI refresh of all values.
    /// </summary>
    public void Refresh()
    {
      foreach (var item in this.GetType().GetProperties())
        OnPropertyChanged(item.Name);
    }

    /// <summary>
    /// Gets the validation error messages for a
    /// property on the Model
    /// </summary>
    /// <returns></returns>
    public string ErrorText
    {
      get
      {
        var result = string.Empty;
        if (Model is Core.BusinessBase obj)
          result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Error, PropertyName);
        return result;
      }
    }

    /// <summary>
    /// Gets the validation warning messages for a
    /// property on the Model
    /// </summary>
    /// <returns></returns>
    public string WarningText
    {
      get
      {
        var result = string.Empty;
        if (Model is Core.BusinessBase obj)
          result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Warning, PropertyName);
        return result;
      }
    }

    /// <summary>
    /// Gets the validation information messages for a
    /// property on the Model
    /// </summary>
    /// <returns></returns>
    public string InformationText
    {
      get
      {
        var result = string.Empty;
        if (Model is Core.BusinessBase obj)
          result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Information, PropertyName);
        return result;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to read the property on the Model
    /// </summary>
    /// <returns></returns>
    public bool CanRead
    {
      get
      {
        if (Model is Security.IAuthorizeReadWrite obj)
          return obj.CanReadProperty(PropertyName);
        else
          return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to change the property on the Model
    /// </summary>
    /// <returns></returns>
    public bool CanWrite
    {
      get
      {
        if (Model is Security.IAuthorizeReadWrite obj)
          return obj.CanWriteProperty(PropertyName);
        else
          return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the property 
    /// on the Model is busy
    /// </summary>
    /// <returns></returns>
    public bool IsBusy
    {
      get
      {
        if (Model is Core.BusinessBase obj)
          return obj.IsPropertyBusy(PropertyName);
        else
          return false;
      }
    }

    /// <summary>
    /// Event raised when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}