﻿//-----------------------------------------------------------------------
// <copyright file="ManagedObjectBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class for an object that is serializable</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Csla.Core.FieldManager;
using Csla.Reflection;
using Csla.Serialization.Mobile;

namespace Csla.Core
{
  /// <summary>
  /// Base class for an object that is serializable
  /// using SerializationFormatterFactory.GetFormatter().
  /// </summary>
  [Serializable]
  public abstract class ManagedObjectBase : MobileObject,
    INotifyPropertyChanged,
    IManageProperties,
    IUseApplicationContext,
    IUseFieldManager
  {
    /// <summary>
    /// Gets the current ApplicationContext.
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }
    ApplicationContext IUseApplicationContext.ApplicationContext { get => ApplicationContext; set => ApplicationContext = value; }

    #region Field Manager

    private FieldDataManager _fieldManager;
    /// <summary>
    /// Gets a reference to the field mananger
    /// for this object.
    /// </summary>
    protected FieldDataManager FieldManager
    {
      get
      {
        if (_fieldManager == null)
          _fieldManager = new FieldDataManager(ApplicationContext, GetType());
        return _fieldManager;
      }
    }

    FieldDataManager IUseFieldManager.FieldManager => FieldManager;

    #endregion

    #region  Register Properties

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the type.
    /// </summary>
    /// <typeparam name="P">
    /// Type of property.
    /// </typeparam>
    /// <param name="objectType">
    /// Type of object to which the property belongs.
    /// </param>
    /// <param name="info">
    /// PropertyInfo object for the property.
    /// </param>
    /// <returns>
    /// The provided IPropertyInfo object.
    /// </returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Type objectType, PropertyInfo<P> info)
    {
      return PropertyInfoManager.RegisterProperty<P>(objectType, info);
    }


    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="T">Type of object to which the property belongs.</typeparam>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <returns>The provided IPropertyInfo object.</returns>
    protected static PropertyInfo<P> RegisterProperty<T, P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(typeof(T), PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="T">Type of Target</typeparam>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="defaultValue">Default Value for the property</param>
    protected static PropertyInfo<P> RegisterProperty<T, P>(Expression<Func<T, object>> propertyLambdaExpression, P defaultValue)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(typeof(T), PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, reflectedPropertyInfo.Name, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <returns>The provided IPropertyInfo object.</returns>
    protected static PropertyInfo<P> RegisterProperty<T, P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(typeof(T), PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="T">Type of Target</typeparam>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    protected static PropertyInfo<P> RegisterProperty<T, P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(typeof(T), PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName, defaultValue));
    }

    #endregion

    #region  Read Properties

    /// <summary>
    /// Gets a property's value from the list of 
    /// managed field values, converting the 
    /// value to an appropriate type.
    /// </summary>
    /// <typeparam name="F">
    /// Type of the field.
    /// </typeparam>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    protected P ReadPropertyConvert<F, P>(PropertyInfo<F> propertyInfo)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, ReadProperty<F>(propertyInfo));
    }

    /// <summary>
    /// Gets a property's value as a specified type.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    protected P ReadProperty<P>(PropertyInfo<P> propertyInfo)
    {
      P result = default(P);
      IFieldData data = FieldManager.GetFieldData(propertyInfo);
      if (data != null)
      {
        if (data is IFieldData<P> fd)
          result = fd.Value;
        else
          result = (P)data.Value;
      }
      else
      {
        result = propertyInfo.DefaultValue;
        FieldManager.LoadFieldData<P>(propertyInfo, result);
      }
      return result;
    }

    /// <summary>
    /// Gets a property's value as a specified type.
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    protected virtual object ReadProperty(IPropertyInfo propertyInfo)
    {
      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
      {
        return MethodCaller.CallPropertyGetter(this, propertyInfo.Name);
      }

      object result = null;
      var info = FieldManager.GetFieldData(propertyInfo);
      if (info != null)
      {
        result = info.Value;
      }
      else
      {
        result = propertyInfo.DefaultValue;
        FieldManager.LoadFieldData(propertyInfo, result);
      }
      return result;
    }

    #endregion

