using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using Csla.Serialization;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Manages properties and property data for
  /// a business object.
  /// </summary>
  /// <remarks></remarks>
  [Serializable()]
  public class FieldDataManager : IUndoableObject
  {
    internal FieldDataManager()
    {
      // prevent creation from outside this assembly
    }

    #region  FieldData

    private FieldDataList mFields;

    private FieldDataList FieldData
    {
      get
      {
        if (mFields == null)
        {
          mFields = new FieldDataList();
        }
        return mFields;
      }
    }

    private bool HasFieldData
    {
      get
      {
        return mFields != null;
      }
    }

    #endregion

    #region  Get/Set/Find fields

    /// <summary>
    /// Gets the <see cref="IFieldData" /> object
    /// for a specific field.
    /// </summary>
    /// <param name="prop">
    /// The property corresponding to the field.
    /// </param>
    public IFieldData GetFieldData(IPropertyInfo prop)
    {
      return GetFieldData(prop.Name);
    }

    /// <summary>
    /// Gets the <see cref="IFieldData" /> object
    /// for a specific field.
    /// </summary>
    /// <param name="key">
    /// The property name corresponding to the field.
    /// </param>
    public IFieldData GetFieldData(string key)
    {
      if (FieldData.ContainsKey(key))
        return FieldData.GetValue(key);
      else
        return null;
    }

    internal string FindPropertyName(object value)
    {
      return FieldData.FindPropertyName(value);
    }

    /// <summary>
    /// Sets the value for a specific field.
    /// </summary>
    /// <typeparam name="P">
    /// Type of field value.
    /// </typeparam>
    /// <param name="prop">
    /// The property corresponding to the field.
    /// </param>
    /// <param name="value">
    /// Value to store for field.
    /// </param>
    public void SetFieldData<P>(IPropertyInfo prop, P value)
    {
      if (!(FieldData.ContainsKey(prop.Name)))
        FieldData.Add(prop.Name, prop.NewFieldData(prop.Name));

      var field = GetFieldData(prop);
      var fd = field as IFieldData<P>;
      if (fd != null)
        fd.Value = value;
      else
        field.Value = value;
    }

    /// <summary>
    /// Sets the value for a specific field without
    /// marking the field as dirty.
    /// </summary>
    /// <typeparam name="P">
    /// Type of field value.
    /// </typeparam>
    /// <param name="prop">
    /// The property corresponding to the field.
    /// </param>
    /// <param name="value">
    /// Value to store for field.
    /// </param>
    public IFieldData LoadFieldData<P>(IPropertyInfo prop, P value)
    {
      IFieldData field = null;
      if (!(FieldData.ContainsKey(prop.Name)))
      {
        field = prop.NewFieldData(prop.Name);
        FieldData.Add(prop.Name, field);
      }
      else
      {
        field = GetFieldData(prop);
      }
      var fd = field as IFieldData<P>;
      if (fd != null)
        fd.Value = value;
      else
        field.Value = value;
      field.MarkClean();
      return field;
    }

    /// <summary>
    /// Removes the value for a specific field.
    /// The <see cref="IFieldData" /> object is
    /// not removed, only the contained field value.
    /// </summary>
    /// <param name="propertyName">
    /// The property name corresponding to the field.
    /// </param>
    public void RemoveField(string propertyName)
    {
      if (FieldData.ContainsKey(propertyName))
        GetFieldData(propertyName).Value = null;
    }

    /// <summary>
    /// Returns a value indicating whether an
    /// <see cref="IFieldData" /> entry exists
    /// for the specified property.
    /// </summary>
    /// <param name="propertyInfo">
    /// The property corresponding to the field.
    /// </param>
    public bool FieldExists(IPropertyInfo propertyInfo)
    {
      return FieldData.ContainsKey(propertyInfo.Name);
    }

    /// <summary>
    /// Returns a list of all child objects
    /// contained in the list of fields.
    /// </summary>
    /// <remarks>
    /// This method returns a list of actual child
    /// objects, not a list of
    /// <see cref="IFieldData" /> container objects.
    /// </remarks>
    public List<object> GetChildren()
    {
      List<object> result = new List<object>();
      if (HasFieldData)
      {
        foreach (var item in FieldData.GetFieldDataList())
          if (item.Value is IEditableBusinessObject || item.Value is IEditableCollection)
            result.Add(item.Value);
      }
      return result;
    }

    #endregion

    #region  IsValid/IsDirty

    /// <summary>
    /// Returns a value indicating whether all
    /// fields are valid.
    /// </summary>
    public bool IsValid()
    {
      if (HasFieldData)
      {
        foreach (var item in FieldData.GetFieldDataList())
          if (item != null && !item.IsValid)
            return false;
      }
      return true;
    }

    /// <summary>
    /// Returns a value indicating whether any
    /// fields are dirty.
    /// </summary>
    public bool IsDirty()
    {
      if (HasFieldData)
      {
        foreach (var item in FieldData.GetFieldDataList())
          if (item != null && item.IsDirty)
            return true;
      }
      return false;
    }

    /// <summary>
    /// Marks all fields as clean
    /// (not dirty).
    /// </summary>
    public void MarkClean()
    {
      if (HasFieldData)
      {
        foreach (var item in FieldData.GetFieldDataList())
          if (item != null && item.IsDirty)
            item.MarkClean();
      }
    }

    #endregion

    #region  IUndoableObject

    private Stack<byte[]> mStateStack = new Stack<byte[]>();

    /// <summary>
    /// Gets the current edit level of the object.
    /// </summary>
    public int EditLevel
    {
      get
      {
        return mStateStack.Count;
      }
    }

    void IUndoableObject.CopyState(int parentEditLevel)
    {

      if (this.EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "CopyState"));

      HybridDictionary state = new HybridDictionary();

      if (HasFieldData)
      {
        foreach (var item in FieldData.GetFieldDataList())
        {
          var child = item.Value as IUndoableObject;
          if (child != null)
          {
            // cascade call to child
            child.CopyState(parentEditLevel);
            // store fact that child exists
            state.Add(item.Name, true);

          }
          else
          {
            // add the IFieldData object
            state.Add(item.Name, item);
          }
        }
      }

      // serialize the state and stack it
      using (MemoryStream buffer = new MemoryStream())
      {
        var formatter = SerializationFormatterFactory.GetFormatter();
        formatter.Serialize(buffer, state);
        mStateStack.Push(buffer.ToArray());
      }
    }

    void IUndoableObject.UndoChanges(int parentEditLevel)
    {
      if (EditLevel > 0)
      {
        if (this.EditLevel - 1 < parentEditLevel)
          throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "UndoChanges"));

        if (HasFieldData)
        {
          HybridDictionary state = null;
          using (MemoryStream buffer = new MemoryStream(mStateStack.Pop()))
          {
            buffer.Position = 0;
            var formatter = SerializationFormatterFactory.GetFormatter();
            state = (HybridDictionary)(formatter.Deserialize(buffer));
          }

          var oldFields = FieldData;
          mFields = new FieldDataList();

          foreach (DictionaryEntry item in state)
          {
            var key = System.Convert.ToString(item.Key);
            if (item.Value is bool)
            {
              // get child object from old field collection
              var child = (IFieldData)(oldFields.GetValue(key));
              // add to new list
              FieldData.Add(key, child);
              // cascade call to child
              ((IUndoableObject)child.Value).UndoChanges(parentEditLevel);
            }
            else
            {
              // restore IFieldData object into field collection
              FieldData.Add(key, (IFieldData)item.Value);
            }
          }
        }
      }
      else
      {
        mStateStack.Pop();
      }
    }

    void IUndoableObject.AcceptChanges(int parentEditLevel)
    {
      if (this.EditLevel - 1 < parentEditLevel)
        throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "AcceptChanges"));

      if (EditLevel > 0)
      {
        // discard latest recorded state
        mStateStack.Pop();

        if (HasFieldData)
        {
          foreach (var item in FieldData.GetFieldDataList())
          {
            var child = item.Value as IUndoableObject;
            if (child != null)
            {
              // cascade call to child
              child.AcceptChanges(parentEditLevel);
            }
          }
        }
      }
    }

    #endregion

    #region  Update Children

    /// <summary>
    /// Invokes the data portal to update
    /// all child objects contained in 
    /// the list of fields.
    /// </summary>
    public void UpdateChildren(params object[] parameters)
    {
      if (HasFieldData)
      {
        foreach (var item in FieldData.GetFieldDataList())
        {
          if (item != null)
          {
            object obj = item.Value;
            if (obj is IEditableBusinessObject || obj is IEditableCollection)
              Csla.DataPortal.UpdateChild(obj, parameters);
          }
        }
      }
    }

    #endregion

  }
}
