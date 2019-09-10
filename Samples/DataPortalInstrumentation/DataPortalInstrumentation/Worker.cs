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
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    [Fetch]
    private void Fetch(int id)
    {
      // TODO: load values into object
      System.Threading.Thread.Sleep(300);
    }

    [Insert]
    private void Insert()
    {
      // TODO: insert object's data
    }

    [Update]
    private void Update()
    {
      // TODO: update object's data
    }

    [DeleteSelf]
    private void DeleteSelf()
    {
      Delete(ReadProperty(IdProperty));
    }

    [Delete]
    private void Delete(int id)
    {
      // TODO: delete object's data
    }

  }
}
