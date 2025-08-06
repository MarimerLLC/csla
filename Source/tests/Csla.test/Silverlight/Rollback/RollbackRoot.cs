//-----------------------------------------------------------------------
// <copyright file="RollbackRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
    private RollbackRoot() { }

    private static PropertyInfo<int> UnInitedProperty = RegisterProperty<int>(c => c.UnInitedP, "UnInitedP");
    public int UnInitedP
    {
      get { return GetProperty(UnInitedProperty); }
      set { SetProperty(UnInitedProperty, value); }
    }

    private static PropertyInfo<string> AnotherProperty = RegisterProperty<string>(c => c.Another, "Another");
    public string Another
    {
      get { return GetProperty(AnotherProperty); }
      set { SetProperty(AnotherProperty, value); }
    }

  }
}