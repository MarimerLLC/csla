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
using System.Threading.Tasks;
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
    /// <param name="parameters">Parameters passed to data portal</param>
    /// <returns></returns>
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
        Console.Error.WriteLine(ex.ToString());
      }
      catch (Exception ex)
      {
        Model = default;
        ViewModelErrorText = ex.Message;
        Console.Error.WriteLine(ex.ToString());
      }
      return Model;
    }

    /// <summary>
    /// Override to provide custom Model refresh behavior
    /// </summary>
    /// <param name="parameters">Parameters passed to data portal</param>
    /// <returns></returns>
    protected virtual async Task<T> DoRefreshAsync(params object[] parameters)
    {
      if (typeof(Core.IReadOnlyObject).IsAssignableFrom(typeof(T)) ||
          typeof(Core.IReadOnlyCollection).IsAssignableFrom(typeof(T)) ||
          typeof(Core.IEditableCollection).IsAssignableFrom(typeof(T)))
      {
        if (Server.DataPortal.GetCriteriaFromArray(parameters) is Server.EmptyCriteria)
          return await DataPortal.FetchAsync();
        else
          return await DataPortal.FetchAsync(parameters);
      }
      else
      {
        if (Server.DataPortal.GetCriteriaFromArray(parameters) is Server.EmptyCriteria)
          return await DataPortal.CreateAsync();
        else
          return await DataPortal.FetchAsync(parameters);
      }
    }

    /// <summary>
    /// Saves the Model
    /// </summary>
    /// <returns></returns>
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
        Saved?.Invoke();
      }
      catch (Exception ex)
      {
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
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the metastate of the property.
    /// </summary>
    /// <typeparam name="P">Property type</typeparam>
    /// <param name="propertyName">Property name</param>
    /// <returns></returns>
    public PropertyInfo<P> GetPropertyInfo<P>(string propertyName)
    {
      PropertyInfo<P> result;
      if (_info.TryGetValue(propertyName, out object temp))
      {
        result = (PropertyInfo<P>)temp;
      }
      else
      {
        result = new PropertyInfo<P>(Model, propertyName);
        _info.Add(propertyName, result);
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
