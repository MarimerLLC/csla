//-----------------------------------------------------------------------
// <copyright file="ObjectFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class to be used when creating a data portal</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
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
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public ObjectFactory(ApplicationContext? applicationContext)
    {
      ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>
    /// Gets a reference to the current ApplicationContext.
    /// </summary>
    protected ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Sets the IsReadOnly property on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">Object on which to operate.</param>
    /// <param name="value">New value for IsReadOnly.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected void SetIsReadOnly(object obj, bool value)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      if (obj is IReadOnlyBindingList list)
        list.IsReadOnly = value;
    }

    /// <summary>
    /// Calls the ValidationRules.CheckRules() method 
    /// on the specified object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected void CheckRules(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      if (obj is IDataPortalTarget target)
        target.CheckRules();
      else
        MethodCaller.CallMethodIfImplemented(obj, "CheckRules", null);
    }

    /// <summary>
    /// Calls the ValidationRules.CheckRules() method 
    /// on the specified object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected async Task CheckRulesAsync(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      if (obj is IDataPortalTarget target)
        await target.CheckRulesAsync().ConfigureAwait(false);
      else
        MethodCaller.CallMethodIfImplemented(obj, "CheckRules", null);
    }

    /// <summary>
    /// Calls the WaitForIdle() method on the specified object with the default timeout, if possible.
    /// </summary>
    /// <param name="obj">Object on which to call the method.</param>
    /// <returns>void</returns>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected async Task WaitForIdle(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      var cslaOptions = ApplicationContext.GetRequiredService<Csla.Configuration.CslaOptions>();
      await WaitForIdle(obj, TimeSpan.FromSeconds(cslaOptions.DefaultWaitForIdleTimeoutInSeconds).ToCancellationToken()).ConfigureAwait(false);
    }

    /// <summary>
    /// Calls the WaitForIdle() method on the specified object with the given timeout, if possible.
    /// </summary>
    /// <param name="obj">Object on which to call the method.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>void</returns>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected async Task WaitForIdle(object obj, CancellationToken ct)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      if (obj is IDataPortalTarget target)
      {
        await target.WaitForIdle(ct).ConfigureAwait(false);
      }
      else if (obj is INotifyBusy notifyBusy)
      {
        await BusyHelper.WaitForIdle(notifyBusy, ct).ConfigureAwait(false);
      }
      else
      {
        MethodCaller.CallMethodIfImplemented(obj, nameof(IDataPortalTarget.WaitForIdle), ct);
      }
    }

    /// <summary>
    /// Calls the MarkOld method on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected void MarkOld(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      if (obj is IDataPortalTarget target)
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
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected void MarkNew(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      if (obj is IDataPortalTarget target)
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
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected void MarkAsChild(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      if (obj is IDataPortalTarget target)
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
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected void LoadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(object obj, PropertyInfo<P> propertyInfo, P? newValue)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      if (obj is IManageProperties target)
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
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected void LoadProperty(object obj, IPropertyInfo propertyInfo, object? newValue)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      if (obj is IManageProperties target)
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
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? ReadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(object obj, PropertyInfo<P> propertyInfo)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      if (obj is IManageProperties target)
        return target.ReadProperty(propertyInfo);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// Reads a property's managed field value.
    /// </summary>
    /// <param name="obj">Object on which to call the method.</param>
    /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected object? ReadProperty(object obj, IPropertyInfo propertyInfo)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      if (obj is IManageProperties target)
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
    /// <exception cref="ArgumentNullException"><paramref name="businessObject"/> is <see langword="null"/>.</exception>
    protected IDisposable BypassPropertyChecks(Csla.Core.BusinessBase businessObject)
    {
      if (businessObject is null)
        throw new ArgumentNullException(nameof(businessObject));

      return businessObject.BypassPropertyChecks;
    }

    /// <summary>
    /// Gets a value indicating whether a managed field
    /// exists for the specified property.
    /// </summary>
    /// <param name="obj">Business object.</param>
    /// <param name="property">Property info object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="property"/> is <see langword="null"/>.</exception>
    protected bool FieldExists(object obj, Csla.Core.IPropertyInfo property)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      if (obj is IManageProperties target)
        return target.FieldExists(property);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// Gets the list of deleted items from 
    /// an editable collection.
    /// </summary>
    /// <typeparam name="C">Type of child objects in the collection.</typeparam>
    /// <param name="obj">Editable collection object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    protected Csla.Core.MobileList<C> GetDeletedList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] C>(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      if (obj is IEditableCollection target)
        return (Csla.Core.MobileList<C>)target.GetDeletedList();
      else
        throw new ArgumentException(Resources.IEditableCollectionRequiredException);
    }
  }
}