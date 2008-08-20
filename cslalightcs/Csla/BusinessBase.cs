using System;
using Csla.Core;
using System.ComponentModel;
using Csla.Properties;
using Csla.Serialization;
using System.Diagnostics;

namespace Csla
{
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
      ValidationRules.CheckRules();
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

    void ISavable.SaveComplete(object newObject, Exception error)
    {
      OnSaved((T)newObject, error);
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
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnSaved(T newObject, Exception error)
    {
      MarkIdle();
      if (Saved != null)
        Saved(this, new SavedEventArgs(newObject, error));
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
    public virtual void BeginSave()
    {
      if (this.IsChild)
        OnSaved(null, new NotSupportedException(Resources.NoSaveChildException));
      else if (EditLevel > 0)
        OnSaved(null, new Validation.ValidationException(Resources.NoSaveEditingException));
      else if (!IsValid && !IsDeleted)
        OnSaved(null, new Validation.ValidationException(Resources.NoSaveInvalidException));
      else
      {
        if (IsDirty)
        {
          MarkBusy();
          DataPortal.BeginUpdate<T>(this, (o, e) =>
          {
            T result = e.Object;
            OnSaved(result, e.Error);
          });
        }
        else
        {
          OnSaved((T)this, null);
        }
      }
    }

    public virtual void BeginSave(EventHandler<SavedEventArgs> handler)
    {
      if (this.IsChild)
      {
        NotSupportedException error = new NotSupportedException(Resources.NoSaveChildException);
        OnSaved(null, error);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error));
      }
      else if (EditLevel > 0)
      {
        Validation.ValidationException error = new Validation.ValidationException(Resources.NoSaveEditingException);
        OnSaved(null, error);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error));
      }
      else if (!IsValid && !IsDeleted)
      {
        Validation.ValidationException error = new Validation.ValidationException(Resources.NoSaveEditingException);
        OnSaved(null, error);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error));
      }
      else
      {
        if (IsDirty)
        {
          DataPortal.BeginUpdate<T>(this, (o, e) =>
          {
            T result = e.Object;
            OnSaved(result, e.Error);
            if (handler != null)
              handler(result, new SavedEventArgs(result, e.Error));
          });
        }
        else
        {
          OnSaved((T)this, null);
          if (handler != null)
            handler(this, new SavedEventArgs(this, null));
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
      if (forceUpdate && IsNew)
      {
        // mark the object as old - which makes it
        // not dirty
        MarkOld();
        // now mark the object as dirty so it can save
        MarkDirty(true);
      }
      this.BeginSave();
    }

    public void BeginSave(bool forceUpdate, EventHandler<SavedEventArgs> handler)
    {
      if (forceUpdate && IsNew)
      {
        // mark the object as old - which makes it
        // not dirty
        MarkOld();
        // now mark the object as dirty so it can save
        MarkDirty(true);
      }
      this.BeginSave(handler);
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

    #endregion
  }
}
