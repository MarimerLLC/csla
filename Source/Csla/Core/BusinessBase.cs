//-----------------------------------------------------------------------
// <copyright file="BusinessBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the non-generic base class from which most</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Csla.Properties;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using Csla.Core.LoadManager;
using Csla.Reflection;
using Csla.Server;
using Csla.Security;
using Csla.Serialization.Mobile;
using Csla.Rules;
using System.Security;
using Csla.Core.FieldManager;
using System.Reflection;

namespace Csla.Core
{

  /// <summary>
  /// This is the non-generic base class from which most
  /// business objects will be derived.
  /// </summary>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public abstract class BusinessBase : UndoableBase,
    IEditableBusinessObject,
    IEditableObject,
    ICloneable,
    IAuthorizeReadWrite,
    IParent,
    IDataPortalTarget,
    IManageProperties,
    Rules.IHostRules,
    ICheckRules,
    INotifyBusy,
    INotifyChildChanged,
    ISerializationNotification
, IDataErrorInfo
  {

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected BusinessBase()
    {
      InitializeIdentity();
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

    private int _identity = -1;

    int IBusinessObject.Identity
    {
      get { return _identity; }
    }

    private void InitializeIdentity()
    {
      _identity = ((IParent)this).GetNextIdentity(_identity);
    }

    [NonSerialized]
    [NotUndoable]
    private IdentityManager _identityManager;

    int IParent.GetNextIdentity(int current)
    {
      if (this.Parent != null)
      {
        return this.Parent.GetNextIdentity(current);
      }
      else
      {
        if (_identityManager == null)
          _identityManager = new IdentityManager();
        return _identityManager.GetNextIdentity(current);
      }
    }
    
    #endregion

    #region Parent/Child link

    [NotUndoable]
    [NonSerialized]
    private IParent _parent;

    /// <summary>
    /// Provide access to the parent reference for use
    /// in child object code.
    /// </summary>
    /// <remarks>
    /// This value will be Nothing for root objects.
    /// </remarks>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Core.IParent Parent
    {
      get { return _parent; }
    }

    /// <summary>
    /// Used by BusinessListBase as a child object is 
    /// created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    protected virtual void SetParent(Core.IParent parent)
    {
      _parent = parent;
      _identityManager = null;
      InitializeIdentity();
    }

    #endregion

    #region IsNew, IsDeleted, IsDirty, IsSavable

    // keep track of whether we are new, deleted or dirty
    private bool _isNew = true;
    private bool _isDeleted;
    private bool _isDirty = true;

    /// <summary>
    /// Returns true if this is a new object, 
    /// false if it is a pre-existing object.
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
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public bool IsNew
    {
      get { return _isNew; }
    }

    /// <summary>
    /// Returns true if this object is marked for deletion.
    /// </summary>
    /// <remarks>
    /// CSLA .NET supports both immediate and deferred deletion of objects. This
    /// property is part of the support for deferred deletion, where an object
    /// can be marked for deletion, but isn't actually deleted until the object
    /// is saved to the database. This property indicates whether or not the
    /// current object has been marked for deletion. If it is true
    /// , the object will
    /// be deleted when it is saved to the database, otherwise it will be inserted
    /// or updated by the save operation.
    /// </remarks>
    /// <returns>A value indicating if this object is marked for deletion.</returns>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public bool IsDeleted
    {
      get { return _isDeleted; }
    }

    /// <summary>
    /// Returns true if this object's 
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
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsDirty
    {
      get { return IsSelfDirty || (_fieldManager != null && FieldManager.IsDirty()); }
    }

    /// <summary>
    /// Returns true if this object's data has been changed.
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
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
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
      MetaPropertyHasChanged("IsNew");
      MetaPropertyHasChanged("IsDeleted");
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
      MetaPropertyHasChanged("IsNew");
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
      MetaPropertyHasChanged("IsDeleted");
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
    /// true to supress the PropertyChanged event that is otherwise
    /// raised to indicate that the object's state has changed.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkDirty(bool suppressEvent)
    {
      bool old = _isDirty;
      _isDirty = true;
      if (!suppressEvent)
        OnUnknownPropertyChanged();
      if (_isDirty != old)
      {
        MetaPropertyHasChanged("IsSelfDirty");
        MetaPropertyHasChanged("IsDirty");
        MetaPropertyHasChanged("IsSavable");
      }
    }

    /// <summary>
    /// Performs processing required when a property
    /// has changed.
    /// </summary>
    /// <param name="property">Property that
    /// has changed.</param>
    /// <remarks>
    /// This method calls CheckRules(propertyName), MarkDirty and
    /// OnPropertyChanged(propertyName). MarkDirty is called such
    /// that no event is raised for IsDirty, so only the specific
    /// property changed event for the current property is raised.
    /// </remarks>
    protected virtual void PropertyHasChanged(Csla.Core.IPropertyInfo property)
    {
      MarkDirty(true);
      CheckPropertyRules(property);
    }

    private void PropertyHasChanged(string propertyName)
    {
      PropertyHasChanged(FieldManager.GetRegisteredProperty(propertyName));
    }

    /// <summary>
    /// Raises OnPropertyChanged for meta properties (IsXYZ) when PropertyChangedMode is not Windows
    /// </summary>
    /// <param name="name">meta property name that has cchanged.</param>
    protected virtual void MetaPropertyHasChanged(string name)
    {
      if (ApplicationContext.PropertyChangedMode != ApplicationContext.PropertyChangedModes.Windows)
        OnMetaPropertyChanged(name);
    }

    /// <summary>
    /// Check rules for the property and notifies UI of properties that may have changed.
    /// </summary>
    /// <param name="property">The property.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void CheckPropertyRules(IPropertyInfo property)
    {
      var propertyNames = BusinessRules.CheckRules(property);
      if (ApplicationContext.PropertyChangedMode == ApplicationContext.PropertyChangedModes.Windows)
        OnPropertyChanged(property);
      else
        foreach (var name in propertyNames)
          OnPropertyChanged(name);
    }

    /// <summary>
    /// Check object rules and notifies UI of properties that may have changed. 
    /// </summary>
    protected virtual void CheckObjectRules()
    {
      var propertyNames = BusinessRules.CheckObjectRules();
      if (ApplicationContext.PropertyChangedMode == ApplicationContext.PropertyChangedModes.Windows)
      {
        OnUnknownPropertyChanged();
      }
      else
        foreach (var name in propertyNames)
          OnPropertyChanged(name);
    }



    /// <summary>
    /// Forces the object's IsDirty flag to false.
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
      MetaPropertyHasChanged("IsSelfDirty");
      MetaPropertyHasChanged("IsDirty");
      MetaPropertyHasChanged("IsSavable");
    }

    /// <summary>
    /// Returns true if this object is both dirty and valid.
    /// </summary>
    /// <remarks>
    /// An object is considered dirty (changed) if 
    /// <see cref="P:Csla.BusinessBase.IsDirty" /> returns true. It is
    /// considered valid if IsValid
    /// returns true. The IsSavable property is
    /// a combination of these two properties. 
    /// </remarks>
    /// <returns>A value indicating if this object is both dirty and valid.</returns>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsSavable
    {
      get
      {
        bool auth;
        if (IsDeleted)
          auth = Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, this);
        else if (IsNew)
          auth = Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, this);
        else
          auth = Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, this);
        return (auth && IsDirty && IsValid && !IsBusy);
      }
    }

    #endregion

    #region Authorization

    [NotUndoable]
    [NonSerialized]
    private ConcurrentDictionary<string, bool> _readResultCache;
    [NotUndoable]
    [NonSerialized]
    private ConcurrentDictionary<string, bool> _writeResultCache;
    [NotUndoable]
    [NonSerialized]
    private ConcurrentDictionary<string, bool> _executeResultCache;
    [NotUndoable]
    [NonSerialized]
    private System.Security.Principal.IPrincipal _lastPrincipal;

    /// <summary>
    /// Returns true if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <param name="property">Property to check.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanReadProperty(Csla.Core.IPropertyInfo property)
    {
      var result = true;

      VerifyAuthorizationCache();

      if (!_readResultCache.TryGetValue(property.Name, out result))
      {
        result = BusinessRules.HasPermission(AuthorizationActions.ReadProperty, property);
        if (BusinessRules.CachePermissionResult(AuthorizationActions.ReadProperty, property))
        {
          // store value in cache
          _readResultCache.AddOrUpdate(property.Name, result, (a,b) => { return result; });
        }
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
        Trace.TraceError("CanReadProperty: {0} is not a registered property of {1}.{2}", propertyName, this.GetType().Namespace, this.GetType().Name);
        return true;
      }
      return CanReadProperty(propertyInfo, throwOnFalse);
    }

    /// <summary>
    /// Returns true if the user is allowed to write the
    /// specified property.
    /// </summary>
    /// <param name="property">Property to write.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanWriteProperty(Csla.Core.IPropertyInfo property)
    {
      bool result = true;

      VerifyAuthorizationCache();

      if (!_writeResultCache.TryGetValue(property.Name, out result))
      {
        result = BusinessRules.HasPermission(AuthorizationActions.WriteProperty, property);
        if (BusinessRules.CachePermissionResult(AuthorizationActions.WriteProperty, property))
        {
          // store value in cache
          _writeResultCache.AddOrUpdate(property.Name, result, (a, b) => { return result; });
        }
      }
      return result;
    }

    /// <summary>
    /// Returns true if the user is allowed to write the
    /// calling property.
    /// </summary>
    /// <returns>true if write is allowed.</returns>
    /// <param name="property">Property to write.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool CanWriteProperty(Csla.Core.IPropertyInfo property, bool throwOnFalse)
    {
      bool result = CanWriteProperty(property);
      if (throwOnFalse && result == false)
      {
        Csla.Security.SecurityException ex = new Csla.Security.SecurityException(
          String.Format("{0} ({1})", Resources.PropertySetNotAllowed, property.Name));
        throw ex;
      }
      return result;
    }

    /// <summary>
    /// Returns true if the user is allowed to write the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool CanWriteProperty(string propertyName)
    {
      return CanWriteProperty(propertyName, false);
    }

    /// <summary>
    /// Returns true if the user is allowed to write the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    private bool CanWriteProperty(string propertyName, bool throwOnFalse)
    {
      var propertyInfo = FieldManager.GetRegisteredProperties().FirstOrDefault(p => p.Name == propertyName);
      if (propertyInfo == null)
      {
        Trace.TraceError("CanReadProperty: {0} is not a registered property of {1}.{2}", propertyName, this.GetType().Namespace, this.GetType().Name);
        return true;
      }
      return CanWriteProperty(propertyInfo, throwOnFalse);
    }

    private void VerifyAuthorizationCache()
    {
      if (_readResultCache == null)
        _readResultCache = new ConcurrentDictionary<string, bool>();
      if (_writeResultCache == null)
        _writeResultCache = new ConcurrentDictionary<string, bool>();
      if (_executeResultCache == null)
        _executeResultCache = new ConcurrentDictionary<string, bool>();
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
        if (BusinessRules.CachePermissionResult(AuthorizationActions.ExecuteMethod, method))
        {
          // store value in cache
          _executeResultCache.AddOrUpdate(method.Name, result, (a, b) => { return result; });
        }
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

#region System.ComponentModel.IEditableObject

    private bool _neverCommitted = true;
    [NotUndoable]
    private bool _disableIEditableObject;

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// IEditableObject interface methods should
    /// be disabled for this object.
    /// </summary>
    /// <value>Defaults to False, indicating that
    /// the IEditableObject methods will behave
    /// normally.</value>
    /// <remarks>
    /// If you disable the IEditableObject methods
    /// then Windows Forms data binding will no longer
    /// automatically call BeginEdit, CancelEdit or
    /// ApplyEdit on your object, and you will have
    /// to call these methods manually to get proper
    /// n-level undo behavior.
    /// </remarks>
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

    /// <summary>
    /// Allow data binding to start a nested edit on the object.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call should be honored, so we have extra code to detect this
    /// and do nothing for subsquent calls.
    /// </remarks>
    void System.ComponentModel.IEditableObject.BeginEdit()
    {
      if (!_disableIEditableObject && !BindingEdit)
      {
        BindingEdit = true;
        BeginEdit();
      }
    }

    /// <summary>
    /// Allow data binding to cancel the current edit.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call to either IEditableObject.CancelEdit or 
    /// IEditableObject.EndEdit
    /// should be honored. We include extra code to detect this and do
    /// nothing for subsequent calls.
    /// </remarks>
    void System.ComponentModel.IEditableObject.CancelEdit()
    {
      if (!_disableIEditableObject && BindingEdit)
      {
        CancelEdit();
        BindingEdit = false;
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

    /// <summary>
    /// Allow data binding to apply the current edit.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call to either IEditableObject.EndEdit or 
    /// IEditableObject.CancelEdit
    /// should be honored. We include extra code to detect this and do
    /// nothing for subsequent calls.
    /// </remarks>
    void System.ComponentModel.IEditableObject.EndEdit()
    {
      if (!_disableIEditableObject && BindingEdit)
      {
        ApplyEdit();
        BindingEdit = false;
      }
    }

#endregion

#region Begin/Cancel/ApplyEdit

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
      BusinessRules.SetTarget(this);
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
      //Next line moved to IEditableObject.ApplyEdit 
      //BindingEdit = false;
    }

    /// <summary>
    /// Notifies the parent object (if any) that this
    /// child object's edits have been accepted.
    /// </summary>
    protected override void AcceptChangesComplete()
    {
      BindingEdit = false;
      base.AcceptChangesComplete();

      // !!!! Will trigger Save here when using DynamicListBase template
      if (Parent != null)
        Parent.ApplyEditChild(this);
    }

#endregion

#region IsChild

    [NotUndoable]
    private bool _isChild;

    /// <summary>
    /// Returns true if this is a child (non-root) object.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public bool IsChild
    {
      get { return _isChild; }
    }

    /// <summary>
    /// Marks the object as being a child object.
    /// </summary>
    protected void MarkAsChild()
    {
      _identity = -1;
      _isChild = true;
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

#region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return ObjectCloner.Clone(this);
    }

#endregion

#region BusinessRules, IsValid

    [NonSerialized]
    [NotUndoable]
    private EventHandler _validationCompleteHandlers;

    /// <summary>
    /// Event raised when validation is complete.
    /// </summary>
    public event EventHandler ValidationComplete
    {
      add
      {
        _validationCompleteHandlers = (EventHandler)
          System.Delegate.Combine(_validationCompleteHandlers, value);
      }
      remove
      {
        _validationCompleteHandlers = (EventHandler)
          System.Delegate.Remove(_validationCompleteHandlers, value);
      }
    }

    /// <summary>
    /// Raises the ValidationComplete event
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnValidationComplete()
    {
      if (_validationCompleteHandlers != null)
        _validationCompleteHandlers(this, EventArgs.Empty);
    }

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

    /// <summary>
    /// Gets the registered rules. Only for unit testing and not visible to code. 
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected BusinessRuleManager GetRegisteredRules()
    {
      return BusinessRules.TypeRules;
    }

    void IHostRules.RuleStart(IPropertyInfo property)
    {
      OnBusyChanged(new BusyChangedEventArgs(property.Name, true));
    }

    void IHostRules.RuleComplete(IPropertyInfo property)
    {
      OnPropertyChanged(property);
      OnBusyChanged(new BusyChangedEventArgs(property.Name, false));
      MetaPropertyHasChanged("IsSelfValid");
      MetaPropertyHasChanged("IsValid");
      MetaPropertyHasChanged("IsSavable");
    }

    void IHostRules.RuleComplete(string property)
    {
      OnPropertyChanged(property);
      MetaPropertyHasChanged("IsSelfValid");
      MetaPropertyHasChanged("IsValid");
      MetaPropertyHasChanged("IsSavable");
    }

    void Rules.IHostRules.AllRulesComplete()
    {
      OnValidationComplete();
      MetaPropertyHasChanged("IsSelfValid");
      MetaPropertyHasChanged("IsValid");
      MetaPropertyHasChanged("IsSavable");
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
      BusinessRules.AddDataAnnotations();
    }

    /// <summary>
    /// Returns true if the object 
    /// and its child objects are currently valid, 
    /// false if the
    /// object or any of its child objects have broken 
    /// rules or are otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling BusinessRules
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior. For instance, you should always override
    /// this method if your object has child objects, since the validity of this object
    /// is affected by the validity of all child objects.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsValid
    {
      get { return IsSelfValid && (_fieldManager == null || FieldManager.IsValid()); }
    }

    /// <summary>
    /// Returns true if the object is currently 
    /// valid, false if the
    /// object has broken rules or is otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling BusinessRules
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior. 
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsSelfValid
    {
      get { return BusinessRules.IsValid; }
    }

    /// <summary>
    /// Provides access to the readonly collection of broken business rules
    /// for this object.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual Rules.BrokenRulesCollection BrokenRulesCollection
    {
      get { return BusinessRules.GetBrokenRules(); }
    }

#endregion

#region Data Access

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    /// <remarks>
    /// Normally you will overload this method to accept a strongly-typed
    /// criteria parameter, rather than overriding the method with a
    /// loosely-typed criteria parameter.
    /// </remarks>
    [RunLocal]
    protected virtual void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }

    /// <summary>
    /// Override this method to allow insertion of a business
    /// object.
    /// </summary>
    protected virtual void DataPortal_Insert()
    { }

    /// <summary>
    /// Override this method to allow update of a business
    /// object.
    /// </summary>
    protected virtual void DataPortal_Update()
    { }

    /// <summary>
    /// Override this method to allow deferred deletion of a business object.
    /// </summary>
    protected virtual void DataPortal_DeleteSelf()
    { }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    /// <remarks>
    /// Normally you will overload this method to accept a strongly-typed
    /// criteria parameter, rather than overriding the method with a
    /// loosely-typed criteria parameter.
    /// </remarks>
    protected virtual void Child_Create()
    {
      BusinessRules.CheckRules();
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

#endregion

#region IDataErrorInfo

    string IDataErrorInfo.Error
    {
      get
      {
        if (!IsSelfValid)
          return BusinessRules.GetBrokenRules().ToString(
            Csla.Rules.RuleSeverity.Error);
        else
          return String.Empty;
      }
    }

    string IDataErrorInfo.this[string columnName]
    {
      get
      {
        string result = string.Empty;
        if (!IsSelfValid)
        {
          Rules.BrokenRule rule =
            BusinessRules.GetBrokenRules().GetFirstBrokenRule(columnName);
          if (rule != null)
            result = rule.Description;
        }
        return result;
      }
    }

#endregion

#region Serialization Notification

    void ISerializationNotification.Deserialized()
    {
      OnDeserializedHandler(new System.Runtime.Serialization.StreamingContext());
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserializedHandler(System.Runtime.Serialization.StreamingContext context)
    {
      BusinessRules.SetTarget(this);
      if (_fieldManager != null)
        FieldManager.SetPropertyList(this.GetType());
      InitializeBusinessRules();
      FieldDataDeserialized();

      OnDeserialized(context);
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    /// <param name="context">Serialization context object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    { }

#endregion

#region Bubbling event Hooks

    /// <summary>
    /// For internal use.
    /// </summary>
    /// <param name="child">Child object.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void AddEventHooks(IBusinessObject child)
    {
      OnAddEventHooks(child);
    }

    /// <summary>
    /// Hook child object events.
    /// </summary>
    /// <param name="child">Child object.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnAddEventHooks(IBusinessObject child)
    {
      INotifyBusy busy = child as INotifyBusy;
      if (busy != null)
        busy.BusyChanged += Child_BusyChanged;

      INotifyUnhandledAsyncException unhandled = child as INotifyUnhandledAsyncException;
      if (unhandled != null)
        unhandled.UnhandledAsyncException += Child_UnhandledAsyncException;

      INotifyPropertyChanged pc = child as INotifyPropertyChanged;
      if (pc != null)
        pc.PropertyChanged += Child_PropertyChanged;

      IBindingList bl = child as IBindingList;
      if (bl != null)
        bl.ListChanged += Child_ListChanged;

      INotifyCollectionChanged ncc = child as INotifyCollectionChanged;
      if (ncc != null)
        ncc.CollectionChanged += Child_CollectionChanged;

      INotifyChildChanged cc = child as INotifyChildChanged;
      if (cc != null)
        cc.ChildChanged += Child_Changed;
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <param name="child">Child object.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void RemoveEventHooks(IBusinessObject child)
    {
      OnRemoveEventHooks(child);
    }

    /// <summary>
    /// Unhook child object events.
    /// </summary>
    /// <param name="child">Child object.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnRemoveEventHooks(IBusinessObject child)
    {
      INotifyBusy busy = child as INotifyBusy;
      if (busy != null)
        busy.BusyChanged -= Child_BusyChanged;

      INotifyUnhandledAsyncException unhandled = child as INotifyUnhandledAsyncException;
      if (unhandled != null)
        unhandled.UnhandledAsyncException -= Child_UnhandledAsyncException;

      INotifyPropertyChanged pc = child as INotifyPropertyChanged;
      if (pc != null)
        pc.PropertyChanged -= Child_PropertyChanged;

      IBindingList bl = child as IBindingList;
      if (bl != null)
        bl.ListChanged -= Child_ListChanged;

      INotifyCollectionChanged ncc = child as INotifyCollectionChanged;
      if (ncc != null)
        ncc.CollectionChanged -= Child_CollectionChanged;

      INotifyChildChanged cc = child as INotifyChildChanged;
      if (cc != null)
        cc.ChildChanged -= Child_Changed;
    }

#endregion

#region Busy / Unhandled exception bubbling

    private void Child_UnhandledAsyncException(object sender, ErrorEventArgs e)
    {
      OnUnhandledAsyncException(e);
    }

    private void Child_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
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
#region Check to see if the property is marked with RelationshipTypes.PrivateField

      var propertyInfo = FieldManager.GetRegisteredProperty(propertyName);

      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) != RelationshipTypes.PrivateField)
        throw new InvalidOperationException(Resources.PrivateFieldException);

#endregion

      if (_bypassPropertyChecks || CanReadProperty(propertyInfo, noAccess == Security.NoAccessBehavior.ThrowException))
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
      if (_bypassPropertyChecks || CanReadProperty(propertyInfo, noAccess == Csla.Security.NoAccessBehavior.ThrowException))
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
      if (_bypassPropertyChecks || CanReadProperty(propertyInfo, false))
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
    protected P GetProperty<P>(IPropertyInfo propertyInfo)
    {
      return (P)GetProperty(propertyInfo);
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

    [NotUndoable]
    [NonSerialized]
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

    object IManageProperties.LazyGetProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator)
    {
      return LazyGetProperty(propertyInfo, valueGenerator);
    }

    object IManageProperties.LazyGetPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory)
    {
      return LazyGetPropertyAsync(propertyInfo, factory);
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
      if (((propertyInfo.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad) && !FieldManager.FieldExists(propertyInfo))
        throw new InvalidOperationException(Resources.PropertyGetNotAllowed);

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
    /// Gets a property's value.
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    protected virtual object ReadProperty(IPropertyInfo propertyInfo)
    {
      if (((propertyInfo.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad) && !FieldManager.FieldExists(propertyInfo))
        throw new InvalidOperationException(Resources.PropertyGetNotAllowed);

      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
      {
        using (BypassPropertyChecks)
        {
          return MethodCaller.CallPropertyGetter(this, propertyInfo.Name);
        }
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
      SetPropertyConvert<P, V>(propertyInfo.Name, ref field, newValue, noAccess);
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
#region Check to see if the property is marked with RelationshipTypes.PrivateField

        var propertyInfo = FieldManager.GetRegisteredProperty(propertyName);

        if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) != RelationshipTypes.PrivateField)
          throw new InvalidOperationException(Resources.PrivateFieldException);

#endregion

        if (_bypassPropertyChecks || CanWriteProperty(propertyInfo, noAccess == Security.NoAccessBehavior.ThrowException))
        {
          bool doChange = false;
          if (field == null)
          {
            if (newValue != null)
              doChange = true;
          }
          else
          {
            if (typeof(P) == typeof(string) && newValue == null)
              newValue = Utilities.CoerceValue<P>(typeof(string), field, string.Empty);
            if (!field.Equals(newValue))
              doChange = true;
          }
          if (doChange)
          {
            if (!_bypassPropertyChecks) OnPropertyChanging(propertyName);
            field = newValue;
            if (!_bypassPropertyChecks) PropertyHasChanged(propertyName);
          }
        }
      }
      catch (System.Security.SecurityException ex)
      {
        throw new Csla.Security.SecurityException(ex.Message);
      }
      catch (Csla.Security.SecurityException)
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
    protected void SetPropertyConvert<P, V>(string propertyName, ref P field, V newValue, Security.NoAccessBehavior noAccess)
    {
      try
      {
#region Check to see if the property is marked with RelationshipTypes.PrivateField

        var propertyInfo = FieldManager.GetRegisteredProperty(propertyName);

        if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) != RelationshipTypes.PrivateField)
          throw new InvalidOperationException(Resources.PrivateFieldException);

#endregion

        if (_bypassPropertyChecks || CanWriteProperty(propertyInfo, noAccess == Security.NoAccessBehavior.ThrowException))
        {
          bool doChange = false;
          if (field == null)
          {
            if (newValue != null)
              doChange = true;
          }
          else
          {
            if (typeof(V) == typeof(string) && newValue == null)
              newValue = Utilities.CoerceValue<V>(typeof(string), null, string.Empty);
            if (!field.Equals(newValue))
              doChange = true;
          }
          if (doChange)
          {
            if (!_bypassPropertyChecks) OnPropertyChanging(propertyName);
            field = Utilities.CoerceValue<P>(typeof(V), field, newValue);
            if (!_bypassPropertyChecks) PropertyHasChanged(propertyName);
          }
        }
      }
      catch (System.Security.SecurityException ex)
      {
        throw new Csla.Security.SecurityException(ex.Message);
      }
      catch (Csla.Security.SecurityException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(
          string.Format(Properties.Resources.PropertyLoadException, propertyName, ex.Message), ex);
      }
    }

    /// <summary>
    /// Sets a property's managed field with the 
    /// supplied value, first checking authorization, and then
    /// calling PropertyHasChanged if the value does change.
    /// </summary>
    /// <typeparam name="P">Property type.</typeparam>
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
      try
      {
        if (_bypassPropertyChecks || CanWriteProperty(propertyInfo, noAccess == Security.NoAccessBehavior.ThrowException))
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
      }
      catch (System.Security.SecurityException ex)
      {
        throw new Csla.Security.SecurityException(ex.Message);
      }
      catch (Csla.Security.SecurityException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(
          string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message), ex);
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
      if (_bypassPropertyChecks || CanWriteProperty(propertyInfo, noAccess == Security.NoAccessBehavior.ThrowException))
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
          if (typeof(P) == typeof(string) && newValue == null)
            newValue = Utilities.CoerceValue<P>(typeof(string), null, string.Empty);
          LoadPropertyValue<P>(propertyInfo, oldValue, newValue, !_bypassPropertyChecks);
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
    /// supplied value, and then
    /// calls PropertyHasChanged if the value does change.
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
        if (_bypassPropertyChecks || CanWriteProperty(propertyInfo, true))
        {
          if (!_bypassPropertyChecks) OnPropertyChanging(propertyInfo);
          FieldManager.SetFieldData(propertyInfo, newValue);
          if (!_bypassPropertyChecks) PropertyHasChanged(propertyInfo);
        }
      }
      catch (System.Security.SecurityException ex)
      {
        throw new Csla.Security.SecurityException(ex.Message);
      }
      catch (Csla.Security.SecurityException)
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
    /// supplied value, and then
    /// calls PropertyHasChanged if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// If the user is not authorized to change the 
    /// property a SecurityException is thrown.
    /// </remarks>
    protected void SetProperty<P>(IPropertyInfo propertyInfo, P newValue)
    {
      SetProperty(propertyInfo, (object)newValue);
    }

#endregion

#region  Load Properties

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value.
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
    /// supplied value.
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

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value and mark field as dirty if value is modified.
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
    protected bool LoadPropertyMarkDirty<P>(PropertyInfo<P> propertyInfo, P newValue)
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

        var valuesDiffer = ValuesDiffer(propertyInfo, newValue, oldValue);
        if (valuesDiffer)
        {

          IBusinessObject old = oldValue as IBusinessObject;
          if (old != null)
            RemoveEventHooks(old);
          IBusinessObject @new = newValue as IBusinessObject;
          if (@new != null)
            AddEventHooks(@new);

          if (typeof(IEditableBusinessObject).IsAssignableFrom(propertyInfo.Type))
          {
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
            ResetChildEditLevel(newValue);
          }
          else if (typeof(IEditableCollection).IsAssignableFrom(propertyInfo.Type))
          {
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
            ResetChildEditLevel(newValue);
          }
          else
          {
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
          }
        }
        return valuesDiffer;
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(string.Format(Properties.Resources.PropertyLoadException, propertyInfo.Name, ex.Message), ex);
      }
    }

    /// <summary>
    /// Check if old and new values are different.
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <param name="propertyInfo">The property info.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="oldValue">The old value.</param>
    /// <returns></returns>
    private static bool ValuesDiffer<P>(PropertyInfo<P> propertyInfo, P newValue, P oldValue)
    {
      var valuesDiffer = false;
      if (oldValue == null)
        valuesDiffer = newValue != null;
      else
      {
        // use reference equals for objects that inherit from CSLA base class
        if (typeof (IBusinessObject).IsAssignableFrom(propertyInfo.Type))
        {
          valuesDiffer = !(ReferenceEquals(oldValue, newValue));
        }
        else
        {
          valuesDiffer = !(oldValue.Equals(newValue));
        }
      }
      return valuesDiffer;
    }

    private void LoadPropertyValue<P>(PropertyInfo<P> propertyInfo, P oldValue, P newValue, bool markDirty)
    {
      var valuesDiffer = ValuesDiffer(propertyInfo, newValue, oldValue);

      if (valuesDiffer)
      {

        IBusinessObject old = oldValue as IBusinessObject;
        if (old != null)
          RemoveEventHooks(old);
        IBusinessObject @new = newValue as IBusinessObject;
        if (@new != null)
          AddEventHooks(@new);


        if (typeof(IEditableBusinessObject).IsAssignableFrom(propertyInfo.Type))
        {
          if (markDirty)
          {
            OnPropertyChanging(propertyInfo);
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
            PropertyHasChanged(propertyInfo);
          }
          else
          {
            FieldManager.LoadFieldData<P>(propertyInfo, newValue);
          }
          ResetChildEditLevel(newValue);
        }
        else if (typeof(IEditableCollection).IsAssignableFrom(propertyInfo.Type))
        {
          if (markDirty)
          {
            OnPropertyChanging(propertyInfo);
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
            PropertyHasChanged(propertyInfo);
          }
          else
          {
            FieldManager.LoadFieldData<P>(propertyInfo, newValue);
          }
          ResetChildEditLevel(newValue);
        }
        else
        {
          if (markDirty)
          {
            OnPropertyChanging(propertyInfo);
            FieldManager.SetFieldData<P>(propertyInfo, newValue);
            PropertyHasChanged(propertyInfo);
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
    /// supplied value.
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
    protected virtual bool LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object newValue)
    {
      // private field 
      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
      {
        LoadProperty(propertyInfo, newValue);
        return false;
      }

#if IOS
      //manually call LoadProperty<T> if the type is nullable otherwise JIT error will occur
      if (propertyInfo.Type == typeof(int?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<int?>)propertyInfo, (int?)newValue);
      }
      else if (propertyInfo.Type == typeof(bool?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<bool?>)propertyInfo, (bool?)newValue);
      }
      else if (propertyInfo.Type == typeof(DateTime?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<DateTime?>)propertyInfo, (DateTime?)newValue);
      }
      else if (propertyInfo.Type == typeof(decimal?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<decimal?>)propertyInfo, (decimal?)newValue);
      }
      else if (propertyInfo.Type == typeof(double?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<double?>)propertyInfo, (double?)newValue);
      }
      else if (propertyInfo.Type == typeof(long?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<long?>)propertyInfo, (long?)newValue);
      }
      else if (propertyInfo.Type == typeof(byte?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<byte?>)propertyInfo, (byte?)newValue);
      }
      else if (propertyInfo.Type == typeof(char?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<char?>)propertyInfo, (char?)newValue);
      }
      else if (propertyInfo.Type == typeof(short?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<short?>)propertyInfo, (short?)newValue);
      }
      else if (propertyInfo.Type == typeof(uint?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<uint?>)propertyInfo, (uint?)newValue);
      }
      else if (propertyInfo.Type == typeof(ulong?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<ulong?>)propertyInfo, (ulong?)newValue);
      }
      else if (propertyInfo.Type == typeof(ushort?))
      {
        return LoadPropertyMarkDirty((PropertyInfo<ushort?>)propertyInfo, (ushort?)newValue);
      }
      else
      {
        return (bool)LoadPropertyByReflection("LoadPropertyMarkDirty", propertyInfo, newValue);
      }
#else
      return (bool)LoadPropertyByReflection("LoadPropertyMarkDirty", propertyInfo, newValue);
#endif
    }


    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value.
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
#if IOS
      //manually call LoadProperty<T> if the type is nullable otherwise JIT error will occur
      if (propertyInfo.Type == typeof(int?))
      {
        LoadProperty((PropertyInfo<int?>)propertyInfo, (int?)newValue);
      }
      else if (propertyInfo.Type == typeof(bool?))
      {
        LoadProperty((PropertyInfo<bool?>)propertyInfo, (bool?)newValue);
      }
      else if (propertyInfo.Type == typeof(DateTime?))
      {
        LoadProperty((PropertyInfo<DateTime?>)propertyInfo, (DateTime?)newValue);
      }
      else if (propertyInfo.Type == typeof(decimal?))
      {
        LoadProperty((PropertyInfo<decimal?>)propertyInfo, (decimal?)newValue);
      }
      else if (propertyInfo.Type == typeof(double?))
      {
        LoadProperty((PropertyInfo<double?>)propertyInfo, (double?)newValue);
      }
      else if (propertyInfo.Type == typeof(long?))
      {
        LoadProperty((PropertyInfo<long?>)propertyInfo, (long?)newValue);
      }
      else if (propertyInfo.Type == typeof(byte?))
      {
        LoadProperty((PropertyInfo<byte?>)propertyInfo, (byte?)newValue);
      }
      else if (propertyInfo.Type == typeof(char?))
      {
        LoadProperty((PropertyInfo<char?>)propertyInfo, (char?)newValue);
      }
      else if (propertyInfo.Type == typeof(short?))
      {
        LoadProperty((PropertyInfo<short?>)propertyInfo, (short?)newValue);
      }
      else if (propertyInfo.Type == typeof(uint?))
      {
        LoadProperty((PropertyInfo<uint?>)propertyInfo, (uint?)newValue);
      }
      else if (propertyInfo.Type == typeof(ulong?))
      {
        LoadProperty((PropertyInfo<ulong?>)propertyInfo, (ulong?)newValue);
      }
      else if (propertyInfo.Type == typeof(ushort?))
      {
        LoadProperty((PropertyInfo<ushort?>)propertyInfo, (ushort?)newValue);
      }
      else
      {
        LoadPropertyByReflection("LoadProperty", propertyInfo, newValue);
      }
