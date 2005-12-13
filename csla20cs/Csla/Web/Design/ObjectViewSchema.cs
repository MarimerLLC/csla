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
      List<ObjectFieldInfo> result = 
        new List<ObjectFieldInfo>();
      Type t = CslaDataSource.GetType(
        _typeAssemblyName, _typeName);
      if (typeof(IEnumerable).IsAssignableFrom(t))
      {
        // this is a list so get the item type
        t = GetItemType(t);
      }
      PropertyDescriptorCollection props = 
        TypeDescriptor.GetProperties(t);
      foreach (PropertyDescriptor item in props)
        if (item.IsBrowsable)
          result.Add(new ObjectFieldInfo(item));
      return result.ToArray();
    }

    private Type GetItemType(Type objectType)
    {
      if (objectType.IsArray)
        return objectType.GetElementType();
      PropertyInfo[] props = 
        objectType.GetProperties(
          BindingFlags.FlattenHierarchy | 
          BindingFlags.Public | 
          BindingFlags.Instance);
      foreach (PropertyInfo item in props)
        if (Attribute.IsDefined(item, typeof(DefaultMemberAttribute)))
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
}
