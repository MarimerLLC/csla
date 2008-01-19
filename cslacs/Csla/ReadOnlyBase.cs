using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Csla.Core;
using Csla.Properties;
using Csla.Core.FieldManager;

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
  public abstract class ReadOnlyBase<T> : ICloneable, Core.IReadOnlyObject, Csla.Security.IAuthorizeReadWrite
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

      if (_readResultCache.ContainsKey(propertyName))
      {
        // cache contains value - get cached value
        result = _readResultCache[propertyName];
      }
      else
      {
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

      if (_executeResultCache.ContainsKey(methodName))
      {
        // cache contains value - get cached value
        result = _executeResultCache[methodName];

      }
      else
      {
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

    #endregion

    #region Serialization Notification

    [OnDeserialized()]
    private void OnDeserializedHandler(StreamingContext context)
    {
      OnDeserialized(context);
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
      return GetProperty<P>(propertyName, field, defaultValue, false);
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
    /// <param name="throwOnNoAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    protected P GetProperty<P>(string propertyName, P field, P defaultValue, bool throwOnNoAccess)
    {
      if (CanReadProperty(propertyName, throwOnNoAccess))
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
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<P>(PropertyInfo<P> propertyInfo, P field)
    {
      return GetProperty<P>(propertyInfo.Name, field, propertyInfo.DefaultValue, false);
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
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<F, P>(PropertyInfo<F> propertyInfo, F field)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo.Name, field, propertyInfo.DefaultValue, false));
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
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    /// <param name="throwOnNoAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<F, P>(PropertyInfo<F> propertyInfo, F field, bool throwOnNoAccess)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo.Name, field, propertyInfo.DefaultValue, throwOnNoAccess));
    }

    /// <summary>
    /// Gets a property's managed field value, 
    /// first checking authorization.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<P>(PropertyInfo<P> propertyInfo)
    {
      return GetProperty<P>(propertyInfo, false);
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
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<F, P>(PropertyInfo<F> propertyInfo)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo, false));
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
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    /// <param name="throwOnNoAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<F, P>(PropertyInfo<F> propertyInfo, bool throwOnNoAccess)
    {
      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo, throwOnNoAccess));
    }

    /// <summary>
    /// Gets a property's value as a specified type, 
    /// first checking authorization.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    /// <param name="throwOnNoAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to read this property.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    protected P GetProperty<P>(PropertyInfo<P> propertyInfo, bool throwOnNoAccess)
    {
      P result = default(P);
      if (CanReadProperty(propertyInfo.Name, throwOnNoAccess))
        result = ReadProperty<P>(propertyInfo);
      else
        result = propertyInfo.DefaultValue;
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
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    protected P ReadProperty<F, P>(PropertyInfo<F> propertyInfo)
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
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
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

    #endregion

    #region  Load Properties

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
    /// <param name="propertyInfo">
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called,
    /// and no PropertyChanging or PropertyChanged events are raised.
    /// Loading values does not cause validation rules to be
    /// invoked.
    /// </remarks>
    protected void LoadProperty<P, F>(PropertyInfo<P> propertyInfo, F newValue)
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

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// <see cref="PropertyInfo" /> object containing property metadata.</param>
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
          _fieldManager = new FieldDataManager();
        }
        return _fieldManager;
      }
    }

    #endregion
  }
}