//-----------------------------------------------------------------------
// <copyright file="Single.cs" company="Marimer LLC">
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
using System.Threading.Tasks;

using Csla.Core;
using Csla.Server;

namespace Csla.Test.DataPortalTest
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [Serializable]
  public class Single : BusinessBase<Single>
  {
    public readonly static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public readonly static PropertyInfo<string> MethodCalledProperty = RegisterProperty(new PropertyInfo<string>("MethodCalled", "MethodCalled"));
    public string MethodCalled
    {
      get { return GetProperty(MethodCalledProperty); }
      set { SetProperty(MethodCalledProperty, value); }
    }

#if !SILVERLIGHT
    public static Single NewObject()
    {
      return Csla.DataPortal.Create<Single>();
    }

    public static Single GetObject(int id)
    {
      return Csla.DataPortal.Fetch<Single>(id);
    }

    public static void DeleteObject(int id)
    {
      Csla.DataPortal.Delete<Single>(id);
    }
#endif

    public static void DeleteObject(int id, EventHandler<DataPortalResult<Single>> handler)
    {
      Csla.DataPortal.BeginDelete<Single>(id, handler);
    }

    public static void DeleteObject(int id, EventHandler<DataPortalResult<Single>> handler, object userState)
    {
      Csla.DataPortal.BeginDelete<Single>(id, handler, userState);
    }

    public Single()
    { }

#if !SILVERLIGHT

    protected override void DataPortal_Create()
    {
      DoCreate(0);
    }
    protected void DataPortal_Create(int id)
    {
      DoCreate(id);
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

    private void DataPortal_Fetch()
    {
      DoFetch(0);
    }

    private void DataPortal_Fetch(int id)
    {
      DoFetch(id);
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

    protected override void DataPortal_DeleteSelf()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Single", "SelfDeleted");
      MethodCalled = "SelfDeleted";
    }

    private void DataPortal_Delete(int id)
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("Single", "Deleted");
      MethodCalled = "Deleted";
    }
#endif
  }

  [Serializable]
  [ObjectFactory(typeof(SingleWithFactoryFactory))]
  public class SingleWithFactory : BusinessBase<SingleWithFactory>
  {
  }

  public class SingleWithFactoryFactory : ObjectFactory
  {
    public async Task<object> Fetch()
    {
      return await Task.Run(() => new SingleWithFactory());
    }

    public async Task<object> Create()
    {
      return await Task.Run(() => new SingleWithFactory());
    }
  }

  [Serializable]
  public class Single2 : BusinessBase<Single2>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

#if !SILVERLIGHT
    private void DataPortal_Create(int id)
    {
      if (id == 9999)
        throw new Exception("bad value");
      Id = id;
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch()
    {
      DataPortal_Fetch(0);
    }

    private void DataPortal_Fetch(int id)
    {
      if (id == 9999)
        throw new Exception("bad value");
      Id = id;
      base.DataPortal_Create();
    }

    protected override void DataPortal_Insert()
    {
      if (Id == 555)
        throw new Exception("bad value");
    }

    protected override void DataPortal_Update()
    {
      if (Id == 555)
        throw new Exception("bad value");
    }

    protected override void DataPortal_DeleteSelf()
    {
      if (Id == 555)
        throw new Exception("bad value");
    }

    private void DataPortal_Delete(int id)
    {
      if (Id == 555)
        throw new Exception("bad value");
      Id = id;
    }
#endif
  }

  [Serializable]
  public class SingleCommand : CommandBase<SingleCommand>
  {
    public static readonly PropertyInfo<int> ValueProperty = RegisterProperty<int>(c => c.Value);
    public int Value
    {
      get { return ReadProperty(ValueProperty); }
      set { LoadProperty(ValueProperty, value); }
    }

#if !SILVERLIGHT
    protected override void DataPortal_Execute()
    {
      if (Value == 555)
        throw new Exception("bad value");
      Value += 1;
    }
#endif
  }
}