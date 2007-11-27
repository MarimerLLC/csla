using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Csla.Serialization;
using System.ComponentModel;
using Csla.Properties;

namespace Csla.Core
{
  /// <summary>
  /// Implements n-level undo capabilities as
  /// described in Chapters 2 and 3.
  /// </summary>
  [Serializable()]
  public abstract class UndoableBase : Csla.Core.BindableBase, Csla.Core.IUndoableObject
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

    /// <summary>
    /// Returns the current edit level of the object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int EditLevel
    {
      get { return _stateStack.Count; }
    }

    void IUndoableObject.CopyState(int parentEditLevel)
    {
      CopyState(parentEditLevel);
    }

    void IUndoableObject.UndoChanges(int parentEditLevel)
    {
      UndoChanges(parentEditLevel);
    }

    void IUndoableObject.AcceptChanges(int parentEditLevel)
    {
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
      FieldInfo[] fields;

      if (this.EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"));

      do
      {
        // get the list of fields in this type
        fields = currentType.GetFields(
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
          // make sure we process only our variables
          if (field.DeclaringType == currentType)
          {
            // see if this field is marked as not undoable
            if (!NotUndoableField(field))
            {
              // the field is undoable, so it needs to be processed.
              object value = field.GetValue(this);

              if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(field.FieldType))
              {
                // make sure the variable has a value
                if (value == null)
                {
                  // variable has no value - store that fact
                  state.Add(GetFieldName(field), null);
                }
                else
                {
                  // this is a child object, cascade the call
                  if (!_bindingEdit)
                    ((Core.IUndoableObject)value).CopyState(this.EditLevel + 1);
                }
              }
              else
              {
                // this is a normal field, simply trap the value
                state.Add(GetFieldName(field), value);
              }
            }
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
        if (this.EditLevel - 1 < parentEditLevel)
          throw new UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"));

        HybridDictionary state;
        using (MemoryStream buffer = new MemoryStream(_stateStack.Pop()))
        {
          buffer.Position = 0;
          ISerializationFormatter formatter =
            SerializationFormatterFactory.GetFormatter();
          state = (HybridDictionary)formatter.Deserialize(buffer);
        }

        Type currentType = this.GetType();
        FieldInfo[] fields;

        do
        {
          // get the list of fields in this type
          fields = currentType.GetFields(
              BindingFlags.NonPublic |
              BindingFlags.Instance |
              BindingFlags.Public);
          foreach (FieldInfo field in fields)
          {
            // make sure we process only our variables
            if (field.DeclaringType == currentType)
            {
              // see if the field is undoable or not
              if (!NotUndoableField(field))
              {
                // the field is undoable, so restore its value
                object value = field.GetValue(this);

                if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(field.FieldType))
                {
                  // this is a child object
                  // see if the previous value was empty
                  if (state.Contains(GetFieldName(field)))
                  {
                    // previous value was empty - restore to empty
                    field.SetValue(this, null);
                  }
                  else
                  {
                    // make sure the variable has a value
                    if (value != null)
                    {
                      // this is a child object, cascade the call.
                      if (!_bindingEdit)
                        ((Core.IUndoableObject)value).UndoChanges(this.EditLevel);
                    }
                  }
                }
                else
                {
                  // this is a regular field, restore its value
                  field.SetValue(this, state[GetFieldName(field)]);
                }
              }
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

      if (this.EditLevel - 1 < parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"));

      if (EditLevel > 0)
      {
        _stateStack.Pop();
        Type currentType = this.GetType();
        FieldInfo[] fields;

        do
        {
          // get the list of fields in this type
          fields = currentType.GetFields(
              BindingFlags.NonPublic |
              BindingFlags.Instance |
              BindingFlags.Public);
          foreach (FieldInfo field in fields)
          {
            // make sure we process only our variables
            if (field.DeclaringType == currentType)
            {
              // see if the field is undoable or not
              if (!NotUndoableField(field))
              {
                // the field is undoable so see if it is a child object
                if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(field.FieldType))
                {
                  object value = field.GetValue(this);
                  // make sure the variable has a value
                  if (value != null)
                  {
                    // it is a child object so cascade the call
                    if (!_bindingEdit)
                      ((Core.IUndoableObject)value).AcceptChanges(this.EditLevel);
                  }
                }
              }
            }
          }
          currentType = currentType.BaseType;
        } while (currentType != typeof(UndoableBase));
      }
      AcceptChangesComplete();
    }

    #region Helper Functions

    private static bool NotUndoableField(FieldInfo field)
    {
      return Attribute.IsDefined(field, typeof(NotUndoableAttribute));
    }

    private static string GetFieldName(FieldInfo field)
    {
      return field.DeclaringType.Name + "!" + field.Name;
    }

    #endregion

  }
}