#else
      LoadPropertyByReflection("LoadProperty", propertyInfo, newValue);
#endif
    }

    /// <summary>
    /// Calls the generic LoadProperty method via reflection.
    /// </summary>
    /// <param name="loadPropertyMethodName">
    /// The LoadProperty method name to call via reflection.</param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <returns></returns>
    private object LoadPropertyByReflection(string loadPropertyMethodName, IPropertyInfo propertyInfo, object newValue)
    {
      var t = this.GetType();
      var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      var method = t.GetMethods(flags).FirstOrDefault(c => c.Name == loadPropertyMethodName && c.IsGenericMethod);
      var gm = method.MakeGenericMethod(propertyInfo.Type);
      var p = new object[] { propertyInfo, newValue };
      return gm.Invoke(this, p);
    }

    /// <summary>
    /// Makes sure that a child object is set up properly
    /// to be a child of this object.
    /// </summary>
    /// <param name="newValue">Potential child object</param>
    private void ResetChildEditLevel(object newValue)
    {
      IEditableBusinessObject child = newValue as IEditableBusinessObject;
      if (child != null)
      {
        child.SetParent(this);
        // set child edit level
        UndoableBase.ResetChildEditLevel(child, this.EditLevel, this.BindingEdit);
        // reset EditLevelAdded 
        child.EditLevelAdded = this.EditLevel;
      }
      else
      {
        IEditableCollection col = newValue as IEditableCollection;
        if (col != null)
        {
          col.SetParent(this);
          IUndoableObject undo = col as IUndoableObject;
          if (undo != null)
          {
            // set child edit level
            UndoableBase.ResetChildEditLevel(undo, this.EditLevel, this.BindingEdit);
          }
        }
      }
    }

    //private AsyncLoadManager
    [NonSerialized]
    [NotUndoable]
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

    private void loadManager_UnhandledAsyncException(object sender, ErrorEventArgs e)
    {
      OnUnhandledAsyncException(e);
    }

    private void loadManager_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

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

