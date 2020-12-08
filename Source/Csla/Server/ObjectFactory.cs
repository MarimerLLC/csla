//-----------------------------------------------------------------------
// <copyright file="ObjectFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class to be used when creating a data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Server
{
  /// <summary>
  /// Base class to be used when creating a data portal
  /// factory object.
  /// </summary>
  public abstract class ObjectFactory
  {
    /// <summary>
    /// Sets the IsReadOnly property on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">Object on which to operate.</param>
    /// <param name="value">New value for IsReadOnly.</param>
    protected void SetIsReadOnly(object obj, bool value)
    {
      var list = obj as Csla.Core.IReadOnlyBindingList;
      if (list != null)
        list.IsReadOnly = value;
    }

    /// <summary>
    /// Calls the ValidationRules.CheckRules() method 
    /// on the specified object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    protected void CheckRules(object obj)
    {
      var target = obj as IDataPortalTarget;
      if (target != null)
        target.CheckRules();
      else
        MethodCaller.CallMethodIfImplemented(obj, "CheckRules", null);
    }

    /// <summary>
    /// Calls the MarkOld method on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    protected void MarkOld(object obj)
    {
      var target = obj as IDataPortalTarget;
      if (target != null)
        target.MarkOld();
      else
        MethodCaller.CallMethodIfImplemented(obj, "MarkOld", null);
    }

    /// <summary>
    /// Calls the MarkNew method on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    protected void MarkNew(object obj)
    {
      var target = obj as IDataPortalTarget;
      if (target != null)
        target.MarkNew();
      else
        MethodCaller.CallMethodIfImplemented(obj, "MarkNew", null);
    }

    /// <summary>
    /// Calls the MarkAsChild method on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    protected void MarkAsChild(object obj)
    {
      var target = obj as IDataPortalTarget;
      if (target != null)
        target.MarkAsChild();
      else
        MethodCaller.CallMethodIfImplemented(obj, "MarkAsChild", null);
    }

    /// <summary>
    /// Loads a property's managed field with the supplied value.
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
      var target = obj as Core.IManageProperties;
      if (target != null)
        target.LoadProperty<P>(propertyInfo, newValue);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// Loads a property's managed field with the supplied value.
    /// </summary>
    /// <param name="obj">Object on which to call the method.</param>
    /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">The new value for the property.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called,
    /// and no PropertyChanging or PropertyChanged events are raised.
    /// Loading values does not cause validation rules to be
    /// invoked.
    /// </remarks>
    protected void LoadProperty(object obj, IPropertyInfo propertyInfo, object newValue)
    {
      var target = obj as Core.IManageProperties;
      if (target != null)
        target.LoadProperty(propertyInfo, newValue);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// Reads a property's managed field value.
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <param name="obj">
    /// Object on which to call the method. 
    /// </param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called.
    /// </remarks>
    protected P ReadProperty<P>(object obj, PropertyInfo<P> propertyInfo)
    {
      var target = obj as Core.IManageProperties;
      if (target != null)
        return target.ReadProperty(propertyInfo);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// Reads a property's managed field value.
    /// </summary>
    /// <param name="obj">Object on which to call the method.</param>
    /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
    /// <returns></returns>
    /// <remarks>
    /// No authorization checks occur when this method is called.
    /// </remarks>
    protected object ReadProperty(object obj, IPropertyInfo propertyInfo)
    {
      var target = obj as IManageProperties;
      if (target != null)
        return target.ReadProperty(propertyInfo);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// By wrapping this property inside Using block
    /// you can set property values on 
    /// <paramref name="businessObject">business object</paramref>
    /// without raising PropertyChanged events
    /// and checking user rights.
    /// </summary>
    /// <param name="businessObject">
    /// Object on with you would like to set property values
    /// </param>
    /// <returns>
    /// An instance of IDisposable object that allows
    /// bypassing of normal authorization checks during
    /// property setting.
    /// </returns>
    protected IDisposable BypassPropertyChecks(Csla.Core.BusinessBase businessObject)
    {
      return businessObject.BypassPropertyChecks;
    }

    /// <summary>
    /// Gets a value indicating whether a managed field
    /// exists for the specified property.
    /// </summary>
    /// <param name="obj">Business object.</param>
    /// <param name="property">Property info object.</param>
    protected bool FieldExists(object obj, Csla.Core.IPropertyInfo property)
    {
      var target = obj as Core.IManageProperties;
      if (target != null)
        return target.FieldExists(property);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// Gets the list of deleted items from 
    /// an editable collection.
    /// </summary>
    /// <typeparam name="C">Type of child objects in the colletion.</typeparam>
    /// <param name="obj">Editable collection object.</param>
    protected Csla.Core.MobileList<C> GetDeletedList<C>(object obj)
    {
      var target = obj as Core.IEditableCollection;
      if (target != null)
        return (Csla.Core.MobileList<C>)target.GetDeletedList();
      else
        throw new ArgumentException(Resources.IEditableCollectionRequiredException);
    }
  }
}