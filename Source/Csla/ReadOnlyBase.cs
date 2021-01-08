//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is a base class from which readonly business classes</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Core.FieldManager;
using Csla.Core.LoadManager;
using Csla.Properties;
using Csla.Reflection;
using Csla.Rules;
using Csla.Security;
using Csla.Serialization.Mobile;
using Csla.Server;
using System.Collections.Concurrent;

namespace Csla
{
  /// <summary>
  /// This is a base class from which readonly business classes
  /// can be derived.
  /// </summary>
  /// <remarks>
  /// This base class only supports data retrieve, not updating or
  /// deleting. Any business classes derived from this base class
  /// should only implement readonly properties.
  /// </remarks>
  /// <typeparam name="T">Type of the business object.</typeparam>
  [Serializable()]
  public abstract class ReadOnlyBase<T> : BindableBase,
    ICloneable,
    IReadOnlyObject,
    ISerializationNotification,
    IAuthorizeReadWrite,
    IDataPortalTarget,
    IManageProperties,
    INotifyBusy,
    IHostRules,
    IReadOnlyBase
    where T : ReadOnlyBase<T>
  {
    #region Object ID Value

    /// <summary>
    /// Override this method to return a unique identifying
    /// value for this object.
    /// </summary>
    protected virtual object GetIdValue()
    {
      return null;
    }

    #endregion

    #region System.Object Overrides

    /// <summary>
    /// Returns a text representation of this object by
    /// returning the <see cref="GetIdValue"/> value
    /// in text form.
    /// </summary>
    public override string ToString()
    {
      object id = GetIdValue();
      if (id == null)
        return base.ToString();
      else
        return id.ToString();
    }

    #endregion

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected ReadOnlyBase()
    {
      Initialize();
      InitializeBusinessRules();
    }

    #region Initialize

    /// <summary>
    /// Override this method to set up event handlers so user
    /// code in a partial class can respond to events raised by
    /// generated code.
    /// </summary>
    protected virtual void Initialize()
    { /* allows subclass to initialize events before any other activity occurs */ }

    #endregion

    #region Identity

    int IBusinessObject.Identity
    {
      get { return 0; }
    }

    #endregion

    #region Authorization

    [NotUndoable()]
    [NonSerialized()]
    private ConcurrentDictionary<string, bool> _readResultCache;
    [NotUndoable()]
    [NonSerialized()]
    private ConcurrentDictionary<string, bool> _executeResultCache;
    [NotUndoable()]
    [NonSerialized()]
    private System.Security.Principal.IPrincipal _lastPrincipal;

    private void InitializeBusinessRules()
    {
      var rules = BusinessRuleManager.GetRulesForType(this.GetType());
      if (!rules.Initialized)
        lock (rules)
          if (!rules.Initialized)
          {
            try
            {
              AddBusinessRules();
              rules.Initialized = true;
            }
            catch (Exception)
            {
              BusinessRuleManager.CleanupRulesForType(this.GetType());
              throw;  // and rethrow exception
            }
          }
    }

    private Csla.Rules.BusinessRules _businessRules;

    /// <summary>
    /// Provides access to the broken rules functionality.
    /// </summary>
    /// <remarks>
    /// This property is used within your business logic so you can
    /// easily call the AddRule() method to associate business
    /// rules with your object's properties.
    /// </remarks>
    protected BusinessRules BusinessRules
    {
      get
      {
        if (_businessRules == null)
          _businessRules = new BusinessRules(this);
        else if (_businessRules.Target == null)
          _businessRules.SetTarget(this);
        return _businessRules;
      }
    }

    void IHostRules.RuleStart(IPropertyInfo property)
    { }

    void IHostRules.RuleComplete(IPropertyInfo property)
    { }

    void IHostRules.RuleComplete(string property)
    { }

    void Rules.IHostRules.AllRulesComplete()
    { }

    /// <summary>
    /// Override this method to add per-type
    /// authorization rules for your type's properties.
    /// </summary>
    /// <remarks>
    /// AddSharedAuthorizationRules is automatically called by CSLA .NET
    /// when your object should associate per-type authorization roles
    /// with its properties.
    /// </remarks>
    protected virtual void AddBusinessRules()
    { }

    /// <summary>
    /// Returns true if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <param name="property">Property to check.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanReadProperty(Csla.Core.IPropertyInfo property)
    {
      bool result = true;

      VerifyAuthorizationCache();

      if (!_readResultCache.TryGetValue(property.Name, out result))
      {
        result = BusinessRules.HasPermission(AuthorizationActions.ReadProperty, property);
        // store value in cache
        _readResultCache.AddOrUpdate(property.Name, result, (a, b) => { return result; });
      }
      return result;
    }

    /// <summary>
    /// Returns true if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns>true if read is allowed.</returns>
    /// <param name="property">Property to read.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool CanReadProperty(Csla.Core.IPropertyInfo property, bool throwOnFalse)
    {
      bool result = CanReadProperty(property);
      if (throwOnFalse && result == false)
      {
        Csla.Security.SecurityException ex = new Csla.Security.SecurityException(
          String.Format("{0} ({1})",
          Resources.PropertyGetNotAllowed, property.Name));
        throw ex;
      }
      return result;
    }

    /// <summary>
    /// Returns true if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool CanReadProperty(string propertyName)
    {
      return CanReadProperty(propertyName, false);
    }

    /// <summary>
    /// Returns true if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    private bool CanReadProperty(string propertyName, bool throwOnFalse)
    {
      var propertyInfo = FieldManager.GetRegisteredProperties().FirstOrDefault(p => p.Name == propertyName);
      if (propertyInfo == null)
      {
#if NETFX_CORE || (ANDROID || IOS)
#else
        Trace.TraceError("CanReadProperty: {0} is not a registered property of {1}.{2}", propertyName, this.GetType().Namespace, this.GetType().Name);
#endif
        return true;
      }
      return CanReadProperty(propertyInfo, throwOnFalse);
    }

    bool Csla.Security.IAuthorizeReadWrite.CanWriteProperty(string propertyName)
    {
      return false;
    }

    bool Csla.Security.IAuthorizeReadWrite.CanWriteProperty(IPropertyInfo property)
    {
      return false;
    }

    private void VerifyAuthorizationCache()
    {
      if (_readResultCache == null)
        _readResultCache = new ConcurrentDictionary<string, bool>();
      if (_executeResultCache == null)
        _executeResultCache = new ConcurrentDictionary<string, bool>();
      if (!ReferenceEquals(Csla.ApplicationContext.User, _lastPrincipal))
      {
        // the principal has changed - reset the cache
        _readResultCache.Clear();
        _executeResultCache.Clear();
        _lastPrincipal = Csla.ApplicationContext.User;
      }
    }

    /// <summary>
    /// Returns true if the user is allowed to execute
    /// the specified method.
    /// </summary>
    /// <param name="method">Method to execute.</param>
    /// <returns>true if execute is allowed.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanExecuteMethod(Csla.Core.IMemberInfo method)
    {
      bool result = true;

      VerifyAuthorizationCache();

      if (!_executeResultCache.TryGetValue(method.Name, out result))
      {
        result = BusinessRules.HasPermission(AuthorizationActions.ExecuteMethod, method);
        _executeResultCache.AddOrUpdate(method.Name, result, (a, b) => { return result; });
      }
      return result;
    }

    /// <summary>
    /// Returns true if the user is allowed to execute
    /// the specified method.
    /// </summary>
    /// <returns>true if execute is allowed.</returns>
    /// <param name="method">Method to execute.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool CanExecuteMethod(Csla.Core.IMemberInfo method, bool throwOnFalse)
    {

      bool result = CanExecuteMethod(method);
      if (throwOnFalse && result == false)
      {
        Csla.Security.SecurityException ex =
          new Csla.Security.SecurityException(string.Format("{0} ({1})", Properties.Resources.MethodExecuteNotAllowed, method.Name));
        throw ex;
      }
      return result;

    }


    /// <summary>
    /// Returns true if the user is allowed to execute
    /// the specified method.
    /// </summary>
    /// <param name="methodName">Name of the method to execute.</param>
    /// <returns>true if execute is allowed.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanExecuteMethod(string methodName)
    {
      return CanExecuteMethod(methodName, false);
    }

    private bool CanExecuteMethod(string methodName, bool throwOnFalse)
    {

      bool result = CanExecuteMethod(new MethodInfo(methodName));
      if (throwOnFalse && result == false)
      {
        Csla.Security.SecurityException ex = new Csla.Security.SecurityException(string.Format("{0} ({1})", Properties.Resources.MethodExecuteNotAllowed, methodName));
        throw ex;
      }
      return result;
    }

#endregion

#region IClonable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual object GetClone()
    {
      return Core.ObjectCloner.Clone(this);
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    public T Clone()
    {
      return (T)GetClone();
    }
#endregion

#region Data Access

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
    }

#endregion

#region Serialization Notification

    void ISerializationNotification.Deserialized()
    {
      OnDeserializedHandler(new System.Runtime.Serialization.StreamingContext());
    }

    [System.Runtime.Serialization.OnDeserialized()]
    private void OnDeserializedHandler(System.Runtime.Serialization.StreamingContext context)
    {
      if (_fieldManager != null)
        FieldManager.SetPropertyList(this.GetType());
      InitializeBusinessRules();
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
      // do nothing - this is here so a subclass
      // could override if needed
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
    /// <typeparam name="P">
    /// Type of property.
    /// </typeparam>
    /// <param name="info">
    /// PropertyInfo object for the property.
    /// </param>
    /// <returns>
    /// The provided IPropertyInfo object.
    /// </returns>
    protected static PropertyInfo<P> RegisterProperty<P>(PropertyInfo<P> info)
    {
      return Core.FieldManager.PropertyInfoManager.RegisterProperty<P>(typeof(T), info);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(string propertyName)
    {
      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <returns></returns>
    [Obsolete]
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, P defaultValue)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, reflectedPropertyInfo.Name, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(string propertyName, string friendlyName)
    {
      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, friendlyName);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(string propertyName, string friendlyName, P defaultValue)
    {
      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, friendlyName, defaultValue);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(string propertyName, RelationshipTypes relationship)
    {
      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, string.Empty, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, relationship);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <returns></returns>
    [Obsolete]
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(string propertyName, string friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship);
    }
    #endregion

    #region Register Methods

    /// <summary>
    /// Indicates that the specified method belongs
    /// to the type.
    /// </summary>
    /// <param name="objectType">
    /// Type of object to which the method belongs.
    /// </param>
    /// <param name="info">
    /// IMemberInfo object for the property.
    /// </param>
    /// <returns>
    /// The provided IMemberInfo object.
    /// </returns>
    protected static Csla.Core.IMemberInfo RegisterMethod(Type objectType, IMemberInfo info)
    {
      var reflected = objectType.GetMethod(info.Name);
      if (reflected == null)
        throw new ArgumentException(string.Format(Resources.NoSuchMethod, info.Name), "info");
      return info;
    }

    /// <summary>
    /// Indicates that the specified method belongs
    /// to the type.
    /// </summary>
    /// <param name="objectType">
    /// Type of object to which the method belongs.
    /// </param>
    /// <param name="methodName">
    /// Name of the method.
    /// </param>
    /// <returns>
    /// The provided IMemberInfo object.
    /// </returns>
    protected static MethodInfo RegisterMethod(Type objectType, string methodName)
    {
      var info = new MethodInfo(methodName);
      RegisterMethod(objectType, info);
      return info;
    }

    /// <summary>
    /// Registers a method for use in Authorization.
    /// </summary>
    /// <param name="methodName">Method name from nameof()</param>
    /// <returns></returns>
    protected static MethodInfo RegisterMethod(string methodName)
    {
      return RegisterMethod(typeof(T), methodName);
    }

    /// <summary>
    /// Registers the method.
    /// </summary>
    /// <param name="methodLambdaExpression">The method lambda expression.</param>
    /// <returns></returns>
    protected static MethodInfo RegisterMethod(Expression<Action<T>> methodLambdaExpression)
    {
      System.Reflection.MethodInfo reflectedMethodInfo = Reflect<T>.GetMethod(methodLambdaExpression);
      return RegisterMethod(reflectedMethodInfo.Name);
    }

#endregion

#region  Get Properties

    /// <summary>
    /// Gets a property's value, first checking authorization.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="field">
    /// The backing field for the property.</param>
    /// <param name="propertyName">
    /// The name of the property.</param>
    /// <param name="defaultValue">
    /// Value to be returned if the user is not
    /// authorized to read the property.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<P>(string propertyName, P field, P defaultValue)
    {
      return GetProperty<P>(propertyName, field, defaultValue, Security.NoAccessBehavior.SuppressException);
    }

    /// <summary>
    /// Gets a property's value, first checking authorization.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="field">
    /// The backing field for the property.</param>
    /// <param name="propertyName">
    /// The name of the property.</param>
    /// <param name="defaultValue">
    /// Value to be returned if the user is not
    /// authorized to read the property.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    protected P GetProperty<P>(string propertyName, P field, P defaultValue, Security.NoAccessBehavior noAccess)
    {
#region Check to see if the property is marked with RelationshipTypes.PrivateField

      var propertyInfo = FieldManager.GetRegisteredProperty(propertyName);

      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) != RelationshipTypes.PrivateField)
        throw new InvalidOperationException(Resources.PrivateFieldException);

#endregion

      if (CanReadProperty(propertyInfo, noAccess == Csla.Security.NoAccessBehavior.ThrowException))
        return field;

      return defaultValue;
    }