#region IsBusy / IsIdle

    [NonSerialized]
    [NotUndoable]
    private bool _isBusy;

    /// <summary>
    /// Mark the object as busy (it is
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
    /// Mark the object as not busy (it is
    /// not running an async operation).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkIdle()
    {
      _isBusy = false;
      OnBusyChanged(new BusyChangedEventArgs("", false));
    }

    /// <summary>
    /// Gets a value indicating if this
    /// object or its child objects are
    /// busy.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsBusy
    {
      get { return IsSelfBusy || (_fieldManager != null && FieldManager.IsBusy()); }
    }

    /// <summary>
    /// Gets a value indicating if this
    /// object is busy.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsSelfBusy
    {
      get { return _isBusy || BusinessRules.RunningAsyncRules || LoadManager.IsLoading; }
    }

    [NotUndoable]
    [NonSerialized]
    private BusyChangedEventHandler _busyChanged;

    /// <summary>
    /// Event indicating that the IsBusy property has changed.
    /// </summary>
    public event BusyChangedEventHandler BusyChanged
    {
      add { _busyChanged = (BusyChangedEventHandler)Delegate.Combine(_busyChanged, value); }
      remove { _busyChanged = (BusyChangedEventHandler)Delegate.Remove(_busyChanged, value); }
    }

    /// <summary>
    /// Raise the BusyChanged event.
    /// </summary>
    /// <param name="args">Event args.</param>
    protected virtual void OnBusyChanged(BusyChangedEventArgs args)
    {
      if (_busyChanged != null)
        _busyChanged(this, args);
      MetaPropertyHasChanged("IsSelfBusy");
      MetaPropertyHasChanged("IsBusy");
    }

    /// <summary>
    /// Gets a value indicating whether a
    /// specific property is busy (has a
    /// currently executing async rule).
    /// </summary>
    /// <param name="property">
    /// Property to check.
    /// </param>
    public virtual bool IsPropertyBusy(Csla.Core.IPropertyInfo property)
    {
      return BusinessRules.GetPropertyBusy(property);
    }

    /// <summary>
    /// Gets a value indicating whether a
    /// specific property is busy (has a
    /// currently executing async rule).
    /// </summary>
    /// <param name="propertyName">
    /// Name of the property.
    /// </param>
    public bool IsPropertyBusy(string propertyName)
    {
      return IsPropertyBusy(FieldManager.GetRegisteredProperty(propertyName));
    }

