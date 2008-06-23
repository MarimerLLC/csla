using System;
using Csla.Serialization;

namespace Csla.Testing.Business.EditableRootTests
{
  partial class MockEditableRoot
  {
    private const string DataPortalUrl = "http://localhost:2752/WcfPortal.svc";

    public MockEditableRoot() { }
    public MockEditableRoot(Guid id, bool isnew)
    {
      //SetProperty<Guid>(IdProperty, id);
      //if(isnew)
    }

    #region  Factory Methods

    public static void CreateNew(Action<MockEditableRoot, Exception> completed)
    {
      DataPortal<MockEditableRoot> dp = new DataPortal<MockEditableRoot>(DataPortalUrl);
      dp.CreateCompleted += (o, e) => { completed(e.Object, e.Error); };
      dp.BeginCreate();
    }

    public static void Fetch(Guid id, Action<MockEditableRoot, Exception> completed)
    {
      DataPortal<MockEditableRoot> dp = new DataPortal<MockEditableRoot>(DataPortalUrl);
      dp.FetchCompleted += (o, e) => { completed(e.Object, e.Error); };
      dp.BeginFetch(new SingleCriteria<MockEditableRoot, Guid>(id));
    }

    public static void Delete(Guid id) { Delete(id, null); }
    public static void Delete(Guid id, Action<MockEditableRoot, Exception> completed)
    {
      DataPortal<MockEditableRoot> dp = new DataPortal<MockEditableRoot>(DataPortalUrl);
      dp.DeleteCompleted += (o, e) => 
      {
        if (completed != null)
          completed(e.Object, e.Error);
      };
      dp.BeginDelete(new SingleCriteria<MockEditableRoot, Guid>(id));
    }

    #endregion
  }
}