    #region  Load Properties

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
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
    protected void LoadPropertyConvert<P, F>(PropertyInfo<P> propertyInfo, F newValue)
    {
      try
      {
        P oldValue = default(P);
        var fieldData = FieldManager.GetFieldData(propertyInfo);
        if (fieldData == null)
        {
          oldValue = propertyInfo.DefaultValue;
          fieldData = FieldManager.LoadFieldData<P>(propertyInfo, oldValue);
        }
        else
        {
          if (fieldData is IFieldData<P> fd)
            oldValue = fd.Value;
          else
            oldValue = (P)fieldData.Value;
        }
        LoadPropertyValue<P>(propertyInfo, oldValue, Utilities.CoerceValue<P>(typeof(F), oldValue, newValue));
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message));
      }
    }

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
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
    protected void LoadProperty<P>(PropertyInfo<P> propertyInfo, P newValue)
    {
      try
      {
        P oldValue = default(P);
        var fieldData = FieldManager.GetFieldData(propertyInfo);
        if (fieldData == null)
        {
          oldValue = propertyInfo.DefaultValue;
          fieldData = FieldManager.LoadFieldData<P>(propertyInfo, oldValue);
        }
        else
        {
          if (fieldData is IFieldData<P> fd)
            oldValue = fd.Value;
          else
            oldValue = (P)fieldData.Value;
        }
        LoadPropertyValue<P>(propertyInfo, oldValue, newValue);
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message));
      }
    }

    private bool LoadPropertyMarkDirty<P>(PropertyInfo<P> propertyInfo, P newValue)
    {
      try
      {
        P oldValue = default(P);
        var fieldData = FieldManager.GetFieldData(propertyInfo);
        if (fieldData == null)
        {
          oldValue = propertyInfo.DefaultValue;
          fieldData = FieldManager.LoadFieldData<P>(propertyInfo, oldValue);
        }
        else
        {
          if (fieldData is IFieldData<P> fd)
            oldValue = fd.Value;
          else
            oldValue = (P)fieldData.Value;
        }
        LoadPropertyValue<P>(propertyInfo, oldValue, newValue);
        return !oldValue.Equals(newValue);
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message));
      }
    }

    private void LoadPropertyValue<P>(PropertyInfo<P> propertyInfo, P oldValue, P newValue)
    {
      bool valuesDiffer;
      if (oldValue == null)
        valuesDiffer = newValue != null;
      else
        valuesDiffer = !oldValue.Equals(newValue);

      if (valuesDiffer)
        FieldManager.LoadFieldData<P>(propertyInfo, newValue);
    }

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
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
    protected virtual void LoadProperty(IPropertyInfo propertyInfo, object newValue)
    {
      var t = GetType();
      var method = t.GetRuntimeMethods().FirstOrDefault(c => c.Name == "LoadProperty" && c.IsGenericMethod);
      var gm = method.MakeGenericMethod(propertyInfo.Type);
      var p = new object[] { propertyInfo, newValue };
      gm.Invoke(this, p);
    }

    /// <summary>
    /// Loads the property vith new value and mark field dirty if value is changed.
    /// </summary>
    /// <param name="propertyInfo">The property info.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns>[true] if changed, else [false] </returns>
    protected virtual bool LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object newValue)
    {
      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
      {
        LoadProperty(propertyInfo, newValue);
        return false;
      }
      var t = GetType();
      var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      var method = t.GetMethods(flags).FirstOrDefault(c => c.Name == "LoadPropertyMarkDirty" && c.IsGenericMethod);
      var gm = method.MakeGenericMethod(propertyInfo.Type);
      var p = new object[] { propertyInfo, newValue };
      return (bool)gm.Invoke(this, p);
    }

    #endregion

    #region INotifyPropertyChanged Members

    [NonSerialized]
    [NotUndoable]
    private PropertyChangedEventHandler _propertyChanged;

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add { _propertyChanged = (PropertyChangedEventHandler)Delegate.Combine(_propertyChanged, value); }
      remove { _propertyChanged = (PropertyChangedEventHandler)Delegate.Remove(_propertyChanged, value); }
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property.</param>
    protected void OnPropertyChanged(string propertyName)
    {
      _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyInfo">The property info object for the changed property.</param>
    protected void OnPropertyChanged(IPropertyInfo propertyInfo)
    {
      OnPropertyChanged(propertyInfo.Name);
    }

    #endregion

    #region MobileObject

    /// <summary>
    /// Override this method to manually retrieve child
    /// object data from the serializations stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="formatter">Reference to the SerializationFormatterFactory.GetFormatter().</param>
    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (_fieldManager != null)
      {
        SerializationInfo child = formatter.SerializeObject(_fieldManager);
        info.AddChild("_fieldManager", child.ReferenceId);
      }

      base.OnGetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to manually serialize child
    /// objects into the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="formatter">Reference to the SerializationFormatterFactory.GetFormatter().</param>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Children.TryGetValue("_fieldManager", out var child))
      {
        int referenceId = child.ReferenceId;
        _fieldManager = (FieldDataManager)formatter.GetObject(referenceId);
      }

      base.OnSetChildren(info, formatter);
    }

    #endregion

    bool IManageProperties.FieldExists(IPropertyInfo property) => FieldManager.FieldExists(property);
    List<IPropertyInfo> IManageProperties.GetManagedProperties() => FieldManager.GetRegisteredProperties();
    object IManageProperties.GetProperty(IPropertyInfo propertyInfo) => throw new NotImplementedException();
    object IManageProperties.LazyGetProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator) => throw new NotImplementedException();
    object IManageProperties.LazyGetPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory) => throw new NotImplementedException();
    object IManageProperties.ReadProperty(IPropertyInfo propertyInfo) => ReadProperty(propertyInfo);
    P IManageProperties.ReadProperty<P>(PropertyInfo<P> propertyInfo) => ReadProperty(propertyInfo);
    P IManageProperties.LazyReadProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator) => throw new NotImplementedException();
    P IManageProperties.LazyReadPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory) => throw new NotImplementedException();
    void IManageProperties.SetProperty(IPropertyInfo propertyInfo, object newValue) => throw new NotImplementedException();
    void IManageProperties.LoadProperty(IPropertyInfo propertyInfo, object newValue) => LoadProperty(propertyInfo, newValue);
    bool IManageProperties.LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object newValue) => throw new NotImplementedException();
    void IManageProperties.LoadProperty<P>(PropertyInfo<P> propertyInfo, P newValue) => LoadProperty(propertyInfo, newValue);
    List<object> IManageProperties.GetChildren() => FieldManager.GetChildren();
    bool IManageProperties.HasManagedProperties => true;
  }
}