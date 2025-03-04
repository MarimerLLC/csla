//-----------------------------------------------------------------------
// <copyright file="PropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes metastate for a property</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Rules;

namespace Csla.Blazor
{
  /// <summary>
  /// Exposes metastate for a property.
  /// </summary>
  public class PropertyInfo : IPropertyInfo, INotifyPropertyChanged
  {
    /// <summary>
    /// Gets the value of the TextSeparator
    /// </summary>
    public string TextSeparator { get; }
    /// <summary>
    /// Gets the model
    /// </summary>
    protected object Model { get; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="model">Model object</param>
    /// <param name="propertyName">Property name</param>
    /// <param name="textSeprator">Text Seprator</param>
    public PropertyInfo(object model, string propertyName, string textSeprator = " ")
    {
      Model = model;
      PropertyName = propertyName;
      if (Model is INotifyPropertyChanged npc)
      {
        npc.PropertyChanged += Npc_PropertyChanged;
      }
      TextSeparator = textSeprator;
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
      foreach (var item in GetType().GetProperties())
        OnPropertyChanged(item.Name);
    }

    /// <summary>
    /// Gets or sets the value of the property
    /// </summary>
    public object Value
    {
      get => Utilities.CallByName(Model, PropertyName, CallType.Get);
      set => Utilities.CallByName(Model, PropertyName, CallType.Set, value);
    }

    /// <summary>
    /// Gets the friendly name for the property.
    /// </summary>
    public string FriendlyName
    {
      get
      {
        var pi = Core.FieldManager.PropertyInfoManager.GetRegisteredProperty(Model.GetType(), PropertyName);
        return pi.FriendlyName;
      }
    }

    /// <summary>
    /// Gets the property name for the property.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the validation error messages for a
    /// property on the Model
    /// </summary>
    public string ErrorText
    {
      get
      {
        var result = string.Empty;
        if (Model is Core.BusinessBase obj)
          result = obj.BrokenRulesCollection.ToString(TextSeparator, RuleSeverity.Error, PropertyName);
        return result;
      }
    }

    /// <summary>
    /// Gets the validation warning messages for a
    /// property on the Model
    /// </summary>
    public string WarningText
    {
      get
      {
        var result = string.Empty;
        if (Model is Core.BusinessBase obj)
          result = obj.BrokenRulesCollection.ToString(TextSeparator, RuleSeverity.Warning, PropertyName);
        return result;
      }
    }

    /// <summary>
    /// Gets the validation information messages for a
    /// property on the Model
    /// </summary>
    public string InformationText
    {
      get
      {
        var result = string.Empty;
        if (Model is Core.BusinessBase obj)
          result = obj.BrokenRulesCollection.ToString(TextSeparator, RuleSeverity.Information, PropertyName);
        return result;
      }
    }

    /// <summary>
    /// Gets the list of broken rules for 
    /// a property on the Model
    /// </summary>
    public List<BrokenRule> BrokenRules
    {
      get
      {
        if (Model is Core.BusinessBase obj)
          return [.. obj.BrokenRulesCollection];
        return [];
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to read the property on the Model
    /// </summary>
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

    /// <summary>
    /// Gets the System.Reflection.PropertyInfo object
    /// representing the property.
    /// </summary>
    public System.Reflection.PropertyInfo GetPropertyInfo() 
      => Model.GetType().GetProperty(PropertyName);
  }
}