#endregion

#region INotifyUnhandledAsyncException Members

    [NotUndoable]
    [NonSerialized]
    private EventHandler<ErrorEventArgs> _unhandledAsyncException;

    /// <summary>
    /// Event indicating that an exception occurred during
    /// the processing of an async operation.
    /// </summary>
    public event EventHandler<ErrorEventArgs> UnhandledAsyncException
    {
      add { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
      remove { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Remove(_unhandledAsyncException, value); }
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="error">Args parameter.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
      if (_unhandledAsyncException != null)
        _unhandledAsyncException(this, error);
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="originalSender">Original sender of
    /// the event.</param>
    /// <param name="error">Exception object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncException(new ErrorEventArgs(originalSender, error));
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
    /// <param name="e">
    /// ChildChangedEventArgs object.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnChildChanged(ChildChangedEventArgs e)
    {
      if (_childChangedHandlers != null)
        _childChangedHandlers.Invoke(this, e);
      MetaPropertyHasChanged("IsDirty");
      MetaPropertyHasChanged("IsValid");
      MetaPropertyHasChanged("IsSavable");
    }

    /// <summary>
    /// Creates a ChildChangedEventArgs and raises the event.
    /// </summary>
    private void RaiseChildChanged(ChildChangedEventArgs e)
    {
      OnChildChanged(e);
    }

    /// <summary>
    /// Creates a ChildChangedEventArgs and raises the event.
    /// </summary>
    private void RaiseChildChanged(
      object childObject, PropertyChangedEventArgs propertyArgs)
    {
      ChildChangedEventArgs args = new ChildChangedEventArgs(childObject, propertyArgs);
      OnChildChanged(args);
    }

    /// <summary>
    /// Creates a ChildChangedEventArgs and raises the event.
    /// </summary>
    private void RaiseChildChanged(
      object childObject, PropertyChangedEventArgs propertyArgs, ListChangedEventArgs listArgs)
    {
      ChildChangedEventArgs args = new ChildChangedEventArgs(childObject, propertyArgs, listArgs);
      OnChildChanged(args);
    }

    /// <summary>
    /// Creates a ChildChangedEventArgs and raises the event.
    /// </summary>
    private void RaiseChildChanged(
      object childObject, PropertyChangedEventArgs propertyArgs, NotifyCollectionChangedEventArgs listArgs)
    {
      ChildChangedEventArgs args = new ChildChangedEventArgs(childObject, propertyArgs, listArgs);
      OnChildChanged(args);
    }

    /// <summary>
    /// Handles any PropertyChanged event from 
    /// a child object and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      // Issue 813
      // MetaPropertyHasChanged calls in OnChildChanged we're leading to exponential growth in OnChildChanged calls
      // Those notifications are for the UI. Ignore them here
      if (!(e is MetaPropertyChangedEventArgs))
      {
        RaiseChildChanged(sender, e);
      }
    }

    /// <summary>
    /// Handles any ListChanged event from 
    /// a child list and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    private void Child_ListChanged(object sender, ListChangedEventArgs e)
    {
      if (e.ListChangedType != ListChangedType.ItemChanged)
        RaiseChildChanged(sender, null, e);
    }

    /// <summary>
    /// Handles any CollectionChanged event
    /// from a child list and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    private void Child_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      RaiseChildChanged(sender, null, e);
    }

    /// <summary>
    /// Handles any ChildChanged event from
    /// a child object and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    private void Child_Changed(object sender, ChildChangedEventArgs e)
    {
      RaiseChildChanged(e);
    }

