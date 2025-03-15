//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is a base class from which readonly business classes</summary>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
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
  /// This is a base class from which readonly business classes
  /// can be derived.
  /// </summary>
  /// <remarks>
  /// This base class only supports data retrieve, not updating or
  /// deleting. Any business classes derived from this base class
  /// should only implement readonly properties.
  /// </remarks>
  /// <typeparam name="T">Type of the business object.</typeparam>
  [Serializable]
  public abstract class ReadOnlyBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T> : BindableBase,
    IDataPortalTarget,
    IManageProperties,
    IReadOnlyBase,
    IUseApplicationContext,
    IUseFieldManager,
    IUseBusinessRules
    where T : ReadOnlyBase<T>
  {
    #region Object ID Value

    /// <summary>
    /// Override this method to return a unique identifying
    /// value for this object.
    /// </summary>
    protected virtual object? GetIdValue()
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
      object? id = GetIdValue();
      if (id == null)
        return base.ToString()!;
      else
        return id.ToString()!;
    }

    #endregion

    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    protected ApplicationContext ApplicationContext { get; set; }

    /// <inheritdoc />
    ApplicationContext IUseApplicationContext.ApplicationContext
    {
      get => ApplicationContext;
      set
      {
        ApplicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext));
        Initialize();
        InitializeBusinessRules();
      }
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable. Necessary for derived classes
    protected ReadOnlyBase()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
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

    int IBusinessObject.Identity => 0;

    #endregion

    #region Authorization

    /// <summary>
    /// Gets or sets if <see cref="CanReadProperty(IPropertyInfo)"/> performs an authorization check or not. <br/>
    /// <see langword="true"/> means read is <i>always</i> allowed. <see langword="false"/> means an authorization check is always performed.
    /// </summary>
    protected virtual bool IsCanReadPropertyAuthorizationCheckDisabled { get; } = false;

    [NotUndoable]
    [NonSerialized]
    private ConcurrentDictionary<string, bool>? _readResultCache;
    [NotUndoable]
    [NonSerialized]
    private ConcurrentDictionary<string, bool>? _executeResultCache;
    [NotUndoable]
    [NonSerialized]
    private System.Security.Principal.IPrincipal _lastPrincipal;

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
          _businessRules = ApplicationContext.CreateInstanceDI<BusinessRules>(ApplicationContext, this);
        else if (_businessRules.Target == null)
          _businessRules.SetTarget(this);
        return _businessRules;
      }
    }

    BusinessRules IUseBusinessRules.BusinessRules => BusinessRules;

    void IHostRules.RuleStart(IPropertyInfo property)
    { }

    void IHostRules.RuleComplete(IPropertyInfo property)
    { }

    void IHostRules.RuleComplete(string property)
    { }

    void IHostRules.AllRulesComplete()
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
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanReadProperty(IPropertyInfo property)
    {
      if (IsCanReadPropertyAuthorizationCheckDisabled)
      {
        return true;
      }

      if (property is null)
        throw new ArgumentNullException(nameof(property));

      VerifyAuthorizationCache();

      if (!_readResultCache!.TryGetValue(property.Name, out var result))
      {
        result = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.ReadProperty, property);
        // store value in cache
        _readResultCache.AddOrUpdate(property.Name, result, (_, _) => { return result; });
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
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool CanReadProperty(IPropertyInfo property, bool throwOnFalse)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      bool result = CanReadProperty(property);
      if (throwOnFalse && result == false)
      {
        throw new SecurityException($"{Resources.PropertyGetNotAllowed} ({property.Name})");
      }
      return result;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool CanReadProperty(string propertyName)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

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
#if !(ANDROID || IOS)
        Trace.TraceError("CanReadProperty: {0} is not a registered property of {1}.{2}", propertyName, GetType().Namespace, GetType().Name);
#endif
        return true;
      }
      return CanReadProperty(propertyInfo, throwOnFalse);
    }

    bool IAuthorizeReadWrite.CanWriteProperty(string propertyName)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return false;
    }

    bool IAuthorizeReadWrite.CanWriteProperty(IPropertyInfo property)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      return false;
    }

    [MemberNotNull(nameof(_readResultCache), nameof(_executeResultCache))]
    private void VerifyAuthorizationCache()
    {
      if (_readResultCache == null)
        _readResultCache = new ConcurrentDictionary<string, bool>();
      if (_executeResultCache == null)
        _executeResultCache = new ConcurrentDictionary<string, bool>();
      if (!ReferenceEquals(ApplicationContext.User, _lastPrincipal))
      {
        // the principal has changed - reset the cache
        _readResultCache.Clear();
        _executeResultCache.Clear();
        _lastPrincipal = ApplicationContext.User;
      }
    }

    /// <summary>
    /// Returns true if the user is allowed to execute
    /// the specified method.
    /// </summary>
    /// <param name="method">Method to execute.</param>
    /// <returns>true if execute is allowed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="method"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanExecuteMethod(IMemberInfo method)
    {
      if (method is null)
        throw new ArgumentNullException(nameof(method));

      VerifyAuthorizationCache();

      if (!_executeResultCache!.TryGetValue(method.Name, out var result))
      {
        result = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.ExecuteMethod, method);
        _executeResultCache.AddOrUpdate(method.Name, result, (_, _) => { return result; });
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
    /// <exception cref="ArgumentNullException"><paramref name="method"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool CanExecuteMethod(IMemberInfo method, bool throwOnFalse)
    {
      if (method is null)
        throw new ArgumentNullException(nameof(method));

      bool result = CanExecuteMethod(method);
      if (throwOnFalse && result == false)
      {
        SecurityException ex =
          new SecurityException($"{Resources.MethodExecuteNotAllowed} ({method.Name})");
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
    /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanExecuteMethod(string methodName)
    {
      if (string.IsNullOrWhiteSpace(methodName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(methodName)), nameof(methodName));

      return CanExecuteMethod(methodName, false);
    }

    private bool CanExecuteMethod(string methodName, bool throwOnFalse)
    {

      bool result = CanExecuteMethod(new MethodInfo(methodName));
      if (throwOnFalse && result == false)
      {
        throw new SecurityException($"{Resources.MethodExecuteNotAllowed} ({methodName})");
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
      return ObjectCloner.GetInstance(ApplicationContext).Clone(this)!;
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

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [Delete]
    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
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
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
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
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
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

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserializedHandler(System.Runtime.Serialization.StreamingContext context)
    {
      if (_fieldManager != null)
        FieldManager.SetPropertyList(GetType());
      InitializeBusinessRules();
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
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="info"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objectType, PropertyInfo<P> info)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      return PropertyInfoManager.RegisterProperty<P>(objectType, info);
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
    /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> info)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));
      return PropertyInfoManager.RegisterProperty<P>(typeof(T), info);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName)
    {
      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));

      return RegisterProperty(PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName)
    {
      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));

      return RegisterProperty(PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

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
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName, P? defaultValue)
    {
      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));

      return RegisterProperty(PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName, P? defaultValue)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

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
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, RelationshipTypes relationship)
    {
      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));

      return RegisterProperty(PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, string.Empty, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, RelationshipTypes relationship)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, relationship);
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
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName, P? defaultValue, RelationshipTypes relationship)
    {
      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));

      return RegisterProperty(PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue, relationship));
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName, P? defaultValue, RelationshipTypes relationship)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

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
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="info"/> is <see langword="null"/>.</exception>
    protected static IMemberInfo RegisterMethod(Type objectType, IMemberInfo info)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      var reflected = objectType.GetMethod(info.Name);
      if (reflected == null)
        throw new ArgumentException(string.Format(Resources.NoSuchMethod, info.Name), nameof(info));
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
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="methodName"/> is <see langword="null"/>.</exception>
    protected static MethodInfo RegisterMethod(Type objectType, string methodName)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (methodName is null)
        throw new ArgumentNullException(nameof(methodName));

      var info = new MethodInfo(methodName);
      RegisterMethod(objectType, info);
      return info;
    }

    /// <summary>
    /// Registers a method for use in Authorization.
    /// </summary>
    /// <param name="methodName">Method name from nameof()</param>
    /// <exception cref="ArgumentNullException"><paramref name="methodName"/> is <see langword="null"/>.</exception>
    protected static MethodInfo RegisterMethod(string methodName)
    {
      if (methodName is null)
        throw new ArgumentNullException(nameof(methodName));

      return RegisterMethod(typeof(T), methodName);
    }

    /// <summary>
    /// Registers the method.
    /// </summary>
    /// <param name="methodLambdaExpression">The method lambda expression.</param>
    /// <exception cref="ArgumentNullException"><paramref name="methodLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static MethodInfo RegisterMethod(Expression<Action<T>> methodLambdaExpression)
    {
      if (methodLambdaExpression is null)
        throw new ArgumentNullException(nameof(methodLambdaExpression));

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
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected P? GetProperty<P>(string propertyName, P? field, P? defaultValue)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return GetProperty<P>(propertyName, field, defaultValue, NoAccessBehavior.SuppressException);
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected P? GetProperty<P>(string propertyName, P? field, P? defaultValue, NoAccessBehavior noAccess)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      #region Check to see if the property is marked with RelationshipTypes.PrivateField

      var propertyInfo = FieldManager.GetRegisteredProperty(propertyName);

      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) != RelationshipTypes.PrivateField)
        throw new InvalidOperationException(Resources.PrivateFieldException);

      #endregion

      if (CanReadProperty(propertyInfo, noAccess == NoAccessBehavior.ThrowException))
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? GetProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, P? field)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      return GetProperty<P>(propertyInfo.Name, field, propertyInfo.DefaultValue, NoAccessBehavior.SuppressException);
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? GetProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, P? field, P? defaultValue, NoAccessBehavior noAccess)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      return GetProperty<P>(propertyInfo.Name, field, defaultValue, noAccess);
    }

    /// <summary>
    /// Lazily initializes a property and returns
    /// the resulting value.
    /// </summary>
    /// <typeparam name="P">Type of the property.</typeparam>
    /// <param name="property">PropertyInfo object containing property metadata.</param>
    /// <param name="valueGenerator">Method returning the new value.</param>
    /// <remarks>
    /// If the user is not authorized to read the property
    /// value, the defaultValue value is returned as a
    /// result.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> or <paramref name="valueGenerator"/> is <see langword="null"/>.</exception>
    protected P? LazyGetProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> property, Func<P> valueGenerator)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (valueGenerator is null)
        throw new ArgumentNullException(nameof(valueGenerator));

      if (!FieldManager.FieldExists(property))
      {
        var result = valueGenerator();
        LoadProperty(property, result);
      }
      return GetProperty<P>(property);
    }

    /// <summary>
    /// Gets a value indicating whether a lazy loaded 
    /// property is currently being retrieved.
    /// </summary>
    /// <param name="propertyInfo">Property to check.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected bool PropertyIsLoading(IPropertyInfo propertyInfo)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      return LoadManager.IsLoadingProperty(propertyInfo);
    }

    /// <summary>
    /// Lazily initializes a property and returns
    /// the resulting value.
    /// </summary>
    /// <typeparam name="P">Type of the property.</typeparam>
    /// <param name="property">PropertyInfo object containing property metadata.</param>
    /// <param name="factory">Async method returning the new value.</param>
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
    /// <exception cref="ArgumentNullException"><paramref name="property"/> or <paramref name="property"/> is <see langword="null"/>.</exception>
    protected P? LazyGetPropertyAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> property, Task<P> factory)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (factory is null)
        throw new ArgumentNullException(nameof(factory));

      if (!FieldManager.FieldExists(property) && !PropertyIsLoading(property))
      {
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? GetPropertyConvert<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] F, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<F> propertyInfo, F? field)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo.Name, field, propertyInfo.DefaultValue, NoAccessBehavior.SuppressException));
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? GetPropertyConvert<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] F, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<F> propertyInfo, F? field, NoAccessBehavior noAccess)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? GetProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      return GetProperty<P>(propertyInfo, NoAccessBehavior.SuppressException);
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? GetPropertyConvert<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] F, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<F> propertyInfo)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      return Utilities.CoerceValue<P>(typeof(F), null, GetProperty<F>(propertyInfo, NoAccessBehavior.SuppressException));
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? GetPropertyConvert<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] F, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<F> propertyInfo, NoAccessBehavior noAccess)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? GetProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, NoAccessBehavior noAccess)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      if (((propertyInfo.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad) && !FieldManager.FieldExists(propertyInfo))
      {
        if (PropertyIsLoading(propertyInfo))
          return propertyInfo.DefaultValue;

        throw new InvalidOperationException(Resources.PropertyGetNotAllowed);
      }

      P? result;
      if (CanReadProperty(propertyInfo, noAccess == NoAccessBehavior.ThrowException))
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected object? GetProperty(IPropertyInfo propertyInfo)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      object? result;
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? ReadPropertyConvert<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] F, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<F> propertyInfo)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected P? ReadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      if (((propertyInfo.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad) && !FieldManager.FieldExists(propertyInfo))
      {
        if (PropertyIsLoading(propertyInfo))
          return default;
        throw new InvalidOperationException(Resources.PropertyGetNotAllowed);
      }

      P? result;
      IFieldData? data = FieldManager.GetFieldData(propertyInfo);
      if (data != null)
      {
        if (data is IFieldData<P> fd)
          result = fd.Value;
        else
          result = (P?)data.Value;
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected virtual object? ReadProperty(IPropertyInfo propertyInfo)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
      {
        return MethodCaller.CallPropertyGetter(this, propertyInfo.Name);
      }

      object? result;
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
    /// <exception cref="ArgumentNullException"><paramref name="property"/> or <paramref name="valueGenerator"/> is <see langword="null"/>.</exception>
    protected P? LazyReadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> property, Func<P> valueGenerator)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (valueGenerator is null)
        throw new ArgumentNullException(nameof(valueGenerator));

      if (!FieldManager.FieldExists(property))
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
    /// <exception cref="ArgumentNullException"><paramref name="property"/> or <paramref name="factory"/> is <see langword="null"/>.</exception>
    protected P? LazyReadPropertyAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> property, Task<P> factory)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (factory is null)
        throw new ArgumentNullException(nameof(factory));

      if (!FieldManager.FieldExists(property) && !PropertyIsLoading(property))
      {
        LoadPropertyAsync(property, factory);
      }
      return ReadProperty<P>(property);
    }

    P? IManageProperties.LazyReadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator) where P: default
    {
      return LazyReadProperty(propertyInfo, valueGenerator);
    }

    P? IManageProperties.LazyReadPropertyAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, Task<P> factory) where P : default
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected void LoadPropertyConvert<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] F>(PropertyInfo<P> propertyInfo, F? newValue)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      try
      {
        P? oldValue;
        var fieldData = FieldManager.GetFieldData(propertyInfo);
        if (fieldData == null)
        {
          oldValue = propertyInfo.DefaultValue;
          fieldData = FieldManager.LoadFieldData(propertyInfo, oldValue);
        }
        else
        {
          oldValue = (P?)fieldData.Value;
        }

        if (ValueComparer.AreNotEqual(oldValue, newValue))
          FieldManager.LoadFieldData(propertyInfo, Utilities.CoerceValue<P>(typeof(F), oldValue, newValue));

      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(string.Format(Resources.PropertyLoadException, propertyInfo.Name, ex.Message));
      }
    }

    void IManageProperties.LoadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, P? newValue) where P : default
    {
      LoadProperty<P>(propertyInfo, newValue);
    }

    bool IManageProperties.FieldExists(IPropertyInfo property)
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected void LoadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, P? newValue)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      try
      {
        P? oldValue;
        var fieldData = FieldManager.GetFieldData(propertyInfo);
        if (fieldData == null)
        {
          oldValue = propertyInfo.DefaultValue;
          fieldData = FieldManager.LoadFieldData(propertyInfo, oldValue);
        }
        else
        {
          oldValue = (P?)fieldData.Value;
        }

        if (ValueComparer.AreNotEqual(oldValue, newValue))
          FieldManager.LoadFieldData(propertyInfo, newValue);
        
      }
      catch (Exception ex)
      {
        throw new PropertyLoadException(string.Format(Resources.PropertyLoadException, propertyInfo.Name, ex.Message));
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
    protected virtual void LoadProperty(IPropertyInfo propertyInfo, object? newValue)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      var t = GetType();
      var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      var method = t.GetMethods(flags).First(c => c.Name == "LoadProperty" && c.IsGenericMethod);
      var gm = method.MakeGenericMethod(propertyInfo.Type);
      var p = new object?[] { propertyInfo, newValue };
      gm.Invoke(this, p);
    }

    //private AsyncLoadManager
    [NonSerialized]
    private AsyncLoadManager? _loadManager;
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

    void loadManager_UnhandledAsyncException(object? sender, Core.ErrorEventArgs e)
    {
      OnUnhandledAsyncException(e);
    }

    void loadManager_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

    /// <summary>
    /// Load a property from an async method. 
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="property"></param>
    /// <param name="factory"></param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> or <paramref name="factory"/> is <see langword="null"/>.</exception>
    protected void LoadPropertyAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] R>(PropertyInfo<R> property, Task<R> factory)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (factory is null)
        throw new ArgumentNullException(nameof(factory));

      LoadManager.BeginLoad(new TaskLoader<R>(property, factory));
    }
    #endregion

    #region  Field Manager

    [NotUndoable]
    private FieldDataManager? _fieldManager;

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
          _fieldManager = new FieldDataManager(ApplicationContext, GetType());
        }
        return _fieldManager;
      }
    }

    FieldDataManager IUseFieldManager.FieldManager => FieldManager;

    #endregion

    #region IsBusy / IsIdle

    /// <summary>
    /// Await this method to ensure business object
    /// is not busy running async rules.
    /// </summary>
    public async Task WaitForIdle()
    {
      var cslaOptions = ApplicationContext.GetRequiredService<Configuration.CslaOptions>();
      await WaitForIdle(TimeSpan.FromSeconds(cslaOptions.DefaultWaitForIdleTimeoutInSeconds)).ConfigureAwait(false);
    }

    /// <summary>
    /// Await this method to ensure business object
    /// is not busy running async rules.
    /// </summary>
    /// <param name="timeout">Timeout duration</param>
    public Task WaitForIdle(TimeSpan timeout)
    {
      return BusyHelper.WaitForIdle(this, timeout);
    }

    /// <summary>
    /// Waits for the object to become idle.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task WaitForIdle(CancellationToken ct)
    {
      return BusyHelper.WaitForIdle(this, ct);
    }

    [NonSerialized]
    [NotUndoable]
    private int _isBusyCounter;

    /// <summary>
    /// Marks the object as being busy (it is
    /// running an async operation).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkBusy()
    {
      int updatedValue = Interlocked.Increment(ref _isBusyCounter);

      if (updatedValue == 1)
      {
        OnBusyChanged(new BusyChangedEventArgs("", true));
      }
    }

    /// <summary>
    /// Marks the object as being not busy
    /// (it is not running an async operation).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkIdle()
    {
      int updatedValue = Interlocked.Decrement(ref _isBusyCounter);
      if (updatedValue < 0)
      {
        _ = Interlocked.CompareExchange(ref _isBusyCounter, 0, updatedValue);
      }
      if (updatedValue == 0)
      {
        OnBusyChanged(new BusyChangedEventArgs("", false));
      }
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// object or any of its child objects are
    /// running an async operation.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsBusy => IsSelfBusy || (_fieldManager != null && FieldManager.IsBusy());

    /// <summary>
    /// Gets a value indicating whether this
    /// object is
    /// running an async operation.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsSelfBusy => _isBusyCounter > 0 || LoadManager.IsLoading;

    void Child_PropertyBusy(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

    [NotUndoable]
    [NonSerialized]
    private BusyChangedEventHandler? _propertyBusy;

    /// <summary>
    /// Event raised when the IsBusy property value
    /// has changed.
    /// </summary>
    public event BusyChangedEventHandler? BusyChanged
    {
      add => _propertyBusy = (BusyChangedEventHandler?)Delegate.Combine(_propertyBusy, value);
      remove => _propertyBusy = (BusyChangedEventHandler?)Delegate.Remove(_propertyBusy, value);
    }

    /// <summary>
    /// Raises the BusyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the property
    /// that has changed.</param>
    /// <param name="busy">New busy value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnBusyChanged(string propertyName, bool busy)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      OnBusyChanged(new BusyChangedEventArgs(propertyName, busy));
    }

    /// <summary>
    /// Raises the BusyChanged event.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="args"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnBusyChanged(BusyChangedEventArgs args)
    {
      if (args is null)
        throw new ArgumentNullException(nameof(args));

      _propertyBusy?.Invoke(this, args);
    }

    #endregion

    #region IDataPortalTarget Members

    void IDataPortalTarget.CheckRules()
    { }

    Task IDataPortalTarget.CheckRulesAsync() => Task.CompletedTask;

    void IDataPortalTarget.MarkAsChild()
    { }

    void IDataPortalTarget.MarkNew()
    { }

    void IDataPortalTarget.MarkOld()
    { }

    void IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      DataPortal_OnDataPortalException(e, ex);
    }

    void IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      Child_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      Child_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      Child_OnDataPortalException(e, ex);
    }

    #endregion

    #region IManageProperties Members

    bool IManageProperties.HasManagedProperties => (_fieldManager != null && _fieldManager.HasFields);

    List<IPropertyInfo> IManageProperties.GetManagedProperties()
    {
      return FieldManager.GetRegisteredProperties();
    }

    object? IManageProperties.GetProperty(IPropertyInfo propertyInfo)
    {
      return GetProperty(propertyInfo);
    }

    object? IManageProperties.LazyGetProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator)
    {
      return LazyGetProperty(propertyInfo, valueGenerator);
    }

    object? IManageProperties.LazyGetPropertyAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, Task<P> factory)
    {
      return LazyGetPropertyAsync(propertyInfo, factory);
    }

    object? IManageProperties.ReadProperty(IPropertyInfo propertyInfo)
    {
      return ReadProperty(propertyInfo);
    }

    P? IManageProperties.ReadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo) where P: default
    {
      return ReadProperty<P>(propertyInfo);
    }

    void IManageProperties.SetProperty(IPropertyInfo propertyInfo, object? newValue)
    {
      throw new NotImplementedException("IManageProperties.SetProperty");
    }

    void IManageProperties.LoadProperty(IPropertyInfo propertyInfo, object? newValue)
    {
      LoadProperty(propertyInfo, newValue);
    }

    bool IManageProperties.LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object? newValue)
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
    /// references into the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
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
    /// references from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Children.TryGetValue("_fieldManager", out var child))
      {
        _fieldManager = (FieldDataManager?)formatter.GetObject(child.ReferenceId);
      }
      base.OnSetChildren(info, formatter);
    }

    #endregion

    #region INotifyUnhandledAsyncException Members

    [NotUndoable]
    [NonSerialized]
    private EventHandler<Core.ErrorEventArgs>? _unhandledAsyncException;

    /// <summary>
    /// Event raised when an exception occurs on a background
    /// thread during an asynchronous operation.
    /// </summary>
    public event EventHandler<Core.ErrorEventArgs>? UnhandledAsyncException
    {
      add => _unhandledAsyncException = (EventHandler<Core.ErrorEventArgs>?)Delegate.Combine(_unhandledAsyncException, value);
      remove => _unhandledAsyncException = (EventHandler<Core.ErrorEventArgs>?)Delegate.Remove(_unhandledAsyncException, value);
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="error">Error arguments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnUnhandledAsyncException(Core.ErrorEventArgs error)
    {
      if (error is null)
        throw new ArgumentNullException(nameof(error));

      _unhandledAsyncException?.Invoke(this, error);
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="originalSender">Original sender of the
    /// event.</param>
    /// <param name="error">Exception that occurred.</param>
    /// <exception cref="ArgumentNullException"><paramref name="originalSender"/> or <paramref name="error"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      if (originalSender is null)
        throw new ArgumentNullException(nameof(originalSender));
      if (error is null)
        throw new ArgumentNullException(nameof(error));

      OnUnhandledAsyncException(new Core.ErrorEventArgs(originalSender, error));
    }

    #endregion
  }
} 