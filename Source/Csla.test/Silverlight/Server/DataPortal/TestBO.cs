//-----------------------------------------------------------------------
// <copyright file="TestBO.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
#if SILVERLIGHT && !ANDROID
using Csla.Serialization;
#else
using System;
using System.Configuration;

#endif


namespace Csla.Testing.Business.DataPortal
{
  [Serializable]
  public class TestBO : BusinessBase<TestBO>
  {
#if !SILVERLIGHT
    
    protected override void DataPortal_Create()
    {
      //base.DataPortal_Create();
      string x = ConfigurationManager.AppSettings["CslaAuthorizationProvider"];
    }
    protected void DataPortal_Fetch(SingleCriteria<TestBO,int> criteria)
    {

    }

    protected void DataPortal_Fetch(object citeria)
    {

    }

    protected void DataPortal_Fetch()
    {

    }

    protected override void DataPortal_Insert()
    {

    }
    protected void DataPortal_Delete(object criteria)
    {

    }
    protected void DataPortal_Delete(SingleCriteria<TestBO,int> criteria)
    {

    }
#else
    public TestBO()
    {
      MarkNew();
    }
#endif

  }
}