//-----------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base type for creating your own viewmodel</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Csla.Reflection;
using Csla.Rules;

namespace Csla.Blazor
{
  /// <summary>
  /// Base type for creating your own viewmodel.
  /// </summary>
  public class ViewModel<T>
  {
    private IDataPortal<T> DataPortal { get; set; }

    /// <summary>
    /// Event raised after Model has been saved
    /// </summary>
    public event Action Saved;
    /// <summary>
    /// Event raised when Model is changing
    /// </summary>
    public event Action<T, T> ModelChanging;
    /// <summary>
    /// Event raised when Model has changed
    /// </summary>
    public event Action ModelChanged;
    /// <summary>
    /// Event raised when the Model object
    /// raises its PropertyChanged event
    /// </summary>
    public event PropertyChangedEventHandler ModelPropertyChanged;

    /// <summary>
    /// Raises the ModelChanging event
    /// </summary>
    /// <param name="oldValue">Old Model value</param>
    /// <param name="newValue">New Model value</param>
    protected virtual void OnModelChanging(T oldValue, T newValue)
    {
      _info.Clear();
      if (oldValue is INotifyPropertyChanged oldObj)
        oldObj.PropertyChanged -= (s, e) => OnModelPropertyChanged(e.PropertyName);
      if (newValue is INotifyPropertyChanged newObj)
        newObj.PropertyChanged += (s, e) => OnModelPropertyChanged(e.PropertyName);
      ModelChanging?.Invoke(oldValue, newValue);
    }

    /// <summary>
    /// Raises the ModelChanged event
    /// </summary>
    protected virtual void OnModelChanged()
    {
      ModelChanged?.Invoke();
    }

    /// <summary>
    /// Raises the ModelPropertyChanged event
    /// </summary>
    /// <param name="propertyName"></param>
    protected virtual void OnModelPropertyChanged(string propertyName)
    {
      ModelPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    public ViewModel(IDataPortal<T> dataPortal)
    {
      DataPortal = dataPortal;
    }

    /// <summary>
    /// Refresh the Model
    /// </summary>
    /// <param name="factory">Async data portal or factory method</param>
    public async Task<T> RefreshAsync(Func<Task<T>> factory)
    {
      Exception = null;
      ViewModelErrorText = null;
      try
      {
        Model = await factory();
      }
      catch (DataPortalException ex)
      {
        Model = default;
        Exception = ex;
        ViewModelErrorText = ex.BusinessExceptionMessage;
      }
      catch (Exception ex)
      {
        Model = default;
        Exception = ex;
        ViewModelErrorText = ex.Message;
      }
      return Model;
    }

    /// <summary>
    /// Saves the Model
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
      Exception = null;
      ViewModelErrorText = null;
      if (Model is Core.ITrackStatus obj && !obj.IsSavable)
      {
        ViewModelErrorText = ModelErrorText;
        return;
      }
      try
      {
        Model = await DoSaveAsync();
        Saved?.Invoke();
      }
      catch (Exception ex)
      {
        Exception = ex;
        ViewModelErrorText = ex.Message;
        Console.Error.WriteLine(ex.ToString());
      }
    }

    /// <summary>
    /// Override to provide custom Model save behavior
    /// </summary>
    /// <returns></returns>
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

    private T _model;
    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T Model 
    {
      get => _model;
      set
      {
        if (!ReferenceEquals(_model, value))
        {
          OnModelChanging(_model, value);
          _model = value;
        }
      }
    }

    private readonly Dictionary<string, object> _info = new Dictionary<string, object>();

    /// <summary>
    /// Get a PropertyInfo object for a property.
    /// PropertyInfo provides access
    /// to the metastate of the property.
    /// </summary>
    /// <param name="property">Property expression</param>
    /// <returns></returns>
    public IPropertyInfo GetPropertyInfo<P>(Expression<Func<P>> property)
    {
      if (property == null)
        throw new ArgumentNullException(nameof(property));

      var keyName = property.GetKey();
      var identifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(property);
      return GetPropertyInfo(keyName, identifier.Model, identifier.FieldName);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property.
    /// PropertyInfo provides access
    /// to the metastate of the property.
    /// </summary>
    /// <param name="property">Property expression</param>
    /// <param name="id">Unique identifier for property in list or array</param>
    /// <returns></returns>
    public IPropertyInfo GetPropertyInfo<P>(Expression<Func<P>> property, string id)
    {
      if (property == null)
        throw new ArgumentNullException(nameof(property));

      var keyName = property.GetKey() + $"[{id}]";
      var identifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(property);
      return GetPropertyInfo(keyName, identifier.Model, identifier.FieldName);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the metastate of the property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <returns></returns>
    public IPropertyInfo GetPropertyInfo(string propertyName)
    {
      var keyName = Model.GetType().FullName + "." + propertyName;
      return GetPropertyInfo(keyName, Model, propertyName);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the metastate of the property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="id">Unique identifier for property in list or array</param>
    /// <returns></returns>
    public IPropertyInfo GetPropertyInfo(string propertyName, string id)
    {
      var keyName = Model.GetType().FullName + "." + propertyName + $"[{id}]";
      return GetPropertyInfo(keyName, Model, propertyName);
    }

    private IPropertyInfo GetPropertyInfo(string keyName, object model, string propertyName)
    {
      PropertyInfo result;
      if (_info.TryGetValue(keyName, out object temp))
      {
        result = (PropertyInfo)temp;
      }
      else
      {
        result = new PropertyInfo(model, propertyName);
        _info.Add(keyName, result);
      }
      return result;
    }

    /// <summary>
    /// Gets any error text generated by refresh or save operations
    /// </summary>
    public string ViewModelErrorText { get; protected set; }

    /// <summary>
    /// Gets the first validation error 
    /// message from the Model
    /// </summary>
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

    /// <summary>
    /// Gets the last exception caught by
    /// the viewmodel during refresh or save
    /// operations.
    /// </summary>
    public Exception Exception { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to create an instance of the
    /// business domain type
    /// </summary>
    /// <returns></returns>
    public static bool CanCreateObject()
    {
      return BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to retrieve an instance of the
    /// business domain type
    /// </summary>
    /// <returns></returns>
    public static bool CanGetObject()
    {
      return BusinessRules.HasPermission(AuthorizationActions.GetObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to edit/save an instance of the
    /// business domain type
    /// </summary>
    /// <returns></returns>
    public static bool CanEditObject()
    {
      return BusinessRules.HasPermission(AuthorizationActions.EditObject, typeof(T));
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to delete an instance of the
    /// business domain type
    /// </summary>
    /// <returns></returns>
    public static bool CanDeleteObject()
    {
      return BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(T));
    }
  }
}
