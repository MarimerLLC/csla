﻿//-----------------------------------------------------------------------
// <copyright file="Single.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Server;

namespace Csla.Test.DataPortalTest
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [Serializable]
  public class Single : BusinessBase<Single>
  {
    public readonly static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public readonly static PropertyInfo<string> MethodCalledProperty = RegisterProperty<string>(c => c.MethodCalled, "MethodCalled");
    public string MethodCalled
    {
      get { return GetProperty(MethodCalledProperty); }
      set { SetProperty(MethodCalledProperty, value); }
    }

    public Single()
    { }

    [Create]
    protected void DataPortal_Create()
    {
      DoCreate(0);
      BusinessRules.CheckRules();
    }

    [Create]
    protected void DataPortal_Create(int id)
    {
      DoCreate(id);
      BusinessRules.CheckRules();
    }

    private void DoCreate(int id)
    {
      Id = id;
      TestResults.Reinitialise();
      TestResults.Add("Single", "Created");
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
      TestResults.Reinitialise();
      TestResults.Add("Single", "Fetched");
      MethodCalled = "Fetched";
      if (id == 9999)
        throw new Exception("Bad data");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      DoInsertUpdate(false);
    }

    [Update]
    protected void DataPortal_Update()
    {
      DoInsertUpdate(true);
    }

    private void DoInsertUpdate(bool isUpdate)
    {
      var insertOrUpdate = isUpdate ? "Updated" : "Inserted";

      TestResults.Reinitialise();
      TestResults.Add("Single", insertOrUpdate);
      MethodCalled = insertOrUpdate;
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Reinitialise();
      TestResults.Add("Single", "SelfDeleted");
      MethodCalled = "SelfDeleted";
    }

    [Delete]
    private void DataPortal_Delete(int id)
    {
      TestResults.Reinitialise();
      TestResults.Add("Single", "Deleted");
      MethodCalled = "Deleted";
    }
  }

  [Serializable]
  [ObjectFactory(typeof(SingleWithFactoryFactory))]
  public class SingleWithFactory : BusinessBase<SingleWithFactory>;

  public class SingleWithFactoryFactory : ObjectFactoryLoader
  {
    public SingleWithFactoryFactory(ApplicationContext applicationContext) : base(applicationContext)
    {

    }

    public async Task<object> Fetch()
    {
      return await Task.Run(() => new SingleWithFactory());
    }

    public async Task<object> Create(int id)
    {
      Console.WriteLine($"Create {id},{Thread.GetCurrentProcessorId()}, {Thread.CurrentThread.ManagedThreadId}");
      await Task.Delay(1);
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

    private void DataPortal_Create(int id)
    {
      if (id == 9999)
        throw new Exception("bad value");
      Id = id;
      BusinessRules.CheckRules();
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
      BusinessRules.CheckRules();
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      if (Id == 555)
        throw new Exception("bad value");
    }

    [Update]
    protected void DataPortal_Update()
    {
      if (Id == 555)
        throw new Exception("bad value");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      if (Id == 555)
        throw new Exception("bad value");
    }

    [Delete]
    private void DataPortal_Delete(int id)
    {
      if (Id == 555)
        throw new Exception("bad value");
      Id = id;
    }
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

    [RunLocal]
    [Create]
    private void Create()
    { }

    [RunLocal]
    [Create]
    private void Create(int value)
    {
      Value = value;
    }

    [Execute]
    protected void DataPortal_Execute()
    {
      if (Value == 555)
        throw new Exception("bad value");
      Value += 1;
    }

    [Execute]
    protected void DataPortal_ExecuteWithInt(int value)
    {
      Value += value;
    }
  }
}