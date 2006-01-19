using System;
using System.Collections;
using System.Web.UI;

namespace Csla.Web
{

  /// <summary>
  /// The object responsible for managing data binding
  /// to a specific CSLA .NET object.
  /// </summary>
  public class CslaDataSourceView : DataSourceView
  {

    private CslaDataSource _owner;
    private string _typeName;
    private string _typeAssemblyName;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="owner">The CslaDataSource object
    /// that owns this view.</param>
    /// <param name="viewName">The name of the view.</param>
    public CslaDataSourceView(CslaDataSource owner, string viewName)
      : base(owner, viewName)
    {
      _owner = owner;
    }

    /// <summary>
    /// Get or set the full type name of the business object
    /// class to be used as a data source.
    /// </summary>
    /// <value>Full type name of the business class.</value>
    public string TypeName
    {
      get { return _typeName; }
      set { _typeName = value; }
    }

    /// <summary>
    /// Get or set the name of the assembly containing the 
    /// business object class to be used as a data source.
    /// </summary>
    /// <value>Assembly name containing the business class.</value>
    public string TypeAssemblyName
    {
      get { return _typeAssemblyName; }
      set { _typeAssemblyName = value; }
    }

    #region Select

    protected override System.Collections.IEnumerable 
      ExecuteSelect(DataSourceSelectArguments arguments)
    {
      // get the object from the page
      SelectObjectArgs args = new SelectObjectArgs();
      _owner.OnSelectObject(args);
      object obj = args.BusinessObject;

      object result;
      if (arguments.RetrieveTotalRowCount)
      {
        if (obj == null)
          result = 0;
        else if (obj is IList)
          result = ((IList)obj).Count;
        else if (obj is IEnumerable)
        {
          IEnumerable temp = (IEnumerable)obj;
          int count = 0;
          foreach (object item in temp)
            count++;
          result = count;
        }
        else
          result = 1;
      }
      else
        result = obj;

      // if the result isn't IEnumerable then
      // wrap it in a collection
      if (!(result is IEnumerable))
      {
        ArrayList list = new ArrayList();
        list.Add(result);
        result = list;
      }

      // now return the object as a result
      return (IEnumerable)result;
    }

    #endregion

    #region Insert

    public override bool CanInsert
    {
      get
      {
        if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(
          CslaDataSource.GetType(_typeAssemblyName, _typeName)))
          return true;
        else
          return false;
      }
    }

    protected override int ExecuteInsert(
      IDictionary values)
    {
      // tell the page to insert the object
      InsertObjectArgs args = 
        new InsertObjectArgs(values);
      _owner.OnInsertObject(args);
      return args.RowsAffected;
    }

    #endregion

    #region Delete

    public override bool CanDelete
    {
      get
      {
        if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(
          CslaDataSource.GetType(_typeAssemblyName, _typeName)))
          return true;
        else
          return false;
      }
    }

    protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
    {
      
      // tell the page to delete the object
      DeleteObjectArgs args = new DeleteObjectArgs(keys, oldValues);
      _owner.OnDeleteObject(args);
      return args.RowsAffected;
    }

    #endregion

    #region Update

    public override bool CanUpdate
    {
      get
      {
        if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(
          CslaDataSource.GetType(_typeAssemblyName, _typeName)))
          return true;
        else
          return false;
      }
    }

    protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
    {
      // tell the page to update the object
      UpdateObjectArgs args = new UpdateObjectArgs(keys, values, oldValues);
      _owner.OnUpdateObject(args);
      return args.RowsAffected;
    }

    #endregion

    #region Other Operations

    public override bool CanPage
    {
      get { return false; }
    }

    public override bool CanRetrieveTotalRowCount
    {
      get { return true; }
    }

    public override bool CanSort
    {
      get { return false; }
    }

    #endregion

  }
}