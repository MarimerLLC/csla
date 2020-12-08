//-----------------------------------------------------------------------
// <copyright file="BusinessBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which most business objects</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Csla.Properties;
using Csla.Core;
using Csla.Reflection;
using System.Threading.Tasks;

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
  public abstract class BusinessBase<T> :
    Core.BusinessBase, Core.ISavable, Core.ISavable<T>, IBusinessBase where T : BusinessBase<T>
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
    public async Task<T> SaveAsync()
    {
      return await SaveAsync(false);
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <param name="forceUpdate">
    /// If true, triggers overriding IsNew and IsDirty. 
    /// If false then it is the same as calling Save().
    /// </param>
    public async Task<T> SaveAsync(bool forceUpdate)
    {
      return await SaveAsync(forceUpdate, null, false);
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
      T result = default(T);
      if (this.IsChild)
        throw new InvalidOperationException(Resources.NoSaveChildException);
      if (EditLevel > 0)
        throw new InvalidOperationException(Resources.NoSaveEditingException);
      if (!IsValid && !IsDeleted)
        throw new Rules.ValidationException(Resources.NoSaveInvalidException);
      if (IsBusy)
        throw new InvalidOperationException(Resources.BusyObjectsMayNotBeSaved);
      if (IsDirty)
      {
        if (isSync)
        {
          result = DataPortal.Update<T>((T)this);
        }
        else
        {
          if (ApplicationContext.AutoCloneOnUpdate)
            MarkBusy();
          try
          {
            result = await DataPortal.UpdateAsync<T>((T)this);
          }
          finally
          {
            if (ApplicationContext.AutoCloneOnUpdate)
            {
              if (result != null)
                result.MarkIdle();
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
      return this.Save();
    }

    /// <summary>
    /// Saves the object to the database, merging
    /// any resulting updates into the existing
    /// object graph.
    /// </summary>
    public async Task SaveAndMergeAsync()
    {
      await SaveAndMergeAsync(false);
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
      new GraphMerger().MergeGraph(this, await SaveAsync(forceUpdate));
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    [Obsolete]
    public void BeginSave()
    {
      BeginSave(null);
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    /// <param name="userState">User state data.</param>
    [Obsolete]
    public void BeginSave(object userState)
    {
      BeginSave(false, null, userState);
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    /// <param name="handler">
    /// Method called when the operation is complete.
    /// </param>
    [Obsolete]
    public void BeginSave(EventHandler<SavedEventArgs> handler)
    {
      BeginSave(false, handler, null);
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    /// <param name="forceUpdate">
    /// If true, triggers overriding IsNew and IsDirty. 
    /// If false then it is the same as calling Save().
    /// </param>
    /// <param name="handler">
    /// Method called when the operation is complete.
    /// </param>
    /// <param name="userState">User state data.</param>
    [Obsolete]
    public async void BeginSave(bool forceUpdate, EventHandler<SavedEventArgs> handler, object userState)
    {
      T result = default(T);
      Exception error = null;
      try
      {
        result = await SaveAsync(forceUpdate, userState, false);
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          error = ex.InnerExceptions[0];
        else
          error = ex;
      }
      catch (Exception ex)
      {
        error = ex;
      }

      if (error != null)
        OnSaved(null, error, userState);
      handler?.Invoke(this, new SavedEventArgs(result, error, userState));
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    /// <param name="forceUpdate">
    /// If true, triggers overriding IsNew and IsDirty. 
    /// If false then it is the same as calling Save().
    /// </param>
    /// <remarks>
    /// This overload is designed for use in web applications
    /// when implementing the Update method in your 
    /// data wrapper object.
    /// </remarks>
    [Obsolete]
    public void BeginSave(bool forceUpdate)
    {
      this.BeginSave(forceUpdate, null);
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    /// <param name="forceUpdate">
    /// If true, triggers overriding IsNew and IsDirty. 
    /// If false then it is the same as calling Save().
    /// </param>
    /// <param name="handler">
    /// Delegate reference to a callback handler that will
    /// be invoked when the async operation is complete.
    /// </param>
    /// <remarks>
    /// This overload is designed for use in web applications
    /// when implementing the Update method in your 
    /// data wrapper object.
    /// </remarks>
    [Obsolete]
    public void BeginSave(bool forceUpdate, EventHandler<SavedEventArgs> handler)
    {
      this.BeginSave(forceUpdate, handler, null);
    }

    /// <summary>
    /// Saves the object to the database, forcing
    /// IsNew to false and IsDirty to True.
    /// </summary>
    /// <param name="handler">
    /// Delegate reference to a callback handler that will
    /// be invoked when the async operation is complete.
    /// </param>
    /// <param name="userState">User state data.</param>
    /// <remarks>
    /// This overload is designed for use in web applications
    /// when implementing the Update method in your 
    /// data wrapper object.
    /// </remarks>
    [Obsolete]
    public void BeginSave(EventHandler<SavedEventArgs> handler, object userState)
    {
      this.BeginSave(false, handler, userState);
    }

    #endregion

    #region ISavable Members

    void Csla.Core.ISavable.SaveComplete(object newObject)
    {
      OnSaved((T)newObject, null, null);
    }

    void Csla.Core.ISavable<T>.SaveComplete(T newObject)
    {
      OnSaved(newObject, null, null);
    }

    object Csla.Core.ISavable.Save()
    {
      return Save();
    }

    object Csla.Core.ISavable.Save(bool forceUpdate)
    {
      return Save(forceUpdate);
    }

    [NonSerialized]
    [NotUndoable]
    private EventHandler<Csla.Core.SavedEventArgs> _nonSerializableSavedHandlers;
    [NotUndoable]
    private EventHandler<Csla.Core.SavedEventArgs> _serializableSavedHandlers;

    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event EventHandler<Csla.Core.SavedEventArgs> Saved
    {
      add
      {
        if (value.Method.IsPublic &&
           (value.Method.DeclaringType.IsSerializable ||
            value.Method.IsStatic))
          _serializableSavedHandlers = (EventHandler<Csla.Core.SavedEventArgs>)
            System.Delegate.Combine(_serializableSavedHandlers, value);
        else
          _nonSerializableSavedHandlers = (EventHandler<Csla.Core.SavedEventArgs>)
            System.Delegate.Combine(_nonSerializableSavedHandlers, value);
      }
      remove
      {
        if (value.Method.IsPublic &&
           (value.Method.DeclaringType.IsSerializable ||
            value.Method.IsStatic))
          _serializableSavedHandlers = (EventHandler<Csla.Core.SavedEventArgs>)
            System.Delegate.Remove(_serializableSavedHandlers, value);
        else
          _nonSerializableSavedHandlers = (EventHandler<Csla.Core.SavedEventArgs>)
            System.Delegate.Remove(_nonSerializableSavedHandlers, value);
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
      Csla.Core.SavedEventArgs args = new Csla.Core.SavedEventArgs(newObject, e, userState);
      if (_nonSerializableSavedHandlers != null)
        _nonSerializableSavedHandlers.Invoke(this, args);
      if (_serializableSavedHandlers != null)
        _serializableSavedHandlers.Invoke(this, args);
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

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, string.Empty, defaultValue));
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
      return RegisterProperty(reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship);
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
    /// Registers a method for use in Authorization.
    /// </summary>
    /// <param name="methodLambdaExpression">The method lambda expression.</param>
    /// <returns></returns>
    protected static MethodInfo RegisterMethod(Expression<Action<T>> methodLambdaExpression)
    {
      System.Reflection.MethodInfo reflectedMethodInfo = Reflect<T>.GetMethod(methodLambdaExpression);
      return RegisterMethod(reflectedMethodInfo.Name);
    }

    #endregion
  }
}