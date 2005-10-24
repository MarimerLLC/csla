using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace Csla.Core
{
    /// <summary>
    /// Implements n-level undo capabilities
    /// </summary>
    /// <remarks>
    /// You should not directly derive from this class. Your
    /// business classes should derive from
    /// <see cref="Csla.BusinessBase" />.
    /// </remarks>
    [Serializable()]
    public abstract class UndoableBase : Csla.Core.BindableBase, Csla.Core.IEditableObject
    {
        // keep a stack of object state values.
        [NotUndoable()]
        private Stack _stateStack = new Stack();

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

        void IEditableObject.CopyState()
        {
          CopyState();
        }

        void IEditableObject.UndoChanges()
        {
          UndoChanges();
        }

        void IEditableObject.AcceptChanges()
        {
          AcceptChanges();
        }

        /// <summary>
        /// Copies the state of the object and places the copy
        /// onto the state stack.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
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

                            if (field.FieldType.GetInterface("Csla.Core.IEditableObject") != null)
                            {
                                // make sure the variable has a value
                                if (value != null)
                                {
                                    // this is a child object, cascade the call
                                    ((Core.IEditableObject)value).CopyState();
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
                using (MemoryStream buffer = new MemoryStream((byte[])_stateStack.Pop()))
                {
                    buffer.Position = 0;
                    BinaryFormatter formatter = new BinaryFormatter();
                    Hashtable state = (Hashtable)formatter.Deserialize(buffer);
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
                            if (field.DeclaringType == currentType)
                            {
                                // see if the field is undoable or not
                                if (!NotUndoableField(field))
                                {
                                    // the field is undoable, so restore its value
                                    object value = field.GetValue(this);

                                    if (field.FieldType.GetInterface("Csla.Core.IEditableObject") != null)
                                    {
                                        // make sure the variable has a value
                                        if (value != null)
                                        {
                                            // this is a child object, cascade the call.
                                            ((Core.IEditableObject)value).UndoChanges();
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
                        // see if the field is undoable or not
                        if (!NotUndoableField(field))
                        {
                            // the field is undoable so see if it is a child object
                            if (field.FieldType.GetInterface("Csla.Core.IEditableObject") != null)
                            {
                                object value = field.GetValue(this);
                                // make sure the variable has a value
                                if (value != null)
                                {
                                    // it is a child object so cascade the call
                                    ((Core.IEditableObject)value).AcceptChanges();
                                }
                            }
                        }
                    }
                    currentType = currentType.BaseType;
                } while (currentType != typeof(UndoableBase));
            }
        }

        #region Helper Functions

        private static bool NotUndoableField(FieldInfo field)
        {
            return Attribute.IsDefined(field, typeof(NotUndoableAttribute));
        }

        #endregion

    }
}