    /// <summary>
    /// Gets a property's value, first checking authorization.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="field">
    /// The backing field for the property.</param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<P>(PropertyInfo<P> propertyInfo, P field)
    {
      return GetProperty<P>(propertyInfo.Name, field, propertyInfo.DefaultValue, Security.NoAccessBehavior.SuppressException);
    }

    /// <summary>
    /// Gets a property's value, first checking authorization.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="field">
    /// The backing field for the property.</param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="defaultValue">
    /// Value to be returned if the user is not
    /// authorized to read the property.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    protected P GetProperty<P>(PropertyInfo<P> propertyInfo, P field, P defaultValue, Security.NoAccessBehavior noAccess)
    {
      return GetProperty<P>(propertyInfo.Name, field, defaultValue, noAccess);
    }

    /// <summary>
    /// Lazily initializes a property and returns
    /// the resulting value.
    /// </summary>
    /// <typeparam name="P">Type of the property.</typeparam>
    /// <param name="property">PropertyInfo object containing property metadata.</param>
    /// <param name="valueGenerator">Method returning the new value.</param>
    /// <returns></returns>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P LazyGetProperty<P>(PropertyInfo<P> property, Func<P> valueGenerator)
    {
      if (!(FieldManager.FieldExists(property)))
      {
        var result = valueGenerator();
        LoadProperty(property, result);
      }
      return GetProperty<P>(property);
    }

