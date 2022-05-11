//-----------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
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
  [Csla.Server.ObjectFactory("Csla.Test.ObjectFactory.RootFactory, Csla.Tests")]
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    internal static PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    private static PropertyInfo<Csla.ApplicationContext.ExecutionLocations> LocationProperty = RegisterProperty<Csla.ApplicationContext.ExecutionLocations>(nameof(Location));
    public Csla.ApplicationContext.ExecutionLocations Location
    {
      get { return GetProperty(LocationProperty); }
      set { SetProperty(LocationProperty, value); }
    }

    private static PropertyInfo<TransactionalTypes> TransactionalTypeProperty = RegisterProperty<TransactionalTypes>(nameof(TransactionalType));
    public TransactionalTypes TransactionalType
    {
      get { return GetProperty(TransactionalTypeProperty); }
      set { SetProperty(TransactionalTypeProperty, value); }
    }

    private static PropertyInfo<string> IsolationLevelProperty = RegisterProperty<string>(nameof(IsolationLevel));
    public string IsolationLevel
    {
      get { return GetProperty(IsolationLevelProperty); }
      set { SetProperty(IsolationLevelProperty, value); }
    }

    private static PropertyInfo<int> TransactionTimeoutProperty = RegisterProperty<int>(nameof(TransactionTimeout));
    public int TransactionTimeout
    {
      get { return GetProperty(TransactionTimeoutProperty); }
      set { SetProperty(TransactionTimeoutProperty, value); }
    }

    public ApplicationContext.ExecutionLocations ExecutionLocation
    {
      get { return ApplicationContext.ExecutionLocation; }
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