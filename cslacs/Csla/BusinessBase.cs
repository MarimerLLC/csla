using System;
using System.ComponentModel;
using Csla.Properties;

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
  /// Please refer to 'Expert C# 2005 Business Objects' for
  /// full details on the use of this base class to create business
  /// objects.
  /// </para>
  /// </remarks>
  /// <typeparam name="T">Type of the business object being defined.</typeparam>
  [Serializable()]
  public abstract class BusinessBase<T> :
    Core.BusinessBase, Core.ISavable where T : BusinessBase<T>
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
    public virtual T Save()
    {
      T result;
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoSaveChildException);
      if (EditLevel > 0)
        throw new Validation.ValidationException(Resources.NoSaveEditingException);
      if (!IsValid && !IsDeleted)
        throw new Validation.ValidationException(Resources.NoSaveInvalidException);
      if (IsDirty)
        result = (T)DataPortal.Update(this);
      else
        result = (T)this;
      OnSaved(result);
      return result;
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

    #endregion

    #region ISavable Members

    object Csla.Core.ISavable.Save()
    {
      return Save();
    }

    void Csla.Core.ISavable.SaveComplete(object newObject)
    {
      OnSaved((T)newObject);
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

    /// <summary>
    /// Raises the Saved event, indicating that the
    /// object has been saved, and providing a reference
    /// to the new object instance.
    /// </summary>
    /// <param name="newObject">The new object instance.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnSaved(T newObject)
    {
      Csla.Core.SavedEventArgs args = new Csla.Core.SavedEventArgs(newObject);
      if (_nonSerializableSavedHandlers != null)
        _nonSerializableSavedHandlers.Invoke(this, args);
      if (_serializableSavedHandlers != null)
        _serializableSavedHandlers.Invoke(this, args);
    }

    #endregion
  }
}
