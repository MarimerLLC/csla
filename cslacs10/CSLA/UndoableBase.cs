using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CSLA.Core
{
  /// <summary>
  /// Implements n-level undo capabilities.
  /// </summary>
  /// <remarks>
  /// You should not directly derive from this class. Your
  /// business classes should derive from 
  /// <see cref="CSLA.BusinessBase" />.
  /// </remarks>
  [Serializable()]
  abstract public class UndoableBase : CSLA.Core.BindableBase
  {
    // keep a stack of object state values
    [NotUndoable()]
    Stack _StateStack = new Stack();

    // variables containing type info for comparisons
    static Type UndoableType = typeof(UndoableBase);
    static Type BusinessType = typeof(BusinessBase);
    static Type CollectionType = typeof(BusinessCollectionBase);

    /// <summary>
    /// Returns the current edit level of the object.
    /// </summary>
    protected int EditLevel
    {
      get
      {
        return _StateStack.Count;
      }
    }

    /// <summary>
    /// Copies the state of the object and places the copy
    /// onto the state stack.
    /// </summary>
    protected internal void CopyState()
    {
      Type currentType = this.GetType();
      Hashtable state = new Hashtable();
      FieldInfo[] fields;
      string fieldName;

      do
      {
        // get the list of fields in this type
        fields = currentType.GetFields( 
          BindingFlags.NonPublic | 
          BindingFlags.Instance | 
          BindingFlags.Public);

        foreach(FieldInfo field in fields)
        {
          // make sure we process only our variables
          if(field.DeclaringType == currentType)
          {
            // see if this field is marked as not undoable
            if(!NotUndoableField(field))
            {
              // the field is undoable, so it needs to be processed
              Object value = field.GetValue(this);

              if(TypeInheritsFrom(field.FieldType, CollectionType))
              {
                // make sure the variable has a value
                if(!(value == null))
                {
                  // this is a child collection, cascade the call
                  BusinessCollectionBase tmp = (BusinessCollectionBase)value;
                  tmp.CopyState();
                }
              }
              else
              {
                if(TypeInheritsFrom(field.FieldType, BusinessType))
                {
                  // make sure the variable has a value
                  if(!(value == null))
                  {
                    // this is a child object, cascade the call
                    BusinessBase tmp = (BusinessBase)value;
                    tmp.CopyState();
                  }
                }
                else
                {
                  // this is a normal field, simply trap the value
                  fieldName = field.DeclaringType.Name + @"!" + field.Name;
                  state.Add(fieldName, value);
                }
              }
            }
          }
        }
        currentType = currentType.BaseType;
      } while(currentType != UndoableType);

      // serialize the state and stack it
      MemoryStream buffer = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(buffer, state);
      _StateStack.Push(buffer.ToArray());
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
    protected internal void UndoChanges()
    {
      // if we are a child object we might be asked to
      // undo below the level where we stacked states,
      // so just do nothing in that case
      if(EditLevel > 0)
      {
        MemoryStream buffer = new MemoryStream((byte[])_StateStack.Pop());
        buffer.Position = 0;
        BinaryFormatter formatter = new BinaryFormatter();
        Hashtable state = (Hashtable)(formatter.Deserialize(buffer));

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

          foreach(FieldInfo field in fields)
          {
            if(field.DeclaringType == currentType)
            {
              // see if the field is undoable or not
              if(!NotUndoableField(field))
              {
                // the field is undoable, so restore its value
                if(TypeInheritsFrom(field.FieldType, CollectionType))
                {
                  // this is a child collection, cascade the call
                  BusinessCollectionBase tmp = (BusinessCollectionBase)field.GetValue(this);
                  tmp.UndoChanges();
                }
                else
                {
                  if(TypeInheritsFrom(field.FieldType, BusinessType))
                  {
                    // this is a child object, cascade the call
                    BusinessBase tmp = (BusinessBase)field.GetValue(this);
                    tmp.UndoChanges();
                  }
                  else
                  {
                    // this is a regular field, restore its value
                    fieldName = field.DeclaringType.Name + @"!" + field.Name;
                    field.SetValue(this, state[fieldName]);
                  }
                }
              }
            }
          }
          currentType = currentType.BaseType;
        } while(currentType != UndoableType);
      }
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
    protected internal void AcceptChanges()
    {
      if(EditLevel > 0)
      {
        _StateStack.Pop();

        Type currentType = this.GetType();
        FieldInfo[] fields;

        do
        {
          // get the list of fields in this type
          fields = currentType.GetFields( 
            BindingFlags.NonPublic | 
            BindingFlags.Instance | 
            BindingFlags.Public);

          foreach(FieldInfo field in fields)
          {
            if(field.DeclaringType == currentType)
            {
              // see if the field is undoable or not
              if(!NotUndoableField(field))
              {
                // the field is undoable so see if it is a collection
                if(TypeInheritsFrom(field.FieldType, CollectionType))
                {
                  // it is a collection so cascade the call
                  BusinessCollectionBase tmp = (BusinessCollectionBase)field.GetValue(this);
                  tmp.AcceptChanges();
                }
                else
                {
                  if(TypeInheritsFrom(field.FieldType, BusinessType))
                  {
                    // it is a child object so cascade the call
                    BusinessBase tmp = (BusinessBase)field.GetValue(this);
                    tmp.AcceptChanges();
                  }
                }
              }
            }
          currentType = currentType.BaseType;
          }
        } while(currentType != UndoableType);
      }
   }

    #region Helper Functions

    private bool NotUndoableField(FieldInfo field)
    {
      return Attribute.IsDefined(field, typeof(NotUndoableAttribute));
    }

    private bool TypeInheritsFrom(Type typeToCheck, Type checkAgainst)
    {
      Type baseType = typeToCheck;

      //   scan up through the inheritance hierarchy, checking each
      // class to see if it is the one we're looking for
      while(!(baseType == null))
      {
        // if we find the target class return True
        if(baseType == checkAgainst) return true;
        baseType = baseType.BaseType;
      }

      // the target class is not in the inheritance hierarchy so
      // return false
      return false;
    }

    #endregion

  }
}