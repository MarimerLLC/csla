//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryBusinessItemFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.ObjectFactory
{
  public class ObjectFactoryBusinessItemFactory : Csla.Server.ObjectFactory
  {
    public ObjectFactoryBusinessItemFactory() { }

#if SILVERLIGHT
    public void CreateObjectFactoryBusinessItem(FactoryProxy<ObjectFactoryBusinessItem>.CompletedHandler handler)
    {
      ObjectFactoryBusinessItem item = new ObjectFactoryBusinessItem();
      item.Id = "random_create";
      item.OperationResult = "CreateObjectFactoryBusinessItem/no parameters";
      MarkNew(item);
      handler(item, null);
    }

    public void CreateObjectFactoryBusinessItem(SingleCriteria<ObjectFactoryBusinessItem, string> criteria, FactoryProxy<ObjectFactoryBusinessItem>.CompletedHandler handler)
    {
      ObjectFactoryBusinessItem item = new ObjectFactoryBusinessItem();
      item.Id = "create_" + criteria.Value;
      item.OperationResult = "CreateObjectFactoryBusinessItem/with parameters";
      MarkNew(item);
      handler(item, null);

    }

    public void FetchObjectFactoryBusinessItem(FactoryProxy<ObjectFactoryBusinessItem>.CompletedHandler handler)
    {
      ObjectFactoryBusinessItem item = new ObjectFactoryBusinessItem();
      item.Id = "random_fetch";
      item.OperationResult = "FetchObjectFactoryBusinessItem/no parameters";
      MarkOld(item);
      handler(item, null);
    }

    public void FetchObjectFactoryBusinessItem(SingleCriteria<ObjectFactoryBusinessItem, string> criteria, FactoryProxy<ObjectFactoryBusinessItem>.CompletedHandler handler)
    {
      ObjectFactoryBusinessItem item = new ObjectFactoryBusinessItem();
      item.Id = "fetch_" + criteria.Value;
      item.OperationResult = "FetchObjectFactoryBusinessItem/with parameters";
      MarkOld(item);
      handler(item, null);
    }

    public void DeleteObjectFactoryBusinessItem(SingleCriteria<ObjectFactoryBusinessItem, string> criteria, FactoryProxy<ObjectFactoryBusinessItem>.CompletedHandler handler)
    {
      ObjectFactoryBusinessItem item = new ObjectFactoryBusinessItem();
      item.Id = "delete_" + criteria.Value;
      item.OperationResult = "DeleteObjectFactoryBusinessItem/with parameters";
      handler(item, null);
    }

    public void UpdateObjectFactoryBusinessItem(ObjectFactoryBusinessItem item, FactoryProxy<ObjectFactoryBusinessItem>.CompletedHandler handler)
    {
      if (item.IsNew)
      {
        item.Id = "inserted";
        item.OperationResult = "UpdateObjectFactoryBusinessItem/with parameters";
        MarkOld(item);
        handler(item, null);
      }
      else
      {
        item.Id = "updated";
        item.OperationResult = "UpdateObjectFactoryBusinessItem/with parameters";
        MarkOld(item);
        handler(item, null);
      }

    }
#endif
  }
}