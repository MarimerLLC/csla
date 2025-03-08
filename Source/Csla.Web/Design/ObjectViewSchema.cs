//-----------------------------------------------------------------------
// <copyright file="ObjectViewSchema.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object providing schema information for a</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Web.UI.Design;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Csla.Web.Design
{

  /// <summary>
  /// Object providing schema information for a
  /// business object.
  /// </summary>
  [Serializable]
  public class ObjectViewSchema : IDataSourceViewSchema
  {
    private string _typeName = "";
    private CslaDataSourceDesigner _designer;

    /// <summary>
    /// Create an instance of the object.
    /// </summary>
    /// <param name="site">Site containing the control.</param>
    /// <param name="typeName">The business class for
    /// which to generate the schema.</param>
    public ObjectViewSchema(CslaDataSourceDesigner site, string typeName)
    {
      _typeName = typeName;
      _designer = site;
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
      ITypeResolutionService typeService = null;
      var result = new List<IDataSourceFieldSchema>();

      if (_designer != null)
      {
        Type objectType = null;
        try
        {
          typeService = (ITypeResolutionService)(_designer.Site.GetService(typeof(ITypeResolutionService)));
          objectType = typeService.GetType(_typeName, true, false);

          if (typeof(IEnumerable).IsAssignableFrom(objectType))
          {
            // this is a list so get the item type
            objectType = Utilities.GetChildItemType(objectType);
          }
          PropertyDescriptorCollection props = TypeDescriptor.GetProperties(objectType);
          foreach (PropertyDescriptor item in props)
          {
            if (item.IsBrowsable)
            {
              result.Add(new ObjectFieldInfo(item));
            }
          }
        }
        catch
        { /* do nothing - just swallow exception */ }
      }

      return result.ToArray();
    }

    /// <summary>
    /// Returns the name of the schema.
    /// </summary>
    public string Name => "Default";
  }
}