#endregion

#region  Field Manager

    private FieldManager.FieldDataManager _fieldManager;

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
          UndoableBase.ResetChildEditLevel(_fieldManager, this.EditLevel, this.BindingEdit);
        }
        return _fieldManager;
      }
    }

    private void FieldDataDeserialized()
    {
      foreach (object item in FieldManager.GetChildren())
      {
        if (item is IBusinessObject business)
          OnAddEventHooks(business);

        if (item is IEditableBusinessObject child)
        {
          child.SetParent(this);
        }
        if (item is IEditableCollection childCollection)
        {
          childCollection.SetParent(this);
        }
      }
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

    IParent Csla.Core.IParent.Parent
    {
      get { return this.Parent; }
    }

#endregion

#region IDataPortalTarget Members

    void Csla.Server.IDataPortalTarget.CheckRules()
    {
      BusinessRules.CheckRules();
    }

    void Csla.Server.IDataPortalTarget.MarkAsChild()
    {
      this.MarkAsChild();
    }

    void Csla.Server.IDataPortalTarget.MarkNew()
    {
      this.MarkNew();
    }

    void Csla.Server.IDataPortalTarget.MarkOld()
    {
      this.MarkOld();
    }

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
      SetProperty(propertyInfo, newValue);
    }

    void IManageProperties.LoadProperty(IPropertyInfo propertyInfo, object newValue)
    {
      LoadProperty(propertyInfo, newValue);
    }

    bool IManageProperties.LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object newValue)
    {
      return LoadPropertyMarkDirty(propertyInfo, newValue);
    }

    List<object> IManageProperties.GetChildren()
    {
      return FieldManager.GetChildren();
    }
#endregion

#region MobileFormatter

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
    {
      base.OnGetState(info, mode);
      info.AddValue("Csla.Core.BusinessBase._isNew", _isNew);
      info.AddValue("Csla.Core.BusinessBase._isDeleted", _isDeleted);
      info.AddValue("Csla.Core.BusinessBase._isDirty", _isDirty);
      info.AddValue("Csla.Core.BusinessBase._neverCommitted", _neverCommitted);
      info.AddValue("Csla.Core.BusinessBase._disableIEditableObject", _disableIEditableObject);
      info.AddValue("Csla.Core.BusinessBase._isChild", _isChild);
      info.AddValue("Csla.Core.BusinessBase._editLevelAdded", _editLevelAdded);
      info.AddValue("Csla.Core.BusinessBase._identity", _identity);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
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
      _identity = info.GetValue<int>("Csla.Core.BusinessBase._identity");
    }

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
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnGetChildren(
      Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      base.OnGetChildren(info, formatter);

      if (_fieldManager != null)
      {
        var fieldManagerInfo = formatter.SerializeObject(_fieldManager);
        info.AddChild("_fieldManager", fieldManagerInfo.ReferenceId);
      }

      if (_businessRules != null)
      {
        var vrInfo = formatter.SerializeObject(_businessRules);
        info.AddChild("_businessRules", vrInfo.ReferenceId);
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
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnSetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      if (info.Children.ContainsKey("_fieldManager"))
      {
        var childData = info.Children["_fieldManager"];
        _fieldManager = (FieldManager.FieldDataManager)formatter.GetObject(childData.ReferenceId);
      }

      if (info.Children.ContainsKey("_businessRules"))
      {
        int refId = info.Children["_businessRules"].ReferenceId;
        _businessRules = (BusinessRules)formatter.GetObject(refId);
      }

      base.OnSetChildren(info, formatter);
    }

#endregion

#region Property Checks ByPass

    [NonSerialized]
    [NotUndoable]
    private bool _bypassPropertyChecks = false;

    /// <summary>
    /// Gets a value whether the business object is currently bypassing property checks?
    /// </summary>
    protected internal bool IsBypassingPropertyChecks { get { return _bypassPropertyChecks; } }
  
    [NonSerialized]
    [NotUndoable]
    private BypassPropertyChecksObject _bypassPropertyChecksObject = null;

    /// <summary>
    /// By wrapping this property inside Using block
    /// you can set property values on current business object
    /// without raising PropertyChanged events
    /// and checking user rights.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected internal BypassPropertyChecksObject BypassPropertyChecks
    {
      get
      {
        return BypassPropertyChecksObject.GetManager(this);
      }
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
      private static object _lock = new object();

      internal BypassPropertyChecksObject(BusinessBase businessObject)
      {
        _businessObject = businessObject;
        _businessObject._bypassPropertyChecks = true;
      }

#region IDisposable Members

      /// <summary>
      /// Disposes the object.
      /// </summary>
      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Disposes the object.
      /// </summary>
      /// <param name="dispose">Dispose flag.</param>
      protected virtual void Dispose(bool dispose)
      {
        DeRef();
      }

      /// <summary>
      /// Gets the BypassPropertyChecks object.
      /// </summary>
      /// <param name="businessObject">The business object.</param>
      /// <returns></returns>
      public static BypassPropertyChecksObject GetManager(BusinessBase businessObject)
      {
        lock (_lock)
        {
          if (businessObject._bypassPropertyChecksObject == null)
            businessObject._bypassPropertyChecksObject = new BypassPropertyChecksObject(businessObject);

          businessObject._bypassPropertyChecksObject.AddRef();
        }
        return businessObject._bypassPropertyChecksObject;
      }

#region  Reference counting

      private int _refCount;

      /// <summary>
      /// Gets the current reference count for this
      /// object.
      /// </summary>
      public int RefCount
      {
        get { return _refCount; }
      }

      private void AddRef()
      {
        _refCount += 1;
      }

      private void DeRef()
      {

        lock (_lock)
        {
          _refCount -= 1;
          if (_refCount == 0)
          {
            _businessObject._bypassPropertyChecks = false;
            _businessObject._bypassPropertyChecksObject = null;
            _businessObject = null;
          }
        }
      }

#endregion
#endregion
    }

#endregion

#region ISuppressRuleChecking Members

    /// <summary>
    /// Sets value indicating no rule methods will be invoked.
    /// </summary>
    void ICheckRules.SuppressRuleChecking()
    {
      BusinessRules.SuppressRuleChecking = true;
    }

    /// <summary>
    /// Resets value indicating all rule methods will be invoked.
    /// </summary>
    void ICheckRules.ResumeRuleChecking()
    {
      BusinessRules.SuppressRuleChecking = false;
    }

    /// <summary>
    /// Invokes all rules for the business object.
    /// </summary>
    void ICheckRules.CheckRules()
    {
      BusinessRules.CheckRules();
    }

    /// <summary>
    /// Invokes all rules for the business object.
    /// </summary>
    Task ICheckRules.CheckRulesAsync()
    {
      return BusinessRules.CheckRulesAsync();
    }

    /// <summary>
    /// Gets the broken rules for this object
    /// </summary>
    public BrokenRulesCollection GetBrokenRules()
    {
      return BrokenRulesCollection;
    }

#endregion
  }
}