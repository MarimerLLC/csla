//-----------------------------------------------------------------------
// <copyright file="CslaDataSource.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A Web Forms data binding control designed to support</summary>
//-----------------------------------------------------------------------
using System;
using System.Web.UI;
using System.ComponentModel;
using System.Reflection;
using Csla.Properties;

namespace Csla.Web
{

  /// <summary>
  /// A Web Forms data binding control designed to support
  /// CSLA .NET business objects as data sources.
  /// </summary>
  [Designer(typeof(Csla.Web.Design.CslaDataSourceDesigner))]
  [DisplayName("CslaDataSource")]
  [Description("CSLA .NET Data Source Control")]
  [ToolboxData("<{0}:CslaDataSource runat=\"server\"></{0}:CslaDataSource>")]
  public class CslaDataSource : DataSourceControl
  {

    private CslaDataSourceView _defaultView;

    /// <summary>
    /// Event raised when an object is to be created and
    /// populated with data.
    /// </summary>
    /// <remarks>Handle this event in a page and set
    /// e.BusinessObject to the populated business object.
    /// </remarks>
    public event EventHandler<SelectObjectArgs> SelectObject;

    /// <summary>
    /// Event raised when an object is to be populated with data
    /// and inserted.
    /// </summary>
    /// <remarks>Handle this event in a page to create an
    /// instance of the object, load the object with data and
    /// insert the object into the database.</remarks>
    public event EventHandler<InsertObjectArgs> InsertObject;

    /// <summary>
    /// Event raised when an object is to be updated.
    /// </summary>
    /// <remarks>Handle this event in a page to update an
    /// existing instance of an object with new data and then
    /// save the object into the database.</remarks>
    public event EventHandler<UpdateObjectArgs> UpdateObject;

    /// <summary>
    /// Event raised when an object is to be deleted.
    /// </summary>
    /// <remarks>Handle this event in a page to delete
    /// an object from the database.</remarks>
    public event EventHandler<DeleteObjectArgs> DeleteObject;

    /// <summary>
    /// Returns the default view for this data control.
    /// </summary>
    /// <param name="viewName">Ignored.</param>
    /// <returns></returns>
    /// <remarks>This control only contains a "Default" view.</remarks>
    protected override DataSourceView GetView(string viewName)
    {
      if (_defaultView == null)
        _defaultView = new CslaDataSourceView(this, "Default");
      return _defaultView;
    }

    /// <summary>
    /// Get or set the name of the assembly (no longer used).
    /// </summary>
    /// <value>Obsolete - do not use.</value>
    public string TypeAssemblyName
    {
      get { return ((CslaDataSourceView)this.GetView("Default")).TypeAssemblyName; }
      set { ((CslaDataSourceView)this.GetView("Default")).TypeAssemblyName = value; }
    }

    /// <summary>
    /// Get or set the full type name of the business object
    /// class to be used as a data source.
    /// </summary>
    /// <value>Full type name of the business class,
    /// including assembly name.</value>
    public string TypeName
    {
      get { return ((CslaDataSourceView)this.GetView("Default")).TypeName; }
      set { ((CslaDataSourceView)this.GetView("Default")).TypeName = value; }
    }

    /// <summary>
    /// Get or set a value indicating whether the
    /// business object data source supports paging.
    /// </summary>
    /// <remarks>
    /// To support paging, the business object
    /// (collection) must implement 
    /// <see cref="Csla.Core.IReportTotalRowCount"/>.
    /// </remarks>
    public bool TypeSupportsPaging
    {
      get { return ((CslaDataSourceView)this.GetView("Default")).TypeSupportsPaging; }
      set { ((CslaDataSourceView)this.GetView("Default")).TypeSupportsPaging = value; }
    }

    /// <summary>
    /// Get or set a value indicating whether the
    /// business object data source supports sorting.
    /// </summary>
    public bool TypeSupportsSorting
    {
      get { return ((CslaDataSourceView)this.GetView("Default")).TypeSupportsSorting; }
      set { ((CslaDataSourceView)this.GetView("Default")).TypeSupportsSorting = value; }
    }

    private static System.Collections.Generic.Dictionary<string,Type> _typeCache = 
      new System.Collections.Generic.Dictionary<string,Type>();

    /// <summary>
    /// Returns a <see cref="Type">Type</see> object based on the
    /// assembly and type information provided.
    /// </summary>
    /// <param name="typeAssemblyName">Optional assembly name.</param>
    /// <param name="typeName">Full type name of the class,
    /// including assembly name.</param>
    /// <remarks></remarks>
    internal static Type GetType(
      string typeAssemblyName, string typeName)
    {
      Type result = null;
      if (!string.IsNullOrEmpty(typeAssemblyName))
      {
        // explicit assembly name provided
        result = Type.GetType(string.Format(
          "{0}, {1}", typeName, typeAssemblyName), true, true);
      }
      else if (typeName.IndexOf(",") > 0)
      {
        // assembly qualified type name provided
        result = Type.GetType(typeName, true, true);
      }
      else
      {
        // no assembly name provided
        result = _typeCache[typeName];
        if (result == null)
          foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
          {
            result = asm.GetType(typeName, false, true);
            if (result != null)
            {
              _typeCache.Add(typeName, result);
              break;
            }
          }
      }

      if (result == null)
        throw new TypeLoadException(String.Format(Resources.TypeLoadException, typeName));

      return result;
    }

    /// <summary>
    /// Returns a list of views available for this control.
    /// </summary>
    /// <remarks>This control only provides the "Default" view.</remarks>
    protected override System.Collections.ICollection GetViewNames()
    {
      return new string[] { "Default" };
    }

    /// <summary>
    /// Raises the SelectObject event.
    /// </summary>
    internal void OnSelectObject(SelectObjectArgs e)
    {
      if (SelectObject != null)
        SelectObject(this, e);
    }

    /// <summary>
    /// Raises the InsertObject event.
    /// </summary>
    internal void OnInsertObject(InsertObjectArgs e)
    {
      if (InsertObject != null)
        InsertObject(this, e);
    }

    /// <summary>
    /// Raises the UpdateObject event.
    /// </summary>
    internal void OnUpdateObject(UpdateObjectArgs e)
    {
      if (UpdateObject != null)
        UpdateObject(this, e);
    }

    /// <summary>
    /// Raises the DeleteObject event.
    /// </summary>
    internal void OnDeleteObject(DeleteObjectArgs e)
    {
      if (DeleteObject != null)
        DeleteObject(this, e);
    }
  }
}
