//-----------------------------------------------------------------------
// <copyright file="BusinessItem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Silverlight;
using Csla.Serialization;
using Csla;
using Csla.Server;

namespace Csla.Testing.Business.ObjectFactory
{
  [Serializable]
  public class BusinessItem : BusinessBase<BusinessItem>
  {
    public BusinessItem() { }
    private static PropertyInfo<string> IdProperty = RegisterProperty<string>(new PropertyInfo<string>("Id", "Id"));

    public string Id
    {
      get
      {
        return GetProperty(IdProperty);
      }
      set
      {
        SetProperty(IdProperty, value);
      }
    }

    private static PropertyInfo<string> OperationResultProperty = RegisterProperty<string>(new PropertyInfo<string>("OperationResult", "Operation Result"));

    public string OperationResult
    {
      get
      {
        return GetProperty(OperationResultProperty);
      }
      set
      {
        SetProperty(OperationResultProperty, value);
      }
    }


    public static void GetBusinessItem(string id, EventHandler<DataPortalResult<BusinessItem>> completed)
    {
      DataPortal<BusinessItem> dp = new DataPortal<BusinessItem>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(new SingleCriteria<BusinessItem,string>(id));
    }

    public static void GetBusinessItem(EventHandler<DataPortalResult<BusinessItem>> completed)
    {
      DataPortal<BusinessItem> dp = new DataPortal<BusinessItem>();
      dp.FetchCompleted += completed;
      dp.BeginFetch();
    }

    public static void NewBusinessItem(string id, EventHandler<DataPortalResult<BusinessItem>> completed)
    {
      DataPortal<BusinessItem> dp = new DataPortal<BusinessItem>();
      dp.CreateCompleted += completed;
      dp.BeginCreate(new SingleCriteria<BusinessItem, string>(id));
    }

    public static void NewBusinessItem(EventHandler<DataPortalResult<BusinessItem>> completed)
    {
      DataPortal<BusinessItem> dp = new DataPortal<BusinessItem>();
      dp.CreateCompleted += completed;
      dp.BeginCreate();
    }

    public static void DeleteBusinessItem(string id, EventHandler<DataPortalResult<BusinessItem>> completed)
    {
      DataPortal<BusinessItem> dp = new DataPortal<BusinessItem>();
      dp.DeleteCompleted += completed;
      dp.BeginDelete(new SingleCriteria<BusinessItem, string>(id));
    }

#if SILVERLIGHT

    public void DataPortal_Fetch(Csla.DataPortalClient.LocalProxy<BusinessItem>.CompletedHandler completed)
    {
      this.Id = "random_fetch";
      this.OperationResult = "DataPortal_Fetch/no parameters";
      this.MarkOld();
      completed.Invoke(this, null);
    }

    public void DataPortal_Fetch(SingleCriteria<BusinessItem, string> criteria, Csla.DataPortalClient.LocalProxy<BusinessItem>.CompletedHandler completed)
    {
      this.Id = "fetch_" + criteria.Value;
      this.OperationResult = "DataPortal_Fetch/with parameters";
      this.MarkOld();
      completed.Invoke(this, null);
    }

    public override void DataPortal_Create(Csla.DataPortalClient.LocalProxy<BusinessItem>.CompletedHandler completed)
    {
      this.Id = "random_create";
      this.OperationResult = "DataPortal_Create/no parameters";
      this.MarkNew();
      completed.Invoke(this, null);
    }

    public void DataPortal_Create(SingleCriteria<BusinessItem, string> criteria, Csla.DataPortalClient.LocalProxy<BusinessItem>.CompletedHandler completed)
    {
      this.Id = "create_" + criteria.Value;
      this.OperationResult = "DataPortal_Create/with parameters";
      this.MarkNew();
      completed.Invoke(this, null);
    }

    public void DataPortal_Delete(SingleCriteria<BusinessItem, string> criteria, Csla.DataPortalClient.LocalProxy<BusinessItem>.CompletedHandler completed)
    {
      this.Id = "delete_" + criteria.Value;
      this.OperationResult = "DeleteObjectFactoryBusinessItem/with parameters";
      this.MarkDeleted();
      completed.Invoke(this, null);
    }

    public override void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<BusinessItem>.CompletedHandler completed)
    {
      this.Id = "random_insert";
      this.OperationResult = "DataPortal_Insert";
      this.MarkOld();
      completed.Invoke(this, null);
    }

    public override void DataPortal_Update(Csla.DataPortalClient.LocalProxy<BusinessItem>.CompletedHandler completed)
    {
      this.Id = "random_update";
      this.OperationResult = "DataPortal_Update";
      this.MarkOld();
      completed.Invoke(this, null);
    }

#endif
  }
}