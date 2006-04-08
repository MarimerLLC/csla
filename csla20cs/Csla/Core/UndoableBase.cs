using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

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

    protected UndoableBase()
    {

    }

    /// <summary>
    /// Returns the current edit level of the object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int EditLevel
    {
      get { return _stateStack.Count; }
    }

    void IUndoableObject.CopyState()
    {
      CopyState();
    }

    void IUndoableObject.UndoChanges()
    {
      UndoChanges();
    }

    void IUndoableObject.AcceptChanges()
    {
      AcceptChanges();
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
    protected internal void CopyState()
    {
      Type currentType = this.GetType();
      HybridDictionary state = new HybridDictionary();
      FieldInfo[] fields;
      string fieldName;

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
                if (value != null)
                {
                  // this is a child object, cascade the call
                  ((Core.IUndoableObject)value).CopyState();
                }
              }
              else
              {
                // this is a normal field, simply trap the value
                fieldName = field.DeclaringType.Name + "!" + field.Name;
                state.Add(fieldName, value);
              }
            }
          }
        }
        currentType = currentType.BaseType;
      } while (currentType != typeof(UndoableBase));

      // serialize the state and stack it
      using (MemoryStream buffer = new MemoryStream())
      {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(buffer, state);
        _stateStack.Push(buffer.ToArray());
      }
      CopyStateComplete();
    }

    /// <summary>
    /// This method is invoked after the UndoChanges
    /// operation is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void UndoChangesComplete()
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
    protected internal void UndoChanges()
    {
      // if we are a child object we might be asked to
      // undo below the level where stacked states,
      // so just do nothing in that case
      if (EditLevel > 0)
      {
        HybridDictionary state;
        using (MemoryStream buffer = new MemoryStream(_stateStack.Pop()))
        {
          buffer.Position = 0;
          BinaryFormatter formatter = new BinaryFormatter();
          state = (HybridDictionary)formatter.Deserialize(buffer);
        }

        Type currentType = this.GetType();
        FieldInfo[] fields;
        string fieldName;

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
                  // make sure the variable has a value
                  if (value != null)
                  {
                    // this is a child object, cascade the call.
                    ((Core.IUndoableObject)value).UndoChanges();
                  }
                }
                else
                {
                  // this is a regular field, restore its value
                  fieldName = field.DeclaringType.Name + "!" + field.Name;
                  field.SetValue(this, state[fieldName]);
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
    protected internal void AcceptChanges()
    {
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
                    ((Core.IUndoableObject)value).AcceptChanges();
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

    #endregion

  }
}