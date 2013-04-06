using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Csla.Core.FieldManager;
using Csla.Core.LoadManager;
using System.ComponentModel;
using Csla.Reflection;
using Csla.Serialization.Mobile;
using Csla.Serialization;

namespace Csla.Core
{
  /// <summary>
  /// Base class for an object that is serializable
  /// using MobileFormatter.
  /// </summary>
  [Serializable]
  public abstract class ManagedObjectBase : MobileObject,
    INotifyPropertyChanged
  {
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
          _fieldManager = new FieldDataManager(GetType());
        return _fieldManager;
      }
    } 

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
      return Core.FieldManager.PropertyInfoManager.RegisterProperty<P>(objectType, info);
    }


    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="T">Type of object to which the property belongs.</typeparam>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <returns>The provided IPropertyInfo object.</returns>
    protected static PropertyInfo<P> RegisterProperty<T,P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(typeof(T), Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name));
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
    protected static PropertyInfo<P> RegisterProperty<T,P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(typeof(T), Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName));
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
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<T,P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(typeof(T), Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName, defaultValue));
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
      FieldManager.IFieldData data = FieldManager.GetFieldData(propertyInfo);
      if (data != null)
      {
        FieldManager.IFieldData<P> fd = data as FieldManager.IFieldData<P>;
        if (fd != null)
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
    protected object ReadProperty(IPropertyInfo propertyInfo)
    {
      var info = FieldManager.GetFieldData(propertyInfo);
      if (info != null)
        return info.Value;
      else
        return null;
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
          var fd = fieldData as FieldManager.IFieldData<P>;
          if (fd != null)
            oldValue = fd.Value;
          else
            oldValue = (P)fieldData.Value;
        }
        LoadPropertyValue<P>(propertyInfo, oldValue, Utilities.CoerceValue<P>(typeof(F), oldValue, newValue), false);
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
          var fd = fieldData as FieldManager.IFieldData<P>;
          if (fd != null)
            oldValue = fd.Value;
          else
            oldValue = (P)fieldData.Value;
        }
        LoadPropertyValue<P>(propertyInfo, oldValue, newValue, false);
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message));
      }
    }

    private void LoadPropertyValue<P>(PropertyInfo<P> propertyInfo, P oldValue, P newValue, bool markDirty)
    {
      var valuesDiffer = false;
      if (oldValue == null)
        valuesDiffer = newValue != null;
      else
        valuesDiffer = !(oldValue.Equals(newValue));

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
    protected void LoadProperty(IPropertyInfo propertyInfo, object newValue)
    {
      FieldManager.LoadFieldData(propertyInfo, newValue);
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
      if (_propertyChanged != null)
        _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region MobileObject

    /// <summary>
    /// Override this method to manually retrieve child
    /// object data from the serializations stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="formatter">Reference to the MobileFormatter.</param>
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
    /// <param name="formatter">Reference to the MobileFormatter.</param>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Children.ContainsKey("_fieldManager"))
      {
        int referenceId = info.Children["_fieldManager"].ReferenceId;
        _fieldManager = (FieldDataManager)formatter.GetObject(referenceId);
      }

      base.OnSetChildren(info, formatter);
    }

    #endregion

    #region OnDeserialized

    #if SILVERLIGHT
    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should be deserialized. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <param name="mode">Serialization mode.</param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      base.OnSetState(info, mode);
    }
#else
    [System.Runtime.Serialization.OnDeserialized()]
    private void OnDeserializedHandler(System.Runtime.Serialization.StreamingContext context)
    {
      OnDeserialized(context);
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    /// <param name="context">Serialization context object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      if (FieldManager != null)
        FieldManager.SetPropertyList(this.GetType());
    }

#endif

    #endregion
  }
}
