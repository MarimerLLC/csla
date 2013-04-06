using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace Csla.Test.ObjectFactory
{
  [Csla.Server.ObjectFactory("Csla.Test.ObjectFactory.RootFactory, Csla.Test")]
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    internal static PropertyInfo<string> DataProperty = RegisterProperty(new PropertyInfo<string>("Data"));
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

    private static PropertyInfo<TransactionalTypes> TransactionalTypeProperty = RegisterProperty(new PropertyInfo<TransactionalTypes>("TransactionalType"));
    public TransactionalTypes TransactionalType
    {
      get { return GetProperty(TransactionalTypeProperty); }
      set { SetProperty(TransactionalTypeProperty, value); }
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
