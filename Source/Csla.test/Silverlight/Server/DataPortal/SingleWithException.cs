﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.DataPortal
{
  [Serializable]
  public class SingleWithException : BusinessBase<SingleWithException>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static readonly PropertyInfo<string> MethodCalledProperty = RegisterProperty(new PropertyInfo<string>("MethodCalled", "MethodCalled"));
    public string MethodCalled
    {
      get { return GetProperty(MethodCalledProperty); }
      set { SetProperty(MethodCalledProperty, value); }
    }

    public void SetAsChild()
    {
      MarkAsChild();
    }

    protected override void DataPortal_Create()
    {
      MethodCalled = "Created";
      base.DataPortal_Create();
    }

    protected override void DataPortal_Insert()
    {
      MethodCalled = "Inserted";
      throw new DataException("boom");
    }

    protected override void DataPortal_Update()
    {
      MethodCalled = "Updated";
      throw new DataException("boom");
    }

    protected override void DataPortal_DeleteSelf()
    {
      throw new DataException("boom");
    }

    private void DataPortal_Delete(int id)
    {
      throw new DataException("boom");
    }

  }
}
