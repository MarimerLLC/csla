//-----------------------------------------------------------------------
// <copyright file="UndoableBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements n-level undo capabilities as</summary>
//-----------------------------------------------------------------------
#if NETFX_CORE || IOS || ANDROID
using System;
using Csla.Serialization.Mobile;
using System.ComponentModel;
using System.Collections.Generic;
using Csla.Properties;
using System.Reflection;

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
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"), this.GetType().Name, null, this.EditLevel, parentEditLevel - 1);

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
        throw new Core.UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"), this.GetType().Name, null, this.EditLevel, parentEditLevel + 1);

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
        throw new Core.UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"), this.GetType().Name, null, this.EditLevel, parentEditLevel + 1);

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
#else
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using Csla.Properties;
using Csla.Reflection;
using Csla.Serialization.Mobile;
using Csla.Serialization;

namespace Csla.Core
{
  /// <summary>
  /// Implements n-level undo capabilities as
  /// described in Chapters 2 and 3.
  /// </summary>
  [Serializable()]
  public abstract class UndoableBase : Csla.Core.BindableBase,
    Csla.Core.IUndoableObject
  {
    // keep a stack of object state values.
    [NotUndoable()]
    private Stack<byte[]> _stateStack = new Stack<byte[]>();
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

      Type currentType = this.GetType();
      HybridDictionary state = new HybridDictionary();

      if (this.EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"), this.GetType().Name, null, this.EditLevel, parentEditLevel - 1);

      do
      {
        var currentTypeName = currentType.FullName;
        // get the list of fields in this type
        List<DynamicMemberHandle> handlers = 
          UndoableHandler.GetCachedFieldHandlers(currentType);
        foreach (var h in handlers)
        {
          var value = h.DynamicMemberGet(this);
          if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(h.MemberType))
          {
            // make sure the variable has a value
            if (value == null)
            {
              // variable has no value - store that fact
              state.Add(GetFieldName(currentTypeName, h.MemberName), null);
            }
            else
            {
              // this is a child object, cascade the call
              ((Core.IUndoableObject)value).CopyState(this.EditLevel + 1, BindingEdit);
            }
          }
          else
          {
            // this is a normal field, simply trap the value
            state.Add(GetFieldName(currentTypeName, h.MemberName), value);
          }
        }

        currentType = currentType.BaseType;
      } while (currentType != typeof(UndoableBase));

      // serialize the state and stack it
      using (MemoryStream buffer = new MemoryStream())
      {
        ISerializationFormatter formatter =
          SerializationFormatterFactory.GetFormatter();
        formatter.Serialize(buffer, state);
        _stateStack.Push(buffer.ToArray());
      }
      CopyStateComplete();
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

      // if we are a child object we might be asked to
      // undo below the level of stacked states,
      // so just do nothing in that case
      if (EditLevel > 0)
      {
        if (this.EditLevel - 1 != parentEditLevel)
          throw new UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"), this.GetType().Name, null, this.EditLevel, parentEditLevel + 1);

        HybridDictionary state;
        using (MemoryStream buffer = new MemoryStream(_stateStack.Pop()))
        {
          buffer.Position = 0;
          ISerializationFormatter formatter =
            SerializationFormatterFactory.GetFormatter();
          state = (HybridDictionary)formatter.Deserialize(buffer);
        }

        Type currentType = this.GetType();

        do
        {
          var currentTypeName = currentType.FullName;

          // get the list of fields in this type
          List<DynamicMemberHandle> handlers = UndoableHandler.GetCachedFieldHandlers(currentType);
          foreach (var h in handlers)
          {
            // the field is undoable, so restore its value
            var value = h.DynamicMemberGet(this);

            if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(h.MemberType))
            {
              // this is a child object
              // see if the previous value was empty
              //if (state.Contains(h.MemberName))
              if (state.Contains(GetFieldName(currentTypeName, h.MemberName)))
              {
                // previous value was empty - restore to empty
                h.DynamicMemberSet(this, null);
              }
              else
              {
                // make sure the variable has a value
                if (value != null)
                {
                  // this is a child object, cascade the call.
                  ((Core.IUndoableObject)value).UndoChanges(this.EditLevel,
                                                             BindingEdit);
                }
              }
            }
            else
            {
              // this is a regular field, restore its value
              h.DynamicMemberSet(this, state[GetFieldName(currentTypeName, h.MemberName)]);
              //h.DynamicMemberSet(this, state[h.MemberName]);
            }
          }

          currentType = currentType.BaseType;
        } while (currentType != typeof(UndoableBase));
      }
      UndoChangesComplete();
    }

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

      if (this.EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"), this.GetType().Name, null, this.EditLevel, parentEditLevel + 1);

      if (EditLevel > 0)
      {
        _stateStack.Pop();
        Type currentType = this.GetType();

        do
        {
          // get the list of fields in this type
          List<DynamicMemberHandle> handlers = UndoableHandler.GetCachedFieldHandlers(currentType);
          foreach (var h in handlers)
          {
            // the field is undoable so see if it is a child object
            if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(h.MemberType))
            {
              object value = h.DynamicMemberGet(this);
              // make sure the variable has a value
              if (value != null)
              {
                // it is a child object so cascade the call
                ((Core.IUndoableObject)value).AcceptChanges(this.EditLevel, BindingEdit);
              }
            }
          }

          currentType = currentType.BaseType;
        } while (currentType != typeof(UndoableBase));
      }
      AcceptChangesComplete();
    }

    /// <summary>
    /// Returns the full name of a field, including
    /// the containing type name.
    /// </summary>
    /// <param name="typeName">Name of the containing type.</param>
    /// <param name="memberName">Name of the member (field).</param>
    private static string GetFieldName(string typeName, string memberName)
    {
      return typeName + "." + memberName;
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

    #region MobileObject overrides

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      info.AddValue("_bindingEdit", _bindingEdit);
      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      _stateStack.Clear();

      _bindingEdit = info.GetValue<bool>("_bindingEdit");
      base.OnSetState(info, mode);
    }

    #endregion
  }
}
#endif