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
  [Serializable]
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

      string originalPath = GetOriginalPath(
        _typeAssemblyName, _typeName);

      System.Security.NamedPermissionSet fulltrust =
        new System.Security.NamedPermissionSet("FullTrust");
      AppDomain tempDomain = AppDomain.CreateDomain(
        "__temp",
        AppDomain.CurrentDomain.Evidence,
        AppDomain.CurrentDomain.SetupInformation,
        fulltrust,
        new System.Security.Policy.StrongName[] { });
      try
      {
        // load the TypeLoader object in the temp AppDomain
        Assembly thisAssembly = Assembly.GetExecutingAssembly();
        TypeLoader loader =
          (TypeLoader)tempDomain.CreateInstanceFromAndUnwrap(
            thisAssembly.CodeBase, typeof(TypeLoader).FullName);

        //// load the business type in the temp AppDomain
        //Type t = loader.GetType(
        //  _typeAssemblyName, _typeName);
        
        //// load the metadata from the Type object
        //if (typeof(IEnumerable).IsAssignableFrom(t))
        //{
        //  // this is a list so get the item type
        //  t = Utilities.GetChildItemType(t);
        //}
        //PropertyDescriptorCollection props =
        //  TypeDescriptor.GetProperties(t);
        //foreach (PropertyDescriptor item in props)
        //  if (item.IsBrowsable)
        //    result.Add(new ObjectFieldInfo(item));
        result = loader.GetFields(
          originalPath, _typeAssemblyName, _typeName);
      }
      finally
      {
        AppDomain.Unload(tempDomain);
      }
      return result.ToArray();
    }

    private string GetOriginalPath(string assemblyName, string typeName)
    {
      //Type t = CslaDataSource.GetType(assemblyName, typeName);
      //Assembly asm = t.Assembly;
      Assembly asm = CslaDataSource.GetAssembly(assemblyName);
      return asm.CodeBase;
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
