//-----------------------------------------------------------------------
// <copyright file="BusinessListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which most business collections</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
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

namespace Csla
{
  /// <summary>
  /// This is the base class from which most business collections
  /// or lists will be derived.
  /// </summary>
  /// <typeparam name="T">Type of the business object being defined.</typeparam>
  /// <typeparam name="C">Type of the child objects contained in the list.</typeparam>
#if TESTING
  [DebuggerStepThrough]
#endif
  [Serializable]
  public abstract class BusinessListBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] C> :
      ObservableBindingList<C>,
      IContainsDeletedList,
      ISavable<T>,
      IDataPortalTarget,
      IBusinessListBase<C>,
      IUseApplicationContext,
      INotifyPropertyChanged,
      IManageProperties,
      IHostRules,
      ICheckRules,
      IUseFieldManager,
      IUseBusinessRules
    where T : BusinessListBase<T, C>
    where C : IEditableBusinessObject
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    protected BusinessListBase()
    {
      ApplicationContext = default!;
    }

    /// <summary>
    /// Gets the current ApplicationContext
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }

    /// <inheritdoc />
    ApplicationContext IUseApplicationContext.ApplicationContext
    {
      get => ApplicationContext;
      set
      {
        ApplicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext));
        InitializeIdentity();
        Initialize();
        InitializeBusinessRules();
        AllowNew = true;
      }
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

    int IBusinessObject.Identity => _identity;

    private void InitializeIdentity()
    {
      _identity = ((IParent)this).GetNextIdentity(_identity);
    }

    [NonSerialized]
    [NotUndoable]
    private IdentityManager? _identityManager;

    int IParent.GetNextIdentity(int current)
    {
      if (Parent != null)
      {
        return Parent.GetNextIdentity(current);
      }
      else
      {
        if (_identityManager == null)
          _identityManager = new IdentityManager();
        return _identityManager.GetNextIdentity(current);
      }
    }

    #endregion

    #region BusinessRules, IsValid

    [NonSerialized]
    [NotUndoable]
    private EventHandler? _validationCompleteHandlers;

    /// <summary>
    /// Event raised when validation is complete.
    /// </summary>
    public event EventHandler? ValidationComplete
    {
      add => _validationCompleteHandlers = (EventHandler?)Delegate.Combine(_validationCompleteHandlers, value);
      remove => _validationCompleteHandlers = (EventHandler?)Delegate.Remove(_validationCompleteHandlers, value);
    }

    /// <summary>
    /// Raises the ValidationComplete event
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnValidationComplete()
    {
      _validationCompleteHandlers?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeBusinessRules()
    {
      var rules = BusinessRuleManager.GetRulesForType(GetType());
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
              BusinessRuleManager.CleanupRulesForType(GetType());
              throw;  // and rethrow exception
            }
          }
    }

    private BusinessRules? _businessRules;

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
          _businessRules = new BusinessRules(ApplicationContext, this);
        else if (_businessRules.Target == null)
          _businessRules.SetTarget(this);
        return _businessRules;
      }
    }

    BusinessRules IUseBusinessRules.BusinessRules => BusinessRules;

    /// <summary>
    /// Gets the registered rules. Only for unit testing and not visible to code. 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected BusinessRuleManager GetRegisteredRules()
    {
      return BusinessRules.TypeRules;
    }

    /// <inheritdoc />
    void IHostRules.RuleStart(IPropertyInfo property)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      OnBusyChanged(new BusyChangedEventArgs(property.Name, true));
    }

    /// <inheritdoc />
    void IHostRules.RuleComplete(IPropertyInfo property)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      OnPropertyChanged(property);
      OnBusyChanged(new BusyChangedEventArgs(property.Name, false));
      MetaPropertyHasChanged("IsSelfValid");
      MetaPropertyHasChanged("IsValid");
      MetaPropertyHasChanged("IsSavable");
    }

    /// <inheritdoc />
    void IHostRules.RuleComplete(string property)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      OnPropertyChanged(property);
      MetaPropertyHasChanged("IsSelfValid");
      MetaPropertyHasChanged("IsValid");
      MetaPropertyHasChanged("IsSavable");
    }

    /// <inheritdoc />
    void IHostRules.AllRulesComplete()
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
    /// Raises OnPropertyChanged for meta properties (IsXYZ) when PropertyChangedMode is not Windows
    /// </summary>
    /// <param name="name">meta property name that has cchanged.</param>
    protected virtual void MetaPropertyHasChanged(string name)
    {
      if (ApplicationContext.PropertyChangedMode != ApplicationContext.PropertyChangedModes.Windows)
        OnMetaPropertyChanged(name);
    }

    #endregion

    #region INotifyPropertyChanged

    [NonSerialized]
    [NotUndoable]
    private PropertyChangedEventHandler? _nonSerializableChangedHandlers;
    [NotUndoable]
    private PropertyChangedEventHandler? _serializableChangedHandlers;

    /// <summary>
    /// Event raised when a property value has changed.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public new event PropertyChangedEventHandler? PropertyChanged
    {
      add
      {
        if (value is null)
          return;
        if (value.Method.IsPublic)
          _serializableChangedHandlers = (PropertyChangedEventHandler?)Delegate.Combine(_serializableChangedHandlers, value);
        else
          _nonSerializableChangedHandlers = (PropertyChangedEventHandler?)Delegate.Combine(_nonSerializableChangedHandlers, value);
      }
      remove
      {
        if (value is null)
          return;
        if (value.Method.IsPublic)
          _serializableChangedHandlers = (PropertyChangedEventHandler?)Delegate.Remove(_serializableChangedHandlers, value);
        else
          _nonSerializableChangedHandlers = (PropertyChangedEventHandler?)Delegate.Remove(_nonSerializableChangedHandlers, value);
      }
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for a specific property.
    /// </summary>
    /// <param name="propertyName">Name of the property that
    /// has changed.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate