using System;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using System.ComponentModel;
using System.Collections.Generic;
using Csla.Properties;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Csla.Core
{
#if TESTING
  [DebuggerNonUserCode]
#endif
  [Serializable]
  public class UndoableBase : BindableBase, IUndoableObject
  {
    // keep a stack of object state values.
    [NotUndoable()]
    private Stack<SerializationInfo> _stateStack = new Stack<SerializationInfo>();
    [NotUndoable]
    private bool _bindingEdit;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected UndoableBase()
    {

    }

    /// <summary>
    /// Gets or sets a value indicating whether n-level undo
    /// was invoked through IEditableObject. FOR INTERNAL
    /// CSLA .NET USE ONLY!
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected bool BindingEdit
    {
      get
      {
        return _bindingEdit;
      }
      set
      {
        _bindingEdit = value;
      }
    }

    int IUndoableObject.EditLevel
    {
      get { return EditLevel; }
    }

    /// <summary>
    /// Returns the current edit level of the object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int EditLevel
    {
      get { return _stateStack.Count; }
    }

    void IUndoableObject.CopyState(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        CopyState(parentEditLevel);
    }

    void IUndoableObject.UndoChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        UndoChanges(parentEditLevel);
    }

    void IUndoableObject.AcceptChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        AcceptChanges(parentEditLevel);
    }

    /// <summary>
    /// This method is invoked before the CopyState
    /// operation begins.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void CopyingState()
    {
    }

    /// <summary>
    /// This method is invoked after the CopyState
    /// operation is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void CopyStateComplete()
    {
    }

    /// <summary>
    /// Copies the state of the object and places the copy
    /// onto the state stack.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void CopyState(int parentEditLevel)
    {
      CopyingState();

      if (this.EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"));

      SerializationInfo state = new SerializationInfo(0);
      OnCopyState(state);
      _stateStack.Push(state);
      
      CopyStateComplete();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnCopyState(SerializationInfo state)
    {
    }

    /// <summary>
    /// This method is invoked before the UndoChanges
    /// operation begins.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void UndoChangesComplete()
    {
    }

    /// <summary>
    /// This method is invoked after the UndoChanges
    /// operation is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void UndoingChanges()
    {
    }

    /// <summary>
    /// Restores the object's state to the most recently
    /// copied values from the state stack.
    /// </summary>
    /// <remarks>
    /// Restores the state of the object to its
    /// previous value by taking the data out of
    /// the stack and restoring it into the fields
    /// of the object.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void UndoChanges(int parentEditLevel)
    {
      UndoingChanges();

      if (parentEditLevel < 0 || this.EditLevel - 1 < parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"));

      SerializationInfo state = _stateStack.Pop();
      OnUndoChanges(state);
      
      UndoChangesComplete();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnUndoChanges(SerializationInfo state)
    { }

    /// <summary>
    /// This method is invoked before the AcceptChanges
    /// operation begins.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void AcceptingChanges()
    {
    }

    /// <summary>
    /// This method is invoked after the AcceptChanges
    /// operation is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void AcceptChangesComplete()
    {
    }

    /// <summary>
    /// Accepts any changes made to the object since the last
    /// state copy was made.
    /// </summary>
    /// <remarks>
    /// The most recent state copy is removed from the state
    /// stack and discarded, thus committing any changes made
    /// to the object's state.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void AcceptChanges(int parentEditLevel)
    {
      AcceptingChanges();

      if (this.EditLevel - 1 < parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"));

      if (EditLevel > 0)
        _stateStack.Pop();
      
      AcceptChangesComplete();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnAcceptChanges()
    {
    }

    #region Helper Functions

    private static bool NotUndoableField(FieldInfo field)
    {
      return Attribute.IsDefined(field, typeof(NotUndoableAttribute));
    }

    private static string GetFieldName(FieldInfo field)
    {
      return field.DeclaringType.FullName + "!" + field.Name;
    }

    #endregion

    #region  Reset child edit level

    internal static void ResetChildEditLevel(IUndoableObject child, int parentEditLevel, bool bindingEdit)
    {
      int targetLevel = parentEditLevel;
      if (bindingEdit && targetLevel > 0 && !(child is FieldManager.FieldDataManager))
        targetLevel--;
      // if item's edit level is too high,
      // reduce it to match list
      while (child.EditLevel > targetLevel)
        child.AcceptChanges(targetLevel, false);
      // if item's edit level is too low,
      // increase it to match list
      while (child.EditLevel < targetLevel)
        child.CopyState(targetLevel, false);
    }

    #endregion
  }
}
