//-----------------------------------------------------------------------
// <copyright file="PrimitiveCriteriaSingle.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
#if SILVERLIGHT
using Csla.Serialization;
#else

#endif

using Csla;
using System;
using Csla.Core;

namespace Csla.Test.DataPortalTest
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [Serializable]
  public class PrimitiveCriteriaSingle : BusinessBase<PrimitiveCriteriaSingle>
  {
    #region Business Methods

    private static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> MethodCalledProperty = RegisterProperty(new PropertyInfo<string>("MethodCalled", "MethodCalled"));
    public string MethodCalled
    {
      get { return GetProperty(MethodCalledProperty); }
      set { SetProperty(MethodCalledProperty, value); }
    }

    protected override object GetIdValue()
    {
      return Id;
    }

    #endregion


    #region Factory Methods



    public static void DeleteObject(int id, EventHandler<DataPortalResult<PrimitiveCriteriaSingle>> handler)
    {
      Csla.DataPortal.BeginDelete<PrimitiveCriteriaSingle>(id, handler);
    }

    public static void GetObject(int id, EventHandler<DataPortalResult<PrimitiveCriteriaSingle>> handler)
    {
      Csla.DataPortal.BeginFetch<PrimitiveCriteriaSingle>(id, handler);
    }

    public static void NewObject(int id, EventHandler<DataPortalResult<PrimitiveCriteriaSingle>> handler)
    {
      Csla.DataPortal.BeginCreate<PrimitiveCriteriaSingle>(id, handler);
    }

    public PrimitiveCriteriaSingle()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access


#if !SILVERLIGHT

    #region DataPortal_Create

    protected void DataPortal_Create(int id)
    {
      DoCreate(id);
    }

    private void DoCreate(int id)
    {
      Id = id;
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("PrimitiveCriteriaSingle", "Created");
      MethodCalled = "Created";
      if (id == 9999)
        throw new Exception("Bad data");
    }

    #endregion

    #region DataPortal_Fetch


    private void DataPortal_Fetch(int id)
    {
      DoFetch(id);
    }

    private void DataPortal_Fetch(string id)
    {
      DoFetch(int.Parse(id));
    }

    private void DataPortal_Fetch(Guid id)
    {
      DoFetch(1234);
    }

    private void DoFetch(int id)
    {
      Id = id;
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("PrimitiveCriteriaSingle", "Fetched");
      MethodCalled = "Fetched";
      if (id == 9999)
        throw new Exception("Bad data");
    }

    #endregion

    #region DataPortal_Insert / DataPortal_Update / DataPortal_Delete

    protected override void DataPortal_Insert()
    {
      DoInsertUpdate(false);
    }

    private void DoInsertUpdate(bool isUpdate)
    {
      var insertOrUpdate = isUpdate ? "Updated" : "Inserted";

      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("PrimitiveCriteriaSingle", insertOrUpdate);
      MethodCalled = insertOrUpdate;
    }



    #endregion


    private void DataPortal_Delete(int id)
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("PrimitiveCriteriaSingle", "Deleted");
      MethodCalled = "Deleted+" + id.ToString();
    }
#endif




    #endregion
  }
}