using System;
using Csla.Serialization;

namespace Csla.Testing.Business.EditableRootTests
{
  partial class MockEditableRoot
  {    
    #region  Factory Methods

    public static void CreateNew(Action<MockEditableRoot, Exception> completed)
    {
        DataPortal<MockEditableRoot> dp = new DataPortal<MockEditableRoot>();
        dp.CreateCompleted += (o, e) => { completed(e.Object, e.Error); };
        dp.SendCreate();
    }

    public static void Fetch(Guid id, Action<MockEditableRoot, Exception> completed)
    {
      DataPortal<MockEditableRoot> dp = new DataPortal<MockEditableRoot>();
      dp.FetchCompleted += (o, e) => { completed(e.Object, e.Error); };
      dp.SendFetch(new SingleCriteria<MockEditableRoot, Guid>(id));
    }

    public static void Delete(Guid id) { Delete(id, null); }
    public static void Delete(Guid id, Action<MockEditableRoot, Exception> completed)
    {
      DataPortal<MockEditableRoot> dp = new DataPortal<MockEditableRoot>();
      dp.DeleteCompleted += (o, e) => 
      {
        if (completed != null)
          completed(e.Object, e.Error);
      };
      dp.SendDelete(new SingleCriteria<MockEditableRoot, Guid>(id));
    }

    #endregion
  }
}
