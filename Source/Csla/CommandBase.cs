//-----------------------------------------------------------------------
// <copyright file="CommandBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which command </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Csla.Server;
using Csla.Properties;
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.DataPortalClient;
using System.Linq.Expressions;
using Csla.Reflection;
using System.Reflection;
using System.Threading.Tasks;

namespace Csla
{
  /// <summary>
  /// This is the base class from which command 
  /// objects will be derived.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Command objects allow the execution of arbitrary server-side
  /// functionality. Most often, this involves the invocation of
  /// a stored procedure in the database, but can involve any other
  /// type of stateless, atomic call to the server instead.
  /// </para><para>
  /// To implement a command object, inherit from CommandBase and
  /// override the DataPortal_Execute method. In this method you can
  /// implement any server-side code as required.
  /// </para><para>
  /// To pass data to/from the server, use instance variables within
  /// the command object itself. The command object is instantiated on
  /// the client, and is passed by value to the server where the 
  /// DataPortal_Execute method is invoked. The command object is then
  /// returned to the client by value.
  /// </para>
  /// </remarks>
  [Serializable]
  public abstract class CommandBase<T> : ManagedObjectBase, 
      ICommandObject, ICloneable,
      Csla.Server.IDataPortalTarget, Csla.Core.IManageProperties,
      ICommandBase
    where T : CommandBase<T>
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public CommandBase()
    {
      Initialize();
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

    #region Data Access

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Fetch(object criteria)
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
    /// Override this method to implement any server-side code
    /// that is to be run when the command is executed.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Execute()
    {
      throw new NotSupportedException(Resources.ExecuteNotSupportedException);
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
    /// occurs during server-side processing.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during processing.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    #endregion

    #region IDataPortalTarget Members

    void IDataPortalTarget.CheckRules()
    { }

    void IDataPortalTarget.MarkAsChild()
    { }

    void IDataPortalTarget.MarkNew()
    { }

    void IDataPortalTarget.MarkOld()
    { }

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
    { }

    void IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    void IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

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

    #region  Register Properties

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

    #region IManageProperties Members

    bool IManageProperties.HasManagedProperties
    {
      get { return (FieldManager != null && FieldManager.HasFields); }
    }

    List<IPropertyInfo> IManageProperties.GetManagedProperties()
    {
      return FieldManager.GetRegisteredProperties();
    }

    object IManageProperties.GetProperty(IPropertyInfo propertyInfo)
    {
      return ReadProperty(propertyInfo);
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
      FieldManager.SetFieldData(propertyInfo, newValue);
    }

    void IManageProperties.LoadProperty(IPropertyInfo propertyInfo, object newValue)
    {
      LoadProperty(propertyInfo, newValue);
    }

    void IManageProperties.LoadProperty<P>(PropertyInfo<P> propertyInfo, P newValue)
    {
      LoadProperty<P>(propertyInfo, newValue);
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

    bool IManageProperties.FieldExists(Core.IPropertyInfo property)
    {
      return FieldManager.FieldExists(property);
    }

    object IManageProperties.LazyGetProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator)
    {
      throw new NotImplementedException();
    }

    object IManageProperties.LazyGetPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory)
    {
      throw new NotImplementedException();
    }

    P IManageProperties.LazyReadProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator)
    {
      throw new NotImplementedException();
    }

    P IManageProperties.LazyReadPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}