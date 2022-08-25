//-----------------------------------------------------------------------
// <copyright file="RollbackRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.RollBack
{
  [Serializable()]
  public class RollbackRoot : BusinessBase<RollbackRoot>
  {
    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public static PropertyInfo<bool> FailProperty = RegisterProperty<bool>(nameof(Fail));

    protected override object GetIdValue()
    {
      return GetProperty(DataProperty);
    }

    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public bool Fail
    {
      get { return GetProperty(FailProperty); }
      set { SetProperty(FailProperty, value); }
    }

    [Serializable()]
    protected class Criteria : CriteriaBase<Criteria>
    {
      private static PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));

      public Criteria()
      {
        Data = "<new>";
      }

      public Criteria(string data)
      {
        this.Data = data;
      }

      public string Data
      {
        get { return ReadProperty(DataProperty); }
        set { LoadProperty(DataProperty, value); }
      }
    }

    public static RollbackRoot NewRoot(IDataPortal<RollbackRoot> dataPortal)
    {
      return dataPortal.Create(new Criteria());
    }

    public static RollbackRoot GetRoot(string data, IDataPortal<RollbackRoot> dataPortal)
    {
      return dataPortal.Fetch(new Criteria(data));
    }

    public static void DeleteRoot(string data, IDataPortal<RollbackRoot> dataPortal)
    {
      dataPortal.Delete(new Criteria(data));
    }

    private void DataPortal_Create(Criteria criteria)
    {
      Data = criteria.Data;
      TestResults.AddOrOverwrite("Root", "Created");
    }

    protected void DataPortal_Fetch(Criteria criteria)
    {
      Data = criteria.Data;
      MarkOld();
      TestResults.AddOrOverwrite("Root", "Fetched");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.AddOrOverwrite("Root", "Inserted");
    }

    [Update]
    protected void DataPortal_Update()
    {
      //we would update here
      TestResults.AddOrOverwrite("Root", "Updated");

      if (Fail)
        throw new Exception("fail Update");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.AddOrOverwrite("Root", "Deleted self");
    }

    [Delete]
	protected void DataPortal_Delete(object criteria)
    {
      //we would delete here
      TestResults.AddOrOverwrite("Root", "Deleted");
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      base.OnDeserialized(context);
      TestResults.AddOrOverwrite("Deserialized", "root Deserialized");
    }
  }
}