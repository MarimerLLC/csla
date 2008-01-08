using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;
using Csla.Properties;

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

  }
}