//-----------------------------------------------------------------------
// <copyright file="BusinessBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which most business objects</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Csla.Core;
using Csla.Properties;
using Csla.Reflection;

namespace Csla
{

  /// <summary>
  /// This is the base class from which most business objects
  /// will be derived.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class is the core of the CSLA .NET framework. To create
  /// a business object, inherit from this class.
  /// </para><para>
  /// Please refer to 'Expert C# 2008 Business Objects' for
  /// full details on the use of this base class to create business
  /// objects.
  /// </para>
  /// </remarks>
  /// <typeparam name="T">Type of the business object being defined.</typeparam>
  [Serializable]
  public abstract class BusinessBase<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    T> :
    BusinessBase, ISavable, ISavable<T>, IBusinessBase where T : BusinessBase<T>
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

    #region Clone

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

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Calling this method starts the save operation, causing the object
    /// to be inserted, updated or deleted within the database based on the
    /// object's current state.
    /// </para><para>
    /// If <see cref="Core.BusinessBase.IsDeleted" /> is true
    /// the object will be deleted. Otherwise, if <see cref="Core.BusinessBase.IsNew" /> 
    /// is true the object will be inserted. 
    /// Otherwise the object's data will be updated in the database.
    /// </para><para>
    /// All this is contingent on <see cref="Core.BusinessBase.IsDirty" />. If
    /// this value is false, no data operation occurs. 
    /// It is also contingent on <see cref="Core.BusinessBase.IsValid" />. 
    /// If this value is false an
    /// exception will be thrown to indicate that the UI attempted to save an
    /// invalid object.
    /// </para><para>
    /// It is important to note that this method returns a new version of the
    /// business object that contains any data updated during the save operation.
    /// You MUST update all object references to use this new version of the
    /// business object in order to have access to the correct object data.
    /// </para><para>
    /// You can override this method to add your own custom behaviors to the save
    /// operation. For instance, you may add some security checks to make sure
    /// the user can save the object. If all security checks pass, you would then
    /// invoke the base Save method via <c>base.Save()</c>.
    /// </para>
    /// </remarks>
    /// <returns>A new object containing the saved values.</returns>
    public T Save()
    {
      try
      {
        return SaveAsync(false, null, true).Result;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          throw ex.InnerExceptions[0];
        else
          throw;
      }
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    public Task<T> SaveAsync()
    {
      return SaveAsync(false);
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <param name="forceUpdate">
    /// If true, triggers overriding IsNew and IsDirty. 
    /// If false then it is the same as calling Save().
    /// </param>
    public Task<T> SaveAsync(bool forceUpdate)
    {
      return SaveAsync(forceUpdate, null, false);
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <param name="forceUpdate">
    /// If true, triggers overriding IsNew and IsDirty. 
    /// If false then it is the same as calling Save().
    /// </param>
    /// <param name="userState">User state data.</param>
    /// <param name="isSync">True if the save operation should be synchronous.</param>
    protected async virtual Task<T> SaveAsync(bool forceUpdate, object userState, bool isSync)
    {
      if (forceUpdate && IsNew)
      {
        // mark the object as old - which makes it
        // not dirty
        MarkOld();
        // now mark the object as dirty so it can save
        MarkDirty(true);
      }
      T result = default;
      if (IsChild)
        throw new InvalidOperationException(Resources.NoSaveChildException);
      if (EditLevel > 0)
        throw new InvalidOperationException(Resources.NoSaveEditingException);
      if (!IsValid && !IsDeleted)
        throw new Rules.ValidationException(Resources.NoSaveInvalidException);
      if (IsBusy)
        throw new InvalidOperationException(Resources.BusyObjectsMayNotBeSaved);
      if (IsDirty)
      {
        var dp = ApplicationContext.CreateInstanceDI<DataPortal<T>>();
        if (isSync)
        {
          result = dp.Update((T)this);
        }
        else
        {
          var dataPortalOptions = ApplicationContext.GetRequiredService<Configuration.DataPortalOptions>();
          if (dataPortalOptions.DataPortalClientOptions.AutoCloneOnUpdate)
            MarkBusy();
          try
          {
            result = await dp.UpdateAsync((T)this);
          }
          finally
          {
            if (dataPortalOptions.DataPortalClientOptions.AutoCloneOnUpdate)
            {
              result?.MarkIdle();
              MarkIdle();
            }
          }
        }
      }
      else
      {
        result = (T)this;
      }
      OnSaved(result, null, userState);
      return result;
    }

    /// <summary>
    /// Saves the object to the database, forcing
    /// IsNew to false and IsDirty to True.
    /// </summary>
    /// <param name="forceUpdate">
    /// If true, triggers overriding IsNew and IsDirty. 
    /// If false then it is the same as calling Save().
    /// </param>
    /// <returns>A new object containing the saved values.</returns>
    /// <remarks>
    /// This overload is designed for use in web applications
    /// when implementing the Update method in your 
    /// data wrapper object.
    /// </remarks>
    public T Save(bool forceUpdate)
    {
      if (forceUpdate && IsNew)
      {
        // mark the object as old - which makes it
        // not dirty
        MarkOld();
        // now mark the object as dirty so it can save
        MarkDirty(true);
      }
      return Save();
    }

    /// <summary>
    /// Saves the object to the database, merging
    /// any resulting updates into the existing
    /// object graph.
    /// </summary>
    public Task SaveAndMergeAsync()
    {
      return SaveAndMergeAsync(false);
    }

    /// <summary>
    /// Saves the object to the database, merging
    /// any resulting updates into the existing
    /// object graph.
    /// </summary>
    /// <param name="forceUpdate">
    /// If true, triggers overriding IsNew and IsDirty. 
    /// If false then it is the same as calling SaveAndMergeAsync().
    /// </param>
    public async Task SaveAndMergeAsync(bool forceUpdate)
    {
      await new GraphMerger(ApplicationContext).MergeGraphAsync(this, await SaveAsync(forceUpdate));
    }

    #endregion

    #region ISavable Members

    void ISavable.SaveComplete(object newObject)
    {
      OnSaved((T)newObject, null, null);
    }

    void ISavable<T>.SaveComplete(T newObject)
    {
      OnSaved(newObject, null, null);
    }

    object ISavable.Save()
    {
      return Save();
    }

    object ISavable.Save(bool forceUpdate)
    {
      return Save(forceUpdate);
    }

    [NonSerialized]
    [NotUndoable]
    private EventHandler<SavedEventArgs> _nonSerializableSavedHandlers;
    [NotUndoable]
    private EventHandler<SavedEventArgs> _serializableSavedHandlers;

    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public event EventHandler<SavedEventArgs> Saved
    {
      add
      {
        if (value.Method.IsPublic)
          _serializableSavedHandlers = (EventHandler<SavedEventArgs>)
            Delegate.Combine(_serializableSavedHandlers, value);
        else
          _nonSerializableSavedHandlers = (EventHandler<SavedEventArgs>)
            Delegate.Combine(_nonSerializableSavedHandlers, value);
      }
      remove
      {
        if (value.Method.IsPublic)
          _serializableSavedHandlers = (EventHandler<SavedEventArgs>)
            Delegate.Remove(_serializableSavedHandlers, value);
        else
          _nonSerializableSavedHandlers = (EventHandler<SavedEventArgs>)
            Delegate.Remove(_nonSerializableSavedHandlers, value);
      }
    }

    async Task<object> ISavable.SaveAsync()
    {
      return await SaveAsync();
    }

    async Task<object> ISavable.SaveAsync(bool forceUpdate)
    {
      return await SaveAsync(forceUpdate);
    }

    /// <summary>
    /// Raises the Saved event, indicating that the
    /// object has been saved, and providing a reference
    /// to the new object instance.
    /// </summary>
    /// <param name="newObject">The new object instance.</param>
    /// <param name="e">Exception that occurred during operation.</param>
    /// <param name="userState">User state object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSaved(T newObject, Exception e, object userState)
    {
      SavedEventArgs args = new SavedEventArgs(newObject, e, userState);
      _nonSerializableSavedHandlers?.Invoke(this, args);
      _serializableSavedHandlers?.Invoke(this, args);
    }

    #endregion

    #region  Register Properties/Methods

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
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(PropertyInfo<P> info)
    {
      return Core.FieldManager.PropertyInfoManager.RegisterProperty<P>(typeof(T), info);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(string propertyName)
    {
      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="relationship">Relationship with property value.</param>
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(string propertyName, RelationshipTypes relationship)
    {
      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, string.Empty, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="relationship">Relationship with property value.</param>
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(Expression<Func<T, object>> propertyLambdaExpression, RelationshipTypes relationship)
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
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(string propertyName, string friendlyName)
    {
      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName)
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
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(string propertyName, string friendlyName, P defaultValue)
    {
      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty(reflectedPropertyInfo.Name, friendlyName, defaultValue);
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
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(string propertyName, string friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue, relationship));
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
    protected static PropertyInfo<P> RegisterProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty(reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship);
    }

    /// <summary>
    /// Registers a method for use in Authorization.
    /// </summary>
    /// <param name="methodName">Method name from nameof()</param>
    protected static MethodInfo RegisterMethod(string methodName)
    {
      return RegisterMethod(typeof(T), methodName);
    }

    /// <summary>
    /// Registers a method for use in Authorization.
    /// </summary>
    /// <param name="methodLambdaExpression">The method lambda expression.</param>
    protected static MethodInfo RegisterMethod(Expression<Action<T>> methodLambdaExpression)
    {
      System.Reflection.MethodInfo reflectedMethodInfo = Reflect<T>.GetMethod(methodLambdaExpression);
      return RegisterMethod(reflectedMethodInfo.Name);
    }

    #endregion
  }
}