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
    private string _data = "";
    private bool _fail = false;

    protected override object GetIdValue()
    {
      return _data;
    }

    public string Data
    {
      get { return _data; }
      set
      {
        if (_data != value)
        {
          _data = value;
          MarkDirty();
        }
      }
    }

    public bool Fail
    {
      get { return _fail; }
      set
      {
        if (_fail != value)
        {
          _fail = value;
          MarkDirty();
        }
      }
    }

    [Serializable()]
    private class Criteria
    {
      public string _data;

      public Criteria()
      {
        _data = "<new>";
      }

      public Criteria(string data)
      {
        this._data = data;
      }
    }

    public static RollbackRoot NewRoot()
    {
      return ((RollbackRoot)(Csla.DataPortal.Create<RollbackRoot>(new Criteria())));
    }

    public static RollbackRoot GetRoot(string data)
    {
      return ((RollbackRoot)(Csla.DataPortal.Fetch<RollbackRoot>(new Criteria(data))));
    }

    public static void DeleteRoot(string data)
    {
      Csla.DataPortal.Delete<RollbackRoot>(new Criteria(data));
    }

    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      _data = crit._data;
      TestResults.Add("Root", "Created");
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      _data = crit._data;
      MarkOld();
      TestResults.Add("Root", "Fetched");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Add("Root", "Inserted");
    }

    [Update]
    protected void DataPortal_Update()
    {
      //we would update here
      TestResults.Add("Root", "Updated");

      if (_fail)
        throw new Exception("fail Update");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Add("Root", "Deleted self");
    }

    [Delete]
		protected void DataPortal_Delete(object criteria)
    {
      //we would delete here
      TestResults.Add("Root", "Deleted");
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      base.OnDeserialized(context);
      TestResults.Add("Deserialized", "root Deserialized");
    }
  }
}