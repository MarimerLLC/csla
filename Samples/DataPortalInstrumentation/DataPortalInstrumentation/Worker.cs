using Csla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataPortalInstrumentation
{
  [Serializable]
  public class Worker : BusinessBase<Worker>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }


    private void DataPortal_Fetch(int id)
    {
      // TODO: load values into object
      System.Threading.Thread.Sleep(300);
    }

    protected override void DataPortal_Insert()
    {
      // TODO: insert object's data
    }

    protected override void DataPortal_Update()
    {
      // TODO: update object's data
    }

    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(ReadProperty(IdProperty));
    }

    private void DataPortal_Delete(int id)
    {
      // TODO: delete object's data
    }

  }
}