    private List<Csla.Core.IPropertyInfo> _lazyLoadingProperties = new List<Csla.Core.IPropertyInfo>();

    /// <summary>
    /// Lazily initializes a property and returns
    /// the resulting value.
    /// </summary>
    /// <typeparam name="P">Type of the property.</typeparam>
    /// <param name="property">PropertyInfo object containing property metadata.</param>
    /// <param name="factory">Async method returning the new value.</param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that the first value returned is almost certainly
    /// the defaultValue because the value is initialized asynchronously.
    /// The real value is provided later along with a PropertyChanged
    /// event to indicate the value has changed.
    /// </para><para>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </para>
    /// </remarks>
    protected P LazyGetPropertyAsync<P>(PropertyInfo<P> property, Task<P> factory)
    {
      if (!(FieldManager.FieldExists(property)) && !_lazyLoadingProperties.Contains(property))
      {
        _lazyLoadingProperties.Add(property);
        LoadPropertyAsync(property, factory);
      }
      return GetProperty<P>(property);
    }

    /// <summary>
    /// Gets a property's value as 
    /// a specified type, first checking authorization.
    /// </summary>
    /// <typeparam name="F">
    /// Type of the field.
    /// </typeparam>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="field">
    /// The backing field for the property.</param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetPropertyConvert<F, P>(PropertyInfo<F> propertyInfo, F field)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo.Name, field, propertyInfo.DefaultValue, Security.NoAccessBehavior.SuppressException));
    }

    /// <summary>
    /// Gets a property's value as a specified type, 
    /// first checking authorization.
    /// </summary>
    /// <typeparam name="F">
    /// Type of the field.
    /// </typeparam>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="field">
    /// The backing field for the property.</param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetPropertyConvert<F, P>(PropertyInfo<F> propertyInfo, F field, Security.NoAccessBehavior noAccess)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo.Name, field, propertyInfo.DefaultValue, noAccess));
    }

    /// <summary>
    /// Gets a property's managed field value, 
    /// first checking authorization.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<P>(PropertyInfo<P> propertyInfo)
    {
      return GetProperty<P>(propertyInfo, Security.NoAccessBehavior.SuppressException);
    }

    /// <summary>
    /// Gets a property's value from the list of 
    /// managed field values, first checking authorization,
    /// and converting the value to an appropriate type.
    /// </summary>
    /// <typeparam name="F">
    /// Type of the field.
    /// </typeparam>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetPropertyConvert<F, P>(PropertyInfo<F> propertyInfo)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo, Security.NoAccessBehavior.SuppressException));
    }

    /// <summary>
    /// Gets a property's value from the list of 
    /// managed field values, first checking authorization,
    /// and converting the value to an appropriate type.
    /// </summary>
    /// <typeparam name="F">
    /// Type of the field.
    /// </typeparam>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetPropertyConvert<F, P>(PropertyInfo<F> propertyInfo, Security.NoAccessBehavior noAccess)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo, noAccess));
    }

    /// <summary>
    /// Gets a property's value as a specified type, 
    /// first checking authorization.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<P>(PropertyInfo<P> propertyInfo, Security.NoAccessBehavior noAccess)
    {
      P result = default(P);
      if (CanReadProperty(propertyInfo, noAccess == Csla.Security.NoAccessBehavior.ThrowException))
        result = ReadProperty<P>(propertyInfo);
      else
        result = propertyInfo.DefaultValue;
      return result;
    }

    /// <summary>
    /// Gets a property's value as a specified type.
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected object GetProperty(IPropertyInfo propertyInfo)
    {
      object result = null;
      if (CanReadProperty(propertyInfo, false))
      {
        // call ReadProperty (may be overloaded in actual class)
        result = ReadProperty(propertyInfo);
      }
      else
      {
        result = propertyInfo.DefaultValue;
      }
      return result;
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
        IFieldData<P> fd = data as IFieldData<P>;
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

    /// <summary>
    /// Gets a property's value as a specified type.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="property">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="valueGenerator">Method returning the new value.</param>
    protected P LazyReadProperty<P>(PropertyInfo<P> property, Func<P> valueGenerator)
    {
      if (!(FieldManager.FieldExists(property)))
      {
        var result = valueGenerator();
        LoadProperty(property, result);
      }
      return ReadProperty<P>(property);
    }

    /// <summary>
    /// Gets a property's value as a specified type.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="property">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="factory">Async method returning the new value.</param>
    protected P LazyReadPropertyAsync<P>(PropertyInfo<P> property, Task<P> factory)
    {
      if (!(FieldManager.FieldExists(property)) && !_lazyLoadingProperties.Contains(property))
      {
        _lazyLoadingProperties.Add(property);
        LoadPropertyAsync(property, factory);
      }
      return ReadProperty<P>(property);
    }

    P IManageProperties.LazyReadProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator)
    {
      return LazyReadProperty(propertyInfo, valueGenerator);
    }

    P IManageProperties.LazyReadPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory)
    {
      return LazyReadPropertyAsync(propertyInfo, factory);
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
          fieldData = FieldManager.LoadFieldData(propertyInfo, oldValue);
        }
        else
        {
          oldValue = (P)fieldData.Value;
        }

        if (oldValue == null)
        {
          if (newValue != null)
            FieldManager.LoadFieldData(propertyInfo, Utilities.CoerceValue<P>(typeof(F), oldValue, newValue));
        }
        else if (!(oldValue.Equals(newValue)))
        {
          FieldManager.LoadFieldData(propertyInfo, Utilities.CoerceValue<P>(typeof(F), oldValue, newValue));
        }
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message));
      }
    }

    void Core.IManageProperties.LoadProperty<P>(PropertyInfo<P> propertyInfo, P newValue)
    {
      LoadProperty<P>(propertyInfo, newValue);
    }

    bool Core.IManageProperties.FieldExists(Core.IPropertyInfo property)
    {
      return FieldManager.FieldExists(property);
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
          fieldData = FieldManager.LoadFieldData(propertyInfo, oldValue);
        }
        else
        {
          oldValue = (P)fieldData.Value;
        }

        if (oldValue == null)
        {
          if (newValue != null)
            FieldManager.LoadFieldData(propertyInfo, newValue);
        }
        else if (!(oldValue.Equals(newValue)))
        {
          FieldManager.LoadFieldData(propertyInfo, newValue);
        }
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
      var t = this.GetType();
      var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      var method = t.GetMethods(flags).Where(c => c.Name == "LoadProperty" && c.IsGenericMethod).FirstOrDefault();
      var gm = method.MakeGenericMethod(propertyInfo.Type);
      var p = new object[] { propertyInfo, newValue };
      gm.Invoke(this, p);
    }

    //private AsyncLoadManager
    [NonSerialized]
    private AsyncLoadManager _loadManager;
    internal AsyncLoadManager LoadManager
    {
      get
      {
        if (_loadManager == null)
        {
          _loadManager = new AsyncLoadManager(this, OnPropertyChanged);
          _loadManager.BusyChanged += loadManager_BusyChanged;
          _loadManager.UnhandledAsyncException += loadManager_UnhandledAsyncException;
        }
        return _loadManager;
      }
    }

    void loadManager_UnhandledAsyncException(object sender, ErrorEventArgs e)
    {
      OnUnhandledAsyncException(e);
    }

    void loadManager_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }
    /*
    /// <summary>
    /// Loads a property value asynchronously.
    /// </summary>
    /// <typeparam name="R">Type of the property</typeparam>
    /// <typeparam name="P">Type of the parameter.</typeparam>
    /// <param name="property">Property to load.</param>
    /// <param name="factory">AsyncFactory delegate.</param>
    //protected void LoadPropertyAsync<R>(PropertyInfo<R> property, AsyncFactoryDelegate<R> factory)
    //{
    //  LoadManager.BeginLoad(new AsyncLoader<R>(property, factory));
    //}

    /// <summary>
    /// Loads a property value asynchronously.
    /// </summary>
    /// <typeparam name="R">Type of the property</typeparam>
    /// <typeparam name="P">Type of the parameter.</typeparam>
    /// <param name="property">Property to load.</param>
    /// <param name="factory">AsyncFactory delegate.</param>
    /// <param name="parameter">Parameter value.</param>
    //protected void LoadPropertyAsync<R, P>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P> factory, P parameter)
    //{
    //  LoadManager.BeginLoad(new AsyncLoader<R>(property, factory, parameter));
    //}
    */

    /// <summary>
    /// Load a property from an async method. 
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="property"></param>
    /// <param name="factory"></param>
    protected void LoadPropertyAsync<R>(PropertyInfo<R> property, Task<R> factory)
    {
      LoadManager.BeginLoad(new TaskLoader<R>(property, factory));
    }
