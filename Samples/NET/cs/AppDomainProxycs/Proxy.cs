using System;
using System.Threading;
using Csla;
using Csla.Server;

namespace AppDomainProxy
{
  class Proxy : Csla.DataPortalClient.IDataPortalProxy
  {
    AppDomain _appDomain;
    IDataPortalServer _portal;

    IDataPortalServer Portal
    {
      get
      {
        if (_appDomain == null)
        {
          AppDomain current = AppDomain.CurrentDomain;
          AppDomainSetup setup = current.SetupInformation;
          setup.ApplicationName = "Csla.DataPortal";
          _appDomain = 
            AppDomain.CreateDomain("ServerDomain", current.Evidence, setup);
          current.DomainUnload += UnloadDomain;
        }
        if (_portal == null)
          _portal = (IDataPortalServer)_appDomain.CreateInstanceAndUnwrap(
            "Csla", "Csla.Server.Hosts.RemotingPortal");
        return _portal;
      }
    }

    void UnloadDomain(object sender, EventArgs e)
    {
      AppDomain.CurrentDomain.DomainUnload -= UnloadDomain;
      AppDomain.Unload(_appDomain);
    }

    #region Create

    private class CreateTask
    {
      public Type ObjectType;
      public object Criteria;
      public DataPortalContext Context;
      public DataPortalResult Result;
    }

    public DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
    {
      Thread t = new Thread(DoCreate);
      CreateTask task = new CreateTask();
      task.ObjectType = objectType;
      task.Criteria = criteria;
      task.Context = context;
      t.Start(task);
      t.Join();
      return task.Result;
    }

    void DoCreate(object state)
    {
      CreateTask task = state as CreateTask;
      task.Result = Portal.Create(task.ObjectType, task.Criteria, task.Context);
    }

    #endregion

    #region Fetch

    private class FetchTask
    {
      public Type ObjectType;
      public object Criteria;
      public DataPortalContext Context;
      public DataPortalResult Result;
    }

    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      Thread t = new Thread(DoFetch);
      FetchTask task = new FetchTask();
      task.ObjectType = objectType;
      task.Criteria = criteria;
      task.Context = context;
      t.Start(task);
      t.Join();
      return task.Result;
    }

    void DoFetch(object state)
    {
      FetchTask task = state as FetchTask;
      task.Result = Portal.Fetch(task.ObjectType, task.Criteria, task.Context);
    }

    #endregion

    #region Update

    private class UpdateTask
    {
      public object Obj;
      public DataPortalContext Context;
      public DataPortalResult Result;
    }

    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      Thread t = new Thread(DoUpdate);
      UpdateTask task = new UpdateTask();
      task.Obj = obj;
      task.Context = context;
      t.Start(task);
      t.Join();
      return task.Result;
    }

    void DoUpdate(object state)
    {
      UpdateTask task = state as UpdateTask;
      task.Result = Portal.Update(task.Obj, task.Context);
    }

    #endregion

    #region Delete

    private class DeleteTask
    {
      public Type ObjectType;
      public object Criteria;
      public DataPortalContext Context;
      public DataPortalResult Result;
    }

    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      Thread t = new Thread(DoDelete);
      DeleteTask task = new DeleteTask();
      task.ObjectType = objectType;
      task.Criteria = criteria;
      task.Context = context;
      t.Start(task);
      t.Join();
      return task.Result;
    }

    void DoDelete(object state)
    {
      DeleteTask task = state as DeleteTask;
      task.Result = Portal.Delete(task.ObjectType, task.Criteria, task.Context);
    }

    #endregion

    public bool IsServerRemote
    {
      get { return true; }
    }

  }
}
