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
    private List<IPropertyInfo> _propertyList;
    private IFieldData[] _fieldData;

    internal FieldDataManager(Type businessObjectType)
    {
      _propertyList = GetConsolidatedList(businessObjectType);
      _fieldData = new IFieldData[_propertyList.Count];
    }

    /// <summary>
    /// Called when parent object is deserialized to
    /// restore property list.
    /// </summary>
    internal void SetPropertyList(Type businessObjectType)
    {
      _propertyList = GetConsolidatedList(businessObjectType);
    }

    /// <summary>
    /// Returns a copy of the property list for
    /// the business object. Returns
    /// null if there are no properties registered
    /// for this object.
    /// </summary>
    public List<IPropertyInfo> GetRegisteredProperties()
    {
      return new List<IPropertyInfo>(_propertyList);
    }

    /// <summary>
    /// Gets a value indicating whether there
    /// are any managed fields available.
    /// </summary>
    public bool HasFields
    {
      get { return _propertyList.Count > 0; }
    }

    #region ConsolidatedPropertyList

    private static Dictionary<Type, List<IPropertyInfo>> _consolidatedLists = new Dictionary<Type, List<IPropertyInfo>>();

    private static List<IPropertyInfo> GetConsolidatedList(Type type)
    {
      List<IPropertyInfo> result = null;
      if (!_consolidatedLists.TryGetValue(type, out result))
      {
        lock (_consolidatedLists)
        {
          if (!_consolidatedLists.TryGetValue(type, out result))
          {
            result = CreateConsolidatedList(type);
            _consolidatedLists.Add(type, result);
          }
        }
      }
      return result;
    }

    private static List<IPropertyInfo> CreateConsolidatedList(Type type)
    {
      List<IPropertyInfo> result = new List<IPropertyInfo>();
      // get inheritance hierarchy
      Type current = type;
      List<Type> hierarchy = new List<Type>();
      do
      {
        hierarchy.Add(current);
        current = current.BaseType;
      } while (current != null && !current.Equals(typeof(BusinessBase)));
      // walk from top to bottom to build consolidated list
      for (int index = hierarchy.Count - 1; index >= 0; index--)
        result.AddRange(PropertyInfoManager.GetPropertyListCache(hierarchy[index]));
      // set Index properties on all unindexed PropertyInfo objects
      int max = -1;
      foreach (var item in result)
      {
        if (item.Index == -1)
        {
          max++;
          item.Index = max;
        }
        else
        {
          max = item.Index;
        }
      }
      // return consolidated list
      return result;
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
    internal IFieldData GetFieldData(IPropertyInfo prop)
    {
      try
      {
        return _fieldData[prop.Index];
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new InvalidOperationException("Property not registered", ex);
      }
    }

    private IFieldData GetOrCreateFieldData(IPropertyInfo prop)
    {
      try
      {
        var field = _fieldData[prop.Index];
        if (field == null)
        {
          field = prop.NewFieldData(prop.Name);
          _fieldData[prop.Index] = field;
        }
        return field;
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new InvalidOperationException("Property not registered", ex);
      }
    }

    internal IPropertyInfo FindProperty(object value)
    {
      var index = 0;
      foreach (var item in _fieldData)
      {
        if (item != null && item.Value != null && item.Value.Equals(value))
          return _propertyList[index];
        index += 1;
      }
      return null;
    }

    /// <summary>
    /// Sets the value for a specific field.
    /// </summary>
    /// <param name="prop">
    /// The property corresponding to the field.
    /// </param>
    /// <param name="value">
    /// Value to store for field.
    /// </param>
    internal void SetFieldData(IPropertyInfo prop, object value)
    {
      Type valueType;
      if (value != null)
        valueType = value.GetType();
      else
        valueType = prop.Type;
      value = Utilities.CoerceValue(prop.Type, valueType, null, value);
      var field = GetOrCreateFieldData(prop);
      field.Value = value;
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
    internal void SetFieldData<P>(IPropertyInfo prop, P value)
    {
      var field = GetOrCreateFieldData(prop);
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
    /// <param name="prop">
    /// The property corresponding to the field.
    /// </param>
    /// <param name="value">
    /// Value to store for field.
    /// </param>
    internal IFieldData LoadFieldData(IPropertyInfo prop, object value)
    {
      Type valueType;
      if (value != null)
        valueType = value.GetType();
      else
        valueType = prop.Type;
      value = Utilities.CoerceValue(prop.Type, valueType, null, value);
      var field = GetOrCreateFieldData(prop);
      field.Value = value;
      field.MarkClean();
      return field;
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
    internal IFieldData LoadFieldData<P>(IPropertyInfo prop, P value)
    {
      var field = GetOrCreateFieldData(prop);
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
    internal void RemoveField(IPropertyInfo prop)
    {
      try
      {
        var field = _fieldData[prop.Index];
        if (field != null)
          field.Value = null;
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new InvalidOperationException("Property not registered", ex);
      }
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
      try
      {
        return _fieldData[propertyInfo.Index] != null;
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new InvalidOperationException("Property not registered", ex);
      }
    }

    #endregion

    #region  IsValid/IsDirty

    /// <summary>
    /// Returns a value indicating whether all
    /// fields are valid.
    /// </summary>
    public bool IsValid()
    {
      foreach (var item in _fieldData)
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
      foreach (var item in _fieldData)
        if (item != null && item.IsDirty)
          return true;
      return false;
    }

    /// <summary>
    /// Marks all fields as clean
    /// (not dirty).
    /// </summary>
    internal void MarkClean()
    {
      foreach (var item in _fieldData)
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
        throw new UndoException(string.Format("Edit level mismatch in {0}", "CopyState"));

      IFieldData[] state = new IFieldData[_propertyList.Count];

      for (var index = 0; index < _fieldData.Length; index++)
      {
        var item = _fieldData[index];
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
        var formatter = new Csla.Serialization.Mobile.MobileFormatter();
        formatter.Serialize(buffer, state);
        mStateStack.Push(buffer.ToArray());
      }
    }

    void Core.IUndoableObject.UndoChanges(int parentEditLevel)
    {
      if (EditLevel > 0)
      {
        if (this.EditLevel - 1 < parentEditLevel)
          throw new UndoException(string.Format("Edit level mismatch in {0}", "UndoChanges"));

        IFieldData[] state = null;
        using (MemoryStream buffer = new MemoryStream(mStateStack.Pop()))
        {
          buffer.Position = 0;
          var formatter = new Csla.Serialization.Mobile.MobileFormatter();
          state = (IFieldData[])(formatter.Deserialize(buffer));
        }

        for (var index = 0; index < _fieldData.Length; index++)
        {
          var oldItem = state[index];
          var item = _fieldData[index];
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
              _fieldData[index] = null;
            }
          }
          else
          {
            // restore IFieldData object into field collection
            _fieldData[index] = state[index];
          }
        }
      }
    }

    void Core.IUndoableObject.AcceptChanges(int parentEditLevel)
    {
      if (this.EditLevel - 1 < parentEditLevel)
        throw new UndoException(string.Format("Edit level mismatch in {0}", "AcceptChanges"));

      if (EditLevel > 0)
      {
        // discard latest recorded state
        mStateStack.Pop();

        foreach (var item in _fieldData)
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

    #region  Child Objects 

    
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
      foreach (var item in _fieldData)
        if (item != null && (item.Value is IEditableBusinessObject || item.Value is IEditableCollection))
          result.Add(item.Value);
      return result;
    }

    #endregion

  }

}