#endregion

#region  Field Manager

    [NotUndoable()]
    private FieldDataManager _fieldManager;

    /// <summary>
    /// Gets the PropertyManager object for this
    /// business object.
    /// </summary>
    protected FieldDataManager FieldManager
    {
      get
      {
        if (_fieldManager == null)
        {
          _fieldManager = new FieldDataManager(this.GetType());
        }
        return _fieldManager;
      }
    }

#endregion

#region IsBusy / IsIdle

    [NonSerialized]
    [NotUndoable]
    private bool _isBusy;

    /// <summary>
    /// Marks the object as being busy (it is
    /// running an async operation).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkBusy()
    {
      if (_isBusy)
        throw new InvalidOperationException(Resources.BusyObjectsMayNotBeMarkedBusy);

      _isBusy = true;
      OnBusyChanged(new BusyChangedEventArgs("", true));
    }

    /// <summary>
    /// Marks the object as being not busy
    /// (it is not running an async operation).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkIdle()
    {
      _isBusy = false;
      OnBusyChanged(new BusyChangedEventArgs("", false));
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// object or any of its child objects are
    /// running an async operation.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsBusy
    {
      get { return IsSelfBusy || (_fieldManager != null && FieldManager.IsBusy()); }
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// object is
    /// running an async operation.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsSelfBusy
    {
      get { return _isBusy || LoadManager.IsLoading; }
    }

    void Child_PropertyBusy(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

    [NotUndoable]
    [NonSerialized]
    private BusyChangedEventHandler _propertyBusy;

    /// <summary>
    /// Event raised when the IsBusy property value
    /// has changed.
    /// </summary>
    public event BusyChangedEventHandler BusyChanged
    {
      add { _propertyBusy = (BusyChangedEventHandler)Delegate.Combine(_propertyBusy, value); }
      remove { _propertyBusy = (BusyChangedEventHandler)Delegate.Remove(_propertyBusy, value); }
    }

    /// <summary>
    /// Raises the BusyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the property
    /// that has changed.</param>
    /// <param name="busy">New busy value.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnBusyChanged(string propertyName, bool busy)
    {
      OnBusyChanged(new BusyChangedEventArgs(propertyName, busy));
    }

    /// <summary>
    /// Raises the BusyChanged event.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnBusyChanged(BusyChangedEventArgs args)
    {
      if (_propertyBusy != null)
        _propertyBusy(this, args);
    }

#endregion

#region IDataPortalTarget Members

    void Csla.Server.IDataPortalTarget.CheckRules()
    { }

    void Csla.Server.IDataPortalTarget.MarkAsChild()
    { }

    void Csla.Server.IDataPortalTarget.MarkNew()
    { }

    void Csla.Server.IDataPortalTarget.MarkOld()
    { }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvoke(e);
    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvokeComplete(e);
    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.DataPortal_OnDataPortalException(e, ex);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvoke(e);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvokeComplete(e);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.Child_OnDataPortalException(e, ex);
    }

#endregion

#region IManageProperties Members

    bool IManageProperties.HasManagedProperties
    {
      get { return (_fieldManager != null && _fieldManager.HasFields); }
    }

    List<IPropertyInfo> IManageProperties.GetManagedProperties()
    {
      return FieldManager.GetRegisteredProperties();
    }

    object IManageProperties.GetProperty(IPropertyInfo propertyInfo)
    {
      return GetProperty(propertyInfo);
    }

    object IManageProperties.LazyGetProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator)
    {
      return LazyGetProperty(propertyInfo, valueGenerator);
    }

    object IManageProperties.LazyGetPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory)
    {
      return LazyGetPropertyAsync(propertyInfo, factory);
    }

    object IManageProperties.ReadProperty(IPropertyInfo propertyInfo)
    {
      return ReadProperty(propertyInfo);
    }

    P IManageProperties.ReadProperty<P>(PropertyInfo<P> propertyInfo)
    {
      return ReadProperty<P>(propertyInfo);
    }

    void IManageProperties.SetProperty(IPropertyInfo propertyInfo, object newValue)
    {
      throw new NotImplementedException("IManageProperties.SetProperty");
    }

    void IManageProperties.LoadProperty(IPropertyInfo propertyInfo, object newValue)
    {
      LoadProperty(propertyInfo, newValue);
    }

    bool IManageProperties.LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object newValue)
    {
      LoadProperty(propertyInfo, newValue);
      return false;
    }

    List<object> IManageProperties.GetChildren()
    {
      return FieldManager.GetChildren();
    }
