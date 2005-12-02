using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Csla.Web.Design
{

  /// <summary>
  /// Object responsible for providing details about
  /// data binding to a specific CSLA .NET object.
  /// </summary>
  public class CslaDesignerDataSourceView : DesignerDataSourceView
  {

    private CslaDataSourceDesigner _owner = null;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public CslaDesignerDataSourceView(CslaDataSourceDesigner owner, string viewName)
      : base(owner, viewName)
    {
      _owner = owner;
    }

    /// <summary>
    /// Returns a set of sample data used to populate
    /// controls at design time.
    /// </summary>
    /// <param name="minimumRows">Minimum number of sample rows
    /// to create.</param>
    /// <param name="isSampleData">Returns True if the data
    /// is sample data.</param>
    public override System.Collections.IEnumerable GetDesignTimeData(int minimumRows, out bool isSampleData)
    {

      IDataSourceViewSchema schema = this.Schema;
      DataTable result = new DataTable();

      // create the columns
      foreach (IDataSourceFieldSchema item in schema.GetFields())
        result.Columns.Add(item.Name, item.DataType);

      // create sample data
      for (int index = 1; index <= minimumRows; index++)
      {
        object[] values = new object[result.Columns.Count];
        int colIndex = 0;
        foreach (DataColumn col in result.Columns)
        {
          if (col.DataType.Equals(typeof(string)))
            values[colIndex] = "abc";
          else if (col.DataType.Equals(typeof(bool)))
            values[colIndex] = false;
          else if (col.DataType.IsPrimitive)
            values[colIndex] = index;
          else if (col.DataType.Equals(typeof(Guid)))
            values[colIndex] = Guid.NewGuid();
          else
            values[colIndex] = null;
          colIndex++;
        }
        result.LoadDataRow(values, LoadOption.OverwriteChanges);
      }

      isSampleData = true;
      return (System.Collections.IEnumerable)(new DataView(result));

    }

    /// <summary>
    /// Returns schema information corresponding to the properties
    /// of the CSLA .NET business object.
    /// </summary>
    /// <remarks>
    /// All public properties are returned except for those marked
    /// with the <see cref="BrowsableAttribute">Browsable attribute</see>
    /// as False.
    /// </remarks>
    public override IDataSourceViewSchema Schema
    {
      get
      {
        return new ObjectSchema(_owner.DataSourceControl.TypeAssemblyName,
          _owner.DataSourceControl.TypeName).GetViews()[0];
      }
    }

    /// <summary>
    /// Get a value indicating whether data binding can retrieve
    /// the total number of rows of data.
    /// </summary>
    public override bool CanRetrieveTotalRowCount
    {
      get { return true; }
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// delete the object.
    /// </summary>
    /// <remarks>
    /// If this returns true, the web page must handle the
    /// <see cref="CslaDataSource.DeleteObject">DeleteObject</see>
    /// event.
    /// </remarks>
    public override bool CanDelete
    {
      get
      {
        Type objectType = CslaDataSource.GetType(
          _owner.DataSourceControl.TypeAssemblyName, _owner.DataSourceControl.TypeName);
        if (typeof(Csla.Core.IEditableObject).IsAssignableFrom(objectType))
          return true;
        else if (objectType.GetMethod("Remove") != null)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// insert an instance of the object.
    /// </summary>
    /// <remarks>
    /// If this returns true, the web page must handle the
    /// <see cref="CslaDataSource.InsertObject">InsertObject</see>
    /// event.
    /// </remarks>
    public override bool CanInsert
    {
      get
      {
        if (typeof(Csla.Core.IEditableObject).IsAssignableFrom(CslaDataSource.GetType(
          _owner.DataSourceControl.TypeAssemblyName, _owner.DataSourceControl.TypeName)))
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// update or edit the object.
    /// </summary>
    /// <remarks>
    /// If this returns true, the web page must handle the
    /// <see cref="CslaDataSource.UpdateObject">UpdateObject</see>
    /// event.
    /// </remarks>
    public override bool CanUpdate
    {
      get
      {
        if (typeof(Csla.Core.IEditableObject).IsAssignableFrom(CslaDataSource.GetType(
          _owner.DataSourceControl.TypeAssemblyName, _owner.DataSourceControl.TypeName)))
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the data source supports
    /// paging.
    /// </summary>
    public override bool CanPage
    {
      get { return false; }
    }

    /// <summary>
    /// Gets a value indicating whether the data source supports
    /// sorting.
    /// </summary>
    public override bool CanSort
    {
      get { return false; }
    }
  }

  /// <summary>
  /// Object providing access to schema information for
  /// a business object.
  /// </summary>
  /// <remarks>
  /// This object returns only one view, which corresponds
  /// to the business object used by data binding.
  /// </remarks>
  public class ObjectSchema : IDataSourceSchema
  {

    private string _typeAssemblyName = string.Empty;
    private string _typeName = string.Empty;

    public ObjectSchema(string assemblyName, string typeName)
    {
      _typeAssemblyName = assemblyName;
      _typeName = typeName;
    }

    /// <summary>
    /// Returns a single element array containing the
    /// schema for the CSLA .NET business object.
    /// </summary>
    public IDataSourceViewSchema[] GetViews()
    {
      return new IDataSourceViewSchema[] { new ObjectViewSchema(_typeAssemblyName, _typeName) };
    }

  }

  /// <summary>
  /// Object providing schema information for a
  /// business object.
  /// </summary>
  public class ObjectViewSchema : IDataSourceViewSchema
  {

    private string _typeAssemblyName = string.Empty;
    private string _typeName = string.Empty;

    /// <summary>
    /// Create an instance of the object.
    /// </summary>
    /// <param name="assemblyName">The assembly containing
    /// the business class for which to generate the schema.</param>
    /// <param name="typeName">The business class for
    /// which to generate the schema.</param>
    public ObjectViewSchema(string assemblyName, string typeName)
    {
      _typeAssemblyName = assemblyName;
      _typeName = typeName;
    }

    /// <summary>
    /// Returns a list of child schemas belonging to the
    /// object.
    /// </summary>
    /// <remarks>This schema object only returns
    /// schema for the object itself, so GetChildren will
    /// always return Nothing (null in C#).</remarks>
    public IDataSourceViewSchema[] GetChildren()
    {
      return null;
    }

    /// <summary>
    /// Returns schema information for each property on
    /// the object.
    /// </summary>
    /// <remarks>All public properties on the object
    /// will be reflected in this schema list except
    /// for those properties where the 
    /// <see cref="BrowsableAttribute">Browsable</see> attribute
    /// is False.
    /// </remarks>
    public IDataSourceFieldSchema[] GetFields()
    {
      List<ObjectFieldInfo> result = new List<ObjectFieldInfo>();
      Type t = CslaDataSource.GetType(_typeAssemblyName, _typeName);
      if (typeof(IEnumerable).IsAssignableFrom(t))
      {
        // this is a list so get the item type
        t = GetItemType(t);
      }
      PropertyDescriptorCollection props = TypeDescriptor.GetProperties(t);
      foreach (PropertyDescriptor item in props)
        if (item.IsBrowsable)
          result.Add(new ObjectFieldInfo(item));
      return result.ToArray();
    }

    private Type GetItemType(Type objectType)
    {
      if (objectType.IsArray)
        return objectType.GetElementType();
      PropertyInfo[] props = objectType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
      foreach (PropertyInfo item in props)
        if (item.GetIndexParameters().Length > 0)
          return item.PropertyType;
      return null;
    }

    /// <summary>
    /// Returns the name of the schema.
    /// </summary>
    public string Name
    {
      get { return "Default"; }
    }

  }

  /// <summary>
  /// Contains schema information for a single
  /// object property.
  /// </summary>
  public class ObjectFieldInfo : IDataSourceFieldSchema
  {

    private PropertyDescriptor _field;
    private bool _retrievedMetaData;
    private bool _primaryKey;
    private bool _isIdentity;
    private bool _isNullable;
    private int _length;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="field">The PropertyInfo object
    /// describing the property.</param>
    public ObjectFieldInfo(PropertyDescriptor field)
    {
      _field = field;
    }

    private void EnsureMetaData()
    {
      if (!_retrievedMetaData)
      {
        DataObjectFieldAttribute attribute1 = (DataObjectFieldAttribute)_field.Attributes[typeof(DataObjectFieldAttribute)];
        if (attribute1 != null)
        {
          _primaryKey = attribute1.PrimaryKey;
          _isIdentity = attribute1.IsIdentity;
          _isNullable = attribute1.IsNullable;
          _length = attribute1.Length;
        }
        _retrievedMetaData = true;
      }
    }

    /// <summary>
    /// Gets the data type of the property.
    /// </summary>
    public Type DataType
    {
      get
      {
        Type type1 = _field.PropertyType;
        if (type1.IsGenericType && (type1.GetGenericTypeDefinition() == typeof(Nullable)))
          return type1.GetGenericArguments()[0];
        return type1;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this property
    /// is an identity key for the object.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public bool Identity
    {
      get { return _isIdentity; }
    }

    /// <summary>
    /// Gets a value indicating whether this property
    /// is readonly.
    /// </summary>
    public bool IsReadOnly
    {
      get { return !_field.IsReadOnly; }
    }

    /// <summary>
    /// Gets a value indicating whether this property
    /// must contain a unique value.
    /// </summary>
    /// <returns>Always returns False.</returns>
    public bool IsUnique
    {
      get { return false; }
    }

    /// <summary>
    /// Gets the length of the property value.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public int Length
    {
      get { return _length; }
    }

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string Name
    {
      get { return _field.Name; }
    }

    /// <summary>
    /// Gets a value indicating whether the property
    /// is nullable
    /// </summary>
    /// <remarks>
    /// Returns True for reference types, and for
    /// value types wrapped in the Nullable generic.
    /// The result can also be set to True through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public bool Nullable
    {
      get
      {
        Type t = _field.PropertyType;
        if (!t.IsValueType || _isNullable)
          return true;
        if (t.IsGenericType)
          return (t.GetGenericTypeDefinition() == typeof(Nullable));
        return false;
      }
    }

    /// <summary>
    /// Gets the property's numeric precision.
    /// </summary>
    /// <returns>Always returns -1.</returns>
    public int Precision
    {
      get { return -1; }
    }

    /// <summary>
    /// Gets a value indicating whether the property
    /// is a primary key value.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public bool PrimaryKey
    {
      get { return _primaryKey; }
    }

    /// <summary>
    /// Gets the property's scale.
    /// </summary>
    /// <returns>Always returns -1.</returns>
    public int Scale
    {
      get { return -1; }
    }
  }
}

