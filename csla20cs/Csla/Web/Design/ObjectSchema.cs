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
      return new IDataSourceViewSchema[] 
        { new ObjectViewSchema(_typeAssemblyName, _typeName) };
    }

  }
}
