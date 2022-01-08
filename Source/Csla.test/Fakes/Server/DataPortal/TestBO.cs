//-----------------------------------------------------------------------
// <copyright file="TestBO.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Configuration;

namespace Csla.Testing.Business.DataPortal
{
  [Serializable]
  public class TestBO : BusinessBase<TestBO>
  {
    [Create]
		protected void DataPortal_Create()
    {
      //base.DataPortal_Create();
      string x = ConfigurationManager.AppSettings["CslaAuthorizationProvider"];
    }
    protected void DataPortal_Fetch(int criteria)
    {

    }

    protected void DataPortal_Fetch(object citeria)
    {

    }

    protected void DataPortal_Fetch()
    {

    }

    [Insert]
    protected void DataPortal_Insert()
    {

    }

    [Delete]
	protected void DataPortal_Delete(object criteria)
    {

    }

    [Delete]
		protected void DataPortal_Delete(int criteria)
    {

    }
  }
}