#endregion

#region MobileFormatter

    /// <summary>
    /// Override this method to insert your child object
    /// references into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnGetChildren(
      Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      base.OnGetChildren(info, formatter);
      if (_fieldManager != null)
      {
        var fieldManagerInfo = formatter.SerializeObject(_fieldManager);
        info.AddChild("_fieldManager", fieldManagerInfo.ReferenceId);
      }
    }

    /// <summary>
    /// Override this method to retrieve your child object
    /// references from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnSetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      if (info.Children.ContainsKey("_fieldManager"))
      {
        var childData = info.Children["_fieldManager"];
        _fieldManager = (FieldDataManager)formatter.GetObject(childData.ReferenceId);
      }
      base.OnSetChildren(info, formatter);
    }

#endregion

#region INotifyUnhandledAsyncException Members

    [NotUndoable]
    [NonSerialized]
    private EventHandler<ErrorEventArgs> _unhandledAsyncException;

    /// <summary>
    /// Event raised when an exception occurs on a background
    /// thread during an asynchronous operation.
    /// </summary>
    public event EventHandler<ErrorEventArgs> UnhandledAsyncException
    {
      add { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
      remove { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Remove(_unhandledAsyncException, value); }
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="error">Error arguments.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
      if (_unhandledAsyncException != null)
        _unhandledAsyncException(this, error);
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="originalSender">Original sender of the
    /// event.</param>
    /// <param name="error">Execption that occurred.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncException(new ErrorEventArgs(originalSender, error));
    }

#endregion
  }
}