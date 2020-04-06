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

    private ChildEntityList _children = ChildEntityList.NewChildEntityList();
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

    public ChildEntityList Children
    {
      get { return _children; }
    }

    public override bool IsDirty
    {
      get
      {
        return base.IsDirty || _children.IsDirty;
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

    public static ParentEntity NewParentEntity()
    {
      return Csla.DataPortal.Create<ParentEntity>();
    }

    public static ParentEntity GetParentEntity(int ID)
    {
      return Csla.DataPortal.Fetch<ParentEntity>(new Criteria(ID));
    }

    public static void DeleteParentEntity(int ID)
    {
      Csla.DataPortal.Delete<ParentEntity>(new Criteria(ID));
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

    [RunLocal()]
    [Create]
    protected override void DataPortal_Create()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Created");
#pragma warning restore CS0618 // Type or member is obsolete
      BusinessRules.CheckRules();
      Console.WriteLine("DataPortal_Create");
    }

    [Fetch]
    protected void DataPortal_Fetch(object criteria)
    {
      Console.WriteLine("DataPortal_Fetch");
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Fetched");
#pragma warning restore CS0618 // Type or member is obsolete
      BusinessRules.CheckRules();
    }

    [Insert]
    protected override void DataPortal_Insert()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Inserted");
#pragma warning restore CS0618 // Type or member is obsolete
      Console.WriteLine("DataPortal_Insert");
    }

    [Update]
    protected override void DataPortal_Update()
    {
      Console.WriteLine("DataPortal_Update");
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Updated");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [DeleteSelf]
    protected override void DataPortal_DeleteSelf()
    {
      Console.WriteLine("DataPortal_DeleteSelf");
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Deleted Self");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Delete]
    protected void DataPortal_Delete(object criteria)
    {
      Console.WriteLine("DataPortal_Delete");
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Deleted");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    #endregion
  }
}