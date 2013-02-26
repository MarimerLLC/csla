//-----------------------------------------------------------------------
// <copyright file="UndoableBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements n-level undo capabilities as</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using System.ComponentModel;
using System.Collections.Generic;
using Csla.Properties;
using System.Reflection;
using System.IO;
#if !__ANDROID__ && !IOS
using System.Runtime.Serialization;
#endif

namespace Csla.Core
{
  /// <summary>
  /// Implements n-level undo capabilities as
  /// described in Chapters 2 and 3.
  /// </summary>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
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
    /// <param name="parentEditLevel">Parent edit level.</param>
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

    /// <summary>
    /// Invoked when a subclass should copy its state
    /// onto the state stack.
    /// </summary>
    /// <param name="state">State stack.</param>
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
    /// <param name="parentEditLevel">Parent edit level</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void UndoChanges(int parentEditLevel)
    {
      UndoingChanges();

      if (this.EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"));

      if (parentEditLevel >= 0)
      {
        SerializationInfo state = _stateStack.Peek();
        OnUndoChanges(state);

        _stateStack.Pop();
        UndoChangesComplete();
      }
    }

    /// <summary>
    /// This method is invoked when an undo operation
    /// begins.
    /// </summary>
    /// <param name="state">Serialization state</param>
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
    /// <param name="parentEditLevel">Parent edit level</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void AcceptChanges(int parentEditLevel)
    {
      AcceptingChanges();

      if (this.EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"));

      if (EditLevel > 0)
        _stateStack.Pop();
      
      AcceptChangesComplete();
    }

    #region Helper Functions

    private static bool NotUndoableField(FieldInfo field)
    {
#if NETFX_CORE
      var attr = field.GetCustomAttribute(typeof(NotUndoableAttribute));
      return (attr != null);
#else
      return Attribute.IsDefined(field, typeof(NotUndoableAttribute));
#endif
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

    #region MobileObject overrides

    /// <summary>
    /// Gets the state of the object for serialization.
    /// </summary>
    /// <param name="info">Serialization state</param>
    /// <param name="mode">Serialization mode</param>
    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      if (mode == StateMode.Serialization)
      {
        if (_stateStack.Count > 0)
        {
          MobileList<SerializationInfo> list = new MobileList<SerializationInfo>(_stateStack.ToArray());
          byte[] xml = MobileFormatter.Serialize(list);
          info.AddValue("_stateStack", xml);
        }
      }

      info.AddValue("_bindingEdit", _bindingEdit);
      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Sets the state of the object from serialization.
    /// </summary>
    /// <param name="info">Serialization state</param>
    /// <param name="mode">Serialization mode</param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      if (mode == StateMode.Serialization)
      {
        _stateStack.Clear();

        if (info.Values.ContainsKey("_stateStack"))
        {
          //string xml = info.GetValue<string>("_stateStack");
          byte[] xml = info.GetValue<byte[]>("_stateStack");
          MobileList<SerializationInfo> list = (MobileList<SerializationInfo>)MobileFormatter.Deserialize(xml);
          SerializationInfo[] layers = list.ToArray();
          Array.Reverse(layers);
          foreach (SerializationInfo layer in layers)
            _stateStack.Push(layer);
        }
      }

      _bindingEdit = info.GetValue<bool>("_bindingEdit");
      base.OnSetState(info, mode);
    }

    #endregion
  }
}