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
  public class Single : BusinessBase<Single>
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

#if !SILVERLIGHT
    public static Single NewObject()
    {
      return Csla.DataPortal.Create<Single>();
    }

    public static Single GetObject(int id)
    {
      return Csla.DataPortal.Fetch<Single>(new Criteria(id));
    }

    public static void DeleteObject(int id)
    {
      Csla.DataPortal.Delete(new Criteria(id));
    }
#endif


    public static void DeleteObject(int id, EventHandler<DataPortalResult<Single>> handler)
    {
      Csla.DataPortal.BeginDelete<Single>(new Criteria(id), handler);
    }

    public static void DeleteObject(int id, EventHandler<DataPortalResult<Single>> handler, object userState)
    {
      Csla.DataPortal.BeginDelete<Single>(new Criteria(id), handler, userState);
    }

    public Single()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

#if TESTING
    [System.Diagnostics.DebuggerNonUserCode]
#endif
    [Serializable]
    public class Criteria : CriteriaBase<Criteria>
    {
      private int _id;
      public int Id
      {
        get { return _id; }
        private set { _id = value; }
      }

      public Criteria(){}
      public Criteria(int id):base(typeof(Single))
      { Id = id; }

      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        base.OnGetState(info, mode);
        info.AddValue("_id", _id);
      }
      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        base.OnSetState(info, mode);
        _id = info.GetValue<int>("_id");
      }
    }

#if !SILVERLIGHT

    #region DataPortal_Create

    protected override void DataPortal_Create()
    {
      DoCreate(0);
    }
    protected void DataPortal_Create(Criteria criteria)
    {
      DoCreate(criteria.Id);
    }

    private void DoCreate(int id)
    {
      Id = id;
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Single", "Created");
      MethodCalled = "Created";
      if (id == 9999)
        throw new Exception("Bad data");
    }

    #endregion

    #region DataPortal_Fetch

    protected void DataPortal_Fetch()
    {
      DoFetch(0);
    }
    private void DataPortal_Fetch(Criteria criteria)
    {
      DoFetch(criteria.Id);
    }

    private void DoFetch(int id)
    {
      Id = id;
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Single", "Fetched");
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

    protected override void DataPortal_Update()
    {
      DoInsertUpdate(true);
    }

    private void DoInsertUpdate(bool isUpdate)
    {
      var insertOrUpdate = isUpdate ? "Updated" : "Inserted";

      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Single", insertOrUpdate);
      MethodCalled = insertOrUpdate;
    }



    #endregion


    protected override void DataPortal_DeleteSelf()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Single", "SelfDeleted");
      MethodCalled = "SelfDeleted";
    }




    private void DataPortal_Delete(Criteria criteria)
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Single", "Deleted");
      MethodCalled = "Deleted";
    }
#endif




    #endregion
  }
}
