//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryBusinessItem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
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
  }
}