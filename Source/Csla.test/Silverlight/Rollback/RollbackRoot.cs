//-----------------------------------------------------------------------
// <copyright file="RollbackRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.DataPortalClient;
using Csla.Serialization;

namespace Csla.Test.Silverlight.Rollback
{
  [Serializable]
  public class RollbackRoot : BusinessBase<RollbackRoot>
  {
    #if SILVERLIGHT
    public RollbackRoot() { }

    public static void BeginCreateLocal(EventHandler<DataPortalResult<RollbackRoot>> callback)
    {
      var dp = new DataPortal<RollbackRoot>(Csla.DataPortal.ProxyModes.LocalOnly);
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }



    public override void DataPortal_Create(LocalProxy<RollbackRoot>.CompletedHandler handler)
    {
      base.DataPortal_Create(handler);
    }
    #else
    private RollbackRoot() { }
    #endif


    private static PropertyInfo<int> UnInitedProperty = RegisterProperty(new PropertyInfo<int>("UnInitedP", "UnInitedP"));
    public int UnInitedP
    {
      get { return GetProperty(UnInitedProperty); }
      set { SetProperty(UnInitedProperty, value); }
    }

    private static PropertyInfo<string> AnotherProperty = RegisterProperty(new PropertyInfo<string>("Another", "Another"));
    public string Another
    {
      get { return GetProperty(AnotherProperty); }
      set { SetProperty(AnotherProperty, value); }
    }

  }
}