//-----------------------------------------------------------------------
// <copyright file="MissingRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace Csla.Test.ObjectFactory
{
  [Csla.Server.ObjectFactory("MissingRootFactory")]
  [Serializable]
  public class MissingRoot : BusinessBase<Root>
  {
    private static PropertyInfo<string> DataProperty = RegisterProperty(new PropertyInfo<string>("Data"));
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    private static PropertyInfo<Csla.ApplicationContext.ExecutionLocations> LocationProperty = RegisterProperty(new PropertyInfo<Csla.ApplicationContext.ExecutionLocations>("Location"));
    public Csla.ApplicationContext.ExecutionLocations Location
    {
      get { return GetProperty(LocationProperty); }
      set { SetProperty(LocationProperty, value); }
    }

    public void MarkAsNew()
    {
      MarkNew();
    }

    public void MarkAsOld()
    {
      MarkOld();
    }
  }
}