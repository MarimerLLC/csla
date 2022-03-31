//-----------------------------------------------------------------------
// <copyright file="ParentEntity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>DO NOT USE in UI - use the factory method instead</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Csla.Test.DataBinding
{
  [Serializable()]
  public class ParentEntity : BusinessBase<ParentEntity>
  {
    #region "Business methods"

    [NotUndoable()]
    private string _notUndoable;

    public string NotUndoable
    {
      get { return _notUndoable; }
      set { _notUndoable = value; }
    }

    public static PropertyInfo<int> IDProperty = RegisterProperty<int>(c => c.ID);
    public int ID
    {
      get { return GetProperty(IDProperty); }
      private set { LoadProperty(IDProperty, value); }
    }

    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public static PropertyInfo<ChildEntityList> ChildrenProperty = RegisterProperty<ChildEntityList>(p => p.Children);
    public ChildEntityList Children
    {
      get { return GetProperty(ChildrenProperty); }
    }

    public override bool IsDirty
    {
      get
      {
        return base.IsDirty || Children.IsDirty;
      }
    }

    #endregion

    protected override void AddBusinessRules()
    {
      //don't need rules for databinding tests
      //ValidationRules.AddRule(Validation.CommonRules.StringRequired, "Data");
    }

    #region "constructors"

    /// <summary>
    /// DO NOT USE in UI - use the factory method instead
    /// </summary>
    /// <remaks>
    ///this constructor is public only to support web forms databinding 
    ///</remaks>
    public ParentEntity()
    {
      //if we need authorization rules:
      //this.AuthorizationRules.AllowWrite("Data", "Admin");
      //this.AuthorizationRules.AllowRead("Data", "Admin");
    }

    #endregion

    #region "Factory Methods"

    public static ParentEntity NewParentEntity(IDataPortal<ParentEntity> dataPortal)
    {
      return dataPortal.Create();
    }

    public static ParentEntity GetParentEntity(int ID, IDataPortal<ParentEntity> dataPortal)
    {
      return dataPortal.Fetch(new Criteria(ID));
    }

    public static void DeleteParentEntity(int ID, IDataPortal<ParentEntity> dataPortal)
    {
      dataPortal.Delete(new Criteria(ID));
    }

    #endregion

    #region "Criteria"

    [Serializable()]
    private class Criteria
    {
      public int _id;

      public Criteria(int id)
      {
        this._id = id;
      }
    }

    #endregion

    #region "Data Access"

    [Create]
	protected void DataPortal_Create([Inject] IChildDataPortal<ChildEntityList> childDataPortal)
    {
      SetProperty(ChildrenProperty, childDataPortal.CreateChild());
      TestResults.Reinitialise();
      TestResults.Add("ParentEntity", "Created");
      BusinessRules.CheckRules();
      Console.WriteLine("DataPortal_Create");
    }

    [Fetch]
    protected void DataPortal_Fetch(object criteria, [Inject] IChildDataPortal<ChildEntityList> childDataPortal)
    {
      SetProperty(ChildrenProperty, childDataPortal.CreateChild());
      Console.WriteLine("DataPortal_Fetch");
      TestResults.Reinitialise();
      TestResults.Add("ParentEntity", "Fetched");
      BusinessRules.CheckRules();
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Reinitialise();
      TestResults.Add("ParentEntity", "Inserted");
      Console.WriteLine("DataPortal_Insert");
    }

    [Update]
	protected void DataPortal_Update()
    {
      Console.WriteLine("DataPortal_Update");
      TestResults.Reinitialise();
      TestResults.Add("ParentEntity", "Updated");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      Console.WriteLine("DataPortal_DeleteSelf");
      TestResults.Reinitialise();
      TestResults.Add("ParentEntity", "Deleted Self");
    }

    [Delete]
	protected void DataPortal_Delete(object criteria)
    {
      Console.WriteLine("DataPortal_Delete");
      TestResults.Reinitialise();
      TestResults.Add("ParentEntity", "Deleted");
    }

    #endregion
  }
}