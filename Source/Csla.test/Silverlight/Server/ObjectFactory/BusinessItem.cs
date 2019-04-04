//-----------------------------------------------------------------------
// <copyright file="BusinessItem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
      dp.BeginFetch(id);
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
      dp.BeginCreate(id);
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
      dp.BeginDelete(id);
    }
  }
}