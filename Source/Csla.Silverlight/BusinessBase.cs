//-----------------------------------------------------------------------
// <copyright file="BusinessBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Base class for editable objects.</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq.Expressions;
using System.Reflection;
using Csla.Core;
using System.ComponentModel;
using Csla.Properties;
using Csla.Reflection;
using Csla.Serialization;
using System.Diagnostics;
using Csla.Rules;

namespace Csla
{
  /// <summary>
  /// Base class for editable objects.
  /// </summary>
  /// <typeparam name="T">Type of the business object.</typeparam>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public class BusinessBase<T> : BusinessBase, ISavable
    where T : BusinessBase<T>
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

    #region ICloneable

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public T Clone()
    {
      return (T)GetClone();
    }

    #endregion

    #region Data Access

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void DataPortal_Create(Csla.DataPortalClient.LocalProxy<T>.CompletedHandler handler)
    {
      BusinessRules.CheckRules();
      handler((T)this, null);
    }

    /// <summary>
    /// Override this method to allow insertion of a business
    /// object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<T>.CompletedHandler handler)
    {
      throw new NotSupportedException(Resources.InsertNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow update of a business
    /// object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void DataPortal_Update(Csla.DataPortalClient.LocalProxy<T>.CompletedHandler handler)
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow deferred deletion of a business object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void DataPortal_DeleteSelf(Csla.DataPortalClient.LocalProxy<T>.CompletedHandler handler)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    #endregion

    #region ISavable Members

    void Csla.Core.ISavable.BeginSave()
    {
      BeginSave();
    }

    void ISavable.SaveComplete(object newObject, Exception error, object userState)
    {
      OnSaved((T)newObject, error, userState);
    }

    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    public event EventHandler<Csla.Core.SavedEventArgs> Saved;

    /// <summary>
    /// Raises the Saved event, indicating that the
    /// object has been saved, and providing a reference
    /// to the new object instance.
    /// </summary>
    /// <param name="newObject">The new object instance.</param>
    /// <param name="error">Exception object.</param>
    /// <param name="userState">User state object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnSaved(T newObject, Exception error, object userState)
    {
      MarkIdle();
      if (Saved != null)
        Saved(this, new SavedEventArgs(newObject, error, userState));
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Calling this method starts the save operation, causing the object
    /// to be inserted, updated or deleted within the database based on the
    /// object's current state.
    /// </para><para>
    /// If <see cref="Core.BusinessBase.IsDeleted" /> is <see langword="true"/>
    /// the object will be deleted. Otherwise, if <see cref="Core.BusinessBase.IsNew" /> 
    /// is <see langword="true"/> the object will be inserted. 
    /// Otherwise the object's data will be updated in the database.
    /// </para><para>
    /// All this is contingent on <see cref="Core.BusinessBase.IsDirty" />. If
    /// this value is <see langword="false"/>, no data operation occurs. 
    /// It is also contingent on <see cref="Core.BusinessBase.IsValid" />. 
    /// If this value is <see langword="false"/> an
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
    public void BeginSave()
    {
      BeginSave(false, null, null);
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <param name="userState">User state object.</param>
    public void BeginSave(object userState)
    {
      BeginSave(false, null, userState);
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <param name="handler">Method called when the save is complete.</param>
    public void BeginSave(EventHandler<SavedEventArgs> handler)
    {
      BeginSave(false, handler, null);
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <param name="handler">Method called when the save is complete.</param>
    /// <param name="userState">User state object.</param>
    public void BeginSave(EventHandler<SavedEventArgs> handler, object userState)
    {
      BeginSave(false, handler, userState);
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <param name="handler">Method called when the save is complete.</param>
    /// <param name="userState">User state object.</param>
    /// <param name="forceUpdate">If true, forces the object to
    /// be updated (not inserted) even if IsNew is true.</param>
    public virtual void BeginSave(bool forceUpdate, EventHandler<SavedEventArgs> handler, object userState)
    {
      if (forceUpdate && IsNew)
      {
        // mark the object as old - which makes it
        // not dirty
        MarkOld();
        // now mark the object as dirty so it can save
        MarkDirty(true);
      }

      if (this.IsChild)
      {
        var error = new InvalidOperationException(Resources.NoSaveChildException);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error, userState));
        OnSaved(null, error, userState);
      }
      else if (EditLevel > 0)
      {
        var error = new InvalidOperationException(Resources.NoSaveEditingException);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error, userState));
        OnSaved(null, error, userState);
      }
      else if (!IsValid && !IsDeleted)
      {
        ValidationException error = new ValidationException(Resources.NoSaveInvalidException);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error, userState));
        OnSaved(null, error, userState);
      }
      else if (IsBusy)
      {
        var error = new InvalidOperationException(Resources.BusyObjectsMayNotBeSaved);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error, userState));
        OnSaved(null, error, userState);
      }
      else
      {
        if (IsDirty)
        {
          MarkBusy();
          if (userState == null)
          {
            DataPortal.BeginUpdate<T>(this, (o, e) =>
            {
              T result = e.Object;
              if (handler != null)
                handler(result, new SavedEventArgs(result, e.Error, userState));
              OnSaved(result, e.Error, userState);
            });
          }
          else
          {
            DataPortal.BeginUpdate<T>(this, (o, e) =>
            {
              T result = e.Object;
              if (handler != null)
                handler(result, new SavedEventArgs(result, e.Error, e.UserState));
              OnSaved(result, e.Error, e.UserState);
            }, userState);
          }
        }
        else
        {
          if (handler != null)
            handler(this, new SavedEventArgs(this, null, userState));
          OnSaved((T)this, null, userState);
        }
      }
    }

    /// <summary>
    /// Saves the object to the database, forcing
    /// IsNew to <see langword="false"/> and IsDirty to True.
    /// </summary>
    /// <param name="forceUpdate">
    /// If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
    /// If <see langword="false"/> then it is the same as calling Save().
    /// </param>
    /// <returns>A new object containing the saved values.</returns>
    /// <remarks>
    /// This overload is designed for use in web applications
    /// when implementing the Update method in your 
    /// data wrapper object.
    /// </remarks>
    public void BeginSave(bool forceUpdate)
    {
      BeginSave(forceUpdate, null, null);
    }

    /// <summary>
    /// Saves the object to the database, forcing
    /// IsNew to <see langword="false"/> and IsDirty to True.
    /// </summary>
    /// <param name="forceUpdate">
    /// If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
    /// If <see langword="false"/> then it is the same as calling Save().
    /// </param>
    /// <param name="userState">User state object.</param>
    public void BeginSave(bool forceUpdate, object userState)
    {
      BeginSave(forceUpdate, null, userState);
    }

    /// <summary>
    /// Saves the object to the database, forcing
    /// IsNew to <see langword="false"/> and IsDirty to True.
    /// </summary>
    /// <param name="forceUpdate">
    /// If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
    /// If <see langword="false"/> then it is the same as calling Save().
    /// </param>
    /// <param name="handler">Method called when the operation is complete.</param>
    public void BeginSave(bool forceUpdate, EventHandler<SavedEventArgs> handler)
    {
      this.BeginSave(forceUpdate, handler, null);
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
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(new PropertyInfo<P>(reflectedPropertyInfo.Name));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(new PropertyInfo<P>(reflectedPropertyInfo.Name, reflectedPropertyInfo.Name, relationship));
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

      return RegisterProperty(new PropertyInfo<P>(reflectedPropertyInfo.Name, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(new PropertyInfo<P>(reflectedPropertyInfo.Name, friendlyName, relationship));
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

      return RegisterProperty(new PropertyInfo<P>(reflectedPropertyInfo.Name, friendlyName, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(new PropertyInfo<P>(reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship));
    }

    #endregion
  }
}