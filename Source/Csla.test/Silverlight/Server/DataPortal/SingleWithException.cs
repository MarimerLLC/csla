using System;
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

    [Create]
		protected void DataPortal_Create()
    {
      MethodCalled = "Created";
      BusinessRules.CheckRules();
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      MethodCalled = "Inserted";
      throw new DataException("boom");
    }

    [Update]
		protected void DataPortal_Update()
    {
      MethodCalled = "Updated";
      throw new DataException("boom");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      throw new DataException("boom");
    }

    [Delete]
		private void DataPortal_Delete(int id)
    {
      throw new DataException("boom");
    }

  }
}
