//-----------------------------------------------------------------------
// <copyright file="FieldDataManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages properties and property data for</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Csla.Properties;
using Csla.Serialization.Mobile;
using Csla.Reflection;
using System.Reflection;
using Csla.Serialization;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Manages properties and property data for
  /// a business object.
  /// </summary>
  /// <remarks></remarks>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public class FieldDataManager : MobileObject, IUndoableObject
  {
    private string _businessObjectType;
    [NonSerialized]
    BusinessBase _parent;
    [NonSerialized()]
    private List<IPropertyInfo> _propertyList;
    private IFieldData[] _fieldData;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public FieldDataManager() { }

    internal FieldDataManager(Type businessObjectType)
    {
      SetPropertyList(businessObjectType);
      _fieldData = new IFieldData[_propertyList.Count];
    }

    /// <summary>
    /// Called when parent object is deserialized to
    /// restore property list.
    /// </summary>
    internal void SetPropertyList(Type businessObjectType)
    {
      _businessObjectType = businessObjectType.AssemblyQualifiedName;
      _propertyList = GetConsolidatedList(businessObjectType);
    }

    /// <summary>
    /// Called by parent to set the back-reference.
    /// </summary>
    internal void SetParent(BusinessBase parent)
    {
      _parent = parent;
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
    /// Returns the IPropertyInfo object corresponding to the
    /// property name.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    /// property name doesn't correspond to a registered property.</exception>
    public IPropertyInfo GetRegisteredProperty(string propertyName)
    {
      var result = GetRegisteredProperties().Where(c => c.Name == propertyName).FirstOrDefault();
      if (result == null)
        throw new ArgumentOutOfRangeException(string.Format(Resources.PropertyNameNotRegisteredException, propertyName));
      return result;
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
      var found = false;
      try
      {
        found = _consolidatedLists.TryGetValue(type, out result);
      }
      catch
      { /* failure will drop into !found block */ }
      if (!found)
      {
        lock (_consolidatedLists)
        {
          if (_consolidatedLists.ContainsKey(type))
          {
            result = _consolidatedLists[type];
          }
          else
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
      ForceStaticFieldInit(type);
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
      {
        var source = PropertyInfoManager.GetPropertyListCache(hierarchy[index]);
        source.IsLocked = true;
        result.AddRange(source);
      }

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
    /// <param name="propertyInfo">
    /// The property corresponding to the field.
    /// </param>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public IFieldData GetFieldData(IPropertyInfo propertyInfo)
    {
      if ((propertyInfo.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
        throw new InvalidOperationException(Resources.PropertyIsPrivateField);

      try
      {
        return _fieldData[propertyInfo.Index];
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new InvalidOperationException(Resources.PropertyNotRegistered, ex);
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
        throw new InvalidOperationException(Resources.PropertyNotRegistered, ex);
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
      if (field is IFieldData<P> fd)
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
        throw new InvalidOperationException(Resources.PropertyNotRegistered, ex);
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
        throw new InvalidOperationException(Resources.PropertyNotRegistered, ex);
      }
    }

    /// <summary>
    /// Gets a value indicating whether the specified field
    /// has been changed.
    /// </summary>
    /// <param name="propertyInfo">
    /// The property corresponding to the field.
    /// </param>
    /// <returns>True if the field has been changed.</returns>
    public bool IsFieldDirty(IPropertyInfo propertyInfo)
    {
      try
      {
        bool result = false;
        var field = _fieldData[propertyInfo.Index];
        if (field != null)
          result = field.IsDirty;
        else
          result = false;
        return result;

      }
      catch (IndexOutOfRangeException ex)
      {
        throw new InvalidOperationException(Properties.Resources.PropertyNotRegistered, ex);
      }
    }

    #endregion

    #region  IsValid/IsDirty/IsBusy

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

    internal bool IsBusy()
    {
      foreach (var item in _fieldData)
        if (item != null && item.IsBusy)
          return true;
      return false;
    }

    #endregion

    #region  IUndoableObject

    private readonly Stack<byte[]> _stateStack = new Stack<byte[]>();

    /// <summary>
    /// Gets the current edit level of the object.
    /// </summary>
    public int EditLevel
    {
      get { return _stateStack.Count; }
    }

    void Core.IUndoableObject.CopyState(int parentEditLevel, bool parentBindingEdit)
    {
      if (this.EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"), this.GetType().Name, _parent != null ? _parent.GetType().Name : null, this.EditLevel, parentEditLevel - 1);

      IFieldData[] state = new IFieldData[_propertyList.Count];

      for (var index = 0; index < _fieldData.Length; index++)
      {
        var item = _fieldData[index];
        if (item != null)
        {
          if (item.Value is IUndoableObject child)
          {
            // cascade call to child
            child.CopyState(parentEditLevel, parentBindingEdit);
            // indicate that there was a value here
            state[index] = new FieldData<bool>(item.Name);
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
        var stateList = new MobileList<IFieldData>(state.ToList());
        formatter.Serialize(buffer, stateList);
        _stateStack.Push(buffer.ToArray());
      }
    }

    void Core.IUndoableObject.UndoChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (EditLevel > 0)
      {
        if (this.EditLevel - 1 != parentEditLevel)
          throw new UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"), this.GetType().Name, _parent != null ? _parent.GetType().Name : null, this.EditLevel, parentEditLevel + 1);

        IFieldData[] state = null;
        using (MemoryStream buffer = new MemoryStream(_stateStack.Pop()))
        {
          buffer.Position = 0;
          var formatter = SerializationFormatterFactory.GetFormatter();
          state = ((MobileList<IFieldData>)(formatter.Deserialize(buffer))).ToArray();
        }

        for (var index = 0; index < _fieldData.Length; index++)
        {
          var oldItem = state[index];
          var item = _fieldData[index];
          if (item != null)
          {
            var undoable = item.Value as IUndoableObject;
            if (undoable != null)
            {
              // current value is undoable
              if (oldItem != null)
                undoable.UndoChanges(parentEditLevel, parentBindingEdit);
              else
                _fieldData[index] = null;
              continue;
            }
          }
          // restore IFieldData object into field collection
          _fieldData[index] = oldItem;
        }
      }
    }

    void Core.IUndoableObject.AcceptChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (this.EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"), this.GetType().Name, _parent != null ? _parent.GetType().Name : null, this.EditLevel, parentEditLevel + 1);

      if (EditLevel > 0)
      {
        // discard latest recorded state
        _stateStack.Pop();

        foreach (var item in _fieldData)
        {
          if (item != null)
          {
            var child = item.Value as IUndoableObject;
            if (child != null)
            {
              // cascade call to child
              child.AcceptChanges(parentEditLevel, parentBindingEdit);
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

    /// <summary>
    /// Invokes the data portal to update
    /// all child objects contained in 
    /// the list of fields.
    /// </summary>
    /// <param name="parameters">Paramters for method</param>
    public void UpdateChildren(params object[] parameters)
    {
      foreach (var item in _fieldData)
      {
        if (item != null)
        {
          object obj = item.Value;
          if (obj is IEditableBusinessObject || obj is IEditableCollection)
            Csla.DataPortal.UpdateChild(obj, parameters);
        }
      }
    }

    /// <summary>
    /// Invokes the data portal to update
    /// all child objects, including those which are not dirty,
    /// contained in the list of fields.
    /// </summary>
    /// <param name="parameters">Paramters for method</param>
    public void UpdateAllChildren(params object[] parameters)
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      foreach (var item in _fieldData)
      {
        if (item != null)
        {
          object obj = item.Value;
          if (obj is IEditableBusinessObject || obj is IEditableCollection)
            portal.UpdateAll(obj, parameters);
        }
      }
    }

    #endregion

    #region IMobileObject Members

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
      info.AddValue("_businessObjectType", _businessObjectType);

      if (mode == StateMode.Serialization && _stateStack.Count > 0)
        info.AddValue("_stateStack", _stateStack.ToArray());

      foreach (IFieldData data in _fieldData)
      {
        if (data != null)
        {
          if (data.Value is IUndoableObject)
            info.AddValue("child_" + data.Name, true, false);
          else if (mode == StateMode.Undo && data.Value is IMobileObject)
            info.AddValue(data.Name, SerializationFormatterFactory.GetFormatter().Serialize(data.Value), data.IsDirty);
          else if(!(data.Value is IMobileObject))
            info.AddValue(data.Name, data.Value, data.IsDirty);
        }
      }
      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Serializes child objects.
    /// </summary>
    /// <param name="info">Serialization state</param>
    /// <param name="formatter">Serializer instance</param>
    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (IFieldData data in _fieldData)
      {
        if (data != null)
        {
          if (data.Value is IMobileObject mobile)
          {
            SerializationInfo childInfo = formatter.SerializeObject(mobile);
            info.AddChild(data.Name, childInfo.ReferenceId, data.IsDirty);
          }
        }
      }
      base.OnGetChildren(info, formatter);
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
      string type = (string)info.Values["_businessObjectType"].Value;
      Type businessObjecType = Csla.Reflection.MethodCaller.GetType(type);
      SetPropertyList(businessObjecType);

      if (mode == StateMode.Serialization)
      {
        _stateStack.Clear();
        if (info.Values.ContainsKey("_stateStack"))
        {
          var stackArray = info.GetValue<byte[][]>("_stateStack");
          foreach (var item in stackArray.Reverse())
            _stateStack.Push(item);
        }

        _fieldData = new IFieldData[_propertyList.Count];
      }

      foreach (IPropertyInfo property in _propertyList)
      {
        if (info.Values.ContainsKey(property.Name))
        {
          SerializationInfo.FieldData value = info.Values[property.Name];

          IFieldData data = GetOrCreateFieldData(property);
          if (value.Value != null &&
            mode == StateMode.Undo &&
            typeof(IMobileObject).IsAssignableFrom(Nullable.GetUnderlyingType(property.Type) ?? property.Type) &&
            !typeof(IUndoableObject).IsAssignableFrom(Nullable.GetUnderlyingType(property.Type) ?? property.Type))
          {
            data.Value = SerializationFormatterFactory.GetFormatter().Deserialize((byte[])value.Value);
          }
          else data.Value = value.Value;

          if (!value.IsDirty)
            data.MarkClean();
        }
        else if (mode == StateMode.Undo && !((property.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField))
        {
          IFieldData data = GetFieldData(property);
          if (data != null)
          {
            if (!info.Values.ContainsKey("child_" + property.Name) || !info.GetValue<bool>("child_" + property.Name))
              _fieldData[property.Index] = null;
            
            // We don't want to reset children during an undo.
            else if (!typeof(IMobileObject).IsAssignableFrom(data.Value.GetType()))
              data.Value = property.DefaultValue;
          }
        }
      }
      base.OnSetState(info, mode);
    }

    /// <summary>
    /// Deserializes child objects.
    /// </summary>
    /// <param name="info">Serialization state</param>
    /// <param name="formatter">Serializer instance</param>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (IPropertyInfo property in _propertyList)
      {
        if (info.Children.ContainsKey(property.Name))
        {
          SerializationInfo.ChildData childData = info.Children[property.Name];

          IFieldData data = GetOrCreateFieldData(property);
          data.Value = formatter.GetObject(childData.ReferenceId);
          if (!childData.IsDirty)
            data.MarkClean();
        }
      }
      base.OnSetChildren(info, formatter);
    }

    #endregion

    /// <summary>
    /// Forces initialization of the static fields declared
    /// by a type, and any of its base class types.
    /// </summary>
    /// <param name="type">Type of object to initialize.</param>
    public static void ForceStaticFieldInit(Type type)
    {
      const BindingFlags attr =
        BindingFlags.Static |
        BindingFlags.Public |
        BindingFlags.DeclaredOnly |
        BindingFlags.NonPublic;
      lock (type)
      {
        var t = type;
        while (t != null)
        {
          var fields = t.GetFields(attr);
          if (fields.Length > 0)
            fields[0].GetValue(null);
          t = t.BaseType;
        }
      }
    }
  }
}