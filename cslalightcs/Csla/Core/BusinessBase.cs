using System;
using System.Linq;
using System.ComponentModel;
using Csla.Core.FieldManager;
using Csla.Properties;
using System.Collections.Specialized;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.DataPortalClient;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Csla.Validation;
using System.Security;
using System.Windows.Controls;
using Csla.Core.LoadManager;

namespace Csla.Core
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public abstract class BusinessBase : UndoableBase,
    ICloneable,
    IParent,
    IDataPortalTarget,
    Csla.Security.IAuthorizeReadWrite,
    IEditableBusinessObject,
    ISerializationNotification,
    IEditableObject,
    INotifyChildChanged,
    ITrackStatus
  {
    #region Constructors

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected BusinessBase()
    {
      Initialize();
      InitializeBusinessRules();
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

    #region Parent/Child link

    [NotUndoable]
    [NonSerialized]
    private Core.IParent _parent;

    /// <summary>
    /// Provide access to the parent reference for use
    /// in child object code.
    /// </summary>
    /// <remarks>
    /// This value will be Nothing for root objects.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected IParent Parent
    {
      get { return _parent; }
    }

    /// <summary>
    /// Used by BusinessListBase as a child object is 
    /// created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    internal void SetParent(IParent parent)
    {
      _parent = parent;
    }

    #endregion

    #region IEditableBusinessObject Members

    int IEditableBusinessObject.EditLevelAdded
    {
      get
      {
        return this.EditLevelAdded;
      }
      set
      {
        this.EditLevelAdded = value;
      }
    }

    void IEditableBusinessObject.DeleteChild()
    {
      this.DeleteChild();
    }

    void IEditableBusinessObject.SetParent(IParent parent)
    {
      this.SetParent(parent);
    }

    #endregion

    #region Begin/Cancel/ApplyEdit

    private bool _neverCommitted = true;

    /// <summary>
    /// Starts a nested edit on the object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When this method is called the object takes a snapshot of
    /// its current state (the values of its variables). This snapshot
    /// can be restored by calling CancelEdit
    /// or committed by calling ApplyEdit.
    /// </para><para>
    /// This is a nested operation. Each call to BeginEdit adds a new
    /// snapshot of the object's state to a stack. You should ensure that 
    /// for each call to BeginEdit there is a corresponding call to either 
    /// CancelEdit or ApplyEdit to remove that snapshot from the stack.
    /// </para><para>
    /// See Chapters 2 and 3 for details on n-level undo and state stacking.
    /// </para>
    /// </remarks>
    public void BeginEdit()
    {
      CopyState(this.EditLevel + 1);
    }

    /// <summary>
    /// Cancels the current edit process, restoring the object's state to
    /// its previous values.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be restored. This resets the object's values
    /// to the point of the last BeginEdit call.
    /// </remarks>
    public void CancelEdit()
    {
      UndoChanges(this.EditLevel - 1);
    }

    /// <summary>
    /// Called when an undo operation has completed.
    /// </summary>
    /// <remarks> 
    /// This method resets the object as a result of
    /// deserialization and raises PropertyChanged events
    /// to notify data binding that the object has changed.
    /// </remarks>
    protected override void UndoChangesComplete()
    {
      BindingEdit = false;
      ValidationRules.SetTarget(this);
      InitializeBusinessRules();
      OnUnknownPropertyChanged();
      base.UndoChangesComplete();
    }

    /// <summary>
    /// Commits the current edit process.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be discarded, thus committing any changes made
    /// to the object's state since the last BeginEdit call.
    /// </remarks>
    public void ApplyEdit()
    {
      _neverCommitted = false;
      AcceptChanges(this.EditLevel - 1);
      BindingEdit = false;
    }

    /// <summary>
    /// Notifies the parent object (if any) that this
    /// child object's edits have been accepted.
    /// </summary>
    protected override void AcceptChangesComplete()
    {
      if (Parent != null)
        Parent.ApplyEditChild(this);
      base.AcceptChangesComplete();
    }

    #endregion

    #region IsChild

    [NotUndoable()]
    private bool _isChild;

    /// <summary>
    /// Returns <see langword="true" /> if this is a child (non-root) object.
    /// </summary>
    protected internal bool IsChild
    {
      get { return _isChild; }
    }

    /// <summary>
    /// Marks the object as being a child object.
    /// </summary>
    protected void MarkAsChild()
    {
      _isChild = true;
    }

    #endregion

    #region IsNew, IsDeleted, IsDirty, IsSavable

    // keep track of whether we are new, deleted or dirty
    private bool _isNew = true;
    private bool _isDeleted;
    private bool _isDirty = true;

    /// <summary>
    /// Returns <see langword="true" /> if this is a new object, 
    /// <see langword="false" /> if it is a pre-existing object.
    /// </summary>
    /// <remarks>
    /// An object is considered to be new if its primary identifying (key) value 
    /// doesn't correspond to data in the database. In other words, 
    /// if the data values in this particular
    /// object have not yet been saved to the database the object is considered to
    /// be new. Likewise, if the object's data has been deleted from the database
    /// then the object is considered to be new.
    /// </remarks>
    /// <returns>A value indicating if this object is new.</returns>
    public bool IsNew
    {
      get { return _isNew; }
    }

    /// <summary>
    /// Returns <see langword="true" /> if this object is marked for deletion.
    /// </summary>
    /// <remarks>
    /// CSLA .NET supports both immediate and deferred deletion of objects. This
    /// property is part of the support for deferred deletion, where an object
    /// can be marked for deletion, but isn't actually deleted until the object
    /// is saved to the database. This property indicates whether or not the
    /// current object has been marked for deletion. If it is <see langword="true" />
    /// , the object will
    /// be deleted when it is saved to the database, otherwise it will be inserted
    /// or updated by the save operation.
    /// </remarks>
    /// <returns>A value indicating if this object is marked for deletion.</returns>
    public bool IsDeleted
    {
      get { return _isDeleted; }
    }

    /// <summary>
    /// Returns <see langword="true" /> if this object's 
    /// data, or any of its fields or child objects data, 
    /// has been changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an object's data is changed, CSLA .NET makes note of that change
    /// and considers the object to be 'dirty' or changed. This value is used to
    /// optimize data updates, since an unchanged object does not need to be
    /// updated into the database. All new objects are considered dirty. All objects
    /// marked for deletion are considered dirty.
    /// </para><para>
    /// Once an object's data has been saved to the database (inserted or updated)
    /// the dirty flag is cleared and the object is considered unchanged. Objects
    /// newly loaded from the database are also considered unchanged.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if this object's data has been changed.</returns>
    public virtual bool IsDirty
    {
      get { return IsSelfDirty || (_fieldManager != null && FieldManager.IsDirty()); }
    }

    /// <summary>
    /// Returns <see langword="true" /> if this object's data has been changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an object's data is changed, CSLA .NET makes note of that change
    /// and considers the object to be 'dirty' or changed. This value is used to
    /// optimize data updates, since an unchanged object does not need to be
    /// updated into the database. All new objects are considered dirty. All objects
    /// marked for deletion are considered dirty.
    /// </para><para>
    /// Once an object's data has been saved to the database (inserted or updated)
    /// the dirty flag is cleared and the object is considered unchanged. Objects
    /// newly loaded from the database are also considered unchanged.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if this object's data has been changed.</returns>
    public virtual bool IsSelfDirty
    {
      get { return _isDirty; }
    }

    /// <summary>
    /// Marks the object as being a new object. This also marks the object
    /// as being dirty and ensures that it is not marked for deletion.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Newly created objects are marked new by default. You should call
    /// this method in the implementation of DataPortal_Update when the
    /// object is deleted (due to being marked for deletion) to indicate
    /// that the object no longer reflects data in the database.
    /// </para><para>
    /// If you override this method, make sure to call the base
    /// implementation after executing your new code.
    /// </para>
    /// </remarks>
    protected virtual void MarkNew()
    {
      _isNew = true;
      _isDeleted = false;
      MarkDirty();
    }

    /// <summary>
    /// Marks the object as being an old (not new) object. This also
    /// marks the object as being unchanged (not dirty).
    /// </summary>
    /// <remarks>
    /// <para>
    /// You should call this method in the implementation of
    /// DataPortal_Fetch to indicate that an existing object has been
    /// successfully retrieved from the database.
    /// </para><para>
    /// You should call this method in the implementation of 
    /// DataPortal_Update to indicate that a new object has been successfully
    /// inserted into the database.
    /// </para><para>
    /// If you override this method, make sure to call the base
    /// implementation after executing your new code.
    /// </para>
    /// </remarks>
    protected virtual void MarkOld()
    {
      _isNew = false;
      MarkClean();
    }

    /// <summary>
    /// Marks an object for deletion. This also marks the object
    /// as being dirty.
    /// </summary>
    /// <remarks>
    /// You should call this method in your business logic in the
    /// case that you want to have the object deleted when it is
    /// saved to the database.
    /// </remarks>
    protected void MarkDeleted()
    {
      _isDeleted = true;
      MarkDirty();
    }

    /// <summary>
    /// Marks an object as being dirty, or changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You should call this method in your business logic any time
    /// the object's internal data changes. Any time any instance
    /// variable changes within the object, this method should be called
    /// to tell CSLA .NET that the object's data has been changed.
    /// </para><para>
    /// Marking an object as dirty does two things. First it ensures
    /// that CSLA .NET will properly save the object as appropriate. Second,
    /// it causes CSLA .NET to tell Windows Forms data binding that the
    /// object's data has changed so any bound controls will update to
    /// reflect the new values.
    /// </para>
    /// </remarks>
    protected void MarkDirty()
    {
      MarkDirty(false);
    }

    /// <summary>
    /// Marks an object as being dirty, or changed.
    /// </summary>
    /// <param name="suppressEvent">
    /// <see langword="true" /> to supress the PropertyChanged event that is otherwise
    /// raised to indicate that the object's state has changed.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkDirty(bool suppressEvent)
    {
      _isDirty = true;
      if (!suppressEvent)
        OnUnknownPropertyChanged();
    }

    /// <summary>
    /// Performs processing required when the current
    /// property has changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method calls CheckRules(propertyName), MarkDirty and
    /// OnPropertyChanged(propertyName). MarkDirty is called such
    /// that no event is raised for IsDirty, so only the specific
    /// property changed event for the current property is raised.
    /// </para><para>
    /// This implementation uses System.Diagnostics.StackTrace to
    /// determine the name of the current property, and so must be called
    /// directly from the property to be checked.
    /// </para>
    /// </remarks>
    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit property name")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    protected void PropertyHasChanged()
    {
      string propertyName =
        new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      PropertyHasChanged(propertyName);
    }

    /// <summary>
    /// Performs processing required when a property
    /// has changed.
    /// </summary>
    /// <param name="propertyName">Name of the property that
    /// has changed.</param>
    /// <remarks>
    /// This method calls CheckRules(propertyName), MarkDirty and
    /// OnPropertyChanged(propertyName). MarkDirty is called such
    /// that no event is raised for IsDirty, so only the specific
    /// property changed event for the current property is raised.
    /// </remarks>
    protected virtual void PropertyHasChanged(string propertyName)
    {
      MarkDirty(true);

      var propertyNames = ValidationRules.CheckRules(propertyName);
      if (ApplicationContext.PropertyChangedMode == ApplicationContext.PropertyChangedModes.Windows)
        OnPropertyChanged(propertyName);
      else
        foreach (var name in propertyNames)
          OnPropertyChanged(name);
    }

    /// <summary>
    /// Forces the object's IsDirty flag to <see langword="false" />.
    /// </summary>
    /// <remarks>
    /// This method is normally called automatically and is
    /// not intended to be called manually.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkClean()
    {
      _isDirty = false;
      if (_fieldManager != null)
        FieldManager.MarkClean();
      OnUnknownPropertyChanged();
    }

    /// <summary>
    /// Returns <see langword="true" /> if this object is both dirty and valid.
    /// </summary>
    /// <remarks>
    /// An object is considered dirty (changed) if 
    /// <see cref="P:Csla.BusinessBase.IsDirty" /> returns <see langword="true" />. It is
    /// considered valid if IsValid
    /// returns <see langword="true" />. The IsSavable property is
    /// a combination of these two properties. 
    /// </remarks>
    /// <returns>A value indicating if this object is both dirty and valid.</returns>
    public virtual bool IsSavable
    {
      get
      {
        bool auth;
        if (IsDeleted)
          auth = Csla.Security.AuthorizationRules.CanDeleteObject(this.GetType());
        else if (IsNew)
          auth = Csla.Security.AuthorizationRules.CanCreateObject(this.GetType());
        else
          auth = Csla.Security.AuthorizationRules.CanEditObject(this.GetType());
        return (auth && IsDirty && IsValid && !IsBusy);
      }
    }

    #endregion

    #region Authorization

    [NotUndoable()]
    [NonSerialized()]
    private Dictionary<string, bool> _readResultCache;
    [NotUndoable()]
    [NonSerialized()]
    private Dictionary<string, bool> _writeResultCache;
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
      if (!(Csla.Security.SharedAuthorizationRules.RulesExistFor(this.GetType())))
      {
        lock (this.GetType())
        {
          if (!(Csla.Security.SharedAuthorizationRules.RulesExistFor(this.GetType())))
          {
            Csla.Security.SharedAuthorizationRules.GetManager(this.GetType(), true);
            AddAuthorizationRules();
          }
        }
      }
    }

    /// <summary>
    /// Override this method to add authorization
    /// rules for your object's properties.
    /// </summary>
    /// <remarks>
    /// AddInstanceAuthorizationRules is automatically called by CSLA .NET
    /// when your object should associate per-instance authorization roles
    /// with its properties.
    /// </remarks>
    protected virtual void AddInstanceAuthorizationRules()
    {

    }

    /// <summary>
    /// Override this method to add per-type
    /// authorization rules for your type's properties.
    /// </summary>
    /// <remarks>
    /// AddAuthorizationRules is automatically called by CSLA .NET
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
    [MethodImpl(MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit property name")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool CanReadProperty(bool throwOnFalse)
    {
      string propertyName =
        new System.Diagnostics.StackTrace().
        GetFrame(1).GetMethod().Name.Substring(4);
      bool result = CanReadProperty(propertyName);
      if (throwOnFalse && result == false)
      {
        System.Security.SecurityException ex = new System.Security.SecurityException(
          String.Format("{0} ({1})",
          Resources.PropertyGetNotAllowed, propertyName));
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
          String.Format("{0} ({1})",
          Resources.PropertyGetNotAllowed, propertyName));
        throw ex;
      }
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit property name")]
    [EditorBrowsable(EditorBrowsableState.Never)]
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
          // in which case all other users are denied
          if (!AuthorizationRules.IsReadAllowed(propertyName))
            result = false;
        }
        else if (AuthorizationRules.HasReadDeniedRoles(propertyName))
        {
          // some users are explicitly denied read access
          if (AuthorizationRules.IsReadDenied(propertyName))
            result = false;
        }
        // store value in cache
        _readResultCache[propertyName] = result;
      }
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit property name")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool CanWriteProperty(bool throwOnFalse)
    {
      string propertyName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      bool result = CanWriteProperty(propertyName);
      if (throwOnFalse && result == false)
      {
        System.Security.SecurityException ex = new System.Security.SecurityException(
          String.Format("{0} ({1})", Resources.PropertySetNotAllowed, propertyName));
        throw ex;
      }
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    public bool CanWriteProperty(string propertyName, bool throwOnFalse)
    {
      bool result = CanWriteProperty(propertyName);
      if (throwOnFalse && result == false)
      {
        System.Security.SecurityException ex = new System.Security.SecurityException(
          String.Format("{0} ({1})", Resources.PropertySetNotAllowed, propertyName));
        throw ex;
      }
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    [Obsolete("Use overload requiring explicit property name")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool CanWriteProperty()
    {
      string propertyName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      return CanWriteProperty(propertyName);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    /// <remarks>
    /// <para>
    /// If a list of allowed roles is provided then only users in those
    /// roles can write. If no list of allowed roles is provided then
    /// the list of denied roles is checked.
    /// </para><para>
    /// If a list of denied roles is provided then users in the denied
    /// roles are denied write access. All other users are allowed.
    /// </para><para>
    /// If neither a list of allowed nor denied roles is provided then
    /// all users will have write access.
    /// </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanWriteProperty(string propertyName)
    {
      bool result = true;

      VerifyAuthorizationCache();

      if (!_writeResultCache.TryGetValue(propertyName, out result))
      {
        result = true;
        if (this.AuthorizationRules.HasWriteAllowedRoles(propertyName))
        {
          // some users are explicitly granted write access
          // in which case all other users are denied
          if (!AuthorizationRules.IsWriteAllowed(propertyName))
            result = false;
        }
        else if (AuthorizationRules.HasWriteDeniedRoles(propertyName))
        {
          // some users are explicitly denied write access
          if (AuthorizationRules.IsWriteDenied(propertyName))
            result = false;
        }
        _writeResultCache[propertyName] = result;
      }
      return result;
    }

    private void VerifyAuthorizationCache()
    {
      if (_readResultCache == null)
        _readResultCache = new Dictionary<string, bool>();
      if (_writeResultCache == null)
        _writeResultCache = new Dictionary<string, bool>();
      if (_executeResultCache == null)
        _executeResultCache = new Dictionary<string, bool>();
      if (!ReferenceEquals(Csla.ApplicationContext.User, _lastPrincipal))
      {
        // the principal has changed - reset the cache
        _readResultCache.Clear();
        _writeResultCache.Clear();
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
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public bool CanExecuteMethod(bool throwOnFalse)
    {
      string methodName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
      bool result = CanExecuteMethod(methodName);
      if (throwOnFalse && result == false)
      {
        System.Security.SecurityException ex = new System.Security.SecurityException(string.Format("{0} ({1})", Properties.Resources.MethodExecuteNotAllowed, methodName));
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
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
    /// roles can execute the method. If no list of allowed roles is 
    /// provided then the list of denied roles is checked.
    /// </para><para>
    /// If a list of denied roles is provided then users in the denied
    /// roles are not allowed to execute the method. 
    /// All other users are allowed.
    /// </para><para>
    /// If neither a list of allowed nor denied roles is provided then
    /// all users will be allowed to execute the method..
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
      if (_bypassPropertyChecks || CanReadProperty(propertyName, noAccess == Security.NoAccessBehavior.ThrowException))
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
      if (_bypassPropertyChecks || CanReadProperty(propertyInfo.Name, noAccess == Csla.Security.NoAccessBehavior.ThrowException))
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
      if (_bypassPropertyChecks || CanReadProperty(propertyInfo.Name, false))
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

    #region  Set Properties

    /// <summary>
    /// Sets a property's backing field with the supplied
    /// value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <param name="field">
    /// A reference to the backing field for the property.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to change the property, this
    /// overload throws a SecurityException.
    /// </remarks>
    protected void SetProperty<P>(PropertyInfo<P> propertyInfo, ref P field, P newValue)
    {
      SetProperty<P>(propertyInfo.Name, ref field, newValue, Security.NoAccessBehavior.ThrowException);
    }

    /// <summary>
    /// Sets a property's backing field with the supplied
    /// value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <param name="field">
    /// A reference to the backing field for the property.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <param name="propertyName">
    /// The name of the property.</param>
    /// <remarks>
    /// If the user is not authorized to change the property, this
    /// overload throws a SecurityException.
    /// </remarks>
    protected void SetProperty<P>(string propertyName, ref P field, P newValue)
    {
      SetProperty<P>(propertyName, ref field, newValue, Security.NoAccessBehavior.ThrowException);
    }

    /// <summary>
    /// Sets a property's backing field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the field being set.
    /// </typeparam>
    /// <typeparam name="V">
    /// Type of the value provided to the field.
    /// </typeparam>
    /// <param name="field">
    /// A reference to the backing field for the property.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// If the user is not authorized to change the property, this
    /// overload throws a SecurityException.
    /// </remarks>
    protected void SetPropertyConvert<P, V>(PropertyInfo<P> propertyInfo, ref P field, V newValue)
    {
      SetPropertyConvert<P, V>(propertyInfo, ref field, newValue, Security.NoAccessBehavior.ThrowException);
    }

    /// <summary>
    /// Sets a property's backing field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the field being set.
    /// </typeparam>
    /// <typeparam name="V">
    /// Type of the value provided to the field.
    /// </typeparam>
    /// <param name="field">
    /// A reference to the backing field for the property.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to change this property.</param>
    /// <remarks>
    /// If the field value is of type string, any incoming
    /// null values are converted to string.Empty.
    /// </remarks>
    protected void SetPropertyConvert<P, V>(PropertyInfo<P> propertyInfo, ref P field, V newValue, Security.NoAccessBehavior noAccess)
    {
      SetProperty<P, V>(propertyInfo.Name, ref field, newValue, noAccess);
    }

    /// <summary>
    /// Sets a property's backing field with the supplied
    /// value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <param name="field">
    /// A reference to the backing field for the property.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <param name="propertyName">
    /// The name of the property.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to change this property.</param>
    protected void SetProperty<P>(string propertyName, ref P field, P newValue, Security.NoAccessBehavior noAccess)
    {
      try
      {
        if (field == null)
        {
          if (newValue != null && (_bypassPropertyChecks || CanWriteProperty(propertyName, noAccess == Security.NoAccessBehavior.ThrowException)))
          {
            if (!_bypassPropertyChecks) OnPropertyChanging(propertyName);
            field = newValue;
            if (!_bypassPropertyChecks) PropertyHasChanged(propertyName);
          }
        }
        else if (!(field.Equals(newValue)) && (_bypassPropertyChecks || CanWriteProperty(propertyName, noAccess == Security.NoAccessBehavior.ThrowException)))
        {
          if (typeof(P) == typeof(string) && newValue == null)
            newValue = Utilities.CoerceValue<P>(typeof(string), field, string.Empty);
          if (!_bypassPropertyChecks) OnPropertyChanging(propertyName);
          field = newValue;
          if (!_bypassPropertyChecks) PropertyHasChanged(propertyName);
        }
      }
      catch (SecurityException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(
          string.Format(Resources.PropertyLoadException, propertyName, ex.Message, ex.Message), ex);
      }
    }

    /// <summary>
    /// Sets a property's backing field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the field being set.
    /// </typeparam>
    /// <typeparam name="V">
    /// Type of the value provided to the field.
    /// </typeparam>
    /// <param name="field">
    /// A reference to the backing field for the property.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <param name="propertyName">
    /// The name of the property.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to change this property.</param>
    /// <remarks>
    /// If the field value is of type string, any incoming
    /// null values are converted to string.Empty.
    /// </remarks>
    protected void SetProperty<P, V>(string propertyName, ref P field, V newValue, Security.NoAccessBehavior noAccess)
    {
      try
      {
        if (field == null)
        {
          if (newValue != null && (_bypassPropertyChecks || CanWriteProperty(propertyName, noAccess == Security.NoAccessBehavior.ThrowException)))
          {
            if (!_bypassPropertyChecks) OnPropertyChanging(propertyName);
            field = Utilities.CoerceValue<P>(typeof(V), field, newValue);
            if (!_bypassPropertyChecks) PropertyHasChanged(propertyName);
          }
        }
        else if (!(field.Equals(newValue)) && (_bypassPropertyChecks || CanWriteProperty(propertyName, noAccess == Security.NoAccessBehavior.ThrowException)))
        {
          if (typeof(V) == typeof(string) && newValue == null)
            newValue = Utilities.CoerceValue<V>(typeof(string), null, string.Empty);
          if (!_bypassPropertyChecks) OnPropertyChanging(propertyName);
          field = Utilities.CoerceValue<P>(typeof(V), field, newValue);
          if (!_bypassPropertyChecks) PropertyHasChanged(propertyName);
        }
      }
      catch (SecurityException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(
          string.Format(Resources.PropertyLoadException, propertyName, ex.Message), ex);
      }
    }

    /// <summary>
    /// Sets a property's managed field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// If the user is not authorized to change the property, this
    /// overload throws a SecurityException.
    /// </remarks>
    protected void SetProperty<P>(PropertyInfo<P> propertyInfo, P newValue)
    {
      SetProperty<P>(propertyInfo, newValue, Security.NoAccessBehavior.ThrowException);
    }

    /// <summary>
    /// Sets a property's managed field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// If the user is not authorized to change the property, this
    /// overload throws a SecurityException.
    /// </remarks>
    protected void SetPropertyConvert<P, F>(PropertyInfo<P> propertyInfo, F newValue)
    {
      SetPropertyConvert<P, F>(propertyInfo, newValue, Security.NoAccessBehavior.ThrowException);
    }

    /// <summary>
    /// Sets a property's managed field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to change this property.</param>
    protected void SetPropertyConvert<P, F>(PropertyInfo<P> propertyInfo, F newValue, Security.NoAccessBehavior noAccess)
    {
      if (_bypassPropertyChecks || CanWriteProperty(propertyInfo.Name, noAccess == Security.NoAccessBehavior.ThrowException))
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
          if (typeof(F) == typeof(string) && newValue == null)
            newValue = Utilities.CoerceValue<F>(typeof(string), null, string.Empty);
          LoadPropertyValue<P>(propertyInfo, oldValue, Utilities.CoerceValue<P>(typeof(F), oldValue, newValue), !_bypassPropertyChecks);
        }
        catch (Exception ex)
        {
          throw new PropertyLoadException(
            string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message), ex);
        }
      }
    }

    /// <summary>
    /// Sets a property's managed field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <param name="noAccess">
    /// True if an exception should be thrown when the
    /// user is not authorized to change this property.</param>
    protected void SetProperty<P>(PropertyInfo<P> propertyInfo, P newValue, Security.NoAccessBehavior noAccess)
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
        if (((oldValue == null && newValue != null) ||
            (oldValue != null && !oldValue.Equals(newValue))) &&
            (_bypassPropertyChecks || CanWriteProperty(propertyInfo.Name, noAccess == Security.NoAccessBehavior.ThrowException)))
        {
          if (typeof(P) == typeof(string) && newValue == null)
            newValue = Utilities.CoerceValue<P>(typeof(string), null, string.Empty);
          LoadPropertyValue<P>(propertyInfo, oldValue, newValue, !_bypassPropertyChecks);
        }
      }
      catch (SecurityException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(
          string.Format(Resources.PropertyLoadException, propertyInfo.Name, ex.Message), ex);
      }
    }

    /// <summary>
    /// Sets a property's managed field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// If the user is not authorized to change the 
    /// property a SecurityException is thrown.
    /// </remarks>
    protected void SetProperty(IPropertyInfo propertyInfo, object newValue)
    {
      try
      {
        if (_bypassPropertyChecks || CanWriteProperty(propertyInfo.Name, true))
        {
          if (!_bypassPropertyChecks) OnPropertyChanging(propertyInfo.Name);
          FieldManager.SetFieldData(propertyInfo, newValue);
          if (!_bypassPropertyChecks) PropertyHasChanged(propertyInfo.Name);
        }
      }
      catch (SecurityException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(
          string.Format(Resources.PropertyLoadException, propertyInfo.Name, ex.Message), ex);
      }
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
        throw new PropertyLoadException(
          string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message), ex);
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
        throw new PropertyLoadException(
          string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message), ex);
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
      {
        IBusinessObject old = oldValue as IBusinessObject;
        if (old != null)
          OnRemoveEventHooksInternal(old);
        IBusinessObject @new = newValue as IBusinessObject;
        if (@new != null)
          OnAddEventHooksInternal(@new);

        if (typeof(IEditableBusinessObject).IsAssignableFrom(propertyInfo.Type))
        {
          if (markDirty)
          {
            OnPropertyChanging(propertyInfo.Name);
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
            PropertyHasChanged(propertyInfo.Name);
          }
          else
          {
            FieldManager.LoadFieldData<P>(propertyInfo, newValue);
          }
          IEditableBusinessObject child = (IEditableBusinessObject)newValue;
          if (child != null)
          {
            child.SetParent(this);
            // set child edit level
            UndoableBase.ResetChildEditLevel(child, this.EditLevel, this.BindingEdit);
            // reset EditLevelAdded 
            child.EditLevelAdded = this.EditLevel;
          }
        }
        else if (typeof(IEditableCollection).IsAssignableFrom(propertyInfo.Type))
        {
          if (markDirty)
          {
            OnPropertyChanging(propertyInfo.Name);
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
            PropertyHasChanged(propertyInfo.Name);
          }
          else
          {
            FieldManager.LoadFieldData<P>(propertyInfo, newValue);
          }
          IEditableCollection child = (IEditableCollection)newValue;
          if (child != null)
          {
            child.SetParent(this);
            IUndoableObject undoChild = child as IUndoableObject;
            if (undoChild != null)
            {
              // set child edit level
              UndoableBase.ResetChildEditLevel(undoChild, this.EditLevel, this.BindingEdit);
            }
          }
        }
        else
        {
          if (markDirty)
          {
            OnPropertyChanging(propertyInfo.Name);
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
            PropertyHasChanged(propertyInfo.Name);
          }
          else
          {
            FieldManager.LoadFieldData<P>(propertyInfo, newValue);
          }
        }
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

    [NotUndoable]
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

    private void loadManager_UnhandledAsyncException(object sender, ErrorEventArgs e)
    {
      OnUnhandledAsyncException(e);
    }

    private void loadManager_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

    protected void LoadPropertyAsync<R, P>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P> factory, P parameter)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, parameter);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    protected void LoadPropertyAsync<R, P1, P2>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P1, P2> factory, P1 p1, P2 p2)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, p1, p2);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    protected void LoadPropertyAsync<R, P1, P2, P3>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P1, P2, P3> factory, P1 p1, P2 p2, P3 p3)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, p1, p2, p3);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    protected void LoadPropertyAsync<R, P1, P2, P3, P4>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P1, P2, P3, P4> factory, P1 p1, P2 p2, P3 p3, P4 p4)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, p1, p2, p3, p4);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    protected void LoadPropertyAsync<R, P1, P2, P3, P4, P5>(PropertyInfo<R> property, AsyncFactoryDelegate<R, P1, P2, P3, P4, P5> factory, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
      AsyncLoader loader = new AsyncLoader(property, factory, LoadProperty, OnPropertyChanged, p1, p2, p3);
      LoadManager.BeginLoad(loader, (EventHandler<DataPortalResult<R>>)loader.LoadComplete);
    }

    #endregion

    #region Child Change Notification

    [NonSerialized]
    [NotUndoable]
    private EventHandler<Csla.Core.ChildChangedEventArgs> _childChangedHandlers;

    /// <summary>
    /// Event raised when a child object has been changed.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event EventHandler<Csla.Core.ChildChangedEventArgs> ChildChanged
    {
      add
      {
        _childChangedHandlers = (EventHandler<Csla.Core.ChildChangedEventArgs>)
          System.Delegate.Combine(_childChangedHandlers, value);
      }
      remove
      {
        _childChangedHandlers = (EventHandler<Csla.Core.ChildChangedEventArgs>)
          System.Delegate.Remove(_childChangedHandlers, value);
      }
    }

    /// <summary>
    /// Raises the ChildChanged event, indicating that a child
    /// object has been changed.
    /// </summary>
    /// <param name="source">
    /// Reference to the object that was changed.
    /// </param>
    /// <param name="listArgs">
    /// ListChangedEventArgs object or null.
    /// </param>
    /// <param name="propertyArgs">
    /// PropertyChangedEventArgs object or null.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnChildChanged(object source, PropertyChangedEventArgs propertyArgs, NotifyCollectionChangedEventArgs listArgs)
    {
      ChildChangedEventArgs args = new ChildChangedEventArgs(source, propertyArgs, listArgs);
      OnChildChangedInternal(this, args);
    }

    protected internal virtual void OnChildChangedInternal(object sender, ChildChangedEventArgs e)
    {
      OnChildChanged(sender, e);

      if (_childChangedHandlers != null)
        _childChangedHandlers.Invoke(this, e);
    }

    protected virtual void OnChildChanged(object sender, ChildChangedEventArgs e)
    {
    }

    private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(sender is INotifyCollectionChanged && (e.PropertyName == "Count" || e.PropertyName == "Item[]")))
        OnChildChanged(sender, e, null);
    }

    private void Child_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnChildChanged(sender, null, e);
    }

    void Child_Changed(object sender, ChildChangedEventArgs e)
    {
      OnChildChangedInternal(this, e);
    }

    #endregion

    #region FieldManager

    private FieldManager.FieldDataManager _fieldManager;

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

    private void FieldDataDeserialized()
    {
      foreach (object item in FieldManager.GetChildren())
      {
        IBusinessObject business = item as IBusinessObject;
        if (business != null)
          OnAddEventHooksInternal(business);

        IEditableBusinessObject child = item as IEditableBusinessObject;
        if (child != null)
        {
          child.SetParent(this);
        }
        IEditableCollection childCollection = item as IEditableCollection;
        if (childCollection != null)
        {
          childCollection.SetParent(this);
        }
      }
    }

    protected T GetProperty<T>(IPropertyInfo propertyInfo)
    {
      IFieldData data = FieldManager.GetFieldData(propertyInfo);
      return (T)(data != null ? data.Value : null);
    }

    protected void SetProperty<T>(IPropertyInfo propertyInfo, T value)
    {
      FieldManager.SetFieldData<T>(propertyInfo, value);
    }

    #endregion

    #region Bubbling event Hooks

    protected internal virtual void OnAddEventHooksInternal(IBusinessObject child)
    {
      INotifyBusy busy = child as INotifyBusy;
      if (busy != null)
        busy.BusyChanged += new BusyChangedEventHandler(Child_BusyChanged);

      INotifyUnhandledAsyncException unhandled = child as INotifyUnhandledAsyncException;
      if (unhandled != null)
        unhandled.UnhandledAsyncException += new EventHandler<ErrorEventArgs>(Child_UnhandledAsyncException);

      INotifyPropertyChanged pc = child as INotifyPropertyChanged;
      if (pc != null)
        pc.PropertyChanged += new PropertyChangedEventHandler(Child_PropertyChanged);

      INotifyCollectionChanged cc = child as INotifyCollectionChanged;
      if (cc != null)
        cc.CollectionChanged += new NotifyCollectionChangedEventHandler(Child_CollectionChanged);

      INotifyChildChanged childChanged = child as INotifyChildChanged;
      if (childChanged != null)
        childChanged.ChildChanged += new EventHandler<ChildChangedEventArgs>(Child_Changed);

      OnAddEventHooks(child);
    }

    protected virtual void OnAddEventHooks(IBusinessObject child)
    {
    }

    protected internal virtual void OnRemoveEventHooksInternal(IBusinessObject child)
    {
      INotifyBusy busy = child as INotifyBusy;
      if (busy != null)
        busy.BusyChanged -= new BusyChangedEventHandler(Child_BusyChanged);

      INotifyUnhandledAsyncException unhandled = child as INotifyUnhandledAsyncException;
      if (unhandled != null)
        unhandled.UnhandledAsyncException -= new EventHandler<ErrorEventArgs>(Child_UnhandledAsyncException);

      INotifyPropertyChanged pc = child as INotifyPropertyChanged;
      if (pc != null)
        pc.PropertyChanged -= new PropertyChangedEventHandler(Child_PropertyChanged);

      INotifyCollectionChanged cc = child as INotifyCollectionChanged;
      if (cc != null)
        cc.CollectionChanged -= new NotifyCollectionChangedEventHandler(Child_CollectionChanged);

      INotifyChildChanged childChanged = child as INotifyChildChanged;
      if (childChanged != null)
        childChanged.ChildChanged -= new EventHandler<ChildChangedEventArgs>(Child_Changed);

      OnRemoveEventHooks(child);
    }

    protected virtual void OnRemoveEventHooks(IBusinessObject child)
    {
    }

    #endregion

    #region Busy / Unhandled exception bubbling

    void Child_UnhandledAsyncException(object sender, ErrorEventArgs e)
    {
      OnUnhandledAsyncException(e);
    }

    void Child_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

    #endregion

    #region UndoableBase overrides

    protected override void OnCopyState(SerializationInfo state)
    {
      OnGetState(state, StateMode.Undo);
      ((IUndoableObject)FieldManager).CopyState(this.EditLevel + 1, false);
      ((IUndoableObject)ValidationRules).CopyState(this.EditLevel + 1, false);

      base.OnCopyState(state);
    }

    protected override void OnUndoChanges(SerializationInfo state)
    {
      OnSetState(state, StateMode.Undo);
      ((IUndoableObject)FieldManager).UndoChanges(this.EditLevel - 1, false);
      ((IUndoableObject)ValidationRules).UndoChanges(this.EditLevel - 1, false);

      base.OnUndoChanges(state);
    }

    protected override void AcceptingChanges()
    {
      ((IUndoableObject)FieldManager).AcceptChanges(this.EditLevel - 1, false);
      ((IUndoableObject)ValidationRules).AcceptChanges(this.EditLevel - 1, false);

      base.AcceptingChanges();
    }
    #endregion

    #region MobileFormatter

    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      base.OnGetState(info, mode);
      info.AddValue("Csla.Core.BusinessBase._isNew", _isNew);
      info.AddValue("Csla.Core.BusinessBase._isDeleted", _isDeleted);
      info.AddValue("Csla.Core.BusinessBase._isDirty", _isDirty);
      info.AddValue("Csla.Core.BusinessBase._neverCommitted", _neverCommitted);
      info.AddValue("Csla.Core.BusinessBase._disableIEditableObject", _disableIEditableObject);
      info.AddValue("Csla.Core.BusinessBase._isChild", _isChild);
      info.AddValue("Csla.Core.BusinessBase._editLevelAdded", _editLevelAdded);
    }

    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      base.OnSetState(info, mode);
      _isNew = info.GetValue<bool>("Csla.Core.BusinessBase._isNew");
      _isDeleted = info.GetValue<bool>("Csla.Core.BusinessBase._isDeleted");
      _isDirty = info.GetValue<bool>("Csla.Core.BusinessBase._isDirty");
      _neverCommitted = info.GetValue<bool>("Csla.Core.BusinessBase._neverCommitted");
      _disableIEditableObject = info.GetValue<bool>("Csla.Core.BusinessBase._disableIEditableObject");
      _isChild = info.GetValue<bool>("Csla.Core.BusinessBase._isChild");
      if (mode != StateMode.Undo)
        _editLevelAdded = info.GetValue<int>("Csla.Core.BusinessBase._editLevelAdded");
    }

    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      base.OnGetChildren(info, formatter);

      if (_fieldManager != null)
      {
        var fieldManagerInfo = formatter.SerializeObject(_fieldManager);
        info.AddChild("_fieldManager", fieldManagerInfo.ReferenceId);
      }

      if (_validationRules != null)
      {
        var vrInfo = formatter.SerializeObject(_validationRules);
        info.AddChild("_validationRules", vrInfo.ReferenceId);
      }
    }

    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Children.ContainsKey("_fieldManager"))
      {
        var childData = info.Children["_fieldManager"];
        _fieldManager = (FieldDataManager)formatter.GetObject(childData.ReferenceId);
      }

      if (info.Children.ContainsKey("_validationRules"))
      {
        int refId = info.Children["_validationRules"].ReferenceId;
        _validationRules = (ValidationRules)formatter.GetObject(refId);
      }

      base.OnSetChildren(info, formatter);
    }

    #endregion

    #region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return Core.ObjectCloner.Clone(this);
    }

    #endregion

    #region ValidationRules, IsValid

    private Validation.ValidationRules _validationRules;

    public event EventHandler ValidationComplete;

    protected virtual void OnValidationComplete()
    {
      if (ValidationComplete != null)
        ValidationComplete(this, EventArgs.Empty);
    }

    private void InitializeBusinessRules()
    {
      AddInstanceBusinessRules();
      if (!(Validation.SharedValidationRules.RulesExistFor(this.GetType())))
      {
        lock (this.GetType())
        {
          if (!(Validation.SharedValidationRules.RulesExistFor(this.GetType())))
          {
            Validation.SharedValidationRules.GetManager(this.GetType(), true);
            try { AddBusinessRules(); }
            catch
            {
              // Prevent incomplete rule managers from being created. This will cause
              // rules to be recreated for every instance until successful. This fixes
              // a bug where it's possible to create an instance with an invalid rule
              // if you catch the InvalidOperationException the first time.
              Validation.SharedValidationRules.RemoveManager(this.GetType());
              throw;
            }
          }
        }
      }
    }

    /// <summary>
    /// Provides access to the broken rules functionality.
    /// </summary>
    /// <remarks>
    /// This property is used within your business logic so you can
    /// easily call the AddRule() method to associate validation
    /// rules with your object's properties.
    /// </remarks>
    protected Validation.ValidationRules ValidationRules
    {
      get
      {
        if (_validationRules == null)
        {
          _validationRules = new Csla.Validation.ValidationRules(this);
          _validationRules.ValidatingRules.CollectionChanged += new NotifyCollectionChangedEventHandler(ValidatingRules_CollectionChanged);
        }
        else
          if (_validationRules.Target == null)
            _validationRules.SetTarget(this);
        return _validationRules;
      }
    }

    void ValidatingRules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        foreach (IAsyncRuleMethod rule in e.OldItems)
        {
          lock (_validationRules.ValidatingRules)
          {
            // This rule could be validating multiple times simultaneously, we only want to call
            // OnPropertyIdle if the rule is completely removed from the list.
            if (!_validationRules.ValidatingRules.Contains(rule))
            {
              foreach (IPropertyInfo property in rule.AsyncRuleArgs.Properties)
              {
                OnPropertyChanged(property.Name);
                OnBusyChanged(new BusyChangedEventArgs(property.Name, false));
              }
            }
          }
        }

        if (!ValidationRules.IsValidating)
          OnValidationComplete();
      }
      else if (e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (IAsyncRuleMethod rule in e.NewItems)
          foreach (IPropertyInfo property in rule.AsyncRuleArgs.Properties)
            OnBusyChanged(new BusyChangedEventArgs(property.Name, true));
      }
    }

    /// <summary>
    /// Override this method in your business class to
    /// be notified when you need to set up business
    /// rules.
    /// </summary>
    /// <remarks>
    /// This method is automatically called by CSLA .NET
    /// when your object should associate per-instance
    /// validation rules with its properties.
    /// </remarks>
    protected virtual void AddInstanceBusinessRules()
    {

    }

    /// <summary>
    /// Override this method in your business class to
    /// be notified when you need to set up shared 
    /// business rules.
    /// </summary>
    /// <remarks>
    /// This method is automatically called by CSLA .NET
    /// when your object should associate per-type 
    /// validation rules with its properties.
    /// </remarks>
    protected virtual void AddBusinessRules()
    {

    }

    /// <summary>
    /// Returns <see langword="true" /> if the object 
    /// and its child objects are currently valid, 
    /// <see langword="false" /> if the
    /// object or any of its child objects have broken 
    /// rules or are otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling ValidationRules
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior. For instance, you should always override
    /// this method if your object has child objects, since the validity of this object
    /// is affected by the validity of all child objects.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    public virtual bool IsValid
    {
      get { return IsSelfValid && (_fieldManager == null || FieldManager.IsValid()); }
    }

    /// <summary>
    /// Returns <see langword="true" /> if the object is currently 
    /// valid, <see langword="false" /> if the
    /// object has broken rules or is otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling ValidationRules
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior. 
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    public virtual bool IsSelfValid
    {
      get { return ValidationRules.IsValid; }
    }

    /// <summary>
    /// Provides access to the readonly collection of broken business rules
    /// for this object.
    /// </summary>
    /// <returns>A Csla.Validation.RulesCollection object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual Validation.BrokenRulesCollection BrokenRulesCollection
    {
      get { return ValidationRules.GetBrokenRules(); }
    }

    #endregion

    #region IsBusy / IsIdle

    [NonSerialized]
    [NotUndoable]
    private bool _isBusy;

    protected void MarkBusy()
    {
      // TODO: Review resource string
      if (_isBusy)
        throw new InvalidOperationException(Resources.BusyObjectsMayNotBeMarkedBusy);

      _isBusy = true;
      OnBusyChanged(new BusyChangedEventArgs("", true));
    }

    protected void MarkIdle()
    {
      _isBusy = false;
      OnBusyChanged(new BusyChangedEventArgs("", false));
    }

    public bool IsBusy
    {
      get { return IsSelfBusy || (_fieldManager != null && FieldManager.IsBusy()); }
    }

    public bool IsSelfBusy
    {
      get { return _isBusy || ValidationRules.IsValidating || LoadManager.IsLoading; }
    }

    [NotUndoable]
    [NonSerialized]
    private BusyChangedEventHandler _busyChanged;

    public event BusyChangedEventHandler BusyChanged
    {
      add { _busyChanged = (BusyChangedEventHandler)Delegate.Combine(_busyChanged, value); }
      remove { _busyChanged = (BusyChangedEventHandler)Delegate.Remove(_busyChanged, value); }
    }

    protected void OnBusyChanged(string propertyName, bool busy)
    {
      OnBusyChanged(new BusyChangedEventArgs(propertyName, busy));
    }

    protected virtual void OnBusyChanged(BusyChangedEventArgs args)
    {
      if (_busyChanged != null)
        _busyChanged(this, args);
    }

    public bool IsPropertyBusy(string propertyName)
    {
      bool isbusy = false;
      if (_validationRules != null)
      {
        lock (_validationRules.ValidatingRules)
        {
          isbusy = (from rules in _validationRules.ValidatingRules
                    from property in rules.AsyncRuleArgs.Properties
                    where property.Name == propertyName
                    select rules).Count() > 0;
        }
      }
      return isbusy;
    }

    #endregion

    #region INotifyUnhandledAsyncException Members

    [NotUndoable]
    [NonSerialized]
    private EventHandler<ErrorEventArgs> _unhandledAsyncException;

    public event EventHandler<ErrorEventArgs> UnhandledAsyncException
    {
      add { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
      remove { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
    }

    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
      if (_unhandledAsyncException != null)
        _unhandledAsyncException(this, error);
    }

    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncException(new ErrorEventArgs(originalSender, error));
    }

    #endregion

    #region  IParent

    /// <summary>
    /// Override this method to be notified when a child object's
    /// <see cref="Core.BusinessBase.ApplyEdit" /> method has
    /// completed.
    /// </summary>
    /// <param name="child">The child object that was edited.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void EditChildComplete(Core.IEditableBusinessObject child)
    {
      // do nothing, we don't really care
      // when a child has its edits applied
    }

    void IParent.ApplyEditChild(Core.IEditableBusinessObject child)
    {
      this.EditChildComplete(child);
    }

    void IParent.RemoveChild(IEditableBusinessObject child)
    {
      var info = FieldManager.FindProperty(child);
      FieldManager.RemoveField(info);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Marks the object for deletion. The object will be deleted as part of the
    /// next save operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// CSLA .NET supports both immediate and deferred deletion of objects. This
    /// method is part of the support for deferred deletion, where an object
    /// can be marked for deletion, but isn't actually deleted until the object
    /// is saved to the database. This method is called by the UI developer to
    /// mark the object for deletion.
    /// </para><para>
    /// To 'undelete' an object, use n-level undo as discussed in Chapters 2 and 3.
    /// </para>
    /// </remarks>
    public virtual void Delete()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.ChildDeleteException);

      MarkDeleted();
    }

    /// <summary>
    /// Called by a parent object to mark the child
    /// for deferred deletion.
    /// </summary>
    internal void DeleteChild()
    {
      if (!this.IsChild)
        throw new NotSupportedException(Resources.NoDeleteRootException);

      BindingEdit = false;
      MarkDeleted();
    }

    #endregion

    #region Edit Level Tracking (child only)

    // we need to keep track of the edit
    // level when we weere added so if the user
    // cancels below that level we can be destroyed
    [NotUndoable]
    private int _editLevelAdded;

    /// <summary>
    /// Gets or sets the current edit level of the
    /// object.
    /// </summary>
    /// <remarks>
    /// Allow the collection object to use the
    /// edit level as needed.
    /// </remarks>
    internal int EditLevelAdded
    {
      get { return _editLevelAdded; }
      set { _editLevelAdded = value; }
    }

    int IUndoableObject.EditLevel
    {
      get
      {
        return this.EditLevel;
      }
    }

    #endregion

    #region IDataPortalTarget Members

    void IDataPortalTarget.MarkAsChild()
    {
      MarkAsChild();
    }

    void IDataPortalTarget.MarkNew()
    {
      MarkNew();
    }

    void IDataPortalTarget.MarkOld()
    {
      MarkOld();
    }

    void IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.DataPortal_OnDataPortalException(e, ex);
    }

    void IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.Child_OnDataPortalException(e, ex);
    }

    #endregion

    #region Data Access

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
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
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    /// <remarks>
    /// Normally you will overload this method to accept a strongly-typed
    /// criteria parameter, rather than overriding the method with a
    /// loosely-typed criteria parameter.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void Child_Create()
    {
      ValidationRules.CheckRules();
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
      OnDeserializedInternal();
      OnDeserialized();
      ValidationRules.SetTarget(this);
      ValidationRules.ValidatingRules.CollectionChanged += new NotifyCollectionChangedEventHandler(ValidatingRules_CollectionChanged);
      if (_fieldManager != null)
        FieldManager.SetPropertyList(this.GetType());
      InitializeBusinessRules();
      InitializeAuthorizationRules();
      FieldDataDeserialized();
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete, it is only implemented
    /// by internal classes to guarantee that they are executed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void OnDeserializedInternal()
    {
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    /// <param name="context">Serialization context object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized()
    {
      // do nothing - this is here so a subclass
      // could override if needed
    }

    #endregion

    #region IEditableObject Members

    [NotUndoable]
    private bool _disableIEditableObject;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool DisableIEditableObject
    {
      get
      {
        return _disableIEditableObject;
      }
      set
      {
        _disableIEditableObject = value;
      }
    }

    void IEditableObject.BeginEdit()
    {
      if (!_disableIEditableObject && !BindingEdit)
      {
        BindingEdit = true;
        BeginEdit();
      }
    }

    void IEditableObject.CancelEdit()
    {
      if (!_disableIEditableObject && BindingEdit)
      {
        CancelEdit();
        if (IsNew && _neverCommitted && EditLevel <= EditLevelAdded)
        {
          // we're new and no EndEdit or ApplyEdit has ever been
          // called on us, and now we've been cancelled back to
          // where we were added so we should have ourselves
          // removed from the parent collection
          if (Parent != null)
            Parent.RemoveChild(this);
        }
      }
    }

    void IEditableObject.EndEdit()
    {
      if (!_disableIEditableObject && BindingEdit)
      {
        ApplyEdit();
      }
    }

    #endregion

    #region Property Checks ByPass

    [NonSerialized]
    [NotUndoable]
    private bool _bypassPropertyChecks = false;

    /// <summary>
    /// By wrapping this property inside Using block
    /// you can set property values on current business object
    /// without raising PropertyChanged events
    /// and checking user rights.
    /// </summary>
    protected internal BypassPropertyChecksObject BypassPropertyChecks
    {
      get { return new BypassPropertyChecksObject(this); }
    }

    /// <summary>
    /// Class that allows setting of property values on 
    /// current business object
    /// without raising PropertyChanged events
    /// and checking user rights.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal class BypassPropertyChecksObject : IDisposable
    {
      private BusinessBase _businessObject;
      private BypassPropertyChecksObject() { }
      internal BypassPropertyChecksObject(BusinessBase businessObject)
      {
        _businessObject = businessObject;
        _businessObject._bypassPropertyChecks = true;
      }
      #region IDisposable Members

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      protected void Dispose(bool dispose)
      {
        _businessObject._bypassPropertyChecks = false;
        _businessObject = null;
      }


      #endregion
    }

    #endregion
  }
}
