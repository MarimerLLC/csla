//-----------------------------------------------------------------------
// <copyright file="PrimitiveCriteriaSingle.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;

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

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> MethodCalledProperty = RegisterProperty<string>(c => c.MethodCalled);
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

    public PrimitiveCriteriaSingle()
    { }

    #region Data Access

    #region DataPortal_Create

    protected void DataPortal_Create(int id)
    {
      DoCreate(id);
    }

    private void DoCreate(int id)
    {
      Id = id;
#pragma warning disable CS0618 // Type or member is obsolete
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("PrimitiveCriteriaSingle", "Created");
#pragma warning restore CS0618 // Type or member is obsolete
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
#pragma warning disable CS0618 // Type or member is obsolete
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("PrimitiveCriteriaSingle", "Fetched");
#pragma warning restore CS0618 // Type or member is obsolete
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

#pragma warning disable CS0618 // Type or member is obsolete
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("PrimitiveCriteriaSingle", insertOrUpdate);
#pragma warning restore CS0618 // Type or member is obsolete
      MethodCalled = insertOrUpdate;
    }



    #endregion


    private void DataPortal_Delete(int id)
    {
#pragma warning disable CS0618 // Type or member is obsolete
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("PrimitiveCriteriaSingle", "Deleted");
#pragma warning restore CS0618 // Type or member is obsolete
      MethodCalled = "Deleted+" + id.ToString();
    }

    #endregion
  }
}