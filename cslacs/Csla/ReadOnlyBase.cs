using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Csla.Core;
using Csla.Properties;
using Csla.Core.FieldManager;
using Csla.Core.LoadManager;
using Csla.Reflection;
using Csla.Server;
using Csla.Security;

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
    IAuthorizeReadWrite, 
    IDataPortalTarget,
    IManageProperties,
    INotifyBusy
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

    #region Constructors

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected ReadOnlyBase()
    {
      Initialize();
      InitializeAuthorizationRules();
    }

    #endregion

    #region Initialize

    /// <summary>
    /// Override this method to set up event handlers so user
    /// code in a partial class can respond to events raised by
    /// generated code.
    /// </summary>
    protected virtual void Initialize()
    { /* allows subclass to initialize events before any other activity occurs */ }

    #endregion

    #region Authorization

    [NotUndoable()]
    [NonSerialized()]
    private Dictionary<string, bool> _readResultCache;
    [NotUndoable()]
    [NonSerialized()]
    private Dictionary<string, bool> _executeResultCache;
    [NotUndoable()]
    [NonSerialized()]
    private System.Security.Principal.IPrincipal _lastPrincipal;

    [NotUndoable()]
    [NonSerialized()]
    private Security.AuthorizationRules _authorizationRules;

    private void InitializeAuthorizationRules()
    {
      AddInstanceAuthorizationRules();
      if (!Security.SharedAuthorizationRules.RulesExistFor(this.GetType()))
      {
        lock (this.GetType())
        {
          if (!Security.SharedAuthorizationRules.RulesExistFor(this.GetType()))
          {
            Security.SharedAuthorizationRules.GetManager(this.GetType(), true);
            AddAuthorizationRules();
          }
        }
      }
    }

    /// <summary>
    /// Override this method to add authorization
    /// rules for your object's properties.
    /// </summary>
    protected virtual void AddInstanceAuthorizationRules()
    {

    }

    /// <summary>
    /// Override this method to add per-type
    /// authorization rules for your type's properties.
    /// </summary>
    /// <remarks>
    /// AddSharedAuthorizationRules is automatically called by CSLA .NET
    /// when your object should associate per-type authorization roles
    /// with its properties.
    /// </remarks>
    protected virtual void AddAuthorizationRules()
    {

    }

    /// <summary>
    /// Provides access to the AuthorizationRules object for this
    /// object.
    /// </summary>
    /// <remarks>
    /// Use this object to add a list of allowed and denied roles for
    /// reading and writing properties of the object. Typically these
    /// values are added once when the business object is instantiated.
    /// </remarks>
    protected Security.AuthorizationRules AuthorizationRules
    {
      get
      {
        if (_authorizationRules == null)
          _authorizationRules = new Security.AuthorizationRules(this.GetType());
        return _authorizationRules;
      }
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit property name")]
    public bool CanReadProperty(bool throwOnFalse)
    {
      string propertyName =
        new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      bool result = CanReadProperty(propertyName);
      if (throwOnFalse && result == false)
      {
        System.Security.SecurityException ex = new System.Security.SecurityException(
          string.Format("{0} ({1})",
          Resources.PropertyGetNotAllowed, propertyName));
        ex.Action = System.Security.Permissions.SecurityAction.Deny;
        throw ex;
      }
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    public bool CanReadProperty(string propertyName, bool throwOnFalse)
    {
      bool result = CanReadProperty(propertyName);
      if (throwOnFalse && result == false)
      {
        System.Security.SecurityException ex = new System.Security.SecurityException(
          string.Format("{0} ({1})",
          Resources.PropertyGetNotAllowed, propertyName));
        ex.Action = System.Security.Permissions.SecurityAction.Deny;
        throw ex;
      }
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit property name")]
    public bool CanReadProperty()
    {
      string propertyName = 
        new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      return CanReadProperty(propertyName);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    /// <remarks>
    /// <para>
    /// If a list of allowed roles is provided then only users in those
    /// roles can read. If no list of allowed roles is provided then
    /// the list of denied roles is checked.
    /// </para><para>
    /// If a list of denied roles is provided then users in the denied
    /// roles are denied read access. All other users are allowed.
    /// </para><para>
    /// If neither a list of allowed nor denied roles is provided then
    /// all users will have read access.
    /// </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanReadProperty(string propertyName)
    {
      bool result = true;

      VerifyAuthorizationCache();

      if (!_readResultCache.TryGetValue(propertyName, out result))
      {
        result = true;
        if (AuthorizationRules.HasReadAllowedRoles(propertyName))
        {
          // some users are explicitly granted read access
          // in which case all other users are denied.
          if (!AuthorizationRules.IsReadAllowed(propertyName))
            result = false;
        }
        else if (AuthorizationRules.HasReadDeniedRoles(propertyName))
        {
          // some users are explicitly denied read access.
          if (AuthorizationRules.IsReadDenied(propertyName))
            result = false;
        }
        // store value in cache
        _readResultCache[propertyName] = result;
      }
      return result;
    }

    bool Csla.Security.IAuthorizeReadWrite.CanWriteProperty(string propertyName)
    {
      return false;
    }

    private void VerifyAuthorizationCache()
    {
      if (_readResultCache == null)
        _readResultCache = new Dictionary<string, bool>();
      if (_executeResultCache == null)
        _executeResultCache = new Dictionary<string, bool>();
      if (!ReferenceEquals(Csla.ApplicationContext.User, _lastPrincipal))
      {
        // the principal has changed - reset the cache
        _readResultCache.Clear();
        _executeResultCache.Clear();
        _lastPrincipal = Csla.ApplicationContext.User;
      }
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to execute
    /// the calling method.
    /// </summary>
    /// <returns><see langword="true" /> if execute is allowed.</returns>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit method name")]
    public bool CanExecuteMethod(bool throwOnFalse)
    {

      string methodName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
      bool result = CanExecuteMethod(methodName);
      if (throwOnFalse && result == false)
      {
        System.Security.SecurityException ex = new System.Security.SecurityException(string.Format("{0} ({1})", Properties.Resources.MethodExecuteNotAllowed, methodName));
        ex.Action = System.Security.Permissions.SecurityAction.Deny;
        throw ex;
      }
      return result;

    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to execute
    /// the specified method.
    /// </summary>
    /// <returns><see langword="true" /> if execute is allowed.</returns>
    /// <param name="methodName">Name of the method to execute.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    public bool CanExecuteMethod(string methodName, bool throwOnFalse)
    {

      bool result = CanExecuteMethod(methodName);
      if (throwOnFalse && result == false)
      {
        System.Security.SecurityException ex = new System.Security.SecurityException(string.Format("{0} ({1})", Properties.Resources.MethodExecuteNotAllowed, methodName));
        ex.Action = System.Security.Permissions.SecurityAction.Deny;
        throw ex;
      }
      return result;

    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to execute
    /// the calling method.
    /// </summary>
    /// <returns><see langword="true" /> if execute is allowed.</returns>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit method name")]
    public bool CanExecuteMethod()
    {

      string methodName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
      return CanExecuteMethod(methodName);

    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to execute
    /// the specified method.
    /// </summary>
    /// <param name="methodName">Name of the method to execute.</param>
    /// <returns><see langword="true" /> if execute is allowed.</returns>
    /// <remarks>
    /// <para>
    /// If a list of allowed roles is provided then only users in those
    /// roles can read. If no list of allowed roles is provided then
    /// the list of denied roles is checked.
    /// </para><para>
    /// If a list of denied roles is provided then users in the denied
    /// roles are denied read access. All other users are allowed.
    /// </para><para>
    /// If neither a list of allowed nor denied roles is provided then
    /// all users will have read access.
    /// </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanExecuteMethod(string methodName)
    {

      bool result = true;

      VerifyAuthorizationCache();

      if (!_executeResultCache.TryGetValue(methodName, out result))
      {
        result = true;
        if (AuthorizationRules.HasExecuteAllowedRoles(methodName))
        {
          // some users are explicitly granted read access
          // in which case all other users are denied
          if (!(AuthorizationRules.IsExecuteAllowed(methodName)))
          {
            result = false;
          }

        }
        else if (AuthorizationRules.HasExecuteDeniedRoles(methodName))
        {
          // some users are explicitly denied read access
          if (AuthorizationRules.IsExecuteDenied(methodName))
          {
            result = false;
          }
        }
        // store value in cache
        _executeResultCache[methodName] = result;
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

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Resources.FetchNotSupportedException);
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

    [OnDeserialized()]
    private void OnDeserializedHandler(StreamingContext context)
    {
      FieldManager.SetPropertyList(this.GetType());
      InitializeAuthorizationRules();
      OnDeserialized(context);
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    /// <param name="context">Serialization context object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized(StreamingContext context)
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
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name));
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

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName));
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

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName, defaultValue));
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
      if (CanReadProperty(propertyName, noAccess == Csla.Security.NoAccessBehavior.ThrowException))
        return field;
      else
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
      if (CanReadProperty(propertyInfo.Name, noAccess == Csla.Security.NoAccessBehavior.ThrowException))
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
      if (CanReadProperty(propertyInfo.Name, false))
      {
        var info = FieldManager.GetFieldData(propertyInfo);
        if (info != null)
          result = info.Value;
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
    protected void LoadProperty(IPropertyInfo propertyInfo, object newValue)
    {
      FieldManager.LoadFieldData(propertyInfo, newValue);
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
          _loadManager = new AsyncLoadManager();
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

    /// <summary>
    /// Loads a property value asynchronously.
    /// </summary>
    /// <typeparam name="R">Type of the property</typeparam>
    /// <typeparam name="P">Type of the parameter.</typeparam>
    /// <param name="property">Property to load.</param>
    /// <param name="factory">AsyncFactory delegate.</param>
    /// <param name="parameter">Parameter value.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void LoadPropertyAsync<R, P>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P> factory, P parameter)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, parameter);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    /// <summary>
    /// Loads a property value asynchronously.
    /// </summary>
    /// <typeparam name="R">Type of the property</typeparam>
    /// <typeparam name="P1">Type of the parameter.</typeparam>
    /// <typeparam name="P2">Type of the parameter.</typeparam>
    /// <param name="property">Property to load.</param>
    /// <param name="factory">AsyncFactory delegate.</param>
    /// <param name="p1">Parameter value.</param>
    /// <param name="p2">Parameter value.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void LoadPropertyAsync<R, P1, P2>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P1, P2> factory, P1 p1, P2 p2)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, p1, p2);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    /// <summary>
    /// Loads a property value asynchronously.
    /// </summary>
    /// <typeparam name="R">Type of the property</typeparam>
    /// <typeparam name="P1">Type of the parameter.</typeparam>
    /// <typeparam name="P2">Type of the parameter.</typeparam>
    /// <typeparam name="P3">Type of the parameter.</typeparam>
    /// <param name="property">Property to load.</param>
    /// <param name="factory">AsyncFactory delegate.</param>
    /// <param name="p1">Parameter value.</param>
    /// <param name="p2">Parameter value.</param>
    /// <param name="p3">Parameter value.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void LoadPropertyAsync<R, P1, P2, P3>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P1, P2, P3> factory, P1 p1, P2 p2, P3 p3)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, p1, p2, p3);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    /// <summary>
    /// Loads a property value asynchronously.
    /// </summary>
    /// <typeparam name="R">Type of the property</typeparam>
    /// <typeparam name="P1">Type of the parameter.</typeparam>
    /// <typeparam name="P2">Type of the parameter.</typeparam>
    /// <typeparam name="P3">Type of the parameter.</typeparam>
    /// <typeparam name="P4">Type of the parameter.</typeparam>
    /// <param name="property">Property to load.</param>
    /// <param name="factory">AsyncFactory delegate.</param>
    /// <param name="p1">Parameter value.</param>
    /// <param name="p2">Parameter value.</param>
    /// <param name="p3">Parameter value.</param>
    /// <param name="p4">Parameter value.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void LoadPropertyAsync<R, P1, P2, P3, P4>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P1, P2, P3, P4> factory, P1 p1, P2 p2, P3 p3, P4 p4)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, p1, p2, p3, p4);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    /// <summary>
    /// Loads a property value asynchronously.
    /// </summary>
    /// <typeparam name="R">Type of the property</typeparam>
    /// <typeparam name="P1">Type of the parameter.</typeparam>
    /// <typeparam name="P2">Type of the parameter.</typeparam>
    /// <typeparam name="P3">Type of the parameter.</typeparam>
    /// <typeparam name="P4">Type of the parameter.</typeparam>
    /// <typeparam name="P5">Type of the parameter.</typeparam>
    /// <param name="property">Property to load.</param>
    /// <param name="factory">AsyncFactory delegate.</param>
    /// <param name="p1">Parameter value.</param>
    /// <param name="p2">Parameter value.</param>
    /// <param name="p3">Parameter value.</param>
    /// <param name="p4">Parameter value.</param>
    /// <param name="p5">Parameter value.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void LoadPropertyAsync<R, P1, P2, P3, P4, P5>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P1, P2, P3, P4, P5> factory, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, p1, p2, p3, p4, p5);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
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
      // TODO: Review resource string
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
      OnBusyChanged(new BusyChangedEventArgs("", true));
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// object or any of its child objects are
    /// running an async operation.
    /// </summary>
    [Browsable(false)]
    public bool IsBusy
    {
      get { return IsSelfBusy || (_fieldManager != null && FieldManager.IsBusy()); }
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// object is
    /// running an async operation.
    /// </summary>
    [Browsable(false)]
    public bool IsSelfBusy
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