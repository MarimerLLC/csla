//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryBusinessItem.cs" company="Marimer LLC">
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
  [Serializable(), ObjectFactory("Csla.Testing.Business.ObjectFactory.ObjectFactoryBusinessItemFactory, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null", 
    "CreateObjectFactoryBusinessItem",
    "FetchObjectFactoryBusinessItem",
    "UpdateObjectFactoryBusinessItem",
    "DeleteObjectFactoryBusinessItem")]
  public class ObjectFactoryBusinessItem : BusinessBase<ObjectFactoryBusinessItem>
  {
    public ObjectFactoryBusinessItem() { }
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


    public static void GetObjectFactoryBusinessItem(string id, EventHandler<DataPortalResult<ObjectFactoryBusinessItem>> completed)
    {
      DataPortal<ObjectFactoryBusinessItem> dp = new DataPortal<ObjectFactoryBusinessItem>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(id);
    }

    public static void GetObjectFactoryBusinessItem(EventHandler<DataPortalResult<ObjectFactoryBusinessItem>> completed)
    {
      DataPortal<ObjectFactoryBusinessItem> dp = new DataPortal<ObjectFactoryBusinessItem>();
      dp.FetchCompleted += completed;
      dp.BeginFetch();
    }

    public static void NewObjectFactoryBusinessItem(string id, EventHandler<DataPortalResult<ObjectFactoryBusinessItem>> completed)
    {
      DataPortal<ObjectFactoryBusinessItem> dp = new DataPortal<ObjectFactoryBusinessItem>();
      dp.CreateCompleted += completed;
      dp.BeginCreate(id);
    }

    public static void NewObjectFactoryBusinessItem(EventHandler<DataPortalResult<ObjectFactoryBusinessItem>> completed)
    {
      DataPortal<ObjectFactoryBusinessItem> dp = new DataPortal<ObjectFactoryBusinessItem>();
      dp.CreateCompleted += completed;
      dp.BeginCreate();
    }

    public static void DeleteObjectFactoryBusinessItem(string id, EventHandler<DataPortalResult<ObjectFactoryBusinessItem>> completed)
    {
      DataPortal<ObjectFactoryBusinessItem> dp = new DataPortal<ObjectFactoryBusinessItem>();
      dp.DeleteCompleted += completed;
      dp.BeginDelete(id);
    }

  }
}