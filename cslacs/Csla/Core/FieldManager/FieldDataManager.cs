using System;
using System.IO;
using System.Collections.Generic;
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

    [NonSerialized()]
    private List<IPropertyInfo> mPropertyList;
    private IFieldData[] mFieldData;

    internal FieldDataManager(List<IPropertyInfo> propertyList)
    {
      mPropertyList = propertyList;
      mFieldData = new IFieldData[mPropertyList.Count];
    }

    /// <summary>
    /// Called when parent object is deserialized to
    /// restore property list.
    /// </summary>
    internal void SetPropertyList(List<IPropertyInfo> propertyList)
    {
      mPropertyList = propertyList;
    }

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
      return mFieldData[prop.Index];
    }

    internal IPropertyInfo FindProperty(object value)
    {
      var index = 0;
      foreach (var item in mFieldData)
      {
        if (item != null && item.Value.Equals(value))
        {
          return mPropertyList[index];
        }
        index += 1;
      }
      return null;
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
      var field = mFieldData[prop.Index];
      if (field == null)
      {
        field = prop.NewFieldData(prop.Name);
        mFieldData[prop.Index] = field;
      }
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
      var field = mFieldData[prop.Index];
      if (field == null)
      {
        field = prop.NewFieldData(prop.Name);
        mFieldData[prop.Index] = field;
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
    /// <param name="prop">
    /// The property corresponding to the field.
    /// </param>
    public void RemoveField(IPropertyInfo prop)
    {
      var field = mFieldData[prop.Index];
      if (field != null)
        field.Value = null;
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
      return mFieldData[propertyInfo.Index] != null;
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
      foreach (var item in mFieldData)
        if (item != null && (item.Value is IEditableBusinessObject || item.Value is IEditableCollection))
          result.Add(item.Value);
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
      foreach (var item in mFieldData)
        if (item != null && !item.IsValid)
          return false;
      return true;
    }

    /// <summary>
    /// Returns a value indicating whether any
    /// fields are dirty.
    /// </summary>
    public bool IsDirty()
    {
      foreach (var item in mFieldData)
        if (item != null && item.IsDirty)
          return true;
      return false;
    }

    /// <summary>
    /// Marks all fields as clean
    /// (not dirty).
    /// </summary>
    public void MarkClean()
    {
      foreach (var item in mFieldData)
        if (item != null && item.IsDirty)
          item.MarkClean();
    }

    #endregion

    #region  IUndoableObject

    private Stack<byte[]> mStateStack = new Stack<byte[]>();

    /// <summary>
    /// Gets the current edit level of the object.
    /// </summary>
    public int EditLevel
    {
      get { return mStateStack.Count; }
    }

    void Core.IUndoableObject.CopyState(int parentEditLevel)
    {
      if (this.EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "CopyState"));

      IFieldData[] state = new IFieldData[mPropertyList.Count];

      for (var index = 0; index < mFieldData.Length; index++)
      {
        var item = mFieldData[index];
        if (item != null)
        {
          var child = item.Value as IUndoableObject;
          if (child != null)
          {
            // cascade call to child
            child.CopyState(parentEditLevel);
          }
          else
          {
            // add the IFieldData object
            state[index] = item;
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

    void Core.IUndoableObject.UndoChanges(int parentEditLevel)
    {
      if (EditLevel > 0)
      {
        if (this.EditLevel - 1 < parentEditLevel)
          throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "UndoChanges"));

        IFieldData[] state = null;
        using (MemoryStream buffer = new MemoryStream(mStateStack.Pop()))
        {
          buffer.Position = 0;
          var formatter = SerializationFormatterFactory.GetFormatter();
          state = (IFieldData[])(formatter.Deserialize(buffer));
        }

        for (var index = 0; index < mFieldData.Length; index++)
        {
          var oldItem = state[index];
          var item = mFieldData[index];
          if (oldItem == null && item != null)
          {
            // potential child object
            var child = item.Value as IUndoableObject;
            if (child != null)
            {
              child.UndoChanges(parentEditLevel);
            }
            else
            {
              // null value
              mFieldData[index] = null;
            }
          }
          else
          {
            // restore IFieldData object into field collection
            mFieldData[index] = state[index];
          }
        }
      }
    }

    void Core.IUndoableObject.AcceptChanges(int parentEditLevel)
    {
      if (this.EditLevel - 1 < parentEditLevel)
        throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "AcceptChanges"));

      if (EditLevel > 0)
      {
        // discard latest recorded state
        mStateStack.Pop();

        foreach (var item in mFieldData)
        {
          if (item != null)
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
      foreach (var item in mFieldData)
      {
        if (item != null)
        {
          object obj = item.Value;
          if (obj is IEditableBusinessObject || obj is IEditableCollection)
            Csla.DataPortal.UpdateChild(obj, parameters);
        }
      }
    }

    #endregion

  }

}