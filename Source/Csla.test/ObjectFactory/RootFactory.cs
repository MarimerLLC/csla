//-----------------------------------------------------------------------
// <copyright file="RootFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Test.ObjectFactory
{
  public class RootFactory : Csla.Server.ObjectFactory
  {
    public RootFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public object Create()
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = "Create";
      obj.Location = obj.ExecutionLocation;
      obj.MarkAsNew();
      return obj;
    }

    public object Create(int test)
    {
      // add overload to test reflection
      return null;
    }

    [RunLocal]
    public object Create(string criteria)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = "Create " + criteria;
      obj.Location = obj.ExecutionLocation;
      obj.MarkAsNew();
      return obj;
    }

    public object Fetch()
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = "Fetch";
      obj.MarkAsOld();
      return obj;
    }

    public object Fetch(string criteria)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = criteria;
      obj.MarkAsOld();
      return obj;
    }

    public object Update(Root obj)
    {
      obj.TransactionalType = _transactionalType;
      obj.Data = "Update";
      obj.MarkAsOld();
      return obj;
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    public object Update2(Root obj)
    {
      obj.TransactionalType = _transactionalType;
      obj.Data = "Update";
      obj.MarkAsOld();
      return obj;
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    public object Update3(Root obj)
    {
      obj.TransactionalType = _transactionalType;
      obj.Data = "Update";
      obj.MarkAsOld();
      return obj;
    }

    public void Delete(string criteria)
    {
      TestResults.Add("ObjectFactory", "Delete");
    }

    private TransactionalTypes _transactionalType;

    public void Invoke(Csla.DataPortalEventArgs args)
    {
      _transactionalType = args.DataPortalContext.TransactionalType;
    }
  }

  public class RootFactoryC : Csla.Server.ObjectFactory
  {
    public RootFactoryC(ApplicationContext applicationContext) 
      : base(applicationContext) { }

    public object Create(int test)
    {
      // add overload to test reflection
      return null;
    }

    [RunLocal]
    public object Create(string criteria)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = "Create " + criteria;
      obj.Location = obj.ExecutionLocation;
      obj.MarkAsNew();
      return obj;
    }

    private TransactionalTypes _transactionalType;

    public void Invoke(Csla.DataPortalEventArgs args)
    {
      _transactionalType = args.DataPortalContext.TransactionalType;
    }
  }

  public class RootFactory1 : Csla.Server.ObjectFactory
  {
    public RootFactory1(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public object Fetch()
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      LoadProperty(obj, Root.DataProperty, "Fetch");
      this.MarkOld(obj);
      return obj;
    }
    
    [Transactional(TransactionalTypes.TransactionScope)]
    public object Update(Root obj)
    {

      obj.IsolationLevel = System.Transactions.Transaction.Current.IsolationLevel.ToString();
      obj.TransactionTimeout = ApplicationContext.DefaultTransactionTimeoutInSeconds;
      obj.TransactionalType = _transactionalType;
      obj.Data = "Update";
      obj.MarkAsOld();
      return obj;
    }

    private TransactionalTypes _transactionalType;

    public void Invoke(Csla.DataPortalEventArgs args)
    {
      _transactionalType = args.DataPortalContext.TransactionalType;
    }
  }

  //public class RootFactory2
  //{
  //  [Transactional(TransactionalTypes.EnterpriseServices)]
  //  public object Update(Root obj)
  //  {
  //    obj.TransactionalType = _transactionalType;
  //    obj.Data = "Update";
  //    obj.MarkAsOld();
  //    return obj;
  //  }
  //
  //  private TransactionalTypes _transactionalType;
  //
  //  public void Invoke(Csla.DataPortalEventArgs args)
  //  {
  //    _transactionalType = args.DataPortalContext.TransactionalType;
  //  }
  //}

  public class RootFactory3 : Csla.Server.ObjectFactory
  {
    public RootFactory3(ApplicationContext applicationContext) 
      : base(applicationContext) { }

    public object Fetch()
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      LoadProperty(obj, Root.DataProperty, "Fetch");
      this.MarkOld(obj);
      return obj;
    }
  }

  public class RootFactory4 : Csla.Server.ObjectFactory
  {
    public RootFactory4(ApplicationContext applicationContext)
      : base(applicationContext) { }

    [RunLocal]
    public object Create()
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Location = obj.ExecutionLocation;
      obj.MarkAsNew();
      return obj;
    }
    
    [Transactional(TransactionalTypes.TransactionScope, TransactionIsolationLevel.ReadCommitted, 100)]
    public object Update(Root obj)
    {

      obj.IsolationLevel = System.Transactions.Transaction.Current.IsolationLevel.ToString();
      obj.TransactionTimeout = 100;
      obj.TransactionalType = _transactionalType;
      obj.Data = "Update";
      obj.MarkAsOld();
      return obj;
    }

    private TransactionalTypes _transactionalType;

    public void Invoke(Csla.DataPortalEventArgs args)
    {
      _transactionalType = args.DataPortalContext.TransactionalType;
    }
  }

  public class RootFactory5 : Csla.Server.ObjectFactory
  {
    public RootFactory5(ApplicationContext applicationContext)
      : base(applicationContext) { }

    [RunLocal]
    public object Create()
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Location = obj.ExecutionLocation;
      obj.MarkAsNew();
      return obj;
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    public object Update(Root obj)
    {

      obj.IsolationLevel = System.Transactions.Transaction.Current.IsolationLevel.ToString();
      obj.TransactionTimeout = ApplicationContext.DefaultTransactionTimeoutInSeconds;
      obj.TransactionalType = _transactionalType;
      obj.Data = "Update";
      obj.MarkAsOld();
      return obj;
    }

    private TransactionalTypes _transactionalType;

    public void Invoke(Csla.DataPortalEventArgs args)
    {
      _transactionalType = args.DataPortalContext.TransactionalType;
    }
  }

  //public class RootFactory6
  //{
  //  [Transactional(TransactionalTypes.TransactionScope, TransactionIsolationLevel.ReadCommitted)]
  //  public object Update(Root obj)
  //  {
  //
  //    obj.IsolationLevel = System.EnterpriseServices.ContextUtil.SystemTransaction.IsolationLevel.ToString();
  //    obj.TransactionalType = _transactionalType;
  //    obj.Data = "Update";
  //    obj.MarkAsOld();
  //    return obj;
  //  }
  //
  //  private TransactionalTypes _transactionalType;
  //
  //  public void Invoke(Csla.DataPortalEventArgs args)
  //  {
  //    _transactionalType = args.DataPortalContext.TransactionalType;
  //  }
  //}
  //
  //public class RootFactory7
  //{
  //  [Transactional(TransactionalTypes.EnterpriseServices)]
  //  public object Update(Root obj)
  //  {
  //    obj.IsolationLevel = System.EnterpriseServices.ContextUtil.SystemTransaction.IsolationLevel.ToString();
  //
  //    obj.TransactionalType = _transactionalType;
  //    obj.Data = "Update";
  //    obj.MarkAsOld();
  //    return obj;
  //  }
  //
  //  private TransactionalTypes _transactionalType;
  //
  //  public void Invoke(Csla.DataPortalEventArgs args)
  //  {
  //    _transactionalType = args.DataPortalContext.TransactionalType;
  //  }
  //}

}