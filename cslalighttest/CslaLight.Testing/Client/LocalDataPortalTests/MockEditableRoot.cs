using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Csla.Serialization;

namespace Csla.Testing.LocalDataPortalTests.EditableRootTests
{
  [Serializable]
  public partial class MockEditableRoot : BusinessBase<MockEditableRoot>
  {
    public static readonly Guid MockEditableRootId = new Guid("{7E7127CF-F22C-4903-BDCE-1714C81A9E89}");

    #region  Business Methods

    private static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(
      typeof(MockEditableRoot),
      new PropertyInfo<Guid>("Id"));

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(
      typeof(MockEditableRoot),
      new PropertyInfo<string>("Name"));

    private static PropertyInfo<string> DataPortalMethodProperty = RegisterProperty<string>(
      typeof(MockEditableRoot),
      new PropertyInfo<string>("DataPortalMethod"));

    public Guid Id
    {
      get { return GetProperty<Guid>(IdProperty); }
    }

    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    public string DataPortalMethod
    {
      get { return GetProperty<string>(DataPortalMethodProperty); }
      set { SetProperty<string>(DataPortalMethodProperty, value); }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    #endregion

    #region  Validation Rules

    //protected override void AddBusinessRules()
    //{
    //  // TODO: add business rules
    //}

    #endregion

    #region  Authorization Rules

    //protected override void AddAuthorizationRules()
    //{
    //  // add AuthorizationRules here
    //}

    //protected static void AddObjectAuthorizationRules()
    //{
    //  // add object-level authorization rules here
    //}

    #endregion

    public MockEditableRoot() { }

    public MockEditableRoot(Guid id, bool isnew)
    {
      SetProperty<Guid>(IdProperty, id);
      if (!isnew)
        MarkOld();
    }

    #region  Factory Methods

    public static void CreateNew(Action<MockEditableRoot, Exception> completed)
    {
      Csla.DataPortal.ProxyTypeName = "Local";
      DataPortal<MockEditableRoot> dp = new DataPortal<MockEditableRoot>();
      dp.CreateCompleted += (o, e) => { completed(e.Object, e.Error); };
      dp.BeginCreate();
    }

    public static void Fetch(Guid id, Action<MockEditableRoot, Exception> completed)
    {
      DataPortal<MockEditableRoot> dp = new DataPortal<MockEditableRoot>();
      dp.FetchCompleted += (o, e) =>
      {
        if (completed != null)
          completed(e.Object, e.Error);
      };
      dp.BeginFetch(new SingleCriteria<MockEditableRoot, Guid>(id));
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
      dp.BeginDelete(new SingleCriteria<MockEditableRoot, Guid>(id));
    }

    #endregion

    #region Data Access

    public void DataPortal_Create(Csla.DataPortalClient.LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      //ValidationRules.CheckRules();

      LoadProperty<string>(DataPortalMethodProperty, "create");

      handler(this, null);
    }

    public void DataPortal_Fetch(
      Csla.DataPortalClient.LocalProxy<MockEditableRoot>.CompletedHandler handler,
      SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      if (criteria.Value != MockEditableRootId)
        throw new Exception();

      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      LoadProperty<string>(DataPortalMethodProperty, "fetch");

      handler(this, null);
    }

    public void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "insert");
      handler(this, null);
    }

    public void DataPortal_Update(Csla.DataPortalClient.LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "update");
      handler(this, null);
    }

    public void DataPortal_DeleteSelf(Csla.DataPortalClient.LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      DataPortal_Delete(handler, new SingleCriteria<MockEditableRoot, Guid>(this.Id));
      handler(this, null);
    }

    public void DataPortal_Delete(
      Csla.DataPortalClient.LocalProxy<MockEditableRoot>.CompletedHandler handler,
      SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      LoadProperty<string>(DataPortalMethodProperty, "delete");
      handler(this, null);
    }

    #endregion

  